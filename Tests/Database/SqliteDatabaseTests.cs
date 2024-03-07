using com.etsoo.Database;
using com.etsoo.Utils;
using Dapper;
using NUnit.Framework;

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

            // Create table 'User' when not exists
            await connection.ExecuteScalarAsync("CREATE TABLE IF NOT EXISTS User (Id int, Name nvarchar(128), Status int)");
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
            var paras = new Dictionary<string, dynamic>() { { "user1", 1001 }, { "name1", "Admin 1" }, { "user2", 1002 }, { "name2", "Admin 2" } };

            // Act
            // Insert rows
            var sql = "INSERT OR IGNORE INTO User (Id, Name) VALUES(@user1, @name1), (@user2, @name2)";

            // Result
            Assert.DoesNotThrowAsync(async () => await connection.ExecuteAsync(sql, paras));
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
            await connection.ExecuteAsync("INSERT OR IGNORE INTO User (Id, Name) VALUES(1003, 'Admin 3')");

            // Act
            var result = await connection.ExecuteScalarAsync("SELECT Name FROM User WHERE Id = 1003");

            // Assert
            Assert.That(result, Is.EqualTo("Admin 3"));
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

            // Create table and add a record for test
            await connection.ExecuteAsync("INSERT OR IGNORE INTO User (id, name) VALUES(1001, 'Admin 1')");

            // Act
            // "Admin 1"
            await connection.QueryToStreamAsync(new("SELECT Name FROM User WHERE id = 1001 LIMIT 1"), stream);

            // Assert
            Assert.That(stream.Length, Is.EqualTo("Admin 1".Length));
        }

        [Test]
        public void ListToParameterTests()
        {
            var json = db.ListToParameter(new int[] { 1, 3, 5 });
            if (json is DbString ds)
            {
                Assert.That(ds.Value, Is.EqualTo("[1,3,5]"));
            }
        }

        [Test]
        public void SqliteUtilsToJsonBool()
        {
            var command = "@a = 1".ToJsonBool();
            Assert.That(command, Is.EqualTo("json(IIF(@a = 1, 'true', 'false'))"));
        }

        [Test]
        public void SqliteUtilsToJsonCommandTest()
        {
            var command = SqliteUtils.ToJsonCommand("u.id, name, (SELECT IIF(t.deleted, 1, 0) FROM tabs AS t WHERE t.author = u.id) AS subValue, (u.wage * 12) AS yearWage");
            Assert.That(command, Is.EqualTo("json_group_array(json_object('id', u.id, 'name', name, 'subValue', (SELECT IIF(t.deleted, 1, 0) FROM tabs AS t WHERE t.author = u.id), 'yearWage', (u.wage * 12)))"));
        }

        [Test]
        public void SqliteUtilsToJsonCommandWithoutArrayTest()
        {
            var command = SqliteUtils.ToJsonCommand("u.id, name, (SELECT IIF(t.deleted, 1, 0) FROM tabs AS t WHERE t.author = u.id) AS subValue, (u.wage * 12) AS yearWage", true);
            Assert.That(command, Is.EqualTo("json_object('id', u.id, 'name', name, 'subValue', (SELECT IIF(t.deleted, 1, 0) FROM tabs AS t WHERE t.author = u.id), 'yearWage', (u.wage * 12))"));
        }

        [Test]
        public void SqliteUtilsToJsonCommandWithFunctionTest()
        {
            var command = SqliteUtils.ToJsonCommand("u.id, name, json(jsonData) AS jsonData, (u.wage * 12) AS yearWage", true);
            Assert.That(command, Is.EqualTo("json_object('id', u.id, 'name', name, 'jsonData', json(jsonData), 'yearWage', (u.wage * 12))"));
        }
    }
}