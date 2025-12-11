using com.etsoo.Database;
using com.etsoo.SourceGenerators.Attributes;
using com.etsoo.Utils;
using com.etsoo.Utils.Serialization;
using System.Text.Json;

namespace Tests.ActionResult
{
    [TestClass]
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
        [TestInitialize]
        public void Setup()
        {

        }

        [TestMethod]
        public async Task Create_NoResult_Test()
        {
            // Arrange
            using var connection = db.NewConnection();

            // Act
            var result = await connection.QueryAsResultAsync(new("SELECT TOP 0 NULL"));

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task Create_NoData_Test()
        {
            // Arrange
            using var connection = db.NewConnection();

            // Act
            var result = await connection.QueryAsResultAsync(new("SELECT 'NoId/Organization' AS [type]"));

            // Assert
            Assert.IsFalse(result?.Ok ?? false);
            Assert.AreEqual("NoId", result?.Type);
            Assert.AreEqual("Organization", result?.Field);
        }

        [TestMethod]
        public async Task Create_SuccessData_Test()
        {
            // Arrange
            using var connection = db.NewConnection();

            // Act
            var result = await connection.QueryAsResultAsync(new("SELECT 1 AS ok, 'test' AS field, 90900 as traceId, 1234 AS id, 44.3 AS amount, CAST(0 AS bit) AS ok"));

            // Assert
            Assert.IsNull(result?.Type);
            Assert.IsTrue(result?.Ok ?? false);
            Assert.AreEqual("90900", result?.TraceId);

            var data = result.Data.As<ActionResultTestData>("id", "ok");
            // Assert
            Assert.AreEqual(1234, data?.Id);
            Assert.AreEqual(44.3M, data?.Amount);

            // Support same name property
            // Should after all ActionResult fields
            Assert.IsFalse(data?.Ok ?? false);
        }

        [TestMethod]
        public async Task Create_StringKeyDictionary_Test()
        {
            // Arrange
            using var connection = db.NewConnection();

            // Act
            var result = await connection.QueryAsResultAsync(new("SELECT 1 AS ok, 'test' AS field, 1234 AS id, 44.3 AS amount"));

            // Assert
            Assert.IsTrue(result?.Ok ?? false);
            Assert.AreEqual("test", result?.Field);
            Assert.AreEqual(2, result?.Data?.Count);
            Assert.IsTrue(result?.Data?.ContainsValue(1234) ?? false);
            Assert.IsTrue(result?.Data?.ContainsKey("amount") ?? false);
        }


        [TestMethod]
        public async Task Transform_WithSolidData_Test()
        {
            var id = 123;
            var msg = "Success";
            var result = com.etsoo.Utils.Actions.ActionResult.Succeed(id, msg);
            await using var stream = SharedUtils.GetStream();
            await result.ToJsonAsync(stream);
            stream.Position = 0;
            var dataResult = await JsonSerializer.DeserializeAsync(stream, CommonJsonSerializerContext.Default.ActionResultIdMsgData);

            // Assert
            Assert.IsTrue(dataResult?.Ok ?? false);
            Assert.AreEqual(id, dataResult?.Data?.Id);
            Assert.AreEqual(msg, dataResult?.Data?.Msg);
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
