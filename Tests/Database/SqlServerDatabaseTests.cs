using com.etsoo.Utils.Database;
using Dapper;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Tests.Utils
{
    [TestFixture]
    public class SqlServerDatabaseTests
    {
        readonly SqlServerDatabase db;

        public SqlServerDatabaseTests()
        {
            // Arrange
            // Create the dabase
            db = new SqlServerDatabase("Server=(local);User ID=test;Password=test;Enlist=false", true);
        }

        /// <summary>
        /// Setup
        /// 初始化
        /// </summary>
        [SetUp]
        public void Setup()
        {
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
            var paras = new Dictionary<string, object>() { { "user1", 1001 }, { "name1", "Admin 1" }, { "user2", 1002 }, { "name2", "Admin 2" } };

            // Act
            // Insert rows
            var sql = "IF NOT EXISTS (SELECT * FROM [User] WHERE Id = @user1) INSERT INTO [User] (Id, Name) VALUES(@user1, @name1); IF NOT EXISTS (SELECT * FROM [User] WHERE Id = @user2) INSERT INTO [User] (Id, Name) VALUES(@user2, @name2)";

            // Result
            await connection.ExecuteAsync(sql, paras);
        }

        [Test]
        public async Task ExecuteReader_Test()
        {
            // Arrange
            using var connection = db.NewConnection();

            var sql = "SELECT 1001 AS id, 'Admin 1' AS name, '1\n2\n3' AS friends";

            // Act
            using var reader = await connection.ExecuteReaderAsync(sql);

            var users = (await TestUserModule.CreateAsync(reader)).AsList();

            // Assert
            Assert.IsTrue(users.Count == 1);
            Assert.IsTrue(users[0].Id == 1001);
            Assert.IsTrue(users[0].Friends?.Length == 3);
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

            // Act
            var result = await connection.ExecuteScalarAsync("SELECT Name FROM [User] WHERE Id = 1001");

            // Assert
            Assert.AreEqual("Admin 1", result);
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

            // Act
            // {"name":"Admin 1"}
            await connection.QueryToStreamAsync(new("SELECT TOP 1 Name FROM [User] WHERE Id = 1001 FOR JSON PATH, WITHOUT_ARRAY_WRAPPER"), stream);

            // Assert
            Assert.IsTrue(stream.Length == "{\"Name\":\"Admin 1\"}".Length);
        }
    }
}