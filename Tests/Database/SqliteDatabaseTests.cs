using com.etsoo.CoreFramework.Database;
using com.etsoo.Utils.Database;
using Dapper;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Tests.Utils
{
    [TestFixture]
    public class SqliteDatabaseTests
    {
        readonly SqliteDatabase db;

        public SqliteDatabaseTests()
        {
            // Arrange
            // Create the dabase
            // Once failed ConnectionString: Data Source = :memory:; Database will reset to empty everytime the Connection.Open() execution.
            db = new SqliteDatabase("Data Source = etsoo.db;");
        }

        /// <summary>
        /// Setup
        /// 初始化
        /// </summary>
        [SetUp]
        public async Task Setup()
        {
            // Connection to the DB
            using var connection = db.NewConnection();

            // Create table e_user when not exists
            await connection.ExecuteScalarAsync("CREATE TABLE IF NOT EXISTS e_user (id int, name nvarchar(128), status int)");
        }

        /// <summary>
        /// Constructor test
        /// 构造函数测试
        /// </summary>
        [Test]
        public void SqliteDatabase_Constructor_Test()
        {
            // Act & Asset
            Assert.DoesNotThrow(() =>
            {
                using var connection = db.NewConnection();
                connection.Open();
                connection.Close();
            });
        }

        /// <summary>
        /// Async test Creating EF Database Context
        /// 异步测试创建EF数据库上下文
        /// </summary>
        [Test]
        public async Task CreateContextAsync_Test()
        {
            // Arrange
            // Create EF database context
            using var context = db.NewDbContext<TestUserModule>();

            // User 1001
            var user1001 = new TestUserModule { Id = 1001, Name = "Admin 1" };
            var user1002 = new TestUserModule { Id = 1002, Name = "Admin 2" };

            // Act
            // Try to add two records
            if (await context.Entities!.FindAsync(user1001.Id) == null)
                await context.Entities.AddAsync(user1001);

            if (await context.Entities.FindAsync(user1002.Id) == null)
                await context.Entities.AddAsync(user1002);
        }

        /// <summary>
        /// Async Test Executing SQL Command
        /// 异步测试执行SQL 命令
        /// </summary>
        [Test]
        public async Task ExecuteAsync_Test()
        {
            // Arrange
            using var connection = db.NewConnection();

            // Parameters
            var paras = new Dictionary<string, dynamic>() { { "user1", 1001 }, { "name1", "Admin 1" }, { "user2", 1002 }, { "name2", "Admin 2" } };

            // Act
            // Insert rows
            var sql = "INSERT OR IGNORE INTO e_user (id, name) VALUES(@user1, @name1), (@user2, @name2)";

            // Result
            await connection.ExecuteAsync(sql, paras);
        }

        /// <summary>
        /// Async test Executing SQL Command to return first row first column value
        /// 测试异步执行SQL命令，返回第一行第一列的值
        /// </summary>
        [Test]
        public async Task ExecuteScalarAsync_Test()
        {
            // Arrange
            using var connection = db.NewConnection();

            // Create table and add a record for test
            await connection.ExecuteAsync("INSERT OR IGNORE INTO e_user (id, name) VALUES(1003, 'Admin 3')");

            // Act
            var result = await connection.ExecuteScalarAsync("SELECT name FROM e_user WHERE id = 1003");

            // Assert
            Assert.AreEqual("Admin 3", result);
        }

        /// <summary>
        /// Async test executing SQL Command to return TextReader of first row first column value, used to read huge text data like json/xml
        /// 异步测试执行SQL命令，返回第一行第一列的TextReader值，用于读取大文本字段，比如返回的JSON/XML数据
        /// </summary>
        [Test]
        public async Task ExecuteToStreamAsync_Test()
        {
            // Arrange
            using var stream = new MemoryStream();
            using var connection = db.NewConnection();

            // Create table and add a record for test
            await connection.ExecuteAsync("INSERT OR IGNORE INTO e_user (id, name) VALUES(1001, 'Admin 1')");

            // Act
            // "Admin 1"
            await connection.QueryToStreamAsync(new("SELECT name FROM e_user WHERE id = 1001 LIMIT 1"), stream);

            // Assert
            Assert.IsTrue(stream.Length == "Admin 1".Length);
        }
    }
}