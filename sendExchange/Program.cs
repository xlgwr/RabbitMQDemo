using System.Text;
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace sendExchange
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("Hello World!");
            IConnectionFactory confacotry = new ConnectionFactory
            {
                HostName = "192.168.53.4",
                Port = 5672,
                UserName = "admin",
                Password = "admin"
            };
            using (IConnection con = confacotry.CreateConnection())
            using (IModel channel = con.CreateModel())
            {
                //交换机名称
                string exchangename = string.Empty;
                string routingKey = string.Empty;
                if (args.Length > 0)
                {
                    routingKey = args[0];
                }
                else
                {
                    routingKey = "routingKey_direct";
                }
                exchangename = "exchange2";
                //声明交换机
                channel.ExchangeDeclare(exchange: exchangename, type: ExchangeType.Direct);
                while (true)
                {
                    Console.WriteLine("消息内容：");
                    string message = Console.ReadLine();
                    byte[] body = Encoding.UTF8.GetBytes(message);
                    //send message
                    channel.BasicPublish(exchange: exchangename, routingKey: routingKey,
                    basicProperties: null, body: body);
                    Console.WriteLine("成功发送信息：" + message);
                }
            }
        }
    }
}
