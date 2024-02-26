using com.etsoo.CoreFramework.Application;
using com.etsoo.Database;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Text;

namespace Tests.CoreFramework
{
    [TestFixture]
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
        [Test]
        public void MiniApplicationTest()
        {
            var db = new SqliteDatabase("Data Source = etsoo.db;");
            var app = new CoreApplication<AppConfiguration, SqliteConnection>(AppConfiguration.Create(), db);
            Assert.AreEqual(app.Configuration.Cultures.Length, 0);
        }

        [Test]
        public void ConfigurationTest()
        {
            // Arrange
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(configurationText));
            var section = new ConfigurationBuilder().AddJsonStream(stream).Build().GetSection("Configuration");

            var privateField = section.GetSection("PrivateKey").Value;
            Assert.AreEqual("Etsoo", privateField);

            var config = section.Get<AppConfiguration>();
            Assert.IsNotNull(config);

            Assert.AreEqual("Etsoo", config?.PrivateKey);
            Assert.AreEqual("TestApp", config?.Name);
            Assert.AreEqual("zh-Hans-CN", config?.Cultures[1]);

            config?.UnsealData((field, input) => new string(input.Reverse().ToArray()));
            Assert.AreEqual("oostE", config?.PrivateKey);
        }
    }
}
