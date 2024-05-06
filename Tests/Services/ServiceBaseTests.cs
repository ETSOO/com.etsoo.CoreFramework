using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Business;
using com.etsoo.CoreFramework.Models;
using com.etsoo.CoreFramework.Services;
using com.etsoo.CoreFramework.User;
using com.etsoo.Database;
using com.etsoo.Utils;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using Moq;
using NUnit.Framework;
using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.Versioning;
using System.Text;

namespace Tests.Services
{
    internal class ServiceTest : ServiceBase<AppConfiguration, SqliteConnection, ICoreApplication<AppConfiguration, SqliteConnection>, ICurrentUser>
    {
        public ServiceTest(ICoreApplication<AppConfiguration, SqliteConnection> app, string flag, ILogger logger) : base(app, null, flag, logger)
        {
        }

        public override void AddSystemParameters(IDbParameters parameters, bool userRequired = true)
        {
        }

        public new string? Decrypt(string encyptedMessage, string passphrase, int? durationSeconds = null, bool? isWebClient = null)
        {
            return base.Decrypt(encyptedMessage, passphrase, durationSeconds, isWebClient);
        }

        public new string Encrypt(string message, string passphrase, int iterations = 10, bool? enhanced = null)
        {
            return base.Encrypt(message, passphrase, iterations, enhanced);
        }
    }

    [TestFixture]
    [SupportedOSPlatform("windows")]
    internal class ServiceBaseTests
    {
        readonly SqliteDatabase db;
        readonly ServiceTest service;

        public ServiceBaseTests()
        {
            db = new SqliteDatabase("Data Source = etsoo.db;");

            var config = new AppConfiguration { Name = "test" };
            var app = new CoreApplication<AppConfiguration, SqliteConnection>(config, db);

            service = new ServiceTest(app, "User", new EventLogLoggerProvider().CreateLogger("SmartERPTests"));
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
        public async Task SqlModelTests()
        {
            await service.SqlDeleteAsync([1003]);

            var user = new SqlUserInsert { Id = 1003, Name = "Admin 3", Status = EntityStatus.Approved };

            var id = await service.SqlInsertAsync<SqlUserInsert, int>(user);
            Assert.That(id, Is.EqualTo(1003));

            var update = new SqlUserUpdate { Id = 1003, Name = "Admin 3 Updated", ChangedFields = ["Name"] };
            var updateResult = await service.SqlUpdateAsync(update);
            Assert.That(updateResult.Ok, Is.True);

            var select = new SqlUserSelect { Id = 1003, QueryPaging = new QueryPagingData { BatchSize = 2 } };
            var selectData = (await service.SqlSelectAsync<SqlUserSelect, UserData>(select)).FirstOrDefault();
            Assert.That(selectData, Is.Not.Null);
            Assert.That(selectData.Name, Is.EqualTo("Admin 3 Updated"));

            var writer = new ArrayBufferWriter<byte>();
            await service.SqlSelectJsonAsync<SqlUserSelect, UserData>(select, writer);
            var json = Encoding.UTF8.GetString(writer.WrittenSpan);
            Assert.That(json, Is.EqualTo("[{\"id\":1003,\"name\":\"Admin 3 Updated\",\"status\":100}]"));

            var deleteResult = await service.SqlDeleteAsync([1003], "User");
            Assert.That(deleteResult.Ok, Is.True);
        }

        [Test]
        public void EncryptionTests()
        {
            // Arrange
            var input = "Hello, world!";
            var passphrase = "My password";

            // Act
            var encrypted = service.Encrypt(input, passphrase, 1);
            var plainText = service.Decrypt(encrypted, passphrase, 120);

            // Assert
            Assert.That(plainText, Is.EqualTo(input));
        }

        [Test]
        public void WebDecryptionTests()
        {
            // Arrange
            var input = "Hello, world!";
            var encrypted = "fDxgI10v!018aa0c6012987c56760bcf00b40914b65544b37d639bc75ac0ee659275563e35fCurMtA1yTMxuFO9xzYwefg==";
            var passphrase = "My password";

            // Act
            var plainText = service.Decrypt(encrypted, passphrase, 120, true);
            var plainTextLong = service.Decrypt(encrypted, passphrase, null, true);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(plainText, Is.Null);
                Assert.That(plainTextLong, Is.EqualTo(input));
            });
        }

        [Test]
        public async Task InitCallTests()
        {
            // Arrange
            var rq = new InitCallRQ { Timestamp = SharedUtils.UTCToJsMiliseconds() };
            var result = await service.InitCallAsync(rq, "My Password");
            Assert.That(result.Ok, Is.True);

            var deviceId = result.Data.Get("DeviceId");
            Assert.That(deviceId, Is.Not.Null);

            var rqNew = new InitCallRQ { Timestamp = SharedUtils.UTCToJsMiliseconds(), DeviceId = deviceId };
            var resultNew = await service.InitCallAsync(rqNew, "My Password");
            Assert.That(resultNew.Ok, Is.True);
        }

        [Test]
        public async Task InlineUpdateAsync_Test()
        {
            // Arrange
            var user = new UserUpdateModule
            {
                Id = 1001,
                Name = "Admin 21",
                ChangedFields = ["Name"]
            };

            // Act
            var (result, data) = await service.InlineUpdateAsync<int, UserUpdateModule>(user, new(["name = IIF(@Id = 1001, t.'name', @Name)"])
            {
                IdField = "id",
                TableName = "User"
            });

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result.Ok, Is.True);
                Assert.That(data, Is.Not.Null);
                Assert.That(data?.RowsAffected, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task QueryAs_Test()
        {
            // Arrange
            var sql = "SELECT * FROM User WHERE id = 1001";
            var command = new CommandDefinition(sql);

            // Act
            var result = await service.QueryAsAsync<TestUserModule>(command);

            // Assert
            Assert.That(result?.Name, Is.EqualTo("Admin 1"));
        }

        [Test]
        public async Task QueryAsResult_NoActionResult()
        {
            // Arrange
            var sql = "SELECT * FROM User WHERE id = -1";
            var command = new CommandDefinition(sql);

            // Act
            var result = await service.QueryAsResultAsync(command);

            // Assert
            var error = ApplicationErrors.NoActionResult;
            Assert.Multiple(() =>
            {
                Assert.That(result.Ok, Is.False);
                Assert.That(result.Type, Is.EqualTo(error.Type));
                Assert.That(result.Title, Is.EqualTo(error.Title));
            });
        }

        [Test]
        public async Task QueryAsResult_NoData()
        {
            // Arrange
            var sql = "SELECT 1 AS ok";
            var command = new CommandDefinition(sql);

            // Act
            var result = await service.QueryAsResultAsync(command);

            // Assert
            Assert.That(result.Ok, Is.True);
        }

        [Test]
        public async Task ReadToStreamAsync_Test()
        {
            // Arrange
            var sql = "SELECT json_group_array(json_object('id', id, 'name', name)) AS json_result FROM (SELECT id, name FROM User WHERE id = 1001 LIMIT 3)";
            var command = new CommandDefinition(sql);
            using var stream = SharedUtils.GetStream();

            // Act
            var result = await service.ReadToStreamAsync(command, stream);
            var json = Encoding.UTF8.GetString(stream.ToArray());

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result, Is.True);
                Assert.That(json, Does.Contain("Admin 1"));
            });
        }

        [Test]
        public async Task ReadToStreamMultipleResultsAsync_Test()
        {
            // Arrange
            var sql = "SELECT json_group_array(json_object('id', id, 'name', name)) AS json_result FROM (SELECT id, name FROM User WHERE id = 1001 LIMIT 3); SELECT json_object('id', id, 'name', name) AS json_result FROM (SELECT id, name FROM User WHERE id = 1001 LIMIT 1)";
            var command = new CommandDefinition(sql);
            using var stream = SharedUtils.GetStream();

            // Act
            var result = await service.ReadToStreamAsync(command, stream, DataFormat.Json, new[] { "users" });
            var json = Encoding.UTF8.GetString(stream.ToArray());

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result, Is.True);
                Assert.That(json, Does.Contain("\"users\":"));
                Assert.That(json, Does.Contain("\"data2\":"));
            });
        }

        [Test]
        public async Task ReadJsonToStreamWithReturnAsync_Test()
        {
            // Arrange
            var sql = "SELECT json_group_array(json_object('id', id, 'name', name)) AS json_result FROM (SELECT id, name FROM User WHERE id = 1001 LIMIT 3); SELECT json_object('id', id, 'name', name) AS json_result FROM (SELECT id, name FROM User WHERE id = 1001 LIMIT 1)";
            var command = new CommandDefinition(sql);
            using var stream = SharedUtils.GetStream();

            // Act
            var mock = new Mock<HttpResponse>();
            mock.Setup((o) => o.BodyWriter).Returns(PipeWriter.Create(new MemoryStream()));

            var result = await service.ReadJsonToStreamWithReturnAsync(command, mock.Object, ["users"]);
            var json = Encoding.UTF8.GetString(result.ToArray());

            // Assert
            Assert.That(json, Does.Contain("id"));
        }
    }
}
