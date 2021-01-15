using com.etsoo.Utils.Actions;
using com.etsoo.Utils.SpanMemory;
using com.etsoo.Utils.String;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;

namespace com.etsoo.Utils.Database
{
    /// <summary>
    /// Database extension
    /// 数据库扩展
    /// </summary>
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Get column names from DataReader
        /// 从DataReader获取列名
        /// </summary>
        /// <param name="reader">DataReader</param>
        /// <returns>Names</returns>
        public static IEnumerable<string> GetColumnNames(this IDataReader reader)
        {
            return Enumerable.Range(0, reader.FieldCount).Select(reader.GetName);
        }

        /// <summary>
        /// Async get field value
        /// 异步获取字段值
        /// </summary>
        /// <typeparam name="T">Generic field type</typeparam>
        /// <param name="reader">DataReader</param>
        /// <param name="name">Field name</param>
        /// <param name="names">Fields</param>
        /// <returns>Value</returns>
        public static async Task<T?> GetValueAsync<T>(this DbDataReader reader, string name, List<string> names)
        {
            // Match cases
            var caseName = (name.Contains('_') ? name.ToPascalCase() : name.ToSnakeCase()).ToString();

            // Index
            var index = names.FindIndex(n => n.Equals(name, StringComparison.OrdinalIgnoreCase) || n.Equals(caseName, StringComparison.OrdinalIgnoreCase));

            // No index found
            if (index == -1 || await reader.IsDBNullAsync(index))
            {
                return default;
            }
            else
            {
                return await reader.GetFieldValueAsync<T>(index);
            }
        }

        /// <summary>
        /// Get dictionary from DataReader
        /// 从DataReader获取字典对象
        /// </summary>
        /// <param name="reader">DataReader</param>
        /// <returns>Dictionary</returns>
        public static IEnumerable<StringKeyDictionaryObject> ToDictionary(this IDataReader reader)
        {
            // Column names
            var columns = GetColumnNames(reader).AsList();

            // Keep reading
            while (reader.Read())
            {
                // Dictionary
                var dic = new StringKeyDictionaryObject();

                // Fill fields
                for (var c = 0; c < columns.Count; c++)
                {
                    dic.Add(columns[c], reader[c]);
                }

                // Return
                yield return dic;
            }
        }

        /// <summary>
        /// Async get dictionary from DataReader
        /// 异步从DataReader获取字典对象
        /// </summary>
        /// <param name="reader">DataReader</param>
        /// <returns>Dictionary</returns>
        public static async Task<IEnumerable<StringKeyDictionaryObject>> ToDictionaryAsync(this DbDataReader reader)
        {
            // Collection
            var list = new List<StringKeyDictionaryObject>();

            // Column names
            var columns = GetColumnNames(reader).AsList();

            // Keep reading
            while (await reader.ReadAsync())
            {
                // Dictionary
                var dic = new StringKeyDictionaryObject();

                for (var c = 0; c < columns.Count; c++)
                {
                    dic.Add(columns[c], reader[c]);
                }

                // Add to the collection
                list.Add(dic);
            }

            return list;
        }

        /// <summary>
        /// Async create action result
        /// 异步创建操作结果
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="command">Command</param>
        /// <returns>Action result</returns>
        public static async Task<IActionResult?> QueryAsResultAsync(this DbConnection connection, CommandDefinition command)
        {
            using var reader = await connection.ExecuteReaderAsync(command, CommandBehavior.SingleResult & CommandBehavior.SingleRow);
            return await ActionResult.CreateAsync(reader);
        }

        /// <summary>
        /// Async execute SQL Command, write to stream of the first row first column value, used to read huge text data like json/xml
        /// 异步执行SQL命令，读取第一行第一列的数据到流，用于读取大文本字段，比如返回的JSON/XML数据
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="command">Command</param>
        /// <param name="stream">Stream to write</param>
        /// <returns>Is content wrote</returns>
        public static async Task<bool> QueryToStreamAsync(this DbConnection connection, CommandDefinition command, Stream stream)
        {
            // The content maybe splitted into severl rows
            using var reader = await connection.ExecuteReaderAsync(command, CommandBehavior.SingleResult);

            // Has content
            var hasContent = reader.HasRows;

            while (await reader.ReadAsync())
            {
                // Get the TextReader
                using var textReader = reader.GetTextReader(0);

                // Write
                await textReader.ReadAllBytesAsyn(stream);
            }

            return hasContent;
        }

        /// <summary>
        /// Async execute SQL Command, write to stream of the first row first column value, used to read huge text data like json/xml
        /// 异步执行SQL命令，读取第一行第一列的数据到流，用于读取大文本字段，比如返回的JSON/XML数据
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="command">Command</param>
        /// <param name="writer">Pipe writer</param>
        /// <returns>Is content wrote</returns>
        public static async Task<bool> QueryToStreamAsync(this DbConnection connection, CommandDefinition command, PipeWriter writer)
        {
            // The content maybe splitted into severl rows
            using var reader = await connection.ExecuteReaderAsync(command, CommandBehavior.SingleResult);

            // Has content
            var hasContent = reader.HasRows;

            while (await reader.ReadAsync())
            {
                // Get the TextReader
                using var textReader = reader.GetTextReader(0);

                // Write
                await textReader.ReadAllBytesAsyn(writer);
            }

            return hasContent;
        }
    }
}
