namespace Be.Vlaanderen.Basisregisters.DataDog.Tracing.SqlStreamStore.Microsoft
{
    using DependencyInjection;
    using global::Microsoft.Extensions.DependencyInjection;
    using global::SqlStreamStore;

    public class TraceSqlStreamStoreModule : IServiceCollectionModule
    {
        private readonly string _serviceName;

        public TraceSqlStreamStoreModule(string serviceName) => _serviceName = serviceName;

        public void Load(IServiceCollection services)
        {
            services.AddSingleton<TraceStreamStore>(_ => new TraceStreamStore(_.GetRequiredService<MsSqlStreamStore>(), _serviceName));
            services.AddSingleton<IStreamStore>(_ => new TraceStreamStore(_.GetRequiredService<MsSqlStreamStore>(), _serviceName));
        }
    }
}
