﻿using com.etsoo.Utils.Database;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace com.etsoo.CoreFramework.Database
{
    /// <summary>
    /// SQL Server database
    /// SQL Server 数据库
    /// </summary>
    public sealed class SqlServerDatabase : CommonDatabase<SqlConnection>
    {
        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="snakeNaming">Is snake naming</param>
        public SqlServerDatabase(string connectionString, bool snakeNaming = false) : base(connectionString, snakeNaming)
        {
        }

        /// <summary>
        /// New database connection
        /// 新数据库链接对象
        /// </summary>
        /// <returns>Connection</returns>
        public override SqlConnection NewConnection()
        {
            return new(ConnectionString);
        }

        /// <summary>
        /// New database context
        /// 新数据库上下文
        /// </summary>
        /// <typeparam name="M">Generic context class</typeparam>
        /// <returns>Context</returns>
        public override SqlServerDbContext<M> NewDbContext<M>() where M : class
        {
            return new(ConnectionString, SnakeNaming);
        }

        /// <summary>
        /// Convert id list to parameter value
        /// 转换编号列表为参数值
        /// </summary>
        /// <typeparam name="T">Id generic</typeparam>
        /// <param name="ids">Id list</param>
        /// <returns>Parameter value</returns>
        public override object AsListParameter<T>(IEnumerable<T> ids)
        {
            // Type => SqlDbType, like Int
            var type = SqlServerUtil.DbTypeToSql(DatabaseUtil.TypeToDbType(typeof(T)).GetValueOrDefault());

            // Parameter UDT name
            var udt = $"et_{type.ToString().ToLower()}_ids";

            return SqlServerUtil.ListToIdRecords(ids, type).AsTableValuedParameter(udt);
        }
    }
}
