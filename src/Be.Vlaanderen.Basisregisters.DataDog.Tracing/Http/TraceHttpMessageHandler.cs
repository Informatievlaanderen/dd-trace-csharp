namespace Be.Vlaanderen.Basisregisters.DataDog.Tracing.Http
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net.Http;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// For wrapping other HttpMessageHandler implementations for tracing purposes.
    /// </summary>
    public class TraceHttpMessageHandler : HttpMessageHandler
    {
        private const string DefaultServiceName = "http";
        private const string TypeName = "http";

        private readonly HttpMessageHandler _innerHandler;
        private readonly ISpanSource _spanSource;

        private string ServiceName { get; }

        /// <summary>
        /// Creates a new TraceHttpMessageHandler using traces from TraceContext.Current.
        /// </summary>
        /// <param name="innerHandler">The inner http handler to trace.</param>
        public TraceHttpMessageHandler(HttpMessageHandler innerHandler)
            : this(innerHandler, DefaultServiceName, TraceContextSpanSource.Instance) { }

        /// <summary>
        /// Creates a new TraceHttpMessageHandler using traces from TraceContext.Current.
        /// </summary>
        /// <param name="innerHandler">The inner http handler to trace.</param>
        /// <param name="serviceName">The service name.</param>
        public TraceHttpMessageHandler(HttpMessageHandler innerHandler, string serviceName)
            : this(innerHandler, serviceName, TraceContextSpanSource.Instance) { }

        /// <summary>
        /// Creates a new TraceHttpMessageHandler using the specified ISpanSource.
        /// </summary>
        /// <param name="innerHandler">The inner http handler to trace.</param>
        /// <param name="spanSource">The span source to open new spans from.</param>
        public TraceHttpMessageHandler(HttpMessageHandler innerHandler, ISpanSource spanSource)
            : this(innerHandler, DefaultServiceName, spanSource) { }

        /// <summary>
        /// Creates a new TraceHttpMessageHandler using the specified ISpanSource.
        /// </summary>
        /// <param name="innerHandler">The inner http handler to trace.</param>
        /// <param name="serviceName">The service name.</param>
        /// <param name="spanSource">The span source to open new spans from.</param>
        public TraceHttpMessageHandler(HttpMessageHandler innerHandler, string serviceName, ISpanSource spanSource)
        {
            _innerHandler = innerHandler ?? throw new ArgumentNullException(nameof(innerHandler));
            _spanSource = spanSource ?? throw new ArgumentNullException(nameof(spanSource));

            ServiceName = string.IsNullOrWhiteSpace(serviceName)
                ? DefaultServiceName
                : serviceName;
        }

        // works around SendAsync being declared internal protected
        private static readonly Func<HttpMessageHandler, HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> SendFunc = GetSendAsync();

        private static Func<HttpMessageHandler, HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> GetSendAsync()
        {
            var info = typeof(HttpMessageHandler).GetTypeInfo();
            var q = from method in info.DeclaredMethods
                where method.Name == nameof(SendAsync)
                where method.ReturnType == typeof(Task<HttpResponseMessage>)
                let p = method.GetParameters()
                where p.Length == 2 && p[0].ParameterType == typeof(HttpRequestMessage) && p[1].ParameterType == typeof(CancellationToken)
                select method;
            var sendAsyncMethod = q.FirstOrDefault();
            if (sendAsyncMethod == null) throw new InvalidOperationException($"Unable to find SendAsync method on handler of type {nameof(HttpMessageHandler)}");
            var handler = Expression.Parameter(typeof(HttpMessageHandler), "handler");
            var request = Expression.Parameter(typeof(HttpRequestMessage), "request");
            var cancellationToken = Expression.Parameter(typeof(CancellationToken), "cancellationToken");
            return Expression.Lambda<Func<HttpMessageHandler, HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>>(
                Expression.Call(handler, sendAsyncMethod, request, cancellationToken),
                "TrackedSendAsync",
                new[] { handler, request, cancellationToken }
            ).Compile();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var host = request.RequestUri.Host;
            var span = _spanSource.Begin($"HTTP {request.Method.Method}", ServiceName, host, TypeName);

            try
            {
                span?.SetMeta("http.url", request.RequestUri.ToString());
                var response = await SendFunc(_innerHandler, request, cancellationToken);
                span?.SetMeta("http.status_code", ((int)response.StatusCode).ToString());
                return response;
            }
            catch (Exception ex)
            {
                span?.SetError(ex);
                throw;
            }
            finally
            {
                span?.Dispose();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                _innerHandler.Dispose();
        }
    }
}
