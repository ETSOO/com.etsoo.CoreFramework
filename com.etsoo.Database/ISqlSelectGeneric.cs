namespace com.etsoo.Database
{
    /// <summary>
    /// SQL select generic interface
    /// SQL 通用选择接口
    /// </summary>
    public interface ISqlSelectGeneric
    {
        /// <summary>
        /// Query paging data
        /// 查询分页数据
        /// </summary>
        QueryData? QueryPaging { get; set; }
    }

    /// <summary>
    /// SQL select generic result interface
    /// SQL 通用选择结果接口
    /// </summary>
    /// <typeparam name="D">Generic result type</typeparam>
    public interface ISqlSelectResult<D>
    {
        /// <summary>
        /// Create SQL select command
        /// 创建SQL选择命令
        /// </summary>
        /// <param name="db">Database</param>
        /// <returns>Sql command text and parameters</returns>
        (string, IDbParameters) CreateSqlSelect(IDatabase db);

        /// <summary>
        /// Create SQL select as JSON command
        /// 创建SQL选择为JSON命令
        /// </summary>
        /// <param name="db">Database</param>
        /// <returns>Sql command text and parameters</returns>
        (string, IDbParameters) CreateSqlSelectJson(IDatabase db);

        /// <summary>
        /// Do SQL select command
        /// 执行 SQL选择命令
        /// </summary>
        /// <param name="db">Database</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task<D[]> DoSqlSelectAsync(IDatabase db, CancellationToken cancellationToken = default);
    }
}
