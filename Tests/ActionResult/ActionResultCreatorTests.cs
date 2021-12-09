using com.etsoo.SourceGenerators.Attributes;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Database;
using com.etsoo.Utils.String;
using NUnit.Framework;

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
            db = new SqlServerDatabase("Server=(local);User ID=smarterp;Password=smarterp;Enlist=false;TrustServerCertificate=true");
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
            var result = await connection.QueryAsResultAsync(new("SELECT 'NoId/Organization' AS [type]"));

            // Assert
            Assert.IsFalse(result!.Ok);
            Assert.AreEqual(result!.Type, "NoId");
            Assert.AreEqual(result!.Field, "Organization");
        }

        [Test]
        public async Task Create_SuccessData_Test()
        {
            // Arrange
            using var connection = db.NewConnection();

            // Act
            var result = await connection.QueryAsResultAsync(new("SELECT 1 AS ok, 'test' AS field, 90900 as traceId, 1234 AS id, 44.3 AS amount, CAST(0 AS bit) AS ok"));

            // Assert
            Assert.IsNull(result?.Type);
            Assert.IsTrue(result!.Ok);
            Assert.AreEqual(result.TraceId, "90900");

            var data = result.Data.As<ActionResultTestData>("id", "ok");
            Assert.AreEqual(data?.Id, 1234);
            Assert.AreEqual(data?.Amount, 44.3M);

            // Support same name property
            // Should after all ActionResult fields
            Assert.IsFalse(data?.Ok);
        }

        [Test]
        public async Task Create_StringKeyDictionary_Test()
        {
            // Arrange
            using var connection = db.NewConnection();

            // Act
            var result = await connection.QueryAsResultAsync(new("SELECT 1 AS ok, 'test' AS field, 1234 AS id, 44.3 AS amount"));

            // Assert
            Assert.IsTrue(result!.Ok);
            Assert.AreEqual(result?.Field, "test");
            Assert.AreEqual(result?.Data?.Count, 2);
            Assert.IsTrue(result?.Data?.ContainsValue(1234));
            Assert.IsTrue(result?.Data?.ContainsKey("amount"));
        }
    }

    [AutoDictionaryGenerator]
    internal partial record ActionResultTestData
    {
        public int Id { get; init; }
        public decimal Amount { get; init; }
        public bool? Ok { get; init; }
    }
}
