namespace Be.Vlaanderen.Basisregisters.DataDog.Tracing
{
    public class TraceSourceArguments
    {
        public long TraceId { get; }

        public long? ParentSpanId { get; }

        public TraceSourceArguments(long traceId) => TraceId = traceId;

        public TraceSourceArguments(long traceId, long parentSpanId) : this(traceId) => ParentSpanId = parentSpanId;
    }
}
