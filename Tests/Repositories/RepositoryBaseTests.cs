using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Repositories;
using com.etsoo.Utils.Database;
using Dapper;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using System.Data.Common;
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
            public RepositoryBaseTest(ICoreApplication<SqliteConnection> app) : base(app) { }
        }

        readonly SqliteDatabase db;
        readonly RepositoryBaseTest repo;

        public RepositoryBaseTests()
        {
            db = new SqliteDatabase("Data Source = etsoo.db;");

            var config = new AppConfiguration("test");
            var app = new CoreApplication<SqliteConnection>(config, db);

            repo = new RepositoryBaseTest(app);
        }

        [SetUp]
        public async Task Setup()
        {
            await db.WithConnection((connection) => {
                return connection.ExecuteScalarAsync("CREATE TABLE IF NOT EXISTS e_user (id int, name nvarchar(128), status int)");
            });

            await db.WithConnection((connection) => {
                return connection.ExecuteScalarAsync("INSERT OR IGNORE INTO e_user (id, name) VALUES(1001, 'Admin 1')");
            });
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
            Assert.IsFalse(result.Success);
            Assert.AreEqual(error.Type, result.Type.ToString());
            Assert.AreEqual(error.Title, result.Title);
        }

        [Test]
        public async Task QueryAsResult_NoData()
        {
            // Arrange
            var sql = "SELECT 1 AS success";
            var command = new CommandDefinition(sql);

            // Act
            var result = await repo.QueryAsResultAsync(command);

            // Assert
            Assert.IsTrue(result.Success);
        }

        [Test]
        public async Task ReadToStreamAsync_Test()
        {
            // Arrange
            var sql = "SELECT json_group_array(json_object('id', id, 'name', name)) AS json_result FROM (SELECT id, name FROM e_user WHERE id = 1001)";
            var command = new CommandDefinition(sql);
            using var stream = new MemoryStream();

            // Act
            var result = await repo.ReadToStreamAsync(command, stream);
            var json = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(json.Contains("Admin 1"));
        }
    }
}
