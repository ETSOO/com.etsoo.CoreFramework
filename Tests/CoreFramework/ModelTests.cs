﻿using com.etsoo.CoreFramework.Business;
using com.etsoo.CoreFramework.Models;
using com.etsoo.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Buffers;
using System.Text;

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
                    OrderBy = new Dictionary<string, bool>
                    {
                        ["Name"] = false,
                        ["Id"] = true
                    }
                }
            };

            var db = CreateDbContext();

            var writer = new ArrayBufferWriter<byte>();
            var users = await db.Users.QueryEtsoo(rq, (u) => u.Id).Select(u => new { u.Id, NewName = u.Name, u.Status }).ToJsonAsync(writer);

            var json = Encoding.UTF8.GetString(writer.WrittenSpan);
            Assert.That(json, Does.Contain("\"newName\":\"Admin 1\""));
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
                    OrderBy = new Dictionary<string, bool>
                    {
                        ["Name"] = false,
                        ["Id"] = true
                    }
                }
            };

            var db = CreateDbContext();

            var writer = new ArrayBufferWriter<byte>();
            var users = await db.Users.QueryEtsoo(rq, (u) => u.Id, (u) => u.Status).Select(u => new { u.Id, NewName = u.Name, u.Status }).ToJsonAsync(writer);

            var json = Encoding.UTF8.GetString(writer.WrittenSpan);
            Assert.That(json, Is.EqualTo("[]"));
        }

        [Test]
        public void AuthRefreshTokenRQTest()
        {
            var rq = new AuthRefreshTokenRQ
            {
                AppId = 1,
                AppKey = "",
                RefreshToken = "v0ezp)kemg4j4Uegu3Y~pO-Ty4>o286RuTuxfs<Wd0rR4JsVX79Rad8L1I@de~>k"
            };
            rq.Sign = rq.SignWith("JwANgd$v=U*cW9-Dg7DA=jejn2UN<t-S");

            Assert.Multiple(() =>
            {
                Assert.That(rq.Validate(), Is.Null);
                Assert.That(rq.Sign, Has.Length.LessThan(128));
            });
        }
    }
}
