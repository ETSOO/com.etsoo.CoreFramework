using com.etsoo.Database;
using com.etsoo.Utils;
using Dapper;
using NUnit.Framework;
using System.Data;

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
            db = new SqlServerDatabase("Server=(local);User ID=test;Password=test;Enlist=false;TrustServerCertificate=true", true);
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
        public async Task ExecuteAsync_ReturnAndOutputTest()
        {
            // Arrange
            using var connection = db.NewConnection();

            var value = 12;
            var parameters = new DbParameters();
            parameters.Add("Value", value);

            // Return value parameter
            const string name = "ReturnValue";
            const string output = "OutputValue";
            parameters.Add(name, dbType: DbType.Byte, direction: ParameterDirection.ReturnValue);
            parameters.Add(output, dbType: DbType.Int16, direction: ParameterDirection.Output);

            parameters.ClearNulls();

            await connection.ExecuteAsync("ep_user_return", parameters, commandType: CommandType.StoredProcedure);

            var result = parameters.Get<int>(name);
            Assert.AreEqual(value, result);

            var outputValue = parameters.Get<short>(output);
            Assert.AreEqual(outputValue, 2 * result);
        }

        [Test]
        public async Task ExecuteReader_Test()
        {
            // Arrange
            using var connection = db.NewConnection();

            var sql = "SELECT 1001 AS id, 'Admin 1' AS name, '1\n2\n3' AS friends";

            // Act
            using var reader = await connection.ExecuteReaderAsync(sql);

            var users = await TestUserModule.CreateListAsync(reader);

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

        [Test]
        public async Task ExecuteScalarAsync_DynamicTest()
        {
            // Arrange
            using var connection = db.NewConnection();

            // Performance lost because of reflection needs to be considered
            var parameters = new DbParameters(new { Id = 1001, Prefix = "Dynamic" });

            // Act
            var result = await connection.ExecuteScalarAsync("SELECT @Prefix + Name FROM [User] WHERE Id = @Id", parameters);

            // Assert
            Assert.AreEqual("DynamicAdmin 1", result);
        }

        /// <summary>
        /// Async test executing SQL Command to return TextReader of first row first column value, used to read huge text data like json/xml
        /// 异步测试执行SQL命令，返回第一行第一列的TextReader值，用于读取大文本字段，比如返回的JSON/XML数据
        /// </summary>
        [Test]
        public async Task ExecuteToStreamAsync_Test()
        {
            // Arrange
            using var stream = SharedUtils.GetStream();
            using var connection = db.NewConnection();

            // Act
            // {"name":"Admin 1"}
            await connection.QueryToStreamAsync(new("SELECT TOP 1 Name FROM [User] WHERE Id = 1001 FOR JSON PATH, WITHOUT_ARRAY_WRAPPER"), stream);

            // Assert
            Assert.IsTrue(stream.Length == "{\"Name\":\"Admin 1\"}".Length);
        }
    }
}