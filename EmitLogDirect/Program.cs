using RabbitMQ.Client;
using System;
using System.Text;

namespace EmitLogDirect
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var conn = RabbitmqHelper.GetConnection())
            {
                using (var channel = conn.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);
                    var message = $"send {(args.Length<=0?"info":args[0] ?? "info")} log";
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicQos(0, 1, false);
                    string routingKey = (args.Length <= 0 ? "info" : args[0] ?? "info");
                    channel.BasicPublish(exchange: "direct_logs", routingKey: routingKey, body: body);
                    Console.WriteLine(" [x] Sent {0}", message);
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
