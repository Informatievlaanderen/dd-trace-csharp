namespace Be.Vlaanderen.Basisregisters.DataDog.Tracing
{
    public class TraceContextSpanSource : ISpanSource
    {
        public static ISpanSource Instance { get; } = new TraceContextSpanSource();

        public ISpan Begin(string name, string serviceName, string resource, string type)
            => TraceContext.Current?.Begin(name, serviceName, resource, type);
    }
}
