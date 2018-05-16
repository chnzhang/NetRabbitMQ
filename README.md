# NetRabbitMQ
.net core rabbitmq 注解方式使用
startup config

/// <summary>
/// 配置消息队列
/// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            //配置mq
            NetRabbitMQAssembly mq = new NetRabbitMQAssembly
            {
                HostName = Configuration.GetSection("rabbitmq")["hostname"],
                UserName = Configuration.GetSection("rabbitmq")["username"],
                Password=Configuration.GetSection("rabbitmq")["password"],
                VirtualHost= Configuration.GetSection("rabbitmq")["virtualhost"],
                Port = Convert.ToInt32(Configuration.GetSection("rabbitmq")["port"]),
                RequestedHeartbeat= Convert.ToInt32(Configuration.GetSection("rabbitmq")["requestedheartbeat"]),
                AssemblyName= "RabbitMQ.NetCore"
            };
            //启动mq
            mq.Start();
        }

controller action or class method

/// <summary>
/// 执行消息队列
/// </summary>
[Produces("application/json")]
[Route("api/Home")]
[NetRabbitMQ]
public class HomeController : Controller
{

    /// <summary>
    /// 执行消息队列
    /// </summary>
    /// <param name="message"></param>
    [HttpGet]
    [NetRabbitMQ(Queue = "micro.delay.queue.exchange")]
    public bool TestWrite(string message)
    {
        Console.WriteLine("time:"+DateTime.Now+"msg:"+message);
        return true;
    }
}

appsittings

  "rabbitmq": {
    "hostname": "192.168.1.199",
    "username": "develop",
    "password": "yxt315",
    "port": 5672,
    "virtualhost": "/",
    "requestedheartbeat": 30
  }
