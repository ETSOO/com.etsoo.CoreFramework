using System;

namespace com.etsoo.SourceGenerators
{
    /// <summary>
    /// Database name
    /// 数据库名称
    /// </summary>
    [Flags]
    public enum DatabaseName : byte
    {
        // SQL Server
        SQLServer = 1,

        // PostgreSQL
        PostgreSQL = 2,

        // SQLite
        SQLite = 4
    }
}
