using com.etsoo.CoreFramework.Business;
using com.etsoo.CoreFramework.Models;
using com.etsoo.Database;
using NUnit.Framework;

namespace Tests.Services
{
    [TestFixture]
    internal class SqlCommandTests
    {
        readonly SqliteDatabase db;

        public SqlCommandTests()
        {
            db = new SqliteDatabase("Data Source = etsoo.db;");
        }

        [Test]
        public void DeleteIntIdsTests()
        {
            var command = db.CreateDeleteCommand("User", [1, 2], "Id");
            Assert.AreEqual("DELETE FROM \"User\" WHERE \"Id\" IN (1,2)", command.CommandText);
        }

        [Test]
        public void DeleteStringIdsTests()
        {
            var command = db.CreateDeleteCommand("User", ["a", "b"], "Id");
            Assert.AreEqual("DELETE FROM \"User\" WHERE \"Id\" IN ('a','b')", command.CommandText);
        }

        [Test]
        public void DeleteModelTest()
        {
            var model = new DeleteTest { Id = 1, Name = "ABC%", Status = EntityStatus.Completed, CreationStart = new DateTime(2021, 1, 1), CreationEnd = new DateTime(2021, 1, 31), Ranges = [2, 4, 8] };
            var result = model.CreateSqlDelete(db);
            Assert.AreEqual("DELETE FROM \"User\" WHERE \"id\" = @Id AND \"name\" LIKE @Name AND \"entity_status\" <> @Status AND \"is_deleted\" IS NULL AND \"creation_start\" >= @CreationStart AND \"creation_end\" < @CreationEnd AND \"range\" IN (2,4,8)", result.Item1);
            Assert.AreEqual(6, result.Item2.ParameterNames.Count());
        }

        [Test]
        public void InsertModelSqliteTest()
        {
            var model = new InsertTest { Name = "ABC", Status = EntityStatus.Completed, IsDeleted = true };
            var result = model.CreateSqlInsert(db);
            Assert.AreEqual("INSERT INTO \"customer\" (\"name\", \"entityStatus\", \"isDeleted\", \"creation\") VALUES (@Name, @Status, @IsDeleted, @Creation) RETURNING \"id\"", result.Item1);
            Assert.AreEqual(4, result.Item2.ParameterNames.Count());
        }

        [Test]
        public void InsertModelSqlServerTest()
        {
            var model = new InsertTest { Name = "ABC", Status = EntityStatus.Completed, IsDeleted = true };
            var result = model.CreateSqlInsert(new SqlServerDatabase(""));
            Assert.AreEqual("INSERT INTO [customer] ([name], [entityStatus], [isDeleted], [creation]) OUTPUT inserted.[id] VALUES (@Name, @Status, @IsDeleted, @Creation)", result.Item1);
            Assert.AreEqual(4, result.Item2.ParameterNames.Count());
        }

        [Test]
        public void UpdateModelTest()
        {
            var model = new UpdateTest
            {
                Id = 1,
                Name = "ABC",
                Status = EntityStatus.Completed,
                IsDeleted = true,
                ChangedFields = ["Name", "IsDeleted"]
            };
            var result = model.CreateSqlUpdate(db);
            Assert.AreEqual("Update \"customer\" SET \"name\" = @Name, \"isDeleted\" = @IsDeleted WHERE \"id\" = @Id AND \"entityStatus\" <> @Status", result.Item1);
            Assert.AreEqual(4, result.Item2.ParameterNames.Count());
        }

        [Test]
        public void SelectModelTest()
        {
            var model = new SelectTest { Id = 1, Name = "ABC%", Status = EntityStatus.Completed, CreationStart = new DateTime(2021, 1, 1), CreationEnd = new DateTime(2021, 1, 31), Ranges = [2, 4, 8], QueryPaging = new QueryData { BatchSize = 16, OrderBy = "Name ASC, Id DESC" } };
            var result = model.CreateSqlSelect(db, ["Id", "Name", "SQLServer:CAST(IIF(Status < 200, 1, 0) AS bit)^SQLite:IIF(Status < 200, 'true', 'false') AS Valid"]);
            Assert.AreEqual("SELECT \"id\", \"name\", IIF(Status < 200, 'true', 'false') AS \"valid\" FROM \"User\" WHERE \"id\" = @Id AND \"name\" LIKE @Name AND \"entity_status\" <> @Status AND \"is_deleted\" IS NULL AND \"creation_start\" >= @CreationStart AND \"creation_end\" < @CreationEnd AND \"range\" IN (2,4,8) ORDER BY Name ASC, Id DESC LIMIT 16", result.Item1);
            Assert.AreEqual(6, result.Item2.ParameterNames.Count());
        }

        [Test]
        public void SelectJsonModelTest()
        {
            var model = new SelectTest { Id = 1, Name = "ABC%", Status = EntityStatus.Completed, CreationStart = new DateTime(2021, 1, 1), CreationEnd = new DateTime(2021, 1, 31), Ranges = [2, 4, 8], QueryPaging = new QueryData { BatchSize = 16, OrderBy = "Name ASC, Id DESC" } };
            var result = model.CreateSqlSelectJson(db, ["Id", "Name", "IsDeleted", "SQLServer:CAST(IIF(Status < 200, 1, 0) AS bit)^SQLite:IIF(Status < 200, 'true', 'false') AS Valid"]);
            Assert.AreEqual("SELECT json_group_array(json_object('id', id, 'name', name, 'isDeleted', is_deleted, 'valid', valid)) FROM (SELECT \"id\", \"name\", \"is_deleted\", IIF(Status < 200, 'true', 'false') AS \"valid\" FROM \"User\" WHERE \"id\" = @Id AND \"name\" LIKE @Name AND \"entity_status\" <> @Status AND \"is_deleted\" IS NULL AND \"creation_start\" >= @CreationStart AND \"creation_end\" < @CreationEnd AND \"range\" IN (2,4,8) ORDER BY Name ASC, Id DESC LIMIT 16)", result.Item1);
            Assert.AreEqual(6, result.Item2.ParameterNames.Count());
        }
    }
}
