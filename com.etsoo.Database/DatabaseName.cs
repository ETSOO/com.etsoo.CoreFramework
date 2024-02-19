namespace com.etsoo.Database
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

        // MySQL
        MySQL = 2,

        // PostgreSQL
        PostgreSQL = 4,

        // SQLite
        SQLite = 8
    }
}