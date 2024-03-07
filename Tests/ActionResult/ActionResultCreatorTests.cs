using com.etsoo.Database;
using com.etsoo.SourceGenerators.Attributes;
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
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Create_NoData_Test()
        {
            // Arrange
            using var connection = db.NewConnection();

            // Act
            var result = await connection.QueryAsResultAsync(new("SELECT 'NoId/Organization' AS [type]"));

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result?.Ok, Is.False);
                Assert.That(result?.Type, Is.EqualTo("NoId"));
                Assert.That(result?.Field, Is.EqualTo("Organization"));
            });
        }

        [Test]
        public async Task Create_SuccessData_Test()
        {
            // Arrange
            using var connection = db.NewConnection();

            // Act
            var result = await connection.QueryAsResultAsync(new("SELECT 1 AS ok, 'test' AS field, 90900 as traceId, 1234 AS id, 44.3 AS amount, CAST(0 AS bit) AS ok"));

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result?.Type, Is.Null);
                Assert.That(result?.Ok, Is.True);
                Assert.That(result?.TraceId, Is.EqualTo("90900"));
            });

            var data = result.Data.As<ActionResultTestData>("id", "ok");
            Assert.Multiple(() =>
            {
                Assert.That(data?.Id, Is.EqualTo(1234));
                Assert.That(data?.Amount, Is.EqualTo(44.3M));

                // Support same name property
                // Should after all ActionResult fields
                Assert.That(data?.Ok, Is.False);
            });
        }

        [Test]
        public async Task Create_StringKeyDictionary_Test()
        {
            // Arrange
            using var connection = db.NewConnection();

            // Act
            var result = await connection.QueryAsResultAsync(new("SELECT 1 AS ok, 'test' AS field, 1234 AS id, 44.3 AS amount"));

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result?.Ok, Is.True);
                Assert.That(result?.Field, Is.EqualTo("test"));
                Assert.That(result?.Data?.Count, Is.EqualTo(2));
                Assert.That(result?.Data?.ContainsValue(1234), Is.True);
                Assert.That(result?.Data?.ContainsKey("amount"), Is.True);
            });
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
