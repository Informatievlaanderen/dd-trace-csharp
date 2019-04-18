namespace Be.Vlaanderen.Basisregisters.DataDog.Tracing
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public class TraceAgent : IObserver<Trace>
    {
        private const int DefaultQueueCapacity = 200;

        private static readonly Encoding Encoding = new UTF8Encoding(false);
        private static readonly JsonSerializer Serializer = new JsonSerializer();
        private static readonly MediaTypeHeaderValue ContentHeader = new MediaTypeHeaderValue("application/json");

        private readonly ILogger _logger;
        private readonly ITargetBlock<RootSpan> _block;
        private readonly HttpClient _client = new HttpClient();

        public TraceAgent()
            : this(null, DefaultQueueCapacity, null) { }

        public TraceAgent(Uri baseUrl)
            : this(baseUrl, DefaultQueueCapacity, null) { }

        public TraceAgent(Uri baseUrl, int queueCapacity)
            : this(baseUrl, queueCapacity, null) { }

        public TraceAgent(
            Uri baseUrl,
            int queueCapacity,
            ILogger logger)
        {
            _logger = logger;
            _client.BaseAddress = baseUrl ?? new Uri("http://localhost:8126");

            var transform = new TransformBlock<RootSpan, byte[]>(SerializeTraces, new ExecutionDataflowBlockOptions { BoundedCapacity = queueCapacity });
            var send = new ActionBlock<byte[]>(PutTraces, new ExecutionDataflowBlockOptions { BoundedCapacity = queueCapacity });
            transform.LinkTo(send, new DataflowLinkOptions { PropagateCompletion = true });
            _block = transform;
            Completion = send.Completion;
        }

        public Task Completion { get; }

        public void OnCompleted() => _block.Complete();

        public void OnError(Exception error) => _block.Fault(error);

        public void OnNext(Trace value) => _block.Post(value.Root);

        private byte[] SerializeTraces(RootSpan trace)
        {
            if (_logger?.IsEnabled(LogLevel.Debug) ?? false)
            {
                using (var writer = new StringWriter())
                {
                    Serializer.Serialize(writer, new[] { trace.Spans });
                    writer.Flush();
                    _logger?.LogDebug("Preparing to put {SpanJson}", writer.ToString());
                }
            }

            using (var ms = new MemoryStream())
            using (var writer = new StreamWriter(ms, Encoding))
            {
                Serializer.Serialize(writer, new[] { trace.Spans });
                writer.Flush();
                return ms.ToArray();
            }
        }

        private async Task PutTraces(byte[] tracesBody)
        {
            try
            {
                var content = new ByteArrayContent(tracesBody);
                content.Headers.ContentType = ContentHeader;
                using (var response = await _client.PutAsync("/v0.3/traces", content))
                {
                    _logger?.LogDebug("PUT responded with status {StatusCode}", response.StatusCode);
                    _logger?.LogDebug("PUT responded with {Body}", await response.Content.ReadAsStringAsync());

                    if (!response.IsSuccessStatusCode)
                        _logger?.LogError("HTTP {StatusCode} from PUT /v0.3/traces", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(0, ex, "PUT /v0.3/traces failed");
            }
        }
    }
}
