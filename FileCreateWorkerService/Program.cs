using FileCreateWorkerService;
using FileCreateWorkerService.Models;
using FileCreateWorkerService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                
                // Ek yapýlandýrma kaynaklarý ekleyebilirsiniz
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;
                var rabbitMQConnectionString = configuration.GetConnectionString("RabbitMQ");

                services.AddSingleton<RabbitMQClientService>();
                services.AddSingleton(sp => new ConnectionFactory()
                {
                    Uri = new Uri(rabbitMQConnectionString),
                    DispatchConsumersAsync = true
                });
                services.AddDbContext<AdventureWorks2019Context>(options => {
                    options.UseSqlServer(configuration.GetConnectionString("SqlCon")); });
                services.AddHostedService<Worker>();
            })
            .Build();

        await host.RunAsync();
    }
}

