﻿using com.etsoo.Database;
using com.etsoo.Utils;
using Dapper;
using NUnit.Framework;
using System.Data;

namespace Tests.Database
{
    [TestFixture]
    public class SqlServerDatabaseTests
    {
        readonly SqlServerDatabase db;

        public SqlServerDatabaseTests()
        {
            // Arrange
            // Create the dabase
            db = new SqlServerDatabase("Server=(local);User ID=test;Password=test;Enlist=false;TrustServerCertificate=true");
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
            Assert.DoesNotThrowAsync(async () =>
            {
                await using var connection = db.NewConnection();
                await connection.OpenAsync();
                await connection.CloseAsync();
            });
        }

        [Test]
        public void JoinJsonFieldsBoolTest()
        {
            var mapping = new Dictionary<string, string>();
            var result = db.JoinJsonFields(["u.Id", "u.Shared:boolean"], mapping, null, NamingPolicy.CamelCase);
            var json = db.JoinJsonFields(mapping, true);
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo("u.[Id], u.[Shared]"));
                Assert.That(mapping, Does.ContainKey("shared").WithValue("Shared"));
                Assert.That(json, Is.EqualTo("Id AS [id], Shared AS [shared]"));
            });
        }

        [Test]
        public void JoinJsonFieldsJsonTest()
        {
            var mapping = new Dictionary<string, string>();
            var result = db.JoinJsonFields(["u.Id", "u.JsonData:json"], mapping, null, NamingPolicy.CamelCase);
            var json = db.JoinJsonFields(mapping, true);
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo("u.[Id], u.[JsonData]"));
                Assert.That(mapping, Does.ContainKey("jsonData").WithValue("JsonData:json"));
                Assert.That(json, Is.EqualTo("Id AS [id], JSON_QUERY(JsonData) AS [jsonData]"));
            });
        }

        [Test]
        public void JoinJsonFieldsEqualTest()
        {
            var mapping = new Dictionary<string, string>();
            var result = db.JoinJsonFields(["Author", "IIF(u.id = @UserId, TRUE, FALSE):boolean AS isSelf"], mapping, null, NamingPolicy.CamelCase);
            var json = db.JoinJsonFields(mapping, false);
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo("[Author], CAST(IIF(u.id = @UserId, 1, 0) AS bit) AS [isSelf]"));
                Assert.That(mapping, Does.ContainKey("isSelf").WithValue("isSelf"));
                Assert.That(json, Is.EqualTo("Author AS [author], isSelf"));
            });
        }

        /// <summary>
        /// Async Test Executing SQL Command
        /// 异步测试执行SQL 命令
        /// </summary>
        [Test]
        public void ExecuteAsync_Test()
        {
            // Arrange
            using var connection = db.NewConnection();

            // Parameters
            var paras = new Dictionary<string, object>() { { "user1", 1001 }, { "name1", "Admin 1" }, { "user2", 1002 }, { "name2", "Admin 2" } };

            // Act
            // Insert rows
            var sql = "IF NOT EXISTS (SELECT * FROM [User] WHERE Id = @user1) INSERT INTO [User] (Id, Name) VALUES(@user1, @name1); IF NOT EXISTS (SELECT * FROM [User] WHERE Id = @user2) INSERT INTO [User] (Id, Name) VALUES(@user2, @name2)";

            // Result
            Assert.DoesNotThrowAsync(async () =>
            {
                await connection.ExecuteAsync(sql, paras);
            });
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
            Assert.That(result, Is.EqualTo(value));

            var outputValue = parameters.Get<short>(output);
            Assert.That(outputValue, Is.EqualTo(2 * result));
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
            Assert.That(users.Count, Is.EqualTo(1));
            Assert.That(users[0].Id, Is.EqualTo(1001));
            Assert.That(users[0].Friends?.Length, Is.EqualTo(3));
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
            Assert.That(result, Is.EqualTo("Admin 1"));
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
            Assert.That(result, Is.EqualTo("DynamicAdmin 1"));
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
            Assert.That(stream.Length, Is.EqualTo("{\"Name\":\"Admin 1\"}".Length));
        }

        [Test]
        public async Task ExecuteToStreamAsyncWithEmpty_Test()
        {
            // Arrange
            using var stream = SharedUtils.GetStream();
            using var connection = db.NewConnection();

            // Act
            // {"name":"Admin 1"}
            var result = await connection.QueryToStreamAsync(new("SELECT TOP 1 Name FROM [User] WHERE Id = 0 FOR JSON PATH"), stream);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}