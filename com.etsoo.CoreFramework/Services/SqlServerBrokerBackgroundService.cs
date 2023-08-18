using com.etsoo.Database;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;

namespace com.etsoo.CoreFramework.Services
{
    /// <summary>
    /// SQL Server service broker background service
    /// SQL Server 服务代理后台服务
    /// </summary>
    public abstract class SqlServerBrokerBackgroundService : BackgroundService
    {
        /// <summary>
        /// Logger
        /// 日志记录器
        /// </summary>
        protected readonly ILogger logger;

        /// <summary>
        /// Database
        /// 数据库
        /// </summary>
        protected readonly IDatabase<SqlConnection> db;

        readonly string queueCommand;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="db">Database</param>
        /// <param name="queueName">Queue name to listen</param>
        /// <param name="receiveRows">Rows to receive</param>
        /// <param name="waitForTimeoutMS">Miliseconds to wait for timeout</param>
        public SqlServerBrokerBackgroundService(ILogger logger, IDatabase<SqlConnection> db, string queueName, int receiveRows = 10, int waitForTimeoutMS = 10000)
        {
            this.logger = logger;
            this.db = db;

            queueCommand = $"WAITFOR (RECEIVE TOP({receiveRows}) message_type_name, message_body, conversation_handle, service_name, service_contract_name FROM {queueName}), TIMEOUT {waitForTimeoutMS}";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await using var connection = db.NewConnection();
                    await connection.OpenAsync(stoppingToken);

                    await using var transaction = connection.BeginTransaction();

                    await using var command = new SqlCommand(queueCommand, connection);
                    command.Transaction = transaction;

                    await using var reader = command.ExecuteReader();

                    logger.LogInformation("WAITFOR RECEIVE ready for read...");

                    while (await reader.ReadAsync(stoppingToken))
                    {
                        var messageTypeName = reader.GetString(0);
                        var messageBytes = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, (byte[])reader[1]);
                        var messageId = reader.GetGuid(2).ToString();
                        var serviceName = reader.GetString(3);
                        var contractName = reader.GetString(4);

                        await DoWorkAsync(messageTypeName, messageBytes, messageId, serviceName, contractName, stoppingToken);
                    }

                    await reader.CloseAsync();
                    await reader.DisposeAsync();

                    await transaction.CommitAsync(stoppingToken);
                    await transaction.DisposeAsync();

                    await connection.CloseAsync();
                    await connection.DisposeAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "ExecuteAsync Failed");
                }
            }
        }

        /// <summary>
        /// Async do work
        /// 异步执行任务
        /// </summary>
        /// <param name="messageTypeName">Message type name</param>
        /// <param name="messageBytes">Message bytes</param>
        /// <param name="messageId">Message id</param>
        /// <param name="serviceName">Service name</param>
        /// <param name="contractName">Contract name</param>
        /// <param name="stoppingToken">Stopping token</param>
        /// <returns>Task</returns>
        protected abstract Task DoWorkAsync(string messageTypeName, byte[] messageBytes, string messageId, string serviceName, string contractName, CancellationToken stoppingToken);
    }
}
