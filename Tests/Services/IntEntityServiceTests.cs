using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Models;
using com.etsoo.CoreFramework.Services;
using com.etsoo.CoreFramework.User;
using com.etsoo.Database;
using com.etsoo.Utils;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using NUnit.Framework;
using System.Data;

namespace Tests.Services
{
    /// <summary>
    /// Int id entity service
    /// </summary>
    internal class IntEntityService : EntityServiceBase<AppConfiguration, SqlConnection, ICoreApplication<AppConfiguration, SqlConnection>, ICurrentUser, int>
    {
        public IntEntityService(ICoreApplication<AppConfiguration, SqlConnection> app, string flag, ILogger logger) : base(app, null, flag, logger, false)
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

        public override void AddSystemParameters(IDbParameters parameters)
        {
            // No additional parameters will be passed
        }

        public IDbParameters CreateStudentParameters(Student student)
        {
            var parameters = FormatParameters(student);
            return parameters;
        }
    }

    /// <summary>
    /// Int id entity repository tests
    /// </summary>
    [TestFixture]
    public class IntEntityServiceTests
    {
        readonly CoreApplication<AppConfiguration, SqlConnection> app;
        readonly IntEntityService service;

        public IntEntityServiceTests()
        {
            var db = new SqlServerDatabase("Server=(local);User ID=test;Password=test;Enlist=false;TrustServerCertificate=true");

            var config = new AppConfiguration { Name = "test" };
            app = new CoreApplication<AppConfiguration, SqlConnection>(config, db);

            service = new IntEntityService(app, "user", new EventLogLoggerProvider().CreateLogger("SmartERPTests"));

            using var conn = db.NewConnection();
            conn.Execute("IF NOT EXISTS (SELECT * FROM [User] WHERE Id = 1001) BEGIN INSERT INTO [User] (Id, Name) VALUES (1001, 'Admin 1') END", commandType: CommandType.Text);
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetCommandName_Test()
        {
            // Arrange & Act
            var command = service.GetCommandName("read", DataFormat.Json.Name.ToLower());

            // Assert
            Assert.AreEqual("ep_user_read_json", command);
        }

        [Test]
        public void UpdateUserSetupFailedTest()
        {
            var model = new UpdateUser
            {
                Id = 1001,
                Name = "Etsoo",
                ChangedFields = new string[] { "Name" }
            };

            Assert.Throws<Exception>(() => model.AsParameters(app));
        }

        [Test]
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

        [Test]
        public async Task ReadAsync_Test()
        {
            var data = await service.ReadAsync<TestUserModule>(-1);
            Assert.IsNull(data);

            data = await service.ReadDirectAsync<TestUserModule>(-1);
            Assert.IsNull(data);
        }

        [Test]
        public async Task UpdateAsync_Test()
        {
            // Arrange
            var user = new UserUpdateModule
            {
                Id = 1021,
                Name = "Admin 21",
                ChangedFields = new[] { "Name" }
            };

            var parameters = new Dictionary<string, object> { ["Flag"] = true };

            // Act
            var (result, data) = await service.QuickUpdateAsync(user, new(new[] { "Name AS Id=IIF(@Flag = 1, @Name + ' Flaged', @Name)", "id" }), "Id = @Id", parameters);

            // Assert
            Assert.IsTrue(result.Ok);
            Assert.IsNotNull(data);
            Assert.AreEqual(1, data?.RowsAffected);
        }

        [Test]
        public async Task DeleteAsync_Test()
        {
            // Act
            var result = await service.DeleteAsync(1001);

            // Assert
            Assert.IsTrue(result.Ok);
        }

        [Test]
        public async Task SortAsync_Test()
        {
            // Arrange
            var data = new Dictionary<int, short> { [1001] = 0, [1002] = 1 };

            // Act
            var result = await service.SortAsync(data);

            // Assert
            Assert.LessOrEqual(1, result);
        }

        [Test]
        public void ReportAsyc_Test()
        {
            using var stream = SharedUtils.GetStream();
            var result = Assert.ThrowsAsync<SqlException>(async () =>
            {
                await service.ReportAsync(stream, "default");
            });

            Assert.AreEqual("ep_user_report_for_default_as_json", result?.Procedure);
        }

        [Test]
        public async Task ListAsyn_ModelTest()
        {
            // Arrange
            var rq = new TiplistRQ<int>() { Items = 2 };
            var parameters = rq.AsParameters(app);
            parameters.ClearNulls();
            var command = new CommandDefinition("ep_user_list_as_json", parameters, commandType: CommandType.StoredProcedure);

            using var stream = SharedUtils.GetStream();
            var result = await app.DB.WithConnection((connection) =>
            {
                return connection.QueryToStreamAsync(command, stream);
            });

            Assert.IsTrue(result);

            stream.Position = 0;
            var content = SharedUtils.StreamToString(stream);
            Assert.IsTrue(content.Contains("Admin 2"));
        }

        [Test]
        public void CreateStudentParametersTest()
        {
            var student = new Student
            {
                Name = "Student Name",
                JsonBooks = new List<Book> { new Book { Name = "Json Book 1", Price = 3.2M }, new Book { Name = "Json Book 2", Price = 3.6M } },
                Books = new List<Book> { new Book { Name = "Book 1", Price = 4.2M }, new Book { Name = "Book 2", Price = 8.3M } }
            };
            var parameters = service.CreateStudentParameters(student);
            Assert.IsTrue(parameters.ParameterNames.Contains("JsonBooks"));
            Assert.IsTrue(parameters.ParameterNames.Contains("Books"));
        }
    }
}
