using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace NetRabbitMQ
{
    public class NetRabbitMQAssembly
    {
        /// <summary>
        /// 主机地址
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 虚拟地址
        /// </summary>
        public string VirtualHost { get; set; }
        
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public int RequestedHeartbeat { get; set; }

        /// <summary>
        /// 调用程序集名
        /// </summary>
        public string AssemblyName { get; set; }

        public void Start()
        {
            List<Type> list=  GetAllControllerTypes(AssemblyName);
            foreach (var item in list)
            {
                object reflectTest = Activator.CreateInstance(item);
                //反射该controller的所有mq方法             
                List<MethodInfo> controllerMethods = item.GetMethods().Where(w => w.CustomAttributes.Where(it => it.AttributeType == typeof(NetRabbitMQAttribute)).FirstOrDefault() != null).ToList();

                foreach(var action in controllerMethods)
                {
                    var queue = action.GetCustomAttributesData().Where(w=>w.AttributeType == typeof(NetRabbitMQAttribute)).FirstOrDefault();
                    string queueName = string.Empty;
                    bool isAutoAck = false;
                    if (queue != null)
                    {
                        foreach (var par in queue.NamedArguments)
                        {                           
                            if (par.MemberName.ToLower() == "queue")
                            {
                                queueName = par.TypedValue.ToString();
                            }
                            if (par.MemberName.ToLower() == "isautoack")
                            {
                                isAutoAck = Convert.ToBoolean(par.TypedValue.ToString());
                            }

                        }
                    }
                   
                    //配置rabbitmq队列
                    NetRabbitMQStart mq = new NetRabbitMQStart();                
                    mq.HostName = HostName;//RabbitMQ服务在本地运行
                    mq.UserName = UserName;//用户名
                    mq.Password = Password;//密码
                    mq.VirtualHost = VirtualHost;
                    mq.Port = Port;
                    mq.RequestedHeartbeat = RequestedHeartbeat;

                    mq.Queue = queueName;
                    mq.Action = action;
                    mq.Controller = reflectTest;
                    mq.IsAutoAck = isAutoAck;

                    //启动队列
                    mq.Start();
                }
            }
        }

        /// <summary>
        /// Gets all controller types.
        /// </summary>
        /// <returns>all types in an assembly where my controllers can be found</returns>
        private List<Type> GetAllControllerTypes(string AssemblyName)
        {         
            var asm = Assembly.Load(AssemblyName); //System.Reflection.Assembly.GetExecutingAssembly();        
            return asm.GetTypes().Where(w=>w.CustomAttributes.Where(it=>it.AttributeType== typeof(NetRabbitMQAttribute)).FirstOrDefault()!=null).ToList();           
        }
    }
}
