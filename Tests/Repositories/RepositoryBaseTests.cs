using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Repositories;
using com.etsoo.Database;
using com.etsoo.Utils;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Moq;
using NUnit.Framework;
using System.IO.Pipelines;
using System.Text;

namespace Tests.Repositories
{
    /// <summary>
    /// Repository base tests
    /// </summary>
    [TestFixture]
    public class RepositoryBaseTests
    {
        /// <summary>
        /// Repository base
        /// </summary>
        private class RepositoryBaseTest : RepoBase<SqliteConnection>
        {
            public RepositoryBaseTest(ICoreApplication<SqliteConnection> app) : base(app, "test") { }
        }

        readonly SqliteDatabase db;
        readonly RepositoryBaseTest repo;

        public RepositoryBaseTests()
        {
            // :memory: would be better for case by case
            db = new SqliteDatabase("Data Source = etsoo.db;");

            var config = new AppConfiguration("test");
            var app = new CoreApplication<SqliteConnection>(config, db);

            repo = new RepositoryBaseTest(app);
        }

        [SetUp]
        public async Task Setup()
        {
            await db.WithConnection((connection) =>
            {
                var sql = @"
                    CREATE TABLE IF NOT EXISTS e_user (id int PRIMARY KEY, name nvarchar(128), status int);
                    INSERT OR IGNORE INTO e_user (id, name) VALUES(1002, 'Admin 2');
                    INSERT OR IGNORE INTO e_user (id, name) VALUES(1001, 'Admin 1');
                ";
                return connection.ExecuteAsync(sql);
            });
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
            var (result, data) = await repo.InlineUpdateAsync<int, UserUpdateModule>(user, new(new[] { "name = IIF(@Id = 1001, t.'name', @Name)" })
            {
                IdField = "id",
                TableName = "e_user"
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
            var sql = "SELECT * FROM e_user WHERE id = 1001";
            var command = new CommandDefinition(sql);

            // Act
            var result = await repo.QueryAsAsync<TestUserModule>(command);

            // Assert
            Assert.IsTrue(result?.Name == "Admin 1");
        }

        [Test]
        public async Task QueryAsResult_NoActionResult()
        {
            // Arrange
            var sql = "SELECT * FROM e_user WHERE id = -1";
            var command = new CommandDefinition(sql);

            // Act
            var result = await repo.QueryAsResultAsync(command);

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
            var result = await repo.QueryAsResultAsync(command);

            // Assert
            Assert.IsTrue(result.Ok);
        }

        [Test]
        public async Task ReadToStreamAsync_Test()
        {
            // Arrange
            var sql = "SELECT json_group_array(json_object('id', id, 'name', name)) AS json_result FROM (SELECT id, name FROM e_user WHERE id = 1001 LIMIT 3)";
            var command = new CommandDefinition(sql);
            using var stream = SharedUtils.GetStream();

            // Act
            var result = await repo.ReadToStreamAsync(command, stream);
            var json = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(json.Contains("Admin 1"));
        }

        [Test]
        public async Task ReadToStreamMultipleResultsAsync_Test()
        {
            // Arrange
            var sql = "SELECT json_group_array(json_object('id', id, 'name', name)) AS json_result FROM (SELECT id, name FROM e_user WHERE id = 1001 LIMIT 3); SELECT json_object('id', id, 'name', name) AS json_result FROM (SELECT id, name FROM e_user WHERE id = 1001 LIMIT 1)";
            var command = new CommandDefinition(sql);
            using var stream = SharedUtils.GetStream();

            // Act
            var result = await repo.ReadToStreamAsync(command, stream, DataFormat.Json, new[] { "users" });
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
            var sql = "SELECT json_group_array(json_object('id', id, 'name', name)) AS json_result FROM (SELECT id, name FROM e_user WHERE id = 1001 LIMIT 3); SELECT json_object('id', id, 'name', name) AS json_result FROM (SELECT id, name FROM e_user WHERE id = 1001 LIMIT 1)";
            var command = new CommandDefinition(sql);
            using var stream = SharedUtils.GetStream();

            // Act
            var mock = new Mock<HttpResponse>();
            mock.Setup((o) => o.BodyWriter).Returns(PipeWriter.Create(new MemoryStream()));

            var result = await repo.ReadJsonToStreamWithReturnAsync(command, mock.Object, new[] { "users" });
            var json = Encoding.UTF8.GetString(result.ToArray());

            // Assert
            Assert.IsTrue(json.Contains("id"));
        }
    }
}
