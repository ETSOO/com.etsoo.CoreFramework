using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Services;
using com.etsoo.CoreFramework.User;
using com.etsoo.Database;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using NUnit.Framework;
using System.Buffers;
using System.Globalization;
using System.Net;
using System.Runtime.Versioning;
using System.Text;

namespace Tests.Services
{
    internal class ServiceWithParameterTest : ServiceBase<AppConfiguration, SqliteConnection, ICoreApplication<AppConfiguration, SqliteConnection>, ICurrentUser>
    {
        public ServiceWithParameterTest(ICoreApplication<AppConfiguration, SqliteConnection> app, string flag, ILogger logger)
            : base(app, new CurrentUser("1001", [], null, "Admin", 255, IPAddress.Loopback, "1", new CultureInfo("zh-CN"), "CN", null, null, null), flag, logger)
        {
        }

        public override void AddSystemParameters(IDbParameters parameters, bool userRequired = true)
        {
            parameters.Add("UserId", 1001);
            parameters.Add("OrganizationId", 1);
        }
    }

    [TestFixture]
    [SupportedOSPlatform("windows")]
    internal class ServiceBaseWithSystemParameterTests
    {
        readonly SqliteDatabase db;
        readonly ServiceWithParameterTest service;

        public ServiceBaseWithSystemParameterTests()
        {
            db = new SqliteDatabase("Data Source = etsoo.db;");

            var config = AppConfiguration.Create();
            var app = new CoreApplication<AppConfiguration, SqliteConnection>(config, db);

            service = new ServiceWithParameterTest(app, "User", new EventLogLoggerProvider().CreateLogger("SmartERPTests"));
        }

        [SetUp]
        public async Task Setup()
        {
            await db.WithConnection((connection) =>
            {
                var sql = @"
                    CREATE TABLE IF NOT EXISTS User (id int PRIMARY KEY, name nvarchar(128), status int) WITHOUT ROWID;
                    INSERT OR IGNORE INTO User (id, name) VALUES(1002, 'Admin 2');
                    INSERT OR IGNORE INTO User (id, name) VALUES(1001, 'Admin 1');
                ";
                return connection.ExecuteAsync(sql);
            });
        }

        [Test]
        public async Task SqlSelectJsonAsyncWithMappingTests()
        {
            // Arrange
            var select = new SqlUserSelect { Id = 1001, QueryPaging = new QueryPagingData { BatchSize = 2 } };
            var writer = new ArrayBufferWriter<byte>();

            // Action
            await service.SqlSelectJsonAsync(select, ["id", "name"], writer, true, (mappings) =>
            {
                mappings.Add("isAdmin", "@UserId = id".ToJsonBool());
            });
            var json = Encoding.UTF8.GetString(writer.WrittenSpan);

            // Assert
            Assert.That(json, Is.EqualTo("[{\"id\":1001,\"name\":\"Admin 1\",\"isAdmin\":true}]"));
        }

        [Test]
        public async Task SqlSelectJsonAsyncWithParametersTests()
        {
            // Arrange
            var select = new SqlUserSelect { Id = 1001, QueryPaging = new QueryPagingData { BatchSize = 2 } };
            var writer = new ArrayBufferWriter<byte>();

            // Action
            await service.SqlSelectJsonAsync(select, ["id", "name", "@UserId AS userId"], writer, true);
            var json = Encoding.UTF8.GetString(writer.WrittenSpan);

            // Assert
            Assert.That(json, Is.EqualTo("[{\"id\":1001,\"name\":\"Admin 1\",\"userId\":1001}]"));
        }
    }
}
