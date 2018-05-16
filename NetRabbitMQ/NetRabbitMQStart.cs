using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NetRabbitMQ
{
    public class NetRabbitMQStart
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public int Port { get; set; }
        public int RequestedHeartbeat { get; set; }

        public string Queue { get; set; }
        public MethodInfo Action { get; set; }
        public object Controller { get; set; }
        public bool IsAutoAck { get; set; }

        public void Start()
        {
            //Console.WriteLine("sart:" + Queue);

            var factory = new ConnectionFactory
            {
                HostName = HostName,
                UserName = UserName,
                Password = Password,
                VirtualHost = VirtualHost,
                Port = Port,
                RequestedHeartbeat = 5
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {

                    channel.QueueDeclare(Queue, true, false, false, null);

                    var consumer = new EventingBasicConsumer(channel);

                    //设置消费方式
                    if (IsAutoAck)
                    {
                        channel.BasicConsume(Queue, true, consumer);//true为自动确认消费
                    }
                    else
                    {
                        channel.BasicConsume(Queue, false, consumer);//false为手动确认消费
                    }

                    consumer.Received += (model, ea) =>
                    {

                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);              
                        var result = Action.Invoke(Controller, new object[] { message });

                        //手动确认消费状态
                        if (!IsAutoAck)
                        {
                            if ((bool)result)
                            {
                                //成功，确认消费
                                channel.BasicAck(ea.DeliveryTag, false);
                            }
                            else
                            {
                                //失败，requeue，true重新进入队列  
                                channel.BasicNack(ea.DeliveryTag, true, true);
                            }
                        }
                        //或者 失败， requeue，true重新进入队列,与basicNack差异缺少multiple参数  
                        //channel.BasicReject(ea.DeliveryTag, true);
                    };
                }
            }

        }
    }
}
