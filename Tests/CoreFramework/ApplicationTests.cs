using com.etsoo.CoreFramework.Application;
using com.etsoo.Database;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Tests.CoreFramework
{
    [TestClass]
    public class ApplicationTests
    {
        readonly string configurationText = @"{
                ""Configuration"": {
                    ""PrivateKey"": ""Etsoo"",
                    ""Cultures"": [ ""en"", ""zh-Hans-CN"" ],
                    ""Name"": ""TestApp""
                }
            }";

        /// <summary>
        /// Mini application test
        /// </summary>
        [TestMethod]
        public void MiniApplicationTest()
        {
            var db = new SqliteDatabase("Data Source = etsoo.db;");
            var app = new CoreApplication<AppConfiguration, SqliteConnection>(AppConfiguration.Create(), db);

            Assert.IsEmpty(app.Configuration.Cultures);
        }

        [TestMethod]
        public void ConfigurationTest()
        {
            // Arrange
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(configurationText));
            var section = new ConfigurationBuilder().AddJsonStream(stream).Build().GetSection("Configuration");

            var privateField = section.GetSection("PrivateKey").Value;

            Assert.AreEqual("Etsoo", privateField);

            var config = section.Get<AppConfiguration>();

            // Assert
            Assert.IsNotNull(config);
            Assert.AreEqual("Etsoo", config?.PrivateKey);
            Assert.AreEqual("TestApp", config?.Name);
            Assert.AreEqual("zh-Hans-CN", config?.Cultures[1]);
        }

        [TestMethod]
        public void EncryptionTest()
        {
            var db = new SqliteDatabase("Data Source = etsoo.db;");
            var app = new CoreApplication<AppConfiguration, SqliteConnection>(AppConfiguration.Create(), db);
            var text = "Hello, world!";
            var encrypted = app.EncriptData(text, "a");
            var decrypted = app.DecriptData(encrypted, "a");

            Assert.AreEqual(text, decrypted);
        }
    }
}
