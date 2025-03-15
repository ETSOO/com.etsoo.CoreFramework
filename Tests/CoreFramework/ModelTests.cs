using com.etsoo.CoreFramework.Business;
using com.etsoo.CoreFramework.Models;
using com.etsoo.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Buffers;
using System.Data;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Tests.CoreFramework
{
    public record User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public EntityStatus Status { get; set; }
    }

    public partial class MyDbContext : DbContext
    {
        public required DbSet<User> Users { get; set; }

        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=etsoo.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Status)
                    .HasConversion<byte>()
                    .HasDefaultValue(EntityStatus.Normal);
            });
        }
    }

    [TestFixture]
    public class ModelTests
    {
        static MyDbContext CreateDbContext()
        {
            var services = new ServiceCollection();
            services.AddDbContext<MyDbContext>();

            var provider = services.BuildServiceProvider();
            return provider.GetRequiredService<MyDbContext>();
        }

        [Test]
        public void LoginIdRQTest()
        {
            var loginIdRQ = new LoginIdRQ
            {
                DeviceId = "X01/cDAy!10395F7FDE101D118CE02CEFA26B699926FE5E2F86F67E3841C1B538BD030962E5ZubItmil/PX4GNPXtOE0vl3S8g1Jgx8aISwsdbHEbE71+IOf1Udtd6xD4nw93Fdc",
                Id = "019862b4a71672968de589405bd96f634b6e6cb4a811935f0cde67ae8239af35afyhUCWj2ZrRJ18oyOz9khHGw+oORxl4RWZiOcCOviFVA=",
                Region = "CN",
                TimeZone = "Asia/Shanghai"
            };

            Assert.That(loginIdRQ.Validate(), Is.Null);
        }

        [Test]
        public void QueryRQTest()
        {
            var rq = new QueryRQ<int>
            {
                ExcludedIds = [1, 2, 3],
                QueryPaging = new QueryPagingData
                {
                    BatchSize = 2
                }
            };

            var db = CreateDbContext();
            var sql = db.Users.QueryEtsoo(rq, (u) => u.Id).ToQueryString();

            Assert.That(sql, Does.Contain("ORDER BY \"u\".\"Id\" DESC"));
        }

        [Test]
        public async Task UpdateTestAsync()
        {
            var db = CreateDbContext();

            // Update
            await db.Database.ExecuteSqlRawAsync("UPDATE \"User\" SET \"Status\" = 0 WHERE (\"Status\" IS NULL OR \"Status\" <> 0)");

            var user = await db.Users.FirstOrDefaultAsync();
            if (user == null)
            {
                Assert.Inconclusive("No user data");
                return;
            }

            user.Name = "Admin Changed";
            user.Status = EntityStatus.Deleted;

            var changedProperties = db.ChangeTracker.Entries().GetChangedProperties();

            db.ChangeTracker.Clear();

            Assert.Multiple(() =>
            {
                Assert.That(changedProperties.Count(), Is.EqualTo(2));
                Assert.That(changedProperties.FirstOrDefault(p => p.Name == "Name")?.CurrentValue, Is.EqualTo("Admin Changed"));
                Assert.That(changedProperties.FirstOrDefault(p => p.Name == "Status")?.OriginalValue, Is.EqualTo("Normal"));
                Assert.That(changedProperties.FirstOrDefault(p => p.Name == "Status")?.CurrentValue, Is.EqualTo("Deleted"));
            });
        }

        [Test]
        public async Task QueryJsonTest()
        {
            var rq = new QueryRQ<int>
            {
                ExcludedIds = [1, 2, 3],
                QueryPaging = new QueryPagingData
                {
                    BatchSize = 2,
                    OrderBy = [new() { Field = "Name" }]
                }
            };

            var db = CreateDbContext();

            var writer = new ArrayBufferWriter<byte>();
            var (hasContent, commandText) = await db.Users.QueryEtsoo(rq, (u) => u.Id).Select(u => new { u.Id, NewName = u.Name, u.Status }).ToJsonAsync(writer);

            var json = Encoding.UTF8.GetString(writer.WrittenSpan);

            Assert.Multiple(() =>
            {
                Assert.That(hasContent, Is.True);
                Assert.That(commandText, Is.EqualTo("SELECT json_group_array(json_object('id', \"Id\", 'newName', \"NewName\", 'status', \"Status\")) FROM (SELECT \"u\".\"Id\", \"u\".\"Name\" AS \"NewName\", \"u\".\"Status\"\r\nFROM \"User\" AS \"u\"\r\nWHERE \"u\".\"Id\" NOT IN (1, 2, 3)\r\nORDER BY \"u\".\"Name\", \"u\".\"Id\" DESC\r\nLIMIT @__p_0)"));
                Assert.That(json, Does.Contain("\"newName\":\"Admin 1\""));
            });
        }

        [Test]
        public void SelectRegexAndSplitRegexTests()
        {
            var sql = """
                SELECT c.id AS "Id", c.name AS "Name", c.preferred_name AS "PreferredName", hide_data(c.pin, NULL) AS "Pin", c.status AS "Status", c.creation AS "Creation", (
                    SELECT count(*)::int
                    FROM (
                        SELECT DISTINCT p.org_id
                        FROM person AS p
                        WHERE c.id = p.core_user_id
                    ) AS p0) AS "Orgs"
                FROM core_user AS c
                ORDER BY c.creation DESC, c.id DESC
                LIMIT @__p_0
                """;

            var match = DatabaseExtensions.SelectRegex().Match(sql);
            if (!match.Success || match.Groups.Count < 2)
            {
                throw new DataException("SELECT command text is not valid");
            }

            // Columns
            var columns = DatabaseExtensions.SplitRegex().Split(match.Groups[1].Value);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(columns, Has.Length.EqualTo(7));
                Assert.That(columns.First().Trim(), Is.EqualTo("c.id AS \"Id\""));
            });
        }

        [Test]
        public async Task QueryJsonSimpleLikeTest()
        {
            var rq = new QueryRQ<int>
            {
                Ids = [1001, 1002, 1003],
                ExcludedIds = [1, 2, 3],
                Enabled = true,
                QueryPaging = new QueryPagingData
                {
                    BatchSize = 1,
                    OrderBy = [new() { Field = "Name" }]
                }
            };

            var db = CreateDbContext();

            var writer = new ArrayBufferWriter<byte>();
            var (_, commandText) = await db.Users
                .QueryEtsoo(rq, (u) => u.Id, (u) => u.Status, (q) =>
                {
                    return q.QueryEtsooKeywords("肖赞", null, (u) => u.Name);
                })
                .Select(u => new { u.Id, NewName = u.Name, u.Status })
                .ToJsonAsync(writer);

            Assert.Multiple(() =>
            {
                Assert.That(commandText, Is.EqualTo("SELECT json_group_array(json_object('id', \"Id\", 'newName', \"NewName\", 'status', \"Status\")) FROM (SELECT \"u\".\"Id\", \"u\".\"Name\" AS \"NewName\", \"u\".\"Status\"\r\nFROM \"User\" AS \"u\"\r\nWHERE \"u\".\"Id\" IN (1001, 1002, 1003) AND \"u\".\"Id\" NOT IN (1, 2, 3) AND \"u\".\"Status\" <= 100 AND \"u\".\"Name\" LIKE '%肖赞%'\r\nORDER BY \"u\".\"Name\", \"u\".\"Id\" DESC\r\nLIMIT @__p_1)"));
            });
        }

        [Test]
        public async Task QueryJsonLikeTest()
        {
            var rq = new QueryRQ<int>
            {
                Id = 1001,
                ExcludedIds = [1, 2, 3],
                Enabled = false,
                QueryPaging = new QueryPagingData
                {
                    BatchSize = 1,
                    OrderBy = [new() { Field = "Name" }]
                }
            };

            var db = CreateDbContext();

            var writer = new ArrayBufferWriter<byte>();
            var (hasContent, commandText) = await db.Users
                .QueryEtsoo(rq, (u) => u.Id, (u) => u.Status, (q) =>
                {
                    return q.QueryEtsooKeywords("肖 -赞 \"肖必须\"", null, (u) => u.Name);
                })
                .Select(u => new { u.Id, NewName = u.Name, u.Status })
                .ToJsonAsync(writer);

            Assert.Multiple(() =>
            {
                Assert.That(hasContent, Is.True); // Empty array
                Assert.That(commandText, Is.EqualTo("SELECT json_group_array(json_object('id', \"Id\", 'newName', \"NewName\", 'status', \"Status\")) FROM (SELECT \"u\".\"Id\", \"u\".\"Name\" AS \"NewName\", \"u\".\"Status\"\r\nFROM \"User\" AS \"u\"\r\nWHERE \"u\".\"Id\" = 1001 AND \"u\".\"Id\" NOT IN (1, 2, 3) AND \"u\".\"Status\" > 100 AND ((\"u\".\"Name\" LIKE '%肖%' AND \"u\".\"Name\" NOT LIKE '%赞%') OR \"u\".\"Name\" LIKE '%肖必须%')\r\nORDER BY \"u\".\"Name\", \"u\".\"Id\" DESC\r\nLIMIT @__p_1)"));
            });
        }

        [Test]
        public async Task QueryJsonEmptyTest()
        {
            var rq = new QueryRQ<int>
            {
                Status = EntityStatus.Approved,
                Enabled = false,
                Ids = [1, 2],
                QueryPaging = new QueryPagingData
                {
                    BatchSize = 2,
                    OrderBy = [new() { Field = "Name" }, new() { Field = "Id", Desc = true }]
                }
            };

            var db = CreateDbContext();

            var writer = new ArrayBufferWriter<byte>();
            var (hasContent, _) = await db.Users.QueryEtsoo(rq, (u) => u.Id, (u) => u.Status).Select(u => new { u.Id, NewName = u.Name, u.Status }).ToJsonAsync(writer);

            var json = Encoding.UTF8.GetString(writer.WrittenSpan);
            Assert.Multiple(() =>
            {
                Assert.That(hasContent, Is.True);
                Assert.That(json, Is.EqualTo("[]"));
            });
        }

        [Test]
        public async Task QueryJsonObjectTest()
        {
            var rq = new QueryRQ<int>
            {
                ExcludedIds = [1, 2, 3],
                QueryPaging = new QueryPagingData
                {
                    BatchSize = 1,
                    OrderBy = [new() { Field = "Name" }]
                }
            };

            var db = CreateDbContext();

            var writer = new ArrayBufferWriter<byte>();
            var (hasContent, commandText) = await db.Users.QueryEtsoo(rq, (u) => u.Id).Select(u => new { u.Id, NewName = u.Name, u.Status }).ToJsonObjectAsync(writer);

            var json = Encoding.UTF8.GetString(writer.WrittenSpan);

            Assert.Multiple(() =>
            {
                Assert.That(hasContent, Is.True);
                Assert.That(commandText, Is.EqualTo("SELECT json_object('id', \"Id\", 'newName', \"NewName\", 'status', \"Status\") FROM (SELECT \"u\".\"Id\", \"u\".\"Name\" AS \"NewName\", \"u\".\"Status\"\r\nFROM \"User\" AS \"u\"\r\nWHERE \"u\".\"Id\" NOT IN (1, 2, 3)\r\nORDER BY \"u\".\"Name\", \"u\".\"Id\" DESC\r\nLIMIT @__p_0)"));
                Assert.That(json, Does.Contain("\"newName\":\"Admin 1\""));
            });
        }

        [Test]
        public async Task QueryJsonObjectWithKeysetsTest()
        {
            var rq = new QueryRQ<int>
            {
                ExcludedIds = [1, 2, 3],
                QueryPaging = new QueryPagingData
                {
                    BatchSize = 1,
                    Keysets = ["肖赞", 1],
                    OrderBy = [new() { Field = "Name" }]
                }
            };

            var db = CreateDbContext();

            var writer = new ArrayBufferWriter<byte>();
            var (hasContent, commandText) = await db.Users.QueryEtsoo(rq, (u) => u.Id).Select(u => new { u.Id, NewName = u.Name, u.Status }).ToJsonObjectAsync(writer);

            var json = Encoding.UTF8.GetString(writer.WrittenSpan);

            Assert.Multiple(() =>
            {
                Assert.That(hasContent, Is.False);
                Assert.That(commandText, Is.EqualTo("SELECT json_object('id', \"Id\", 'newName', \"NewName\", 'status', \"Status\") FROM (SELECT \"u\".\"Id\", \"u\".\"Name\" AS \"NewName\", \"u\".\"Status\"\r\nFROM \"User\" AS \"u\"\r\nWHERE \"u\".\"Id\" NOT IN (1, 2, 3) AND (\"u\".\"Name\" > '肖赞' OR (\"u\".\"Name\" = '肖赞' AND \"u\".\"Id\" < 1))\r\nORDER BY \"u\".\"Name\", \"u\".\"Id\" DESC\r\nLIMIT @__p_0)"));
            });
        }

        [Test]
        public async Task AuthRefreshTokenRQTest()
        {
            var rq = new AuthRefreshTokenRQ
            {
                AppId = 1,
                AppKey = "",
                TimeZone = "Asia/Shanghai",
                RefreshToken = "v0ezp)kemg4j4Uegu3Y~pO-Ty4>o286RuTuxfs<Wd0rR4JsVX79Rad8L1I@de~>k"
            };
            rq.Sign = rq.SignWith("JwANgd$v=U*cW9-Dg7DA=jejn2UN<t-S");

            using var jsonContent = JsonContent.Create(rq, ModelJsonSerializerContext.Default.AuthRefreshTokenRQ);
            var newRQ = JsonSerializer.Deserialize(await jsonContent.ReadAsStreamAsync(), ModelJsonSerializerContext.Default.AuthRefreshTokenRQ);

            Assert.Multiple(() =>
            {
                Assert.That(rq.Validate(), Is.Null);
                Assert.That(rq.Sign, Has.Length.LessThan(128));
                Assert.That(newRQ?.Sign, Is.EqualTo(rq.Sign));
                Assert.That(newRQ?.TotalMinutes(), Is.InRange(0.00001, 1));
            });
        }

        [Test]
        public void ToJsonInternalAsyncLogicTest()
        {
            var commandText = "SELECT c1.id AS \"Id\", COALESCE(c1.local_name, c0.name) AS \"Name\", c0.identity_type AS \"IdentityType\", c0.require_local_url AS \"RequireLocalUrl\", COALESCE(c1.local_url, c0.web_url) AS \"WebUrl\", COALESCE(c1.local_help_url, c0.help_url) AS \"HelpUrl\", c0.logo AS \"Logo\", c1.expiry AS \"Expiry\", CASE\r\n    WHEN c1.expiry IS NULL OR c1.expiry <= now() + INTERVAL '-90 days' THEN NULL\r\n    ELSE CAST(date_part('epoch', c1.expiry - now()) / 86400.0 AS integer)\r\nEND AS \"ExpiryDays\", c1.status AS \"Status\", c1.creation AS \"Creation\", (SELECT name FROM user LIMIT 1) AS \"Test\"\r\nFROM (\r\n    SELECT c.id, c.core_app_id, c.creation, c.expiry, c.local_help_url, c.local_name, c.local_url, c.status\r\n    FROM core_organization_app AS c\r\n    WHERE c.core_organization_id = @__User_OrganizationInt_0\r\n    ORDER BY c.creation DESC, c.id DESC\r\n    LIMIT @__p_1\r\n) AS c1\r\nINNER JOIN core_app AS c0 ON c1.core_app_id = c0.id\r\nORDER BY c1.creation DESC, c1.id DESC";

            var match = DatabaseExtensions.SelectRegex().Match(commandText);
            if (!match.Success || match.Groups.Count < 2)
            {
                throw new DataException("SELECT command text is not valid");
            }

            // Columns
            var columns = DatabaseExtensions.SplitRegex().Split(match.Groups[1].Value);

            char[] trimChars = ['"', '\''];

            // Fields
            var fields = columns.Select(c =>
            {
                c = c.Trim('\r', '\n', '\t');

                var pos = c.LastIndexOf(" AS ", StringComparison.OrdinalIgnoreCase);
                string source;
                if (pos == -1)
                {
                    source = c.Split('.').Last();
                }
                else
                {
                    source = c[(pos + 4)..];
                }

                var name = JsonNamingPolicy.CamelCase.ConvertName(source.Trim(trimChars));
                return new { Source = source, Name = name };
            });

            // Assert
            Assert.That(fields.Count(), Is.EqualTo(12));
            var nameField = fields.First(f => f.Source == "\"Name\"");
            Assert.That(nameField.Name, Is.EqualTo("name"));
            var testField = fields.First(f => f.Source == "\"Test\"");
            Assert.That(testField.Name, Is.EqualTo("test"));
        }
    }
}
