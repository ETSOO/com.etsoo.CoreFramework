using com.etsoo.CoreFramework.ActionResult;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.etsoo.CoreFramework.Database
{
    /// <summary>
    /// SQLite Database
    /// SQLite 数据库
    /// </summary>
    public class SqliteDatabase : CommonDatabase
    {
        /// <summary>
        /// Add parameters to command
        /// DBNull.Value for non-empty NULL
        /// 给命令添加参数
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="paras">Parameters</param>
        public static void AddParameters(SqliteCommand command, IDictionary<string, dynamic>? paras)
        {
            if (paras == null)
                return;

            command.Parameters.AddRange(paras.Where(item => item.Value != null).Select(item =>
            {
                if (item.Value is SqliteParameter p)
                    return p;
                else
                    return new SqliteParameter(item.Key, item.Value);
            }));
        }

        /// <summary>
        /// New connection
        /// 新链接对象
        /// </summary>
        public SqliteConnection NewConnection
        {
            get { return new SqliteConnection(ConnectionString); }
        }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        public SqliteDatabase(string connectionString) : base(connectionString)
        {

        }

        /// <summary>
        /// Create EF Database Context
        /// 创建EF数据库上下文
        /// </summary>
        /// <typeparam name="M">Model class</typeparam>
        /// <returns>Database Context</returns>
        public override CommonDbContext<M> CreateContext<M>()
        {
            return new SqliteDbContext<M>(ConnectionString);
        }

        /// <summary>
        /// Async Execute SQL Command
        /// 异步执行SQL命令
        /// </summary>
        /// <param name="sql">SQL Command</param>
        /// <param name="paras">Parameters</param>
        /// <param name="isStoredProcedure">Is stored procedure</param>
        /// <returns>Rows affacted</returns>
        public override async Task<int> ExecuteAsync(string sql, IDictionary<string, dynamic>? paras, bool? isStoredProcedure = false)
        {
            // Usings
            using var connection = NewConnection;
            using var command = new SqliteCommand(sql, connection);

            // Add parameters
            AddParameters(command, paras);

            // Command type
            if (isStoredProcedure == null)
                await command.PrepareAsync();
            else if (isStoredProcedure.Value)
                command.CommandType = CommandType.StoredProcedure;

            // Open connection
            await connection.OpenAsync();

            // Execute
            var result = await command.ExecuteNonQueryAsync();

            // Close
            await connection.CloseAsync();

            // Return
            return result;
        }

        /// <summary>
        /// Async Execute SQL Command to return first row first column value
        /// 异步执行SQL命令，返回第一行第一列的值
        /// </summary>
        /// <param name="sql">SQL Command</param>
        /// <param name="paras">Parameters</param>
        /// <param name="isStoredProcedure">Is stored procedure</param>
        /// <returns>First row first column's value</returns>
        public override async Task<object?> ExecuteScalarAsync(string sql, IDictionary<string, dynamic>? paras, bool? isStoredProcedure = false)
        {
            // Usings
            using var connection = NewConnection;
            using var command = new SqliteCommand(sql, connection);

            // Add parameters
            AddParameters(command, paras);

            // Command type
            if (isStoredProcedure == null)
                await command.PrepareAsync();
            else if (isStoredProcedure.Value)
                command.CommandType = CommandType.StoredProcedure;

            // Open connection
            await connection.OpenAsync();

            // Execute
            var result = await command.ExecuteScalarAsync();

            // Close
            await connection.CloseAsync();

            // Return
            return result;
        }

        /// <summary>
        /// Async execute SQL Command to return operation result
        /// 异步执行SQL命令，返回操作结果对象
        /// </summary>
        /// <param name="sql">SQL Command</param>
        /// <param name="paras">Parameters</param>
        /// <param name="isStoredProcedure">Is stored procedure</param>
        /// <returns>Operation result</returns>
        public override async Task<ActionResult<T, F>?> ExecuteResultAsync<T, F>(string sql, IDictionary<string, dynamic>? paras, bool? isStoredProcedure = false)
        {
            // Usings
            using var connection = NewConnection;
            using var command = new SqliteCommand(sql, connection);

            // Add parameters
            AddParameters(command, paras);

            // Command type
            if (isStoredProcedure == null)
                await command.PrepareAsync();
            else if (isStoredProcedure.Value)
                command.CommandType = CommandType.StoredProcedure;

            // Open connection
            await connection.OpenAsync();

            return null;
        }

        /// <summary>
        /// Async ESQL Command, write to stream of the first row first column value, used to read huge text data like json/xml
        /// 异步执行SQL命令，读取第一行第一列的数据到流，用于读取大文本字段，比如返回的JSON/XML数据
        /// </summary>
        /// <param name="stream">Stream to write</param>
        /// <param name="sql">SQL Command</param>
        /// <param name="paras">Parameters</param>
        /// <param name="isStoredProcedure">Is stored procedure</param>
        /// <returns>Is content wrote</returns>
        public override async Task<bool> ExecuteToStreamAsync(Stream stream, string sql, IDictionary<string, dynamic>? paras, bool? isStoredProcedure = false)
        {
            // Has content
            var hasContent = false;

            // Usings
            using var connection = NewConnection;
            using var command = new SqliteCommand(sql, connection);

            // Add parameters
            AddParameters(command, paras);

            // Command type
            if (isStoredProcedure == null)
                await command.PrepareAsync();
            else if (isStoredProcedure.Value)
                command.CommandType = CommandType.StoredProcedure;

            // Open connection
            await connection.OpenAsync();

            // Execute
            // Without CommandBehavior.SingleRow, supports multiple rows
            // https://stackoverflow.com/questions/51087037/sql-server-json-truncated-even-when-using-nvarcharmax
            using (var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleResult & CommandBehavior.SingleRow))
            {
                while (await reader.ReadAsync())
                {
                    // Get the TextReader
                    using var textReader = reader.GetTextReader(0);

                    var buffer = new char[2048];
                    int read;
                    while ((read = await textReader.ReadBlockAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        var bytes = Encoding.UTF8.GetBytes(buffer, 0, read);
                        await stream.WriteAsync(new ReadOnlyMemory<byte>(bytes));
                        await stream.FlushAsync();
                    }

                    textReader.Close();
                    textReader.Dispose();

                    hasContent = true;
                }

                await reader.CloseAsync();
                await reader.DisposeAsync();
            }

            // Close
            await connection.CloseAsync();

            return hasContent;
        }
    }
}