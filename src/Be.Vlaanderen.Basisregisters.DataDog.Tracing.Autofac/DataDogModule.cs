namespace Be.Vlaanderen.Basisregisters.DataDog.Tracing.Autofac
{
    using System;
    using Tracing;
    using global::Autofac;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class DataDogModule : Module
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

        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(c => new ApiDataDogToggle(_dataDog));
            containerBuilder.Register(c => new ApiDebugDataDogToggle(_debugDataDog));

            if (!_dataDog)
                return;

            containerBuilder
                .Register(context =>
                {
                    var loggerFactory = context.Resolve<ILoggerFactory>();

                    return new TraceAgent(
                        string.IsNullOrWhiteSpace(_hostIp)
                            ? null
                            : new Uri($"http://{_hostIp}:8126"),
                        200,
                        loggerFactory.CreateLogger<DataDogModule>());
                })
                .As<TraceAgent>()
                .SingleInstance();

            containerBuilder
                .Register<Func<string, TraceSource>>(c =>
                {
                    var context = c.Resolve<IComponentContext>();

                    return traceId =>
                    {
                        var source = new TraceSource(traceId);
                        source.Subscribe(context.Resolve<TraceAgent>());
                        return source;
                    };
                })
                .As<Func<string, TraceSource>>()
                .SingleInstance();
        }
    }
}
