using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Models;
using com.etsoo.CoreFramework.Services;
using com.etsoo.Utils.Database;
using com.etsoo.Utils.Localization;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using NUnit.Framework;
using Tests.Repositories;

namespace Tests.Services
{
    internal class ServiceTest : ServiceBase<SqlConnection, IntEntityRepository>
    {
        public ServiceTest(ICoreApplication<SqlConnection> app, IntEntityRepository repo, ILogger logger) : base(app, repo, logger)
        {
        }

        public new string? Decrypt(string encyptedMessage, string passphrase, int? durationSeconds = null, bool? isWebClient = null)
        {
            return base.Decrypt(encyptedMessage, passphrase, durationSeconds, isWebClient);
        }

        public new string Encrypt(string message, string passphrase, int iterations = 10, bool? enhanced = null)
        {
            return base.Encrypt(message, passphrase, iterations, enhanced);
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
            service = new ServiceTest(app, repo, new EventLogLoggerProvider().CreateLogger("SmartERPTests"));
        }

        [Test]
        public void EncryptionTests()
        {
            // Arrange
            var input = "Hello, world!";
            var passphrase = "My password";
            
            // Act
            var encrypted = service.Encrypt(input, passphrase, 1);
            var plainText = service.Decrypt(encrypted, passphrase, 120);

            // Assert
            Assert.AreEqual(input, plainText);
        }

        [Test]
        public void WebDecryptionTests()
        {
            // Arrange
            var input = "Hello, world!";
            var encrypted = "fDxgI10v!018aa0c6012987c56760bcf00b40914b65544b37d639bc75ac0ee659275563e35fCurMtA1yTMxuFO9xzYwefg==";
            var passphrase = "My password";

            // Act
            var plainText = service.Decrypt(encrypted, passphrase, 120, true);
            var plainTextLong = service.Decrypt(encrypted, passphrase, null, true);

            // Assert
            Assert.IsNull(plainText);
            Assert.AreEqual(input, plainTextLong);
        }

        [Test]
        public async Task InitCallTests()
        {
            // Arrange
            var rq = new InitCallRQ { Timestamp = LocalizationUtils.UTCToJsMiliseconds() };
            var result = await service.InitCallAsync(rq, "My Password");
            Assert.IsTrue(result.Ok);

            var deviceId = result.Data.Get("DeviceId");
            Assert.IsNotNull(deviceId);

            var rqNew = new InitCallRQ { Timestamp = LocalizationUtils.UTCToJsMiliseconds(), DeviceId = deviceId };
            var resultNew = await service.InitCallAsync(rqNew, "My Password");
            Assert.IsTrue(resultNew.Ok);
        }
    }
}
