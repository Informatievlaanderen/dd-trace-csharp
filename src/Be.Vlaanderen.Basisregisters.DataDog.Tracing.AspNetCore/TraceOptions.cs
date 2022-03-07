namespace Be.Vlaanderen.Basisregisters.DataDog.Tracing.AspNetCore
{
    using System;
    using Microsoft.AspNetCore.Http;

    public class TraceOptions
    {
        public string ServiceName { get; set; } = "web";

        public Func<HttpRequest, TraceSource> TraceSource { get; set; }

        public Func<string, bool> ShouldTracePath { get; set; }

        public bool AnalyticsEnabled { get; set; }
        public bool LogForwardedForEnabled { get; set; }
    }
}
