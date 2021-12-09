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
            var rq = new InitCallRQ { Timestamp = LocalizationUtils.UTCToJsMiliseconds(), DeviceId="107EF9F0A24AC89D816C46FF92DF80D6DDC2E2F3591AFA8BD6476DA8BFB7AFD3A1EZO3tgoSc5vx1gD4KebC0B8v4vwb64Lgg7Y5JYBd+qbT+6mZj2F/C+2c4iqJQtg0" };
            var result = await service.InitCallAsync(rq, "My Password");
            Assert.IsTrue(result.Ok);
        }
    }
}
