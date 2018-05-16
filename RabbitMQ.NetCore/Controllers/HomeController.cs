using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetRabbitMQ;

namespace RabbitMQ.NetCore.Controllers
{
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
}