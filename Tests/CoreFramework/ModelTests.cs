using com.etsoo.CoreFramework.Business;
using com.etsoo.CoreFramework.Models;
using com.etsoo.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Buffers;
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
                entity.Property(e => e.Status).HasConversion<byte>().HasDefaultValue(EntityStatus.Normal);
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
                Region = "CN"
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
        public async Task QueryJsonSimpleLikeTest()
        {
            var rq = new QueryRQ<int>
            {
                QueryPaging = new QueryPagingData
                {
                    BatchSize = 1,
                    OrderBy = [new() { Field = "Name" }]
                }
            };

            var db = CreateDbContext();

            var writer = new ArrayBufferWriter<byte>();
            var (_, commandText) = await db.Users
                .QueryEtsoo(rq, (u) => u.Id)
                .QueryEtsooKeywords("肖赞", "Like", (u) => u.Name)
                .Select(u => new { u.Id, NewName = u.Name, u.Status })
                .ToJsonAsync(writer);

            Assert.Multiple(() =>
            {
                Assert.That(commandText, Is.EqualTo("SELECT json_group_array(json_object('id', \"Id\", 'newName', \"NewName\", 'status', \"Status\")) FROM (SELECT \"u0\".\"Id\", \"u0\".\"Name\" AS \"NewName\", \"u0\".\"Status\"\r\nFROM (\r\n    SELECT \"u\".\"Id\", \"u\".\"Name\", \"u\".\"Status\"\r\n    FROM \"User\" AS \"u\"\r\n    ORDER BY \"u\".\"Name\", \"u\".\"Id\" DESC\r\n    LIMIT @__p_0\r\n) AS \"u0\"\r\nWHERE \"u0\".\"Name\" LIKE '%肖赞%'\r\nORDER BY \"u0\".\"Name\", \"u0\".\"Id\" DESC)"));
            });
        }

        [Test]
        public async Task QueryJsonLikeTest()
        {
            var rq = new QueryRQ<int>
            {
                QueryPaging = new QueryPagingData
                {
                    BatchSize = 1,
                    OrderBy = [new() { Field = "Name" }]
                }
            };

            var db = CreateDbContext();

            var writer = new ArrayBufferWriter<byte>();
            var (hasContent, commandText) = await db.Users
                .QueryEtsoo(rq, (u) => u.Id)
                .QueryEtsooKeywords("肖 -赞 \"肖必须\"", "Like", (u) => u.Name)
                .Select(u => new { u.Id, NewName = u.Name, u.Status })
                .ToJsonAsync(writer);

            Assert.Multiple(() =>
            {
                Assert.That(hasContent, Is.True); // Empty array
                Assert.That(commandText, Is.EqualTo("SELECT json_group_array(json_object('id', \"Id\", 'newName', \"NewName\", 'status', \"Status\")) FROM (SELECT \"u0\".\"Id\", \"u0\".\"Name\" AS \"NewName\", \"u0\".\"Status\"\r\nFROM (\r\n    SELECT \"u\".\"Id\", \"u\".\"Name\", \"u\".\"Status\"\r\n    FROM \"User\" AS \"u\"\r\n    ORDER BY \"u\".\"Name\", \"u\".\"Id\" DESC\r\n    LIMIT @__p_0\r\n) AS \"u0\"\r\nWHERE (\"u0\".\"Name\" LIKE '%肖%' AND \"u0\".\"Name\" NOT LIKE '%赞%') OR \"u0\".\"Name\" LIKE '%肖必须%'\r\nORDER BY \"u0\".\"Name\", \"u0\".\"Id\" DESC)"));
            });
        }

        [Test]
        public async Task QueryJsonEmptyTest()
        {
            var rq = new QueryRQ<int>
            {
                Status = EntityStatus.Approved,
                Disabled = true,
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
            });
        }
    }
}
