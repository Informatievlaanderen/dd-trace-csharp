namespace Be.Vlaanderen.Basisregisters.DataDog.Tracing
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    internal class Span : ISpan
    {
        public event Action<Span> BeginChild;

        protected internal bool Sealed;

        [JsonProperty("trace_id")]
        public long TraceId { get; set; }
        [JsonProperty("span_id")]
        public long SpanId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("resource")]
        public string Resource { get; set; }
        [JsonProperty("service")]
        public string Service { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("start")]
        public long Start { get; set; }
        [JsonProperty("duration")]
        public long Duration { get; set; }
        [JsonProperty("parent_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? ParentId { get; set; }
        [JsonProperty("error", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Error { get; set; }
        [JsonProperty("meta", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, string> Meta { get; set; }

        protected virtual void OnBeginChild(Span child) => BeginChild?.Invoke(child);

        protected virtual void OnEnd() { }

        public void Dispose()
        {
            lock (this)
            {
                EnsureNotSealed();
                Duration = Util.GetTimestamp() - Start;
                Sealed = true;
            }

            OnEnd();
        }

        protected void EnsureNotSealed()
        {
            if (Sealed)
                throw new InvalidOperationException("This span has already ended.");
        }

        public ISpan Begin(string name, string serviceName, string resource, string type)
        {
            EnsureNotSealed();

            var child = new Span
            {
                TraceId = TraceId,
                SpanId = Util.NewSpanId(),
                Name = name,
                Resource = resource,
                ParentId = SpanId,
                Type = type,
                Service = serviceName,
                Start = Util.GetTimestamp()
            };

            OnBeginChild(child);
            return child;
        }

        public void SetMeta(string name, string value)
        {
            lock (this)
            {
                EnsureNotSealed();
                (Meta ?? (Meta = new Dictionary<string, string>()))[name] = value;
            }
        }

        public void SetError(Exception ex)
        {
            lock (this)
            {
                EnsureNotSealed();
                Error = 1;

                var meta = Meta ?? (Meta = new Dictionary<string, string>());
                meta["error.msg"] = ex.Message;
                meta["error.type"] = ex.GetType().Name;

                var stack = ex.StackTrace;
                if (stack == null)
                {
                    meta.Remove("error.stack");
                }
                else
                {
                    meta["error.stack"] = stack;
                }
            }
        }
    }
}
