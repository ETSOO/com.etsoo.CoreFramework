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
        public ServiceTest(ICoreApplication<AppConfiguration, SqliteConnection> app, string flag, ILogger logger) : base(app, null, flag, logger, false)
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

            service = new ServiceTest(app, "test", new EventLogLoggerProvider().CreateLogger("SmartERPTests"));
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
            await service.SqlDeleteAsync("User", [1003]);

            var user = new SqlUserInsert { Id = 1003, Name = "Admin 3", Status = EntityStatus.Approved };

            var id = await service.SqlInsertAsync<SqlUserInsert, int>(user);
            Assert.AreEqual(1003, id);

            var update = new SqlUserUpdate { Id = 1003, Name = "Admin 3 Updated", ChangedFields = ["Name"] };
            var updateResult = await service.SqlUpdateAsync(update);
            Assert.IsTrue(updateResult.Ok);

            var select = new SqlUserSelect { Id = 1003, QueryPaging = new QueryData { BatchSize = 2 } };
            var selectData = (await service.SqlSelectAsync<SqlUserSelect, UserData>(select)).FirstOrDefault();
            Assert.IsNotNull(selectData);
            Assert.AreEqual("Admin 3 Updated", selectData.Name);

            var writer = new ArrayBufferWriter<byte>();
            await service.SqlSelectJsonAsync<SqlUserSelect, UserData>(select, writer);
            var json = Encoding.UTF8.GetString(writer.WrittenSpan);
            Assert.AreEqual("[{\"id\":1003,\"name\":\"Admin 3 Updated\",\"status\":100}]", json);

            var deleteResult = await service.SqlDeleteAsync("User", [1003]);
            Assert.IsTrue(deleteResult.Ok);
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
            Assert.AreEqual(input, plainText);
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

            // Assert
            Assert.IsNull(plainText);
            Assert.AreEqual(input, plainTextLong);
        }

        [Test]
        public async Task InitCallTests()
        {
            // Arrange
            var rq = new InitCallRQ { Timestamp = SharedUtils.UTCToJsMiliseconds() };
            var result = await service.InitCallAsync(rq, "My Password");
            Assert.IsTrue(result.Ok);

            var deviceId = result.Data.Get("DeviceId");
            Assert.IsNotNull(deviceId);

            var rqNew = new InitCallRQ { Timestamp = SharedUtils.UTCToJsMiliseconds(), DeviceId = deviceId };
            var resultNew = await service.InitCallAsync(rqNew, "My Password");
            Assert.IsTrue(resultNew.Ok);
        }

        [Test]
        public async Task InlineUpdateAsync_Test()
        {
            // Arrange
            var user = new UserUpdateModule
            {
                Id = 1001,
                Name = "Admin 21",
                ChangedFields = new[] { "Name" }
            };

            // Act
            var (result, data) = await service.InlineUpdateAsync<int, UserUpdateModule>(user, new(new[] { "name = IIF(@Id = 1001, t.'name', @Name)" })
            {
                IdField = "id",
                TableName = "User"
            });

            // Assert
            Assert.IsTrue(result.Ok);
            Assert.IsNotNull(data);
            Assert.AreEqual(1, data?.RowsAffected);
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
            Assert.IsTrue(result?.Name == "Admin 1");
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
            Assert.IsFalse(result.Ok);
            Assert.AreEqual(error.Type, result.Type);
            Assert.AreEqual(error.Title, result.Title);
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
            Assert.IsTrue(result.Ok);
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

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(json.Contains("Admin 1"));
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

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(json.Contains("\"users\":"));
            Assert.IsTrue(json.Contains("\"data2\":"));
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
            Assert.IsTrue(json.Contains("id"));
        }
    }
}
