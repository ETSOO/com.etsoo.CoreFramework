namespace com.etsoo.Database
{
    /// <summary>
    /// SQL insert interface
    /// SQL插入接口
    /// </summary>
    public interface ISqlInsert
    {
        /// <summary>
        /// Create SQL insert command
        /// 创建SQL插入命令
        /// </summary>
        /// <param name="db">Database</param>
        /// <returns>Result</returns>
        (string, IDbParameters) CreateSqlInsert(IDatabase db);

        /// <summary>
        /// Do SQL insert
        /// 执行SQL插入
        /// </summary>
        /// <typeparam name="T">Generic return id type</typeparam>
        /// <param name="db">Database</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Rows affected</returns>
        Task<T?> DoSqlInsertAsync<T>(IDatabase db, CancellationToken cancellationToken = default);
    }
}
