﻿namespace com.etsoo.Database
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
        QueryPagingData? QueryPaging { get; set; }

        /// <summary>
        /// Create SQL select command
        /// 创建SQL选择命令
        /// </summary>
        /// <param name="db">Database</param>
        /// <param name="fields">Fields to select</param>
        /// <param name="conditionDelegate">Query condition delegate</param>
        /// <returns>Result</returns>
        (string, IDbParameters) CreateSqlSelect(IDatabase db, IEnumerable<string> fields, SqlConditionDelegate? conditionDelegate = null);

        /// <summary>
        /// Create SQL select as JSON command
        /// 创建SQL选择为JSON命令
        /// </summary>
        /// <param name="db">Database</param>
        /// <param name="fields">Fields to select</param>
        /// <param name="mappingDelegate">Query fields mapping delegate</param>
        /// <param name="conditionDelegate">Query condition delegate</param>
        /// <returns>Result</returns>
        (string, IDbParameters) CreateSqlSelectJson(IDatabase db, IEnumerable<string> fields, SqlMappingDelegate? mappingDelegate = null, SqlConditionDelegate? conditionDelegate = null);

        /// <summary>
        /// Do SQL select
        /// 执行SQL选择
        /// </summary>
        /// <typeparam name="D">Generic selected data type</typeparam>
        /// <param name="db">Database</param>
        /// <param name="callback">Callback before execution</param>
        /// <param name="conditionDelegate">Query condition delegate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task<D[]> DoSqlSelectAsync<D>(IDatabase db, SqlCommandDelegate? callback = null, SqlConditionDelegate? conditionDelegate = null, CancellationToken cancellationToken = default) where D : IDataReaderParser<D>;

    }
}