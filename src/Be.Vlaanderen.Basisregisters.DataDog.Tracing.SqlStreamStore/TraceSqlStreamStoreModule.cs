namespace Be.Vlaanderen.Basisregisters.DataDog.Tracing.SqlStreamStore
{
    using Autofac;
    using global::SqlStreamStore;

    public class TraceSqlStreamStoreModule : Module
    {
        private readonly string _serviceName;

        public TraceSqlStreamStoreModule(string serviceName) => _serviceName = serviceName;

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(context => new TraceStreamStore(context.Resolve<MsSqlStreamStore>(), _serviceName))
                .As<TraceStreamStore>()
                .As<IStreamStore>()
                .SingleInstance();
        }
    }
}
