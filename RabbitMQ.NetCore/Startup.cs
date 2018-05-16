using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NetRabbitMQ;
using Microsoft.Extensions.Configuration;

namespace RabbitMQ.NetCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; set; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
