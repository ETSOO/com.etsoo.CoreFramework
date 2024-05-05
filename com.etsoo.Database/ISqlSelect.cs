namespace com.etsoo.Database
{
    /// <summary>
    /// SQL select interface
    /// SQL选择接口
    /// </summary>
    public interface ISqlSelect
    {
        /// <summary>
        /// Query paging data
        /// 查询分页数据
        /// </summary>
        QueryData? QueryPaging { get; set; }

        /// <summary>
        /// Create SQL select command
        /// 创建SQL选择命令
        /// </summary>
        /// <param name="db">Database</param>
        /// <param name="fields">Fields to select</param>
        /// <returns>Result</returns>
        (string, IDbParameters) CreateSqlSelect(IDatabase db, IEnumerable<string> fields);

        /// <summary>
        /// Create SQL select as JSON command
        /// 创建SQL选择为JSON命令
        /// </summary>
        /// <param name="db">Database</param>
        /// <param name="fields">Fields to select</param>
        /// <returns>Result</returns>
        (string, IDbParameters) CreateSqlSelectJson(IDatabase db, IEnumerable<string> fields);

        /// <summary>
        /// Do SQL select
        /// 执行SQL选择
        /// </summary>
        /// <typeparam name="D">Generic selected data type</typeparam>
        /// <param name="db">Database</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task<D[]> DoSqlSelectAsync<D>(IDatabase db, CancellationToken cancellationToken = default) where D : IDataReaderParser<D>;

    }
}