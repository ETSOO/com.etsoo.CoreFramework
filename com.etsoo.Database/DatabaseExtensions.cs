using com.etsoo.Utils.Actions;
using com.etsoo.Utils.SpanMemory;
using com.etsoo.Utils.String;
using Dapper;
using System.Buffers;
using System.Data;
using System.Data.Common;
using System.Text;

namespace com.etsoo.Database
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
        public static async ValueTask<T?> GetValueAsync<T>(this DbDataReader reader, string name, List<string> names)
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
        /// To specified naming case
        /// 转化为指定命名规则格式
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="namingPolicy">Naming policy</param>
        /// <returns>Result</returns>
        public static string ToNamingCase(this string input, NamingPolicy? namingPolicy)
        {
            if (namingPolicy == null || namingPolicy == NamingPolicy.PascalCase)
                return input;

            return namingPolicy switch
            {
                NamingPolicy.SnakeCase => input.ToSnakeCase().ToString(),
                NamingPolicy.CamelCase => input[..1].ToLower() + input[1..],
                _ => input
            };
        }

        /// <summary>
        /// Async create action result
        /// 异步创建操作结果
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="command">Command</param>
        /// <returns>Action result</returns>
        public static async ValueTask<ActionResult?> QueryAsResultAsync(this DbConnection connection, CommandDefinition command)
        {
            await using var reader = await connection.ExecuteReaderAsync(command, CommandBehavior.SingleResult & CommandBehavior.SingleRow);
            return await ActionResult.CreateAsync(reader);
        }

        /// <summary>
        /// Async execute SQL Command, write to stream of the first row first column value, used to read huge text data like json/xml
        /// 异步执行SQL命令，读取第一行第一列的数据到流，用于读取大文本字段，比如返回的JSON/XML数据
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="command">Command</param>
        /// <param name="writer">Buffer writer, like SharedUtils.GetStream() or PipeWriter</param>
        /// <returns>Is content wrote</returns>
        public static async Task<bool> QueryToStreamAsync(this DbConnection connection, CommandDefinition command, IBufferWriter<byte> writer)
        {
            // The content maybe splitted into severl rows
            await using var reader = await connection.ExecuteReaderAsync(command, CommandBehavior.SingleResult);

            // Has content
            var hasContent = reader.HasRows;

            while (await reader.ReadAsync(command.CancellationToken))
            {
                // Get the TextReader
                using var textReader = reader.GetTextReader(0);

                // Write
                await textReader.ReadAllBytesAsyn(writer);
            }

            return hasContent;
        }

        /// <summary>
        /// Async execute SQL Command with multiple collections, write to stream of the first row first column value, used to read huge text data like json/xml
        /// 异步执行SQL命令，读取多个数据集第一行第一列的数据到流，用于读取大文本字段，比如返回的JSON/XML数据
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="command">Command</param>
        /// <param name="writer">Buffer writer, like SharedUtils.GetStream() or PipeWriter</param>
        /// <param name="format">Data format</param>
        /// <param name="collectionNames">Collection names</param>
        /// <returns>Is content wrote</returns>
        public static async Task<bool> QueryToStreamAsync(this DbConnection connection, CommandDefinition command, IBufferWriter<byte> writer, DataFormat format, IEnumerable<string>? collectionNames = null)
        {
            // Multiple results
            await using var reader = await connection.ExecuteReaderAsync(command, CommandBehavior.Default);

            var i = 1;
            var hasContent = false;
            var count = collectionNames?.Count();

            // JSON/XML starts
            Encoding.UTF8.GetBytes(format.RootStart, writer);

            do
            {
                // Collection node
                // Names like data1, data2, ...
                var name = collectionNames == null || i > count ? $"data{i}" : collectionNames.ElementAt(i - 1);
                Encoding.UTF8.GetBytes(format.CreateElementStart(name, i == 1), writer);

                if (reader.HasRows)
                {
                    hasContent = true;

                    // The content maybe splitted into severl rows
                    while (await reader.ReadAsync(command.CancellationToken))
                    {
                        // NULL may returned
                        if (await reader.IsDBNullAsync(0))
                        {
                            Encoding.UTF8.GetBytes(format.BlankValue, writer);
                            break;
                        }

                        // Get the TextReader
                        using var textReader = reader.GetTextReader(0);

                        // Write
                        await textReader.ReadAllBytesAsyn(writer);
                    }
                }
                else
                {
                    Encoding.UTF8.GetBytes(format.BlankValue, writer);
                }

                // End
                Encoding.UTF8.GetBytes(format.CreateElementEnd(name), writer);

                i++;
            } while (await reader.NextResultAsync(command.CancellationToken));

            // JSON / XML ends
            Encoding.UTF8.GetBytes(format.RootEnd, writer);

            return hasContent;
        }
    }
}
