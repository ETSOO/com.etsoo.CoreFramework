using com.etsoo.CoreFramework.Business;
using com.etsoo.CoreFramework.Models;
using com.etsoo.SourceGenerators;
using com.etsoo.SourceGenerators.Attributes;

namespace Tests.Services
{
    [SqlInsertCommand("User", NamingPolicy.CamelCase, Database = DatabaseName.SQLServer | DatabaseName.SQLite, Debug = true)]
    internal partial record SqlUserInsert
    {
        public int Id { get; init; }
        public required string Name { get; init; }
        public EntityStatus? Status { get; init; }
    }

    [SqlUpdateCommand("User", NamingPolicy.CamelCase, Database = DatabaseName.SQLServer | DatabaseName.SQLite)]
    internal partial record SqlUserUpdate
    {
        [SqlColumn(Key = true)]
        public int Id { get; init; }
        public string? Name { get; init; }
        public EntityStatus? Status { get; init; }
    }

    [SqlSelectCommand("User", NamingPolicy.CamelCase, Database = DatabaseName.SQLServer | DatabaseName.SQLite)]
    internal partial record SqlUserSelect
    {
        public int Id { get; init; }
    }

    [SqlSelectCommand("User", NamingPolicy.CamelCase, Database = DatabaseName.SQLServer | DatabaseName.SQLite)]
    internal partial record SqlUserTiplist : QueryRQ<int>
    {
        [SqlColumn(ColumnName = "id", QuerySign = SqlQuerySign.NotEqual)]
        public override IEnumerable<int>? ExcludedIds { get; set; }

        [SqlColumn(ColumnName = "name", QuerySign = SqlQuerySign.Like)]
        public override string? Keyword { get; set; }
    }

    [AutoDataReaderGenerator(UtcDateTime = true)]
    internal partial record UserData
    {
        public int Id { get; init; }
        public required string Name { get; init; }
        public EntityStatus? Status { get; init; }
    }

    [SqlSelectGenericCommand("User", NamingPolicy.CamelCase, Database = DatabaseName.SQLServer | DatabaseName.SQLite)]
    [SqlSelectResult(typeof(UserData))]
    internal partial record UserQuery
    {
        public int? Id { get; init; }
        public string? Name { get; init; }
    }
}
