namespace TestWebApp
{
    using System.IO;
    using Microsoft.AspNetCore.Hosting;
    using Serilog;
    using Serilog.Formatting.Compact;

    public class Program
    {
        public static void Main(string[] args)
        {
            new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .ConfigureLogging((hostingContext, logging) =>
                {
                    var loggerConfiguration = new LoggerConfiguration()
                        .ReadFrom.Configuration(hostingContext.Configuration)
                        .WriteTo.Console(new RenderedCompactJsonFormatter())
                        .Enrich.FromLogContext()
                        .Enrich.WithMachineName()
                        .Enrich.WithThreadId()
                        .Enrich.WithEnvironmentUserName();

                    var logger = Log.Logger = loggerConfiguration.CreateLogger();

                    logging.AddSerilog(logger);
                })
                .UseStartup<Startup>()
                .Build()
                .Run();
        }
    }
}
