using com.etsoo.CoreFramework.Business;
using com.etsoo.Database;

namespace Tests.Services
{
    [TestClass]
    public class SqlCommandTests
    {
        readonly SqliteDatabase db;

        public SqlCommandTests()
        {
            db = new SqliteDatabase("Data Source = etsoo.db;");
        }

        [TestMethod]
        public void DeleteIntIdsTests()
        {
            var command = db.CreateDeleteCommand("User", [1, 2], "Id");
            Assert.AreEqual("DELETE FROM \"User\" WHERE \"Id\" IN (1,2)", command.CommandText);
        }

        [TestMethod]
        public void DeleteStringIdsTests()
        {
            var command = db.CreateDeleteCommand("User", ["a", "b"], "Id");
            Assert.AreEqual("DELETE FROM \"User\" WHERE \"Id\" IN ('a','b')", command.CommandText);
        }

        [TestMethod]
        public void DeleteModelTest()
        {
            var model = new DeleteTest { Id = 1, Name = "ABC%", Status = EntityStatus.Completed, CreationStart = new DateTime(2021, 1, 1), CreationEnd = new DateTime(2021, 1, 31), Ranges = [2, 4, 8] };
            var result = model.CreateSqlDelete(db);
            Assert.AreEqual("DELETE FROM \"User\" WHERE \"id\" = @Id AND (@Name IS NULL OR \"name\" LIKE @Name) AND \"entity_status\" <> @Status AND ((@IsDeleted IS NULL AND \"is_deleted\" IS NULL) OR \"is_deleted\" = @IsDeleted) AND (@CreationStart IS NULL OR \"creation_start\" >= @CreationStart) AND (@CreationEnd IS NULL OR \"creation_end\" < @CreationEnd) AND (@Ranges IS NULL OR \"range\" IN (2,4,8))", result.Item1);
            Assert.AreEqual(7, result.Item2.ParameterNames.Count());
        }

        [TestMethod]
        public void InsertModelSqliteTest()
        {
            var model = new InsertTest { Name = "ABC", Status = EntityStatus.Completed, IsDeleted = true };
            var result = model.CreateSqlInsert(db);
            Assert.AreEqual("INSERT INTO \"customer\" (\"name\", \"entityStatus\", \"isDeleted\", \"creation\") VALUES (@Name, @Status, @IsDeleted, @Creation) RETURNING \"id\"", result.Item1);
            Assert.AreEqual(4, result.Item2.ParameterNames.Count());
        }

        [TestMethod]
        public void InsertModelSqlServerTest()
        {
            var model = new InsertTest { Name = "ABC", Status = EntityStatus.Completed, IsDeleted = true };
            var result = model.CreateSqlInsert(new SqlServerDatabase(""));
            Assert.AreEqual("INSERT INTO [customer] ([name], [entityStatus], [isDeleted], [creation]) OUTPUT inserted.[id] VALUES (@Name, @Status, @IsDeleted, @Creation)", result.Item1);
            Assert.AreEqual(4, result.Item2.ParameterNames.Count());
        }

        [TestMethod]
        public void InsertIgnoreModelSqliteTest()
        {
            var model = new InsertIgnoreTest { Id = "1001", Name = "ABC", Status = EntityStatus.Completed, IsDeleted = true };
            var result = model.CreateSqlInsert(db);
            Assert.AreEqual("INSERT OR IGNORE INTO \"customer\" (\"id\", \"name\", \"entityStatus\", \"isDeleted\", \"creation\") VALUES (@Id, @Name, @Status, @IsDeleted, @Creation) RETURNING \"id\"", result.Item1);
            Assert.AreEqual(5, result.Item2.ParameterNames.Count());
        }

        [TestMethod]
        public void InsertIgnoreModelSqlServerTest()
        {
            var model = new InsertIgnoreTest { Id = "1001", Name = "ABC", Status = EntityStatus.Completed, IsDeleted = true };
            var result = model.CreateSqlInsert(new SqlServerDatabase(""));
            Assert.AreEqual("INSERT INTO [customer] ([id], [name], [entityStatus], [isDeleted], [creation]) OUTPUT inserted.[id] VALUES (@Id, @Name, @Status, @IsDeleted, @Creation) WHERE NOT EXISTS (SELECT * FROM [customer] WHERE [id] = @Id)", result.Item1);
            Assert.AreEqual(5, result.Item2.ParameterNames.Count());
        }

        [TestMethod]
        public void UpdateModelTest()
        {
            // Achieve update a field while the field is also a query field
            var model = new UpdateTest
            {
                Id = 1,
                Name = "ABC",
                Status = EntityStatus.Completed,
                StatusUpdate = EntityStatus.Deleted,
                IsDeleted = true,
                ChangedFields = ["Name", "StatusUpdate", "IsDeleted"]
            };
            var result = model.CreateSqlUpdate(db);
            Assert.AreEqual("Update \"customer\" SET \"name\" = @Name, \"entityStatus\" = @StatusUpdate, \"isDeleted\" = @IsDeleted WHERE \"id\" = @Id AND \"entityStatus\" <> @Status", result.Item1);
            Assert.AreEqual(5, result.Item2.ParameterNames.Count());
        }

        [TestMethod]
        public void SelectModelTest()
        {
            var model = new SelectTest { Id = 1, Name = "ABC%", Status = EntityStatus.Completed, CreationStart = new DateTime(2021, 1, 1), CreationEnd = new DateTime(2021, 1, 31), Ranges = [2, 4, 8], QueryPaging = new QueryPagingData { BatchSize = 16, OrderBy = [new() { Field = "Name" }, new() { Field = "Id", Desc = true }] } };
            var result = model.CreateSqlSelect(db, ["Id", "Name", "SQLServer:CAST(IIF(Status < 200, 1, 0) AS bit)^SQLite:IIF(Status < 200, 'true', 'false') AS Valid"]);
            Assert.AreEqual("SELECT \"id\", \"name\", IIF(Status < 200, 'true', 'false') AS \"valid\" FROM \"User\" WHERE \"id\" = @Id AND \"name\" LIKE @Name AND \"entity_status\" <> @Status AND \"is_deleted\" IS NULL AND \"creation_start\" >= @CreationStart AND \"creation_end\" < @CreationEnd AND \"range\" IN (2,4,8) ORDER BY \"Name\" ASC, \"Id\" DESC LIMIT 16", result.Item1);
            Assert.AreEqual(6, result.Item2.ParameterNames.Count());
        }

        [TestMethod]
        public void SelectModelKeysetTest()
        {
            var model = new SelectTest { Id = 1, Name = "ABC%", Status = EntityStatus.Completed, CreationStart = new DateTime(2021, 1, 1), CreationEnd = new DateTime(2021, 1, 31), Ranges = [2, 4, 8], QueryPaging = new QueryPagingData { BatchSize = 16, Keysets = ["亿速思维", 10], OrderBy = [new() { Field = "Name" }, new() { Field = "Id", Desc = true, Unique = true }] } };
            var result = model.CreateSqlSelect(db, ["Id", "Name", "SQLServer:CAST(IIF(Status < 200, 1, 0) AS bit)^SQLite:IIF(Status < 200, 'true', 'false') AS Valid"]);
            Assert.AreEqual("SELECT \"id\", \"name\", IIF(Status < 200, 'true', 'false') AS \"valid\" FROM \"User\" WHERE (Name > @Name OR (Name = @Name AND Id < @Id)) AND \"id\" = @Id AND \"name\" LIKE @Name AND \"entity_status\" <> @Status AND \"is_deleted\" IS NULL AND \"creation_start\" >= @CreationStart AND \"creation_end\" < @CreationEnd AND \"range\" IN (2,4,8) ORDER BY \"Name\" ASC, \"Id\" DESC LIMIT 16", result.Item1);
            Assert.AreEqual(6, result.Item2.ParameterNames.Count());
        }

        [TestMethod]
        public void SelectModelTestMore()
        {
            var model = new SelectTestMore { Id = 1, Name = "Name", Cid = "ABC" };
            var result = model.CreateSqlSelect(db, ["Id", "Name"]);
            Assert.AreEqual("SELECT \"id\", \"name\" FROM \"User\" WHERE \"id\" = @Id AND (\"name\" LIKE @Name OR \"description\" LIKE @Name) AND \"cid\" LIKE @Cid", result.Item1);
            Assert.AreEqual(3, result.Item2.ParameterNames.Count());
        }

        private void AddEnabledCondition(List<string> conditions, bool? enabled)
        {
            if (!enabled.HasValue) return;

            if (enabled.Value)
            {
                conditions.Add("status < 200");
            }
            else
            {
                conditions.Add("status >= 200");
            }
        }

        public void SelectModelTestMoreEnabledTrue()
        {
            var model = new SelectTestMore { Id = 1, Name = "Name", Cid = "ABC", Enabled = true };
            var result = model.CreateSqlSelect(db, ["Id", "Name"], (conditions) => AddEnabledCondition(conditions, model.Enabled));
            Assert.AreEqual("SELECT \"id\", \"name\" FROM \"User\" WHERE \"id\" = @Id AND (\"name\" LIKE @Name OR \"description\" LIKE @Name) AND \"cid\" LIKE @Cid AND status < 200", result.Item1);
            Assert.AreEqual(3, result.Item2.ParameterNames.Count());
        }

        public void SelectModelTestMoreEnabledFalse()
        {
            var model = new SelectTestMore { Id = 1, Name = "Name", Cid = "ABC", Enabled = false };
            var result = model.CreateSqlSelect(db, ["Id", "Name"], (conditions) => AddEnabledCondition(conditions, model.Enabled));
            Assert.AreEqual("SELECT \"id\", \"name\" FROM \"User\" WHERE \"id\" = @Id AND (\"name\" LIKE @Name OR \"description\" LIKE @Name) AND \"cid\" LIKE @Cid AND status >= 200", result.Item1);
            Assert.AreEqual(3, result.Item2.ParameterNames.Count());
        }

        [TestMethod]
        public void SelectJsonModelTest()
        {
            var model = new SelectTest { Id = 1, Name = "ABC%", Status = EntityStatus.Completed, CreationStart = new DateTime(2021, 1, 1), CreationEnd = new DateTime(2021, 1, 31), Ranges = [2, 4, 8], QueryPaging = new QueryPagingData { BatchSize = 16, OrderBy = [new() { Field = "Name" }, new() { Field = "Id", Desc = true }] } };
            var result = model.CreateSqlSelectJson(db, ["Id", "Name", "IsDeleted", "SQLServer:CAST(IIF(Status < 200, 1, 0) AS bit)^SQLite:IIF(Status < 200, 'true', 'false') AS Valid"]);
            Assert.AreEqual("SELECT json_group_array(json_object('id', id, 'name', name, 'isDeleted', is_deleted, 'valid', valid)) FROM (SELECT \"id\", \"name\", \"is_deleted\", IIF(Status < 200, 'true', 'false') AS \"valid\" FROM \"User\" WHERE \"id\" = @Id AND \"name\" LIKE @Name AND \"entity_status\" <> @Status AND \"is_deleted\" IS NULL AND \"creation_start\" >= @CreationStart AND \"creation_end\" < @CreationEnd AND \"range\" IN (2,4,8) ORDER BY \"Name\" ASC, \"Id\" DESC LIMIT 16)", result.Item1);
            Assert.AreEqual(6, result.Item2.ParameterNames.Count());
        }

        [TestMethod]
        public void SqlUserTiplistTest()
        {
            var model = new SqlUserTiplist { Id = 1, ExcludedIds = [2, 4, 8], Keyword = "ABC" };
            var result = model.CreateSqlSelectJson(db, ["id", "name AS label"]);
            Assert.AreEqual("SELECT json_group_array(json_object('id', id, 'label', label)) FROM (SELECT \"id\", name AS \"label\" FROM \"User\" WHERE \"id\" NOT IN (2,4,8) AND \"name\" LIKE @Keyword AND \"id\" = @Id AND \"excludedIds\" IN (2,4,8) AND \"keyword\" = @Keyword)", result.Item1);
            Assert.AreEqual(3, result.Item2.ParameterNames.Count());
        }

        [TestMethod]
        public void SelectGenericModelTest()
        {
            var model = new SelectGenericTest { Id = 1, Name = "ABC%", Status = EntityStatus.Completed, CreationStart = new DateTime(2021, 1, 1), CreationEnd = new DateTime(2021, 1, 31), Ranges = [2, 4, 8], QueryPaging = new QueryPagingData { BatchSize = 16, OrderBy = [new() { Field = "Name" }, new() { Field = "Id", Desc = true }] } };
            var result = model.Default.SelectResult2.CreateSqlSelect(db);
            Assert.AreEqual("SELECT \"creation\" AS creation, \"id\" AS id, \"name\" AS name, \"status\" AS status FROM \"User\" WHERE \"id\" = @Id AND \"name\" LIKE @Name AND \"entity_status\" <> @Status AND \"is_deleted\" IS NULL AND \"creation_start\" >= @CreationStart AND \"creation_end\" < @CreationEnd AND \"range\" IN (2,4,8) ORDER BY \"Name\" ASC, \"Id\" DESC LIMIT 16", result.Item1);
            Assert.AreEqual(7, result.Item2.ParameterNames.Count());
        }

        [TestMethod]
        public void SelectGenericJsonModelTest()
        {
            var model = new SelectGenericTest { Id = 1, Name = "ABC%", Status = EntityStatus.Completed, CreationStart = new DateTime(2021, 1, 1), CreationEnd = new DateTime(2021, 1, 31), Ranges = [2, 4, 8], QueryPaging = new QueryPagingData { BatchSize = 16, OrderBy = [new() { Field = "Name" }, new() { Field = "Id", Desc = true }] } };
            var result = model.Default.SelectResult2.CreateSqlSelectJson(db);
            Assert.AreEqual("SELECT json_object('creation', creation, 'id', id, 'name', name, 'status', status) FROM (SELECT \"creation\" AS creation, \"id\" AS id, \"name\" AS name, \"status\" AS status FROM \"User\" WHERE \"id\" = @Id AND \"name\" LIKE @Name AND \"entity_status\" <> @Status AND \"is_deleted\" IS NULL AND \"creation_start\" >= @CreationStart AND \"creation_end\" < @CreationEnd AND \"range\" IN (2,4,8) ORDER BY \"Name\" ASC, \"Id\" DESC LIMIT 16)", result.Item1);
            Assert.AreEqual(7, result.Item2.ParameterNames.Count());
        }

        [TestMethod]
        public void ParserInnerFieldsTest()
        {
            var fields = InsertTest.ParserInnerFields;
            Assert.AreEqual("entityStatus AS Status", fields.ToArray()[1]);
        }
    }
}
