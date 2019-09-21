using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ReceiveLogsTopic
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var conn=RabbitmqHelper.GetConnection())
            {
                using (var channel= conn.CreateModel())
                {
                    channel.ExchangeDeclare("log_topic",ExchangeType.Topic);
                    var qName = channel.QueueDeclare().QueueName;
                    channel.QueueBind(qName, "log_topic","*.default.#");
                    channel.QueueBind(qName, "log_topic", "#");
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) => 
                    {
                        var body = ea.Body;
                        var msg = Encoding.UTF8.GetString(body);
                        Console.WriteLine($"[reveive msg:]{msg}");
                    };
                    channel.BasicConsume(qName,true,consumer);
                    Console.WriteLine("press [enter] to exit");
                    Console.ReadLine();
                }
            }
        }
    }

    public class MyConsumer : DefaultBasicConsumer
    {
        private readonly IModel _channel;

        public MyConsumer(IModel channel)
        {
            _channel = channel;
        }

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            Console.WriteLine("--consumer msg--");
            Console.WriteLine("consumerTag:"+ consumerTag);
            Console.WriteLine("deliveryTag:"+ deliveryTag);
            Console.WriteLine("redelivered:"+ redelivered);
            Console.WriteLine("exchange:"+ exchange);
            Console.WriteLine("routingKey:"+ routingKey);
            Console.WriteLine("body:" + Encoding.UTF8.GetString(body));
            //requeue重回队列
            _channel.BasicNack(deliveryTag,false,requeue:false);
        }
    }
}
