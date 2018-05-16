using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace NetRabbitMQ
{
    public class NetRabbitMQAttribute:Attribute
    {
        /// <summary>
        /// 消息队列名称
        /// </summary>
       public string Queue { get; set; }

        /// <summary>
        /// 是否自动确认消费 
        /// </summary>
        public bool IsAutoAck { get; set; } = false;

    }
}
