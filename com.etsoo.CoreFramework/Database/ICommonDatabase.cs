﻿using com.etsoo.CoreFramework.ActionResult;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace com.etsoo.CoreFramework.Database
{
    /// <summary>
    /// Common Database Interface
    /// 通用数据库接口
    /// </summary>
    public interface ICommonDatabase
    {
        /// <summary>
        /// Connection string
        /// 链接字符串
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// Data parameter parser
        /// 数据参数解析器
        /// </summary>
        IDataParameterParser? DataParameterParser { get; set; }

        /// <summary>
        /// Add data parameter
        /// 添加数据列表参数
        /// </summary>
        /// <typeparam name="T">Type generic</typeparam>
        /// <param name="items">Data list</param>
        /// <param name="parameters">Operation parameters</param>
        /// <param name="name">Parameter name</param>
        /// <param name="hasRowIndex">Has row index</param>
        void AddDataParameter<T>(IEnumerable<T> items, Dictionary<string, dynamic> parameters, string name, bool? hasRowIndex) where T : IComparable;

        /// <summary>
        /// Create EF Database Context
        /// 创建EF数据库上下文
        /// </summary>
        /// <typeparam name="M">Model class</typeparam>
        /// <returns>Database Context</returns>
        CommonDbContext<M> CreateContext<M>() where M : class, new();

        /// <summary>
        /// Async Execute SQL Command, return rows affacted
        /// 异步执行SQL命令，返回影响的行数
        /// </summary>
        /// <param name="sql">SQL Command</param>
        /// <param name="isStoredProcedure">Is stored procedure</param>
        /// <returns>Rows affacted</returns>
        Task<int> ExecuteAsync(string sql, bool? isStoredProcedure = false);

        /// <summary>
        /// Async Execute SQL Command, return rows affacted
        /// 异步执行SQL命令，返回影响的行数
        /// </summary>
        /// <param name="sql">SQL Command</param>
        /// <param name="paras">Parameters</param>
        /// <param name="isStoredProcedure">Is stored procedure</param>
        /// <returns>Rows affacted</returns>
        Task<int> ExecuteAsync(string sql, IDictionary<string, dynamic>? paras, bool? isStoredProcedure = false);

        /// <summary>
        /// Async Execute SQL Command to return first row first column value
        /// 异步执行SQL命令，返回第一行第一列的值
        /// </summary>
        /// <param name="sql">SQL Command</param>
        /// <param name="isStoredProcedure">Is stored procedure</param>
        /// <returns>First row first column's value</returns>
        Task<object?> ExecuteScalarAsync(string sql, bool? isStoredProcedure = false);

        /// <summary>
        /// Async Execute SQL Command to return first row first column value
        /// 异步执行SQL命令，返回第一行第一列的值
        /// </summary>
        /// <param name="sql">SQL Command</param>
        /// <param name="paras">Parameters</param>
        /// <param name="isStoredProcedure">Is stored procedure</param>
        /// <returns>First row first column's value</returns>
        Task<object?> ExecuteScalarAsync(string sql, IDictionary<string, dynamic>? paras, bool? isStoredProcedure = false);

        /// <summary>
        /// Async execute SQL Command to return operation result
        /// 异步执行SQL命令，返回操作结果对象
        /// </summary>
        /// <param name="sql">SQL Command</param>
        /// <param name="isStoredProcedure">Is stored procedure</param>
        /// <returns>Operation result</returns>
        Task<ActionResult<T, F>?> ExecuteResultAsync<T, F>(string sql, bool? isStoredProcedure = false);

        /// <summary>
        /// Async execute SQL Command to return operation result
        /// 异步执行SQL命令，返回操作结果对象
        /// </summary>
        /// <param name="sql">SQL Command</param>
        /// <param name="paras">Parameters</param>
        /// <param name="isStoredProcedure">Is stored procedure</param>
        /// <returns>Operation result</returns>
        Task<ActionResult<T, F>?> ExecuteResultAsync<T, F>(string sql, IDictionary<string, dynamic>? paras, bool? isStoredProcedure = false);

        /// <summary>
        /// Async Execute SQL Command, write to stream of the first row first column value, used to read huge text data like json/xml
        /// 异步执行SQL命令，读取第一行第一列的数据到流，用于读取大文本字段，比如返回的JSON/XML数据
        /// </summary>
        /// <param name="stream">Stream to write</param>
        /// <param name="sql">SQL Command</param>
        /// <param name="isStoredProcedure">Is stored procedure</param>
        /// <returns>Is content wrote</returns>
        Task<bool> ExecuteToStreamAsync(Stream stream, string sql, bool? isStoredProcedure = false);

        /// <summary>
        /// Async Execute SQL Command, write to stream of the first row first column value, used to read huge text data like json/xml
        /// 异步执行SQL命令，读取第一行第一列的数据到流，用于读取大文本字段，比如返回的JSON/XML数据
        /// </summary>
        /// <param name="stream">Stream to write</param>
        /// <param name="sql">SQL Command</param>
        /// <param name="paras">Parameters</param>
        /// <param name="isStoredProcedure">Is stored procedure</param>
        /// <returns>Is content wrote</returns>
        Task<bool> ExecuteToStreamAsync(Stream stream, string sql, IDictionary<string, dynamic>? paras, bool? isStoredProcedure = false);
    }
}