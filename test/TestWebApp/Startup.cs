namespace TestWebApp
{
    using System;
    using System.IO;
    using System.Text;
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing;
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing.AspNetCore;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvcCore()
                .AddMvcOptions(mvc =>
                {
                    mvc.EnableEndpointRouting = false;
                    mvc.Filters.Add(new DataDogTracingFilter());
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            var source = new TraceSource(42);
            SetupSourceListener(source);

            app
                .UseDataDogTracing(new TraceOptions {
                    TraceSource = _ => source
                })
                .UseMvc();
        }

        private static void SetupSourceListener(TraceSource source)
        {
            var serializer = new JsonSerializer { Formatting = Formatting.Indented };
            source.Subscribe(t =>
            {
                var sb = new StringBuilder("========== Begin Trace ==========");
                using (var writer = new StringWriter(sb))
                {
                    writer.WriteLine();
                    serializer.Serialize(writer, t);
                    writer.WriteLine("========== End Trace ==========");
                    writer.Flush();
                }
                Console.WriteLine(sb.ToString());
            });
        }
    }
}
