namespace Be.Vlaanderen.Basisregisters.DataDog.Tracing.AspNetCore
{
    using System;
    using System.Linq;
    using System.Text;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http.Extensions;
    using Tracing;
    using Serilog.Context;

    public static class DataDogTracingApplicationBuilderExtensions
    {
        public const string ApiKeyHeaderName = "x-api-key";
        public const string ApiTokenHeaderName = "x-api-token";

        public static IApplicationBuilder UseDataDogTracing(
            this IApplicationBuilder app,
            TraceOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.ServiceName))
            {
                options.ServiceName = "web";
            }

            if (options.ShouldTracePath == null)
            {
                options.ShouldTracePath = x => true;
            }

            if (options.TraceSource == null)
            {
                throw new ArgumentException("Missing TraceSource function.", nameof(options.TraceSource));
            }

            app.Use(async (context, next) =>
            {
                var path = context.Request.Path.HasValue
                    ? context.Request.Path.Value
                    : string.Empty;

                if (!options.ShouldTracePath(path))
                {
                    await next();
                    return;
                }

                var source = options.TraceSource(context.Request);

                using (LogContext.PushProperty("TraceId", source.TraceId))
                using (LogContext.PushProperty("DatadogTraceId", source.TraceId))
                using (var span = source.Begin("aspnet.request", options.ServiceName, path, "web"))
                using (new TraceContextScope(span))
                {
                    span.SetMeta("span.kind", "server");
                    span.SetMeta("manual.keep", "true");

                    if (options.AnalyticsEnabled)
                    {
                        span.SetMeta("_dd1.sr.eausr", "true");
                    }

                    span.SetMeta("http.method", context.Request.Method);
                    span.SetMeta("http.request.headers.host", context.Request.Host.ToUriComponent());
                    span.SetMeta("http.url", context.Request.GetEncodedUrl());

                    if (options.LogForwardedForEnabled && context.Request.Headers.TryGetValue("X-Forwarded-For", out var values))
                        span.SetMeta("http.request.headers.x-forwarded-for", values.ToString());

                    if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var headerApiKey) && !string.IsNullOrEmpty(headerApiKey))
                        span.SetMeta("http.request.headers.apiKey", headerApiKey);

                    context.Request.Headers.TryGetValue(ApiTokenHeaderName, out var potentialHeaderApiTokens);
                    if (potentialHeaderApiTokens.Count > 0 && potentialHeaderApiTokens.Any(x => !string.IsNullOrWhiteSpace(x)))
                    {
                        var headerApiToken = potentialHeaderApiTokens.First(x => !string.IsNullOrEmpty(x));
                        span.SetMeta("http.request.headers.apiToken", headerApiToken);
                    }

                    span.SetMeta("http.path", path);

                    span.SetMeta("http.query", (context.Request.QueryString.HasValue
                        ? context.Request.QueryString.Value
                        : string.Empty)!);

                    try
                    {
                        await next();
                    }
                    catch (Exception ex)
                    {
                        span.SetError(ex);
                        throw;
                    }

                    span.SetMeta("http.status_code", context.Response.StatusCode.ToString());
                }
            });

            return app;
        }
    }
}
