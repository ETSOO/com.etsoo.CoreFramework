namespace com.etsoo.Database
{
    /// <summary>
    /// SQL update interface
    /// SQL更新接口
    /// </summary>
    public interface ISqlUpdate : IUpdateModel
    {
        /// <summary>
        /// Create SQL update command
        /// 创建SQL更新命令
        /// </summary>
        /// <param name="db">Database</param>
        /// <returns>Result</returns>
        (string, IDbParameters) CreateSqlUpdate(IDatabase db);

        /// <summary>
        /// Do SQL update
        /// 执行SQL更新
        /// </summary>
        /// <param name="db">Database</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Rows affected</returns>
        Task<int> DoSqlUpdateAsync(IDatabase db, CancellationToken cancellationToken = default);
    }
}
