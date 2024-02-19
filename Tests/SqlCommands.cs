using com.etsoo.CoreFramework.Business;
using com.etsoo.SourceGenerators;
using com.etsoo.SourceGenerators.Attributes;

namespace Tests
{
    [SqlDeleteCommand("User", NamingPolicy.SnakeCase, Database = DatabaseName.SQLite)]
    public partial record DeleteTest
    {
        public int Id { get; init; }

        [SqlColumn(QuerySign = SqlQuerySign.Like)]
        public string? Name { get; init; }

        [SqlColumn(ColumnName = "entity_status", QuerySign = SqlQuerySign.NotEqual)]
        public EntityStatus Status { get; init; }

        [SqlColumn(KeepNull = true)]
        public bool? IsDeleted { get; init; }

        [SqlColumn(Ignore = true)]
        public DateTime Creation { get; init; }

        [SqlColumn(QuerySign = SqlQuerySign.GreaterOrEqual)]
        public DateTime? CreationStart { get; init; }

        [SqlColumn(QuerySign = SqlQuerySign.Less)]
        public DateTime? CreationEnd { get; init; }

        [SqlColumn(ColumnName = "range")]
        public IEnumerable<int>? Ranges { get; init; }
    }

    [SqlInsertCommand("customer", NamingPolicy.CamelCase, Database = DatabaseName.SQLServer | DatabaseName.SQLite)]
    public partial record InsertTest
    {
        public required string Name { get; init; }

        [SqlColumn(ColumnName = "entityStatus")]
        public EntityStatus Status { get; init; }

        public bool? IsDeleted { get; init; }

        public DateTime Creation { get; init; } = new DateTime(2024, 2, 17);
    }

    [SqlUpdateCommand("customer", NamingPolicy.CamelCase, Database = DatabaseName.SQLServer | DatabaseName.SQLite)]
    public partial record UpdateTest
    {
        [SqlColumn(Key = true)]
        public int Id { get; init; }

        public string? Name { get; init; }

        [SqlColumn(ColumnName = "entityStatus", Key = true, QuerySign = SqlQuerySign.NotEqual)]
        public EntityStatus Status { get; init; }

        public bool? IsDeleted { get; init; }

        [SqlColumn(Ignore = true)]
        public DateTime Creation { get; init; }
    }

    [SqlSelectCommand("User", NamingPolicy.SnakeCase, Database = DatabaseName.SQLite)]
    public partial record SelectTest
    {
        public int? Id { get; init; }

        [SqlColumn(QuerySign = SqlQuerySign.Like)]
        public string? Name { get; init; }

        [SqlColumn(ColumnName = "entity_status", QuerySign = SqlQuerySign.NotEqual)]
        public EntityStatus Status { get; init; }

        [SqlColumn(KeepNull = true)]
        public bool? IsDeleted { get; init; }

        [SqlColumn(Ignore = true)]
        public DateTime Creation { get; init; }

        [SqlColumn(QuerySign = SqlQuerySign.GreaterOrEqual)]
        public DateTime? CreationStart { get; init; }

        [SqlColumn(QuerySign = SqlQuerySign.Less)]
        public DateTime? CreationEnd { get; init; }

        [SqlColumn(ColumnName = "range")]
        public IEnumerable<int>? Ranges { get; init; }
    }
}
