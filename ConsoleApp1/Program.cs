using com.etsoo.ImageUtils.Barcode;
using com.etsoo.MessageQueue;
using com.etsoo.MessageQueue.QueueProcessors;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class RowData
    {
        public DateTime Creation { get; set; }
        public decimal USD { get; set; }
        public decimal EUR { get; set; }
    }

    internal record SimpleData
    {
        public int Num { get; init; }
        public bool Bool { get; init; }
    }

    internal class SimpleProcessor(Action<MessageReceivedProperties, SimpleData?> messageAction) : IMessageQueueProcessor
    {
        private readonly Action<MessageReceivedProperties, SimpleData?> _messageAction = messageAction;

        public bool CanDeserialize(MessageReceivedProperties properties)
        {
            return true;
        }

        public async Task ExecuteAsync(ReadOnlyMemory<byte> body, MessageReceivedProperties properties, CancellationToken cancellationToken)
        {
            var data = await MessageQueueUtils.FromJsonBytesAsync<SimpleData>(body, cancellationToken);
            _messageAction(properties, data);
        }
    }

    class Program
    {
        static readonly string connectionString = "Server=(local);User ID=smarterp;Password=smarterp;";

        static async Task Main(string[] args)
        {
            /*
            var stream = await HtmlUtils.GetStreamAsync(new Uri("https://www.safe.gov.cn/AppStructured/hlw/RMBQuery.do"));
            var items = await HtmlUtils.ParseTable(stream, "table#InfoTable", (titles, data, index) =>
            {
                if (index > 10) return null;

                var creation = SharedUtils.GetAccordingValue<DateTime>(titles, data, "日期", 0);
                var usd = SharedUtils.GetAccordingValue<decimal>(titles, data, "美元", 1);
                var eur = SharedUtils.GetAccordingValue<decimal>(titles, data, "欧元", 2);
                if (creation == null || usd == null || eur == null) return null;

                return new RowData
                {
                    Creation = creation.Value,
                    USD = usd.Value,
                    EUR = eur.Value
                };
            });
            */

            //await CreateQRCode();

            /*
            var consumer = new LocalRabbitMQConsumer(
                new LocalRabbitMQConsumerOptions { QueueName = "SmartERP" },
                new[] { new SimpleProcessor(action) },
                logger
               );
            await consumer.ReceiveAsync(source.Token);
            */

            /*
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole(); // Register console logger provider
            });

            var logger = loggerFactory.CreateLogger<Program>();

            var source = new CancellationTokenSource();

            var action = (MessageReceivedProperties properties, SimpleData? data) =>
            {
            };

            var connectionString = "Endpoint=sb://etsoo-sb-au-east.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=aQKdC0XHHo3wLEEooO30wrZDE6qT1+dbk+ASbAY8vJw=";
            var client = AzureServiceBusUtils.CreateServiceBusSender(new AzureServiceBusProducerOptions
            {
                ConnectionString = connectionString,
                QueueOrTopicName = "smarterpqueue"
            });
            var producer = new AzureServiceBusProducer(client);

            var messageId = await producer.SendJsonAsync(new SimpleData { Num = 1, Bool = true });

            Console.WriteLine("Message id: " + messageId);

            var subscriber = AzureServiceBusUtils.CreateServiceBusProcessor(new AzureServiceBusConsumerOptions
            {
                ConnectionString = connectionString,
                QueueName = "smarterpqueue",
                ProcessorOptions = new ServiceBusProcessorOptions
                {
                    AutoCompleteMessages = false
                }
            });
            var consumer = new AzureServiceBusConsumer(
                subscriber,
                new[] { new SimpleProcessor(action) },
                logger
               );
            await consumer.ReceiveAsync(source.Token);

            source.Cancel();

            await using var stream = File.OpenRead("D:\\a.jpg");
            await using var targetStream = File.OpenWrite("D:\\a1.jpg");
            await ImageSharpUtils.ResizeImageStreamAsync(stream, 500, null, targetStream);
            Console.WriteLine("Done");
            */

            Console.Read();
        }

        public static int solution(int[] A)
        {
            // write your code in C# 6.0 with .NET 4.5 (Mono)
            var total = A.Sum();
            var list = A.Select(a => (double)a).ToList();

            var target = total / 2.0;

            var filters = 0;
            var current = 0D;
            while (current < target)
            {
                var max = list.Max();

                var reduced = max / 2.0;
                var index = list.IndexOf(max);
                list[index] = reduced;

                current += reduced;
            }

            return filters;
        }

        public static string Simplify(string input)
        {
            var chars = input.AsSpan();
            char? last = null;
            var index = 0;
            var newIndex = 0;
            var len = 0;
            Span<char> newChars = new char[chars.Length];

            for (var i = 0; i < chars.Length; i++)
            {
                var c = chars[i];
                if (last == null || !last.Value.Equals(c))
                {
                    last = c;
                    len++;
                }
                else
                {
                    chars.Slice(index, len).CopyTo(newChars.Slice(newIndex, len));
                    newIndex += len;
                    index = i + 1;
                    len = 0;
                }
            }

            chars.Slice(index, len).CopyTo(newChars.Slice(newIndex, len));

            return newChars.Trim('\0').ToString();
        }

        static async Task CreateQRCode()
        {
            var options = new BarcodeOptions
            {
                Type="QRCode",
                Content="https://cn.etsoo.com/",
                ForegroundText="#3f51b5",
                Width = 600,
                Height = 600
            };

            using var stream = File.Create("D:\\test.png");
            await BarcodeUtils.CreateAsync(stream, options);

            Console.Write("QRCode created");
        }

        static void TestTask()
        {
            try
            {
                Console.WriteLine("TestTask at " + DateTime.Now.ToLongTimeString() + ", " + Thread.CurrentThread.ManagedThreadId);

                Thread.Sleep(3000);
                Task.Delay(3000);
                throw new InvalidOperationException();
            }
            catch
            {
                Console.WriteLine("TestTask exception at " + DateTime.Now.ToLongTimeString() + ", " + Thread.CurrentThread.ManagedThreadId);
            }
        }

        static bool IsPalindrome(string s)
        {
            // Remove empties and conver to upper case
            s = Regex.Replace(s, "\\s", "").ToUpper();

            var len = s.Length;

            // Get the middle position, len = 5, mid = 2
            var mid = len / 2;

            for (var i = 0; i < mid; i++)
            {
                // Check the relative chars
                if (!s[i].Equals(s[len - i - 1]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
