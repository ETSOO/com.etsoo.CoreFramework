using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Repositories;
using com.etsoo.Utils.Database;
using Microsoft.Data.SqlClient;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Tests.Repositories
{
    /// <summary>
    /// Int id entity repository tests
    /// </summary>
    [TestFixture]
    public class IntEntityRepositoryTests
    {
        /// <summary>
        /// Int id entity repository
        /// </summary>
        private class IntEntityRepository : EntityRepository<SqlConnection, int>
        {
            public IntEntityRepository(ICoreApplication<SqlConnection> app) : base(app, "user") { }

            /// <summary>
            /// Get command name, concat with AppId and Flag, normally is stored procedure name, pay attention to SQL injection
            /// 获取命令名称，附加程序编号和实体标识，一般是存储过程名称，需要防止注入攻击
            /// </summary>
            /// <param name="part1">Part 1</param>
            /// <param name="part2">Part 2</param>
            /// <returns>Command name</returns>
            public new string GetCommandName(ReadOnlySpan<char> part1, ReadOnlySpan<char> part2)
            {
                return base.GetCommandName(part1, part2);
            }
        }

        readonly IntEntityRepository repo;

        public IntEntityRepositoryTests()
        {
            var db = new SqlServerDatabase("Server=(local);User ID=test;Password=test;Enlist=false", true);

            var config = new AppConfiguration("test");
            var app = new CoreApplication<SqlConnection>(config, db);

            repo = new IntEntityRepository(app);
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetCommandName_Test()
        {
            // Arrange & Act
            var command = repo.GetCommandName("read", DataFormat.JSON.ToString().ToLower());

            // Assert
            Assert.AreEqual("ep_user_read_json", command);
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
            var result = await repo.CreateAsync(user);

            // Assert
            if(result.Success)
            {
                Assert.AreEqual(1021, result.DataAsIdModal().Id);
            }
        }

        [Test]
        public async Task DeleteAsync_Test()
        {
            // Act
            var result = await repo.DeleteAsync(1001);

            // Assert
            Assert.IsTrue(result.Success);
        }
    }
}
