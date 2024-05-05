namespace com.etsoo.Database
{
    /// <summary>
    /// SQL delete interface
    /// SQL删除接口
    /// </summary>
    public interface ISqlDelete
    {
        /// <summary>
        /// Create SQL delete command
        /// 创建SQL删除命令
        /// </summary>
        /// <param name="db">Database</param>
        /// <returns>Result</returns>
        (string, IDbParameters) CreateSqlDelete(IDatabase db);

        /// <summary>
        /// Do SQL delete
        /// 执行SQL删除
        /// </summary>
        /// <param name="db">Database</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Rows affected</returns>
        Task<int> DoSqlDeleteAsync(IDatabase db, CancellationToken cancellationToken = default);
    }
}