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
            Assert.That(app.Configuration.Cultures.Length, Is.EqualTo(0));
        }

        [Test]
        public void ConfigurationTest()
        {
            // Arrange
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(configurationText));
            var section = new ConfigurationBuilder().AddJsonStream(stream).Build().GetSection("Configuration");

            var privateField = section.GetSection("PrivateKey").Value;
            Assert.That(privateField, Is.EqualTo("Etsoo"));

            var config = section.Get<AppConfiguration>();

            Assert.Multiple(() =>
            {
                Assert.That(config, Is.Not.Null);
                Assert.That(config?.PrivateKey, Is.EqualTo("Etsoo"));
                Assert.That(config?.Name, Is.EqualTo("TestApp"));
                Assert.That(config?.Cultures[1], Is.EqualTo("zh-Hans-CN"));
            });

            config?.UnsealData((field, input) => new string(input.Reverse().ToArray()));
            Assert.That(config?.PrivateKey, Is.EqualTo("oostE"));
        }
    }
}
