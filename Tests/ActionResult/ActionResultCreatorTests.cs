using com.etsoo.CoreFramework.Database;
using com.etsoo.Utils.Database;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Tests.ActionResult
{
    [TestFixture]
    public class ActionResultCreatorTests
    {
        readonly SqlServerDatabase db;

        public ActionResultCreatorTests()
        {
            // Arrange
            // Create the dabase
            db = new SqlServerDatabase("Initial Catalog=ftp_server;Server=(local);User ID=ftp;Password=ftp;Enlist=false");
        }

        /// <summary>
        /// Setup
        /// 初始化
        /// </summary>
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public async Task Create_NoResult_Test()
        {
            // Arrange
            using var connection = db.NewConnection();

            // Act
            var result = await connection.QueryAsResultAsync(new("SELECT TOP 0 NULL"));

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task Create_NoData_Test()
        {
            // Arrange
            using var connection = db.NewConnection();

            // Act
            var result = await connection.QueryAsResultAsync(new("SELECT 1 AS success, 'test' AS field"));

            // Assert
            Assert.IsTrue(result!.Success);
            Assert.IsTrue(result.Field == "test");
        }

        [Test]
        public async Task Create_SuccessData_Test()
        {
            // Arrange
            using var connection = db.NewConnection();

            // Act
            var result = await connection.QueryAsResultAsync(new("SELECT 1 AS success, 'test' AS field, 1234 AS id, 44.3 AS amount"));

            // Assert
            Assert.IsTrue(result!.Success);
            Assert.IsTrue(result.Data.GetExact<int>("id") == 1234);
            Assert.IsTrue(result.Data.GetExact<decimal>("amount") == 44.3M);
            Assert.IsNull(result.Data.GetExact<bool?>("ok"));
        }
    }
}
