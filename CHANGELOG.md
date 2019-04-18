# [3.2.0](https://github.com/informatievlaanderen/datadog-tracing/compare/v3.1.1...v3.2.0) (2019-04-18)


### Features

* add traceid to logcontext ([4ee013b](https://github.com/informatievlaanderen/datadog-tracing/commit/4ee013b))

## [3.1.1](https://github.com/informatievlaanderen/datadog-tracing/compare/v3.1.0...v3.1.1) (2019-04-18)


### Bug Fixes

* properly register tracesource factory ([7cd0732](https://github.com/informatievlaanderen/datadog-tracing/commit/7cd0732))

# [3.1.0](https://github.com/informatievlaanderen/datadog-tracing/compare/v3.0.0...v3.1.0) (2019-04-18)


### Features

* add extra debug logging for traceagent ([014d5be](https://github.com/informatievlaanderen/datadog-tracing/commit/014d5be))

# [3.0.0](https://github.com/informatievlaanderen/datadog-tracing/compare/v2.0.1...v3.0.0) (2019-04-18)


### Bug Fixes

* trace id has to be a long ([a1e791e](https://github.com/informatievlaanderen/datadog-tracing/commit/a1e791e))


### BREAKING CHANGES

* Trace Id has to be a long instead of a string

## [2.0.1](https://github.com/informatievlaanderen/datadog-tracing/compare/v2.0.0...v2.0.1) (2019-04-18)


### Bug Fixes

* properly register with autofac ([58edd1a](https://github.com/informatievlaanderen/datadog-tracing/commit/58edd1a))

# [2.0.0](https://github.com/informatievlaanderen/datadog-tracing/compare/v1.2.0...v2.0.0) (2019-04-17)


### Features

* provide your own trace id ([d26bc7d](https://github.com/informatievlaanderen/datadog-tracing/commit/d26bc7d))


### BREAKING CHANGES

* UseDataDogTracing now expects a function to return a TraceSource per request

# [1.2.0](https://github.com/informatievlaanderen/datadog-tracing/compare/v1.1.1...v1.2.0) (2019-04-17)


### Features

* update dependencies ([f136ea1](https://github.com/informatievlaanderen/datadog-tracing/commit/f136ea1))

## [1.1.1](https://github.com/informatievlaanderen/datadog-tracing/compare/v1.1.0...v1.1.1) (2019-01-16)


### Bug Fixes

* properly dispose TraceDbConnection ([e1736f8](https://github.com/informatievlaanderen/datadog-tracing/commit/e1736f8))

# [1.1.0](https://github.com/informatievlaanderen/datadog-tracing/compare/v1.0.1...v1.1.0) (2019-01-15)


### Features

* make servicename configurable for dbconnection ([d2f62a1](https://github.com/informatievlaanderen/datadog-tracing/commit/d2f62a1))

## [1.0.1](https://github.com/informatievlaanderen/datadog-tracing/compare/v1.0.0...v1.0.1) (2018-12-18)

# 1.0.0 (2018-12-17)


### Features

* open source changes needed for 'agentschap Informatie Vlaanderen' ([b85e49e](https://github.com/informatievlaanderen/datadog-tracing/commit/b85e49e))