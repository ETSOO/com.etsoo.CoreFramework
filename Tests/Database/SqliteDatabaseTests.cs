using com.etsoo.Database;
using com.etsoo.Utils;
using Dapper;

namespace Tests.Database
{
    [TestClass]
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
        [TestInitialize]
        public void Setup()
        {
            // Connection to the DB
            using var connection = db.NewConnection();

            // Create table 'User' when not exists (synchronously wait)
            connection.ExecuteScalarAsync("CREATE TABLE IF NOT EXISTS User (Id int, Name nvarchar(128), Status int)").GetAwaiter().GetResult();
        }

        /// <summary>
        /// Constructor test
        /// 构造函数测试
        /// </summary>
        [TestMethod]
        public void SqliteDatabase_Constructor_Test()
        {
            // Act & Asset
            using var connection = db.NewConnection();
            connection.OpenAsync().GetAwaiter().GetResult();
            connection.CloseAsync().GetAwaiter().GetResult();
        }

        [TestMethod]
        public void JoinJsonFieldsBoolTest()
        {
            var mapping = new Dictionary<string, string>();
            var result = db.JoinJsonFields(["u.Id", "u.Shared:boolean"], mapping, NamingPolicy.CamelCase, NamingPolicy.CamelCase);
            var json = db.JoinJsonFields(mapping, true);

            // Assert
            Assert.AreEqual("u.\"id\", u.\"shared\"", result);
            Assert.IsTrue(mapping.ContainsKey("shared"));
            Assert.AreEqual("shared:boolean", mapping["shared"]);
            Assert.AreEqual("json_object('id', id, 'shared', json(IIF(shared, 'true', 'false')))", json);
        }

        [TestMethod]
        public void JoinJsonFieldsJsonTest()
        {
            var mapping = new Dictionary<string, string>();
            var result = db.JoinJsonFields(["u.Id", "u.JsonData:json"], mapping, NamingPolicy.CamelCase, NamingPolicy.CamelCase);
            var json = db.JoinJsonFields(mapping, true);

            // Assert
            Assert.AreEqual("u.\"id\", u.\"jsonData\"", result);
            Assert.IsTrue(mapping.ContainsKey("jsonData"));
            Assert.AreEqual("jsonData:json", mapping["jsonData"]);
            Assert.AreEqual("json_object('id', id, 'jsonData', json(jsonData))", json);
        }

        [TestMethod]
        public void JoinJsonFieldsEqualTest()
        {
            var mapping = new Dictionary<string, string>();
            var result = db.JoinJsonFields(["Author", "IIF(u.id = @UserId, TRUE, FALSE):boolean AS isSelf"], mapping, NamingPolicy.CamelCase, NamingPolicy.CamelCase);
            var json = db.JoinJsonFields(mapping, false);

            // Assert
            Assert.AreEqual("\"author\", IIF(u.id = @UserId, TRUE, FALSE) AS \"isSelf\"", result);
            Assert.IsTrue(mapping.ContainsKey("isSelf"));
            Assert.AreEqual("isSelf:boolean", mapping["isSelf"]);
            Assert.AreEqual("json_group_array(json_object('author', author, 'isSelf', json(IIF(isSelf, 'true', 'false'))))", json);
        }

        /// <summary>
        /// Async Test Executing SQL Command
        /// 异步测试执行SQL 命令
        /// </summary>
        [TestMethod]
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
            // Execute and ensure no exception (synchronously wait)
            connection.ExecuteAsync(sql, paras).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Async test Executing SQL Command to return first row first column value
        /// 测试异步执行SQL命令，返回第一行第一列的值
        /// </summary>
        [TestMethod]
        public async Task ExecuteScalarAsync_Test()
        {
            // Arrange
            using var connection = db.NewConnection();

            // Create table and add a record for test
            await connection.ExecuteAsync("INSERT OR IGNORE INTO User (Id, Name) VALUES(1003, 'Admin 3')");

            // Act
            var result = await connection.ExecuteScalarAsync("SELECT Name FROM User WHERE Id = 1003");

            // Assert
            Assert.AreEqual("Admin 3", result);
        }

        /// <summary>
        /// Async test executing SQL Command to return TextReader of first row first column value, used to read huge text data like json/xml
        /// 异步测试执行SQL命令，返回第一行第一列的TextReader值，用于读取大文本字段，比如返回的JSON/XML数据
        /// </summary>
        [TestMethod]
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
            Assert.AreEqual("Admin 1".Length, stream.Length);
        }

        [TestMethod]
        public void ListToParameterTests()
        {
            var json = db.ListToParameter(new int[] { 1, 3, 5 });
            if (json is DbString ds)
            {
                Assert.AreEqual("[1,3,5]", ds.Value);
            }
        }

        [TestMethod]
        public void SqliteUtilsToJsonBool()
        {
            var command = "@a = 1".ToJsonBool();
            Assert.AreEqual("json(IIF(@a = 1, 'true', 'false'))", command);
        }

        [TestMethod]
        public void SqliteUtilsToJsonCommandTest()
        {
            var command = SqliteUtils.ToJsonCommand("u.id, name, (SELECT IIF(t.deleted, 1, 0) FROM tabs AS t WHERE t.author = u.id) AS subValue, (u.wage * 12) AS yearWage");
            Assert.AreEqual("json_group_array(json_object('id', u.id, 'name', name, 'subValue', (SELECT IIF(t.deleted, 1, 0) FROM tabs AS t WHERE t.author = u.id), 'yearWage', (u.wage * 12)))", command);
        }

        [TestMethod]
        public void SqliteUtilsToJsonCommandWithoutArrayTest()
        {
            var command = SqliteUtils.ToJsonCommand("u.id, name, (SELECT IIF(t.deleted, 1, 0) FROM tabs AS t WHERE t.author = u.id) AS subValue, (u.wage * 12) AS yearWage", true);
            Assert.AreEqual("json_object('id', u.id, 'name', name, 'subValue', (SELECT IIF(t.deleted, 1, 0) FROM tabs AS t WHERE t.author = u.id), 'yearWage', (u.wage * 12))", command);
        }

        [TestMethod]
        public void SqliteUtilsToJsonCommandWithFunctionTest()
        {
            var command = SqliteUtils.ToJsonCommand("u.id, name, json(jsonData) AS jsonData, (u.wage * 12) AS yearWage", true);
            Assert.AreEqual("json_object('id', u.id, 'name', name, 'jsonData', json(jsonData), 'yearWage', (u.wage * 12))", command);
        }
    }
}