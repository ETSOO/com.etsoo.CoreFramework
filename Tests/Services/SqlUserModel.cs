using com.etsoo.CoreFramework.Business;
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

    [AutoDataReaderGenerator(UtcDateTime = true)]
    internal partial record UserData
    {
        public int Id { get; init; }
        public required string Name { get; init; }
        public EntityStatus? Status { get; init; }
    }
}
