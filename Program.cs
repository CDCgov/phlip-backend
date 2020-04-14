using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Esquire.Data;
using Serilog;
using Serilog.Events;

namespace Esquire
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File((System.IO.Path.Combine ("logs/esquire-.log")), 
                    outputTemplate: "{Timestamp:o} {RequestId,13} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day, 
                    fileSizeLimitBytes: 30_000_000,
                    rollOnFileSizeLimit: true)
                .CreateLogger();
            
            var host = BuildWebHost(args);
            using (var scope = host.Services.CreateScope()) {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ProjectContext>();
                    DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to get DbContext ProjectContext.");
                    Console.WriteLine(ex);
                }
            }
            host.Run();

        }

        public static IWebHost BuildWebHost(string[] args){
            var configuration = new ConfigurationBuilder().AddCommandLine(args).Build();

            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(configuration)
                .UseStartup<Startup>()
                .UseSerilog()
                .Build();
        }
            
    }
}
