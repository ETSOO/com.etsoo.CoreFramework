using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Serialization;
using com.etsoo.Utils.SpanMemory;
using com.etsoo.Utils.String;
using Dapper;
using System.Buffers;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;

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
        /// Check the field is modified with the model
        /// 测试字段是否被修改
        /// </summary>
        /// <param name="model">Model</param>
        /// <param name="field">Test field</param>
        /// <returns>Result</returns>
        public static bool IsModified(this IUpdateModel model, string field)
        {
            return model.ChangedFields?.Contains(field, StringComparer.OrdinalIgnoreCase) is true;
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

        /// <summary>
        /// Etsoo query paging
        /// 亿速思维查询分页
        /// https://learn.microsoft.com/en-us/answers/questions/1336973/in-what-use-cases-are-lambda-expressions-most-util
        /// https://stackoverflow.com/questions/36298868/how-to-dynamically-order-by-certain-entity-properties-in-entity-framework-7-cor
        /// https://www.codeproject.com/Articles/5358166/A-Dynamic-Where-Implementation-for-Entity-Framewor
        /// </summary>
        /// <typeparam name="TSource">Generic queryable collection type</typeparam>
        /// <param name="source">Source</param>
        /// <param name="data">Paging data</param>
        /// <returns>Result</returns>
        [RequiresUnreferencedCode("Expression requires unreferenced code")]
        public static IQueryable<TSource> QueryEtsooPaging<TSource>(this IQueryable<TSource> source, QueryPagingData data)
        {
            if (data.OrderBy?.Count > 0)
            {
                var len = data.OrderBy.Count;

                var expression = source.Expression;

                for (var o = 0; o < len; o++)
                {
                    var orderBy = data.OrderBy.ElementAt(o);

                    var field = orderBy.Key;

                    // Create a parameter expression representing the source type (TSource) with the name "x"
                    var parameter = Expression.Parameter(typeof(TSource), "x");

                    // Create a member expression representing the property/field to order by, using the parameter expression
                    var selector = Expression.PropertyOrField(parameter, field);

                    // Determine the method name for ordering based on whether it is descending and if it is the first order clause
                    var method = orderBy.Value ?
                        (o == 0 ? "OrderByDescending" : "ThenByDescending") : (o == 0 ? "OrderBy" : "ThenBy");

                    // Create a method call expression to call the appropriate OrderBy/ThenBy method on the IQueryable
                    expression = Expression.Call(
                        typeof(Queryable), // The type that contains the static methods (OrderBy, ThenBy, etc.)
                        method, // The method name determined above
                        [source.ElementType, selector.Type], // The generic type arguments for the method
                        expression, // The source IQueryable's expression
                        Expression.Quote(Expression.Lambda(selector, parameter)) // The lambda expression for the selector, quoted to prevent it from being compiled
                    );
                }

                source = source.Provider.CreateQuery<TSource>(expression);
            }

            if (data.Keysets?.Any() is true)
            {
                // Keyset paging
                var len = data.Keysets.Count();

                for (var k = 0; k < len; k++)
                {
                    var keysetItem = data.Keysets.ElementAt(k);
                    var orderBy = data.OrderBy?.ElementAtOrDefault(k) ?? new KeyValuePair<string, bool>("id", true);

                    var field = orderBy.Key;

                    // Create a parameter expression representing the source type (TSource) with the name "x"
                    var parameter = Expression.Parameter(typeof(TSource), "x");

                    // Create a member expression representing the property/field to filter on
                    var member = Expression.PropertyOrField(parameter, field);

                    // Create a constant expression representing the value to compare against
                    var keyset = keysetItem is JsonElement jKey ? jKey.GetValue(member.Type) : keysetItem;
                    var constant = Expression.Constant(keyset);

                    // When the keyset is string or guid, use the CompareTo method
                    var isString = member.Type == typeof(string);
                    if (isString || member.Type == typeof(Guid))
                    {
                        // Call the CompareTo method on the member expression with the constant expression as the argument
                        var compareToMethod = member.Type.GetMethod("CompareTo", [member.Type])!;

                        // Create a method call expression to call the CompareTo method on the member expression
                        var compareToExpression = Expression.Call(member, compareToMethod, constant);

                        // Create a binary expression representing the equality comparison
                        var zero = Expression.Constant(0);

                        var body = orderBy.Value ?
                            ((k + 1) < len ? Expression.LessThanOrEqual(compareToExpression, zero) : Expression.LessThan(compareToExpression, zero)) :
                            ((k + 1) < len ? Expression.GreaterThanOrEqual(compareToExpression, zero) : Expression.GreaterThan(compareToExpression, zero));

                        // Create a lambda expression representing the predicate
                        var predicate = Expression.Lambda<Func<TSource, bool>>(body, parameter);

                        // Call the Where method with the constructed predicate
                        source = source.Where(predicate);
                    }
                    else
                    {
                        // Create a binary expression representing the equality comparison
                        // Unique field of ordering is put in the end
                        var body = orderBy.Value ?
                            ((k + 1) < len ? Expression.LessThanOrEqual(member, constant) : Expression.LessThan(member, constant)) :
                            ((k + 1) < len ? Expression.GreaterThanOrEqual(member, constant) : Expression.GreaterThan(member, constant));

                        // Create a lambda expression representing the predicate
                        var predicate = Expression.Lambda<Func<TSource, bool>>(body, parameter);

                        // Call the Where method with the constructed predicate
                        source = source.Where(predicate);
                    }
                }
            }
            else
            {
                // Offset paging
                // Skip rows
                var currentPage = data.CurrentPage.GetValueOrDefault();
                if (currentPage > 0)
                {
                    source = source.Skip((int)(currentPage - 1) * data.BatchSize);
                }
            }

            // Rows to read
            return source.Take(data.BatchSize);
        }

        /// <summary>
        /// Etsoo query contains
        /// 亿速思维查询包含
        /// </summary>
        /// <typeparam name="TSource">Generic source type</typeparam>
        /// <typeparam name="TKey">Generic key type</typeparam>
        /// <param name="source">Source</param>
        /// <param name="collection">Contain collection</param>
        /// <param name="idSelector">Id selector</param>
        /// <param name="exclude">Exclude instead of contain</param>
        /// <returns>Result</returns>
        [RequiresDynamicCode("Expression requires dynamic code")]
        public static IQueryable<TSource> QueryEtsooContains<TSource, TKey>(this IQueryable<TSource> source, IEnumerable<TKey> collection, Expression<Func<TSource, TKey>> idSelector, bool exclude = false)
        {
            // Get the Contains method from the Enumerable type
#pragma warning disable IL2060 // Call to 'System.Reflection.MethodInfo.MakeGenericMethod' can not be statically analyzed. It's not possible to guarantee the availability of requirements of the generic method.
            var containsMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TKey));
#pragma warning restore IL2060

            // Create a constant expression representing the value to compare against
            var constant = Expression.Constant(collection);

            // Create a method call expression to call the Contains method on the constant expression
            var containsExpression = Expression.Call(containsMethod, constant, idSelector.Body);

            // Create a lambda expression representing the predicate
            Expression predicateExpression = exclude ? Expression.Not(containsExpression) : containsExpression;
            var predicate = Expression.Lambda<Func<TSource, bool>>(predicateExpression, idSelector.Parameters[0]);

            // Call the Where method with the constructed predicate
            return source.Where(predicate);
        }

        /// <summary>
        /// Etsoo query equal
        /// 亿速思维查询相等
        /// </summary>
        /// <typeparam name="TSource">Generic source type</typeparam>
        /// <param name="source">Source</param>
        /// <param name="value">Equal value</param>
        /// <param name="left">Equal left expression</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Result</returns>
        public static IQueryable<TSource> QueryEtsooEqual<TSource>(this IQueryable<TSource> source, object? value, Expression left, ParameterExpression parameter)
        {
            // Create a constant expression representing the value to compare against
            var constant = Expression.Constant(value);

            // Create a binary expression representing the equality comparison
            var body = Expression.Equal(left, constant);

            // Create a lambda expression representing the predicate
            var predicate = Expression.Lambda<Func<TSource, bool>>(body, parameter);

            // Call the Where method with the constructed predicate
            return source.Where(predicate);
        }
    }
}
