using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace WorkerApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;
                    services.AddHostedService<Worker>();
                    var connectionFactory = new ConnectionFactory()
                    {
                        DispatchConsumersAsync = true,
                        HostName = configuration.GetValue<string>("RabbitMQ:Hostname"),
                        UserName = configuration.GetValue<string>("RabbitMQ:Username"),
                        Password = configuration.GetValue<string>("RabbitMQ:Password"),
                    };
                    services.AddSingleton<ConnectionFactory>(x => { return connectionFactory; });
                });
    }
}
