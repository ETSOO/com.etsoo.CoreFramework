using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Business;
using com.etsoo.CoreFramework.Services;
using com.etsoo.CoreFramework.User;
using com.etsoo.Database;
using com.etsoo.Utils;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using System.Buffers;
using System.Data;
using System.Runtime.Versioning;
using System.Text;

namespace Tests.Services
{
    /// <summary>
    /// Int id entity service
    /// </summary>
    internal class IntEntityService : EntityServiceBase<AppConfiguration, SqlConnection, ICoreApplication<AppConfiguration, SqlConnection>, ICurrentUser, int>
    {
        public IntEntityService(ICoreApplication<AppConfiguration, SqlConnection> app, string flag, ILogger logger) : base(app, null!, flag, logger)
        {
        }


        /// <summary>
        /// Get command name, concat with AppId and Flag, normally is stored procedure name, pay attention to SQL injection
        /// 获取命令名称，附加程序编号和实体标识，一般是存储过程名称，需要防止注入攻击
        /// </summary>
        /// <param name="part1">Part 1</param>
        /// <param name="part2">Part 2</param>
        /// <returns>Command name</returns>
        public new string GetCommandName(params string[] parts)
        {
            return base.GetCommandName(parts);
        }

        public override void AddSystemParameters(IDbParameters parameters, bool userRequired = true)
        {
            // No additional parameters will be passed
        }

        public IDbParameters CreateStudentParameters(Student student)
        {
            var parameters = FormatParameters(student);
            return parameters;
        }

        public IDbParameters QueryStudentParameters(StudentQuery student)
        {
            var parameters = FormatParameters(student);
            return parameters;
        }
    }

    /// <summary>
    /// Int id entity repository tests
    /// </summary>
    [TestClass]
    [SupportedOSPlatform("windows")]
    public class IntEntityServiceTests
    {
        readonly CoreApplication<AppConfiguration, SqlConnection> app;
        readonly IntEntityService service;

        public IntEntityServiceTests()
        {
            var db = new SqlServerDatabase("Server=(local);User ID=test;Password=test;Enlist=false;TrustServerCertificate=true");

            var config = new AppConfiguration { Name = "test", PrivateKey = "@s$a!" };
            app = new CoreApplication<AppConfiguration, SqlConnection>(config, db);

            service = new IntEntityService(app, "user", new EventLogLoggerProvider().CreateLogger("SmartERPTests"));

            using var conn = db.NewConnection();
            conn.Execute("IF NOT EXISTS (SELECT * FROM [User] WHERE Id = 1001) BEGIN INSERT INTO [User] (Id, Name) VALUES (1001, 'Admin 1') END", commandType: CommandType.Text);
        }

        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod]
        public void GetCommandName_Test()
        {
            // Arrange & Act
            var command = service.GetCommandName("read", DataFormat.Json.Name.ToLower());

            // Assert
            Assert.AreEqual("ep_user_read_json", command);
        }

        [TestMethod]
        public void UpdateUserSetupFailedTest()
        {
            var model = new UpdateUser
            {
                Id = 1001,
                Name = "Etsoo",
                ChangedFields = ["Name"]
            };

            Assert.Throws<Exception>(() => model.AsParameters(app));
        }

        [TestMethod]
        public async Task CreateAsync_Test()
        {
            // Arrange
            var user = new TestUserModule
            {
                Id = 1021,
                Name = "Admin 1"
            };

            // Act
            var result = await service.CreateAsync(user);

            // Assert
            if (result.Ok)
            {
                Assert.AreEqual(1021, result.Data["Id"]);
            }
        }

        [TestMethod]
        public async Task ReadAsync_Test()
        {
            var data = await service.ReadAsync<TestUserModule>(-1);
            Assert.IsNull(data);

            data = await service.ReadDirectAsync<TestUserModule>(-1);
            Assert.IsNull(data);
        }

        [TestMethod]
        public async Task UpdateAsync_Test()
        {
            // Arrange
            var user = new UserUpdateModule
            {
                Id = 1021,
                Name = "Admin 21",
                ChangedFields = ["Name"]
            };

            var parameters = new Dictionary<string, object> { ["Flag"] = true };

            // Act
            var (result, data) = await service.QuickUpdateAsync(user, new(["Name AS Id=IIF(@Flag = 1, @Name + ' Flaged', @Name)", "id"]), "Id = @Id", parameters);

            // Assert
            Assert.IsTrue(result.Ok);
            Assert.IsNotNull(data);
            Assert.AreEqual(1, data?.RowsAffected);
        }

        [TestMethod]
        public async Task DeleteAsync_Test()
        {
            // Act
            var result = await service.DeleteAsync(1001);

            // Assert
            Assert.IsTrue(result.Ok);
        }

        [TestMethod]
        public async Task SortAsync_Test()
        {
            // Arrange
            var data = new Dictionary<int, short> { [1001] = 0, [1002] = 1 };

            // Act
            var result = await service.SortAsync(data, cancellationToken: TestContext.CancellationToken);

            // Assert
            Assert.IsLessThanOrEqualTo(2, result);
        }

        [TestMethod]
        public async Task ReportAsyc_Test()
        {
            using var stream = SharedUtils.GetStream();
            var ex = await Assert.ThrowsAsync<SqlException>(async () => await service.ReportAsync(stream, "default"));
            Assert.AreEqual("ep_user_report_for_default_as_json", ex?.Procedure);
        }

        [TestMethod]
        public async Task ListAsyn_ModelTest()
        {
            // Arrange
            var parameters = new DbParameters();
            var command = new CommandDefinition("ep_user_list_as_json", parameters, commandType: CommandType.StoredProcedure);

            using var stream = SharedUtils.GetStream();
            var result = await app.DB.WithConnection((connection) =>
            {
                return connection.QueryToStreamAsync(command, stream);
            });
            Assert.IsTrue(result);

            stream.Position = 0;
            var content = SharedUtils.StreamToString(stream);
            Assert.Contains("Admin 2", content);
        }

        [TestMethod]
        public void CreateStudentParametersTest()
        {
            var student = new Student
            {
                Name = "Student Name",
                JsonBooks = [new() { Name = "Json Book 1", Price = 3.2M }, new() { Name = "Json Book 2", Price = 3.6M }],
                Books = [new() { Name = "Book 1", Price = 4.2M }, new() { Name = "Book 2", Price = 8.3M }]
            };
            var parameters = service.CreateStudentParameters(student);
            Assert.IsTrue(parameters.ParameterNames.Contains("JsonBooks"));
            Assert.IsTrue(parameters.ParameterNames.Contains("Books"));
        }

        [TestMethod]
        public void CreateStudentQueryParametersTest()
        {
            var student = new StudentQuery
            {
                Name = "Student Name",
                QueryPaging = new QueryPagingData { BatchSize = 10, CurrentPage = 2 }
            };

            var parameters = service.QueryStudentParameters(student);
            Assert.IsTrue(parameters.ParameterNames.Contains("Name"));
            Assert.IsTrue(parameters.ParameterNames.Contains("BatchSize"));
            Assert.IsTrue(parameters.ParameterNames.Contains("CurrentPage"));
        }

        [TestMethod]
        public async Task SqlModelTests()
        {
            await service.SqlDeleteAsync([1113]);

            var user = new SqlUserInsert { Id = 1113, Name = "Admin 3", Status = EntityStatus.Approved };

            var id = await service.SqlInsertAsync<SqlUserInsert, int>(user);
            Assert.AreEqual(1113, id);

            var update = new SqlUserUpdate { Id = 1113, Name = "Admin 3 Updated", ChangedFields = ["Name"] };
            var updateResult = await service.SqlUpdateAsync(update);
            Assert.IsTrue(updateResult.Ok);

            var select = new SqlUserSelect { Id = 1113, QueryPaging = new QueryPagingData { BatchSize = 2 } };
            var selectData = (await service.SqlSelectAsync<SqlUserSelect, UserData>(select)).FirstOrDefault();
            Assert.IsNotNull(selectData);
            Assert.AreEqual("Admin 3 Updated", selectData.Name);

            var query = new UserQuery { Id = 1113 };
            var selectResult = (await service.SqlSelectAsync(query.Default.UserData)).FirstOrDefault();
            Assert.AreEqual("Admin 3 Updated", selectResult?.Name);

            var writer = new ArrayBufferWriter<byte>();
            await service.SqlSelectJsonAsync<SqlUserSelect, UserData>(select, writer);
            var json = Encoding.UTF8.GetString(writer.WrittenSpan);
            Assert.AreEqual("[{\"id\":1113,\"name\":\"Admin 3 Updated\",\"status\":100}]", json);

            writer.Clear();
            await service.SqlSelectJsonAsync(query.Default.UserData, writer);
            json = Encoding.UTF8.GetString(writer.WrittenSpan);
            Assert.AreEqual("[{\"id\":1113,\"name\":\"Admin 3 Updated\",\"status\":100}]", json);

            var deleteResult = await service.SqlDeleteAsync([1113], "User");
            Assert.IsTrue(deleteResult.Ok);
        }

        public TestContext TestContext { get; set; }
    }
}
