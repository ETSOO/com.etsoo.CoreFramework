﻿using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Repositories;
using com.etsoo.Database;
using Dapper;
using Microsoft.Data.SqlClient;
using NUnit.Framework;

namespace Tests.Repositories
{
    /// <summary>
    /// Int id entity repository
    /// </summary>
    internal class IntEntityRepository : EntityRepo<SqlConnection, int>
    {
        public IntEntityRepository(ICoreApplication<SqlConnection> app, string flag) : base(app, flag, null) { }

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

        public override void AddSystemParameters(DynamicParameters parameters)
        {
            // No additional parameters will be passed
        }
    }

    /// <summary>
    /// Int id entity repository tests
    /// </summary>
    [TestFixture]
    public class IntEntityRepositoryTests
    {
        readonly IntEntityRepository repo;

        public IntEntityRepositoryTests()
        {
            var db = new SqlServerDatabase("Server=(local);User ID=test;Password=test;Enlist=false;TrustServerCertificate=true", true);

            var config = new AppConfiguration("test");
            var app = new CoreApplication<SqlConnection>(config, db);

            repo = new IntEntityRepository(app, "user");
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetCommandName_Test()
        {
            // Arrange & Act
            var command = repo.GetCommandName("read", DataFormat.Json.Name.ToLower());

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
            if (result.Ok)
            {
                Assert.AreEqual(1021, result.Data["Id"]);
            }
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

            // Act
            var (result, data) = await repo.QuickUpdateAsync(user, new(new[] { "Name=IIF(@Id = 1001, '', @Name)" }));

            // Assert
            Assert.IsTrue(result.Ok);
            Assert.IsNotNull(data);
            Assert.AreEqual(1, data?.RowsAffected);
        }

        [Test]
        public async Task DeleteAsync_Test()
        {
            // Act
            var result = await repo.DeleteAsync(1001);

            // Assert
            Assert.IsTrue(result.Ok);
        }

        [Test]
        public async Task SortAsync_Test()
        {
            // Arrange
            var data = new Dictionary<int, short> { [1001] = 0, [1002] = 1 };

            // Act
            var result = await repo.SortAsync(data);

            // Assert
            Assert.LessOrEqual(1, result);
        }
    }
}
