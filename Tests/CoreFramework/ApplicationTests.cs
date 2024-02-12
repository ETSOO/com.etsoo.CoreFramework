using com.etsoo.CoreFramework.Application;
using com.etsoo.Database;
using Microsoft.Data.Sqlite;
using NUnit.Framework;

namespace Tests.CoreFramework
{
    [TestFixture]
    public class ApplicationTests
    {
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
    }
}
