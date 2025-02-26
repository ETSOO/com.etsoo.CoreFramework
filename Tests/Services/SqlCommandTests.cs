﻿using com.etsoo.CoreFramework.Business;
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
            Assert.That(command.CommandText, Is.EqualTo("DELETE FROM \"User\" WHERE \"Id\" IN (1,2)"));
        }

        [Test]
        public void DeleteStringIdsTests()
        {
            var command = db.CreateDeleteCommand("User", ["a", "b"], "Id");
            Assert.That(command.CommandText, Is.EqualTo("DELETE FROM \"User\" WHERE \"Id\" IN ('a','b')"));
        }

        [Test]
        public void DeleteModelTest()
        {
            var model = new DeleteTest { Id = 1, Name = "ABC%", Status = EntityStatus.Completed, CreationStart = new DateTime(2021, 1, 1), CreationEnd = new DateTime(2021, 1, 31), Ranges = [2, 4, 8] };
            var result = model.CreateSqlDelete(db);
            Assert.Multiple(() =>
            {
                Assert.That(result.Item1, Is.EqualTo("DELETE FROM \"User\" WHERE \"id\" = @Id AND (@Name IS NULL OR \"name\" LIKE @Name) AND \"entity_status\" <> @Status AND ((@IsDeleted IS NULL AND \"is_deleted\" IS NULL) OR \"is_deleted\" = @IsDeleted) AND (@CreationStart IS NULL OR \"creation_start\" >= @CreationStart) AND (@CreationEnd IS NULL OR \"creation_end\" < @CreationEnd) AND (@Ranges IS NULL OR \"range\" IN (2,4,8))"));
                Assert.That(result.Item2.ParameterNames.Count(), Is.EqualTo(7));
            });
        }

        [Test]
        public void InsertModelSqliteTest()
        {
            var model = new InsertTest { Name = "ABC", Status = EntityStatus.Completed, IsDeleted = true };
            var result = model.CreateSqlInsert(db);
            Assert.Multiple(() =>
            {
                Assert.That(result.Item1, Is.EqualTo("INSERT INTO \"customer\" (\"name\", \"entityStatus\", \"isDeleted\", \"creation\") VALUES (@Name, @Status, @IsDeleted, @Creation) RETURNING \"id\""));
                Assert.That(result.Item2.ParameterNames.Count(), Is.EqualTo(4));
            });
        }

        [Test]
        public void InsertModelSqlServerTest()
        {
            var model = new InsertTest { Name = "ABC", Status = EntityStatus.Completed, IsDeleted = true };
            var result = model.CreateSqlInsert(new SqlServerDatabase(""));
            Assert.Multiple(() =>
            {
                Assert.That(result.Item1, Is.EqualTo("INSERT INTO [customer] ([name], [entityStatus], [isDeleted], [creation]) OUTPUT inserted.[id] VALUES (@Name, @Status, @IsDeleted, @Creation)"));
                Assert.That(result.Item2.ParameterNames.Count(), Is.EqualTo(4));
            });
        }

        [Test]
        public void InsertIgnoreModelSqliteTest()
        {
            var model = new InsertIgnoreTest { Id = "1001", Name = "ABC", Status = EntityStatus.Completed, IsDeleted = true };
            var result = model.CreateSqlInsert(db);
            Assert.Multiple(() =>
            {
                Assert.That(result.Item1, Is.EqualTo("INSERT OR IGNORE INTO \"customer\" (\"id\", \"name\", \"entityStatus\", \"isDeleted\", \"creation\") VALUES (@Id, @Name, @Status, @IsDeleted, @Creation) RETURNING \"id\""));
                Assert.That(result.Item2.ParameterNames.Count(), Is.EqualTo(5));
            });
        }

        [Test]
        public void InsertIgnoreModelSqlServerTest()
        {
            var model = new InsertIgnoreTest { Id = "1001", Name = "ABC", Status = EntityStatus.Completed, IsDeleted = true };
            var result = model.CreateSqlInsert(new SqlServerDatabase(""));
            Assert.Multiple(() =>
            {
                Assert.That(result.Item1, Is.EqualTo("INSERT INTO [customer] ([id], [name], [entityStatus], [isDeleted], [creation]) OUTPUT inserted.[id] VALUES (@Id, @Name, @Status, @IsDeleted, @Creation) WHERE NOT EXISTS (SELECT * FROM [customer] WHERE [id] = @Id)"));
                Assert.That(result.Item2.ParameterNames.Count(), Is.EqualTo(5));
            });
        }

        [Test]
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
            Assert.Multiple(() =>
            {
                Assert.That(result.Item1, Is.EqualTo("Update \"customer\" SET \"name\" = @Name, \"entityStatus\" = @StatusUpdate, \"isDeleted\" = @IsDeleted WHERE \"id\" = @Id AND \"entityStatus\" <> @Status"));
                Assert.That(result.Item2.ParameterNames.Count(), Is.EqualTo(5));
            });
        }

        [Test]
        public void SelectModelTest()
        {
            var model = new SelectTest { Id = 1, Name = "ABC%", Status = EntityStatus.Completed, CreationStart = new DateTime(2021, 1, 1), CreationEnd = new DateTime(2021, 1, 31), Ranges = [2, 4, 8], QueryPaging = new QueryPagingData { BatchSize = 16, OrderBy = [new() { Field = "Name" }, new() { Field = "Id", Desc = true }] } };
            var result = model.CreateSqlSelect(db, ["Id", "Name", "SQLServer:CAST(IIF(Status < 200, 1, 0) AS bit)^SQLite:IIF(Status < 200, 'true', 'false') AS Valid"]);
            Assert.Multiple(() =>
            {
                Assert.That(result.Item1, Is.EqualTo("SELECT \"id\", \"name\", IIF(Status < 200, 'true', 'false') AS \"valid\" FROM \"User\" WHERE \"id\" = @Id AND \"name\" LIKE @Name AND \"entity_status\" <> @Status AND \"is_deleted\" IS NULL AND \"creation_start\" >= @CreationStart AND \"creation_end\" < @CreationEnd AND \"range\" IN (2,4,8) ORDER BY \"Name\" ASC, \"Id\" DESC LIMIT 16"));
                Assert.That(result.Item2.ParameterNames.Count(), Is.EqualTo(6));
            });
        }

        [Test]
        public void SelectModelKeysetTest()
        {
            var model = new SelectTest { Id = 1, Name = "ABC%", Status = EntityStatus.Completed, CreationStart = new DateTime(2021, 1, 1), CreationEnd = new DateTime(2021, 1, 31), Ranges = [2, 4, 8], QueryPaging = new QueryPagingData { BatchSize = 16, Keysets = ["亿速思维", 10], OrderBy = [new() { Field = "Name" }, new() { Field = "Id", Desc = true, Unique = true }] } };
            var result = model.CreateSqlSelect(db, ["Id", "Name", "SQLServer:CAST(IIF(Status < 200, 1, 0) AS bit)^SQLite:IIF(Status < 200, 'true', 'false') AS Valid"]);
            Assert.Multiple(() =>
            {
                Assert.That(result.Item1, Is.EqualTo("SELECT \"id\", \"name\", IIF(Status < 200, 'true', 'false') AS \"valid\" FROM \"User\" WHERE (Name > @Name OR (Name = @Name AND Id < @Id)) AND \"id\" = @Id AND \"name\" LIKE @Name AND \"entity_status\" <> @Status AND \"is_deleted\" IS NULL AND \"creation_start\" >= @CreationStart AND \"creation_end\" < @CreationEnd AND \"range\" IN (2,4,8) ORDER BY \"Name\" ASC, \"Id\" DESC LIMIT 16"));
                Assert.That(result.Item2.ParameterNames.Count(), Is.EqualTo(6));
            });
        }

        [Test]
        public void SelectModelTestMore()
        {
            var model = new SelectTestMore { Id = 1, Name = "Name", Cid = "ABC" };
            var result = model.CreateSqlSelect(db, ["Id", "Name"]);
            Assert.Multiple(() =>
            {
                Assert.That(result.Item1, Is.EqualTo("SELECT \"id\", \"name\" FROM \"User\" WHERE \"id\" = @Id AND (\"name\" LIKE @Name OR \"description\" LIKE @Name) AND \"cid\" LIKE @Cid"));
                Assert.That(result.Item2.ParameterNames.Count(), Is.EqualTo(3));
            });
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
            Assert.Multiple(() =>
            {
                Assert.That(result.Item1, Is.EqualTo("SELECT \"id\", \"name\" FROM \"User\" WHERE \"id\" = @Id AND (\"name\" LIKE @Name OR \"description\" LIKE @Name) AND \"cid\" LIKE @Cid AND status < 200"));
                Assert.That(result.Item2.ParameterNames.Count(), Is.EqualTo(3));
            });
        }

        public void SelectModelTestMoreEnabledFalse()
        {
            var model = new SelectTestMore { Id = 1, Name = "Name", Cid = "ABC", Enabled = false };
            var result = model.CreateSqlSelect(db, ["Id", "Name"], (conditions) => AddEnabledCondition(conditions, model.Enabled));
            Assert.Multiple(() =>
            {
                Assert.That(result.Item1, Is.EqualTo("SELECT \"id\", \"name\" FROM \"User\" WHERE \"id\" = @Id AND (\"name\" LIKE @Name OR \"description\" LIKE @Name) AND \"cid\" LIKE @Cid AND status >= 200"));
                Assert.That(result.Item2.ParameterNames.Count(), Is.EqualTo(3));
            });
        }

        [Test]
        public void SelectJsonModelTest()
        {
            var model = new SelectTest { Id = 1, Name = "ABC%", Status = EntityStatus.Completed, CreationStart = new DateTime(2021, 1, 1), CreationEnd = new DateTime(2021, 1, 31), Ranges = [2, 4, 8], QueryPaging = new QueryPagingData { BatchSize = 16, OrderBy = [new() { Field = "Name" }, new() { Field = "Id", Desc = true }] } };
            var result = model.CreateSqlSelectJson(db, ["Id", "Name", "IsDeleted", "SQLServer:CAST(IIF(Status < 200, 1, 0) AS bit)^SQLite:IIF(Status < 200, 'true', 'false') AS Valid"]);
            Assert.Multiple(() =>
            {
                Assert.That(result.Item1, Is.EqualTo("SELECT json_group_array(json_object('id', id, 'name', name, 'isDeleted', is_deleted, 'valid', valid)) FROM (SELECT \"id\", \"name\", \"is_deleted\", IIF(Status < 200, 'true', 'false') AS \"valid\" FROM \"User\" WHERE \"id\" = @Id AND \"name\" LIKE @Name AND \"entity_status\" <> @Status AND \"is_deleted\" IS NULL AND \"creation_start\" >= @CreationStart AND \"creation_end\" < @CreationEnd AND \"range\" IN (2,4,8) ORDER BY \"Name\" ASC, \"Id\" DESC LIMIT 16)"));
                Assert.That(result.Item2.ParameterNames.Count(), Is.EqualTo(6));
            });
        }

        [Test]
        public void SqlUserTiplistTest()
        {
            var model = new SqlUserTiplist { Id = 1, ExcludedIds = [2, 4, 8], Keyword = "ABC" };
            var result = model.CreateSqlSelectJson(db, ["id", "name AS label"]);
            Assert.Multiple(() =>
            {
                Assert.That(result.Item1, Is.EqualTo("SELECT json_group_array(json_object('id', id, 'label', label)) FROM (SELECT \"id\", name AS \"label\" FROM \"User\" WHERE \"id\" NOT IN (2,4,8) AND \"name\" LIKE @Keyword AND \"id\" = @Id AND \"excludedIds\" IN (2,4,8) AND \"keyword\" = @Keyword)"));
                Assert.That(result.Item2.ParameterNames.Count(), Is.EqualTo(3));
            });
        }

        [Test]
        public void SelectGenericModelTest()
        {
            var model = new SelectGenericTest { Id = 1, Name = "ABC%", Status = EntityStatus.Completed, CreationStart = new DateTime(2021, 1, 1), CreationEnd = new DateTime(2021, 1, 31), Ranges = [2, 4, 8], QueryPaging = new QueryPagingData { BatchSize = 16, OrderBy = [new() { Field = "Name" }, new() { Field = "Id", Desc = true }] } };
            var result = model.Default.SelectResult2.CreateSqlSelect(db);
            Assert.Multiple(() =>
            {
                Assert.That(result.Item1, Is.EqualTo("SELECT \"creation\" AS creation, \"id\" AS id, \"name\" AS name, \"status\" AS status FROM \"User\" WHERE \"id\" = @Id AND \"name\" LIKE @Name AND \"entity_status\" <> @Status AND \"is_deleted\" IS NULL AND \"creation_start\" >= @CreationStart AND \"creation_end\" < @CreationEnd AND \"range\" IN (2,4,8) ORDER BY \"Name\" ASC, \"Id\" DESC LIMIT 16"));
                Assert.That(result.Item2.ParameterNames.Count(), Is.EqualTo(7));
            });
        }

        [Test]
        public void SelectGenericJsonModelTest()
        {
            var model = new SelectGenericTest { Id = 1, Name = "ABC%", Status = EntityStatus.Completed, CreationStart = new DateTime(2021, 1, 1), CreationEnd = new DateTime(2021, 1, 31), Ranges = [2, 4, 8], QueryPaging = new QueryPagingData { BatchSize = 16, OrderBy = [new() { Field = "Name" }, new() { Field = "Id", Desc = true }] } };
            var result = model.Default.SelectResult2.CreateSqlSelectJson(db);
            Assert.Multiple(() =>
            {
                Assert.That(result.Item1, Is.EqualTo("SELECT json_object('creation', creation, 'id', id, 'name', name, 'status', status) FROM (SELECT \"creation\" AS creation, \"id\" AS id, \"name\" AS name, \"status\" AS status FROM \"User\" WHERE \"id\" = @Id AND \"name\" LIKE @Name AND \"entity_status\" <> @Status AND \"is_deleted\" IS NULL AND \"creation_start\" >= @CreationStart AND \"creation_end\" < @CreationEnd AND \"range\" IN (2,4,8) ORDER BY \"Name\" ASC, \"Id\" DESC LIMIT 16)"));
                Assert.That(result.Item2.ParameterNames.Count(), Is.EqualTo(7));
            });
        }

        [Test]
        public void ParserInnerFieldsTest()
        {
            var fields = InsertTest.ParserInnerFields;
            Assert.Multiple(() =>
            {
                Assert.That(fields.ToArray()[1], Is.EqualTo("entityStatus AS Status"));
            });
        }
    }
}
