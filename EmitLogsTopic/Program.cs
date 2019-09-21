using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EmitLogTopic
{
    class Program
    {
        static void Main(string[] args)
        {

            using (var conn = RabbitmqHelper.GetConnection())
            {
                using (var channel = conn.CreateModel())
                {
                    //并行(异步回调)
                    //这两个事件如果是using写法要写到创建交换器和队列并且进行绑定之前，否则不会触发
                    channel.BasicNacks += (sender, e) =>
                    {
                        //生产者发送消息到broker（服务器）后失败被生产者的listener监听到，就走无应答方法
                        Console.WriteLine(" --no ack-- ");
                    };

                    channel.BasicAcks += (sender, e) =>
                    {
                        //有应答
                        Console.WriteLine(" --ack-- ");
                    };
                    
                    channel.BasicReturn += (sender, e) =>
                    {
                        //borker 无人消费，或者是错误的，交换机或路由都不能匹配的消息,要设置mandatory
                        Console.WriteLine(" --ruturn ack-- ");
                    };
                    channel.ExchangeDeclare("log_topic", ExchangeType.Topic);
                    var msg = $"send {(args.Length == 0 ? "default" : args[0] ?? "default")} topic message";
                    var body = Encoding.UTF8.GetBytes(msg);
                    channel.ConfirmSelect();//不管是串行还是并行，都要加入声明确认这句话
                    channel.BasicPublish("log_topic", (args.Length < 1 ? "#" : args[1] ?? "#"),true, null, body);
                    Console.WriteLine(" [x] Sent {0}", msg);
                    //串行的写法
                    //if (channel.WaitForConfirms())
                    //{
                    //    Console.WriteLine("[x] Sent success");
                    //}
                    //else
                    //{
                    //    Console.WriteLine("[x] Sent faild");
                    //}
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
