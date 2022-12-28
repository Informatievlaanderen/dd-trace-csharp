namespace Be.Vlaanderen.Basisregisters.DataDog.Tracing.Microsoft
{
    using System;
    using Tracing;
    using DependencyInjection;
    using global::Microsoft.Extensions.Configuration;
    using global::Microsoft.Extensions.DependencyInjection;
    using global::Microsoft.Extensions.Logging;

    public class DataDogModule : IServiceCollectionModule
    {
        private readonly bool _dataDog;
        private readonly bool _debugDataDog;
        private readonly string _hostIp;

        public DataDogModule(IConfiguration configuration)
        {
            if (!bool.TryParse(configuration["Datadog:Enabled"], out _dataDog))
                _dataDog = false;

            if (!bool.TryParse(configuration["Datadog:Debug"], out _debugDataDog))
                _debugDataDog = false;

            _hostIp = configuration["DataDog:HostIp"];
        }

        public void Load(IServiceCollection services)
        {
            services.AddTransient(_ => new ApiDataDogToggle(_dataDog));
            services.AddTransient(_ => new ApiDebugDataDogToggle(_debugDataDog));

            if (!_dataDog)
            {
                return;
            }

            services
                .AddSingleton<TraceAgent>(c =>
                {
                    var loggerFactory = c.GetRequiredService<ILoggerFactory>();

                    return new TraceAgent(
                        string.IsNullOrWhiteSpace(_hostIp)
                            ? null
                            : new Uri($"http://{_hostIp}:8126"),
                        200,
                        loggerFactory.CreateLogger<DataDogModule>());
                });

            services
                .AddSingleton<Func<long, TraceSource>>(c =>
                {
                    return traceId =>
                    {
                        var source = new TraceSource(traceId);
                        source.Subscribe(c.GetRequiredService<TraceAgent>());
                        return source;
                    };
                });

            services
                .AddSingleton<Func<TraceSourceArguments, TraceSource>>(c =>
                {
                    return args =>
                    {
                        var source = new TraceSource(args);
                        source.Subscribe(c.GetRequiredService<TraceAgent>());
                        return source;
                    };
                });
        }
    }
}
