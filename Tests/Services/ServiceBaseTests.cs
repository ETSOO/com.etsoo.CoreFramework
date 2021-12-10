using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Models;
using com.etsoo.CoreFramework.Services;
using com.etsoo.Utils.Database;
using com.etsoo.Utils.Localization;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Tests.Repositories;

namespace Tests.Services
{
    internal class ServiceTest : ServiceBase<SqlConnection, IntEntityRepository>
    {
        public ServiceTest(ICoreApplication<SqlConnection> app, IntEntityRepository repo, ILogger logger) : base(app, repo, logger)
        {
        }
    }

    [TestFixture]
    internal class ServiceBaseTests
    {
        readonly ServiceTest service;

        public ServiceBaseTests()
        {
            var db = new SqlServerDatabase("Server=(local);User ID=test;Password=test;Enlist=false;TrustServerCertificate=true", true);

            var config = new AppConfiguration("test");
            var app = new CoreApplication<SqlConnection>(config, db);

            var repo = new IntEntityRepository(app, "user");
            service = new ServiceTest(app, repo, null!);
        }

        [Test]
        public async Task InitCallTests()
        {
            // Arrange
            var rq = new InitCallRQ { Timestamp = LocalizationUtils.UTCToJsMiliseconds() };
            var result = await service.InitCallAsync(rq, "My Password");
            Assert.IsTrue(result.Ok);
        }
    }
}
