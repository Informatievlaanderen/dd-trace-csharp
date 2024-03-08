# DataDog Tracing for C# [![Build Status](https://github.com/Informatievlaanderen/datadog-tracing/workflows/Build/badge.svg)](https://github.com/Informatievlaanderen/datadog-tracing/actions)

## Packages

- [DataDog.Tracing](https://www.nuget.org/packages/DataDog.Tracing/) - Core library
- [DataDog.Tracing.Sql](https://www.nuget.org/packages/DataDog.Tracing.Sql/) - For tracing ADO .NET implementations. (e.g. SqlCommand, MySqlCommand, ...)
- [DataDog.Tracing.AspNetCore](https://www.nuget.org/packages/DataDog.Tracing.AspNetCore/) - For tracing ASP.NET Core applications.

```bash
# Install from nuget.org
dotnet add package DataDog.Tracing
```

**Note** that only .NET Core is supported since APM is not currently supported on DataDog's Windows agent.
