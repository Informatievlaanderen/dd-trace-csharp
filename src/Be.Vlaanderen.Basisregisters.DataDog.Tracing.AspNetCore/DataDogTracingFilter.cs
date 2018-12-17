namespace Be.Vlaanderen.Basisregisters.DataDog.Tracing.AspNetCore
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using Tracing;

    public class DataDogTracingFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var span = TraceContext.Current;
            if (span == null)
                return;

            var routeValues = context.ActionDescriptor.RouteValues;
            routeValues.TryGetValue("action", out var action);
            routeValues.TryGetValue("controller", out var controller);

            action = action ?? "unknown";
            controller = controller ?? "unknown";
            span.Resource = $"{controller}.{action}";
        }
    }
}
