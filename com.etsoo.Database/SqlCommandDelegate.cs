namespace com.etsoo.Database
{
    /// <summary>
    /// SQL command delegate
    /// SQL 命令委托
    /// </summary>
    public delegate void SqlCommandDelegate(string sql, IDbParameters parameters);
}
