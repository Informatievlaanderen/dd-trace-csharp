namespace Be.Vlaanderen.Basisregisters.DataDog.Tracing.AspNetCore
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Tracing;
    using Serilog.Context;

    public static class DataDogTracingApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDataDogTracing(
            this IApplicationBuilder app,
            Func<HttpRequest, TraceSource> getTraceSource,
            string serviceName = "web",
            Func<string, bool> shouldTracePathFunc = null)
        {
            if (shouldTracePathFunc == null)
                shouldTracePathFunc = x => true;

            app.Use(async (context, next) =>
            {
                var path = context.Request.Path.HasValue
                    ? context.Request.Path.Value
                    : string.Empty;

                if (!shouldTracePathFunc(path))
                {
                    await next();
                    return;
                }

                var source = getTraceSource(context.Request);

                using (LogContext.PushProperty("TraceId", source.TraceId))
                using (var span = source.Begin("aspnet.request", serviceName, path, "web"))
                using (new TraceContextScope(span))
                {
                    span.SetMeta("span.kind", "server");
                    span.SetMeta("manual.keep", "true");

                    span.SetMeta("http.method", context.Request.Method);
                    span.SetMeta("http.request.headers.host", context.Request.Host.ToUriComponent());
                    span.SetMeta("http.url", context.Request.GetEncodedUrl());

                    span.SetMeta("http.path", path);
                    span.SetMeta("http.query", context.Request.QueryString.HasValue
                        ? context.Request.QueryString.Value
                        : string.Empty);

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
