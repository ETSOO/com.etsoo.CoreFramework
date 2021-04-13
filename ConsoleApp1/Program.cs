using com.etsoo.CoreFramework.MessageQueue;
using com.etsoo.CoreFramework.Models;
using com.etsoo.CoreFramework.Properties;
using com.etsoo.Utils.Localization;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;

namespace ConsoleApp1
{
    class Program
    {
        static readonly string connectionString = "Server=(local);User ID=smarterp;Password=smarterp;";

        static readonly ConnectionFactory factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest", DispatchConsumersAsync = true };

        static async Task Main(string[] args)
        {
            //await Task.CompletedTask;
            Resources.Culture = LocalizationUtil.SetCulture("zh-CN");
            Console.WriteLine(Resources.ResourceManager.GetString("NoUserFound"));

            Resources.Culture = LocalizationUtil.SetCulture("en-US");
            Console.WriteLine(Resources.NoUserFound);

            //PRCParallelCalls();
            //await RPCCallsAsync();
        }

        static void PRCParallelCalls()
        {
            // Queue
            var queue = "rpc_queue";

            // RPC Server
            using var server = new RabbitMQEx(factory);
            server.PreparePRCServer((body, routingKey, redelivered) => {
                var message = Encoding.UTF8.GetString(body.ToArray());
                //Console.WriteLine($"Message received at {DateTime.UtcNow:ss:fff} from {routingKey} : {message}");

                var bytes = Encoding.UTF8.GetBytes("Echo: " + message);
                return Task.FromResult(new ReadOnlyMemory<byte>(bytes));
            }, queue);


            var inputs = new string[] { "a1", "a2", "a3", "a4", "a5", "a6", "a7", "a8", "a9", "a10" };

            // Each thread a channel
            Parallel.ForEach(inputs, async (input) =>
            {
                // PRC Client
                using var client = new RabbitMQEx(server.Connection);
                client.PreparePRCClient();

                Console.WriteLine($"Message sent at {DateTime.UtcNow:ss:fff}: {input}");
                var result = await client.PRCCallAsync(Encoding.UTF8.GetBytes(input), queue);
                Console.WriteLine($"Client {Thread.CurrentThread.ManagedThreadId} received at {DateTime.UtcNow:ss:fff} for {input}: {Encoding.UTF8.GetString(result.Span)}");
            });

            Console.ReadKey();
        }

        static async Task RPCCallsAsync()
        {
            // Queue
            var queue = "rpc_queue";

            // RPC Server
            using var server = new RabbitMQEx(factory);
            server.PreparePRCServer((body, routingKey, redelivered) => {
                var message = Encoding.UTF8.GetString(body.ToArray());
                // Console.WriteLine($"Message received at {DateTime.UtcNow:ss:fff} from {routingKey} : {message}");

                var bytes = Encoding.UTF8.GetBytes("Echo: " + message);
                return Task.FromResult(new ReadOnlyMemory<byte>(bytes));
            }, queue);

            // PRC Client
            using var client = new RabbitMQEx(server.Connection);
            client.PreparePRCClient();

            var inputs = new string[] { "a1", "a2", "a3", "a4", "a5", "a6", "a7", "a8", "a9", "a10" };

            foreach (var input in inputs)
            {
                Console.WriteLine($"Message sent at {DateTime.UtcNow:ss:fff}: {input}");
                var result = await client.PRCCallAsync(Encoding.UTF8.GetBytes(input), queue);
                Console.WriteLine($"Client received at {DateTime.UtcNow:ss:fff} for {input}: {Encoding.UTF8.GetString(result.Span)}");
            }

            Console.ReadKey();
        }

        static void TableDependency()
        {
            // Exchange
            var exchange = "topic_logqueue";

            // Producer
            using var producer = new RabbitMQEx(factory);
            producer.PrepareProduce(exchange, durable: true);
            producer.ProduceConfirm((deliveryTag, multiple, success) => {
                Console.WriteLine($"Message acknowledged at {DateTime.UtcNow:ss:fff} for {deliveryTag}: {success}");
                return Task.CompletedTask;
            });

            // Consumer
            using var consumer = new RabbitMQEx(factory);
            consumer.PrepareConsume((body, routingKey, redelivered) =>
            {
                var message = Encoding.UTF8.GetString(body.ToArray());
                Console.WriteLine($"Message received at {DateTime.UtcNow:ss:fff} from {routingKey} : {message}");
                return Task.FromResult(true);
            }, exchange, routingKey: "system.test", durable: true, autoAck: false);

            // Definition, Insert change only
            using var dep = new SqlTableDependency<LogQueue>(connectionString, "SysLogQueue", notifyOn: DmlTriggerType.Insert);

            // Change callback
            dep.OnChanged += (object sender, RecordChangedEventArgs<LogQueue> e) =>
            {
                var entity = e.Entity;

                Console.WriteLine("--------------------");
                Console.WriteLine($"Change received at {DateTime.UtcNow:ss:fff}, happened at {entity.Creation:ss:fff}");
                Console.WriteLine($"Data: {entity.Data}");

                // Body
                var body = entity.Data == null ? Array.Empty<byte>() : Encoding.UTF8.GetBytes(entity.Data);

                // Produce
                producer.Produce(body, exchange, "system.test", true);

                Console.WriteLine("Message pushed...");
            };

            // Start
            dep.Start();

            // Tip
            Console.WriteLine("Service is running. Press a key to exit");
            Console.ReadKey();

            // Stop
            dep.Stop();
        }
    }
}
