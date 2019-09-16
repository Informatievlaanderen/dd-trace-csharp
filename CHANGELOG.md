# [3.9.0](https://github.com/informatievlaanderen/datadog-tracing/compare/v3.8.0...v3.9.0) (2019-09-16)


### Features

* add generic EF tracedbconnection ([1f40d32](https://github.com/informatievlaanderen/datadog-tracing/commit/1f40d32))

# [3.8.0](https://github.com/informatievlaanderen/datadog-tracing/compare/v3.7.2...v3.8.0) (2019-09-16)


### Features

* add typed version of TraceDbConnection for dependency injection ([6f46c4e](https://github.com/informatievlaanderen/datadog-tracing/commit/6f46c4e))

## [3.7.2](https://github.com/informatievlaanderen/datadog-tracing/compare/v3.7.1...v3.7.2) (2019-08-27)


### Bug Fixes

* make datadog tracing check more for nulls ([3cdf821](https://github.com/informatievlaanderen/datadog-tracing/commit/3cdf821))

## [3.7.1](https://github.com/informatievlaanderen/datadog-tracing/compare/v3.7.0...v3.7.1) (2019-08-26)


### Bug Fixes

* deal with databasename on open ([174f026](https://github.com/informatievlaanderen/datadog-tracing/commit/174f026))

# [3.7.0](https://github.com/informatievlaanderen/datadog-tracing/compare/v3.6.0...v3.7.0) (2019-08-22)


### Features

* bump to .net 2.2.6 ([ff32175](https://github.com/informatievlaanderen/datadog-tracing/commit/ff32175))

# [3.6.0](https://github.com/informatievlaanderen/datadog-tracing/compare/v3.5.0...v3.6.0) (2019-08-20)


### Features

* add spanid to ispan ([2d387f5](https://github.com/informatievlaanderen/datadog-tracing/commit/2d387f5))

# [3.5.0](https://github.com/informatievlaanderen/datadog-tracing/compare/v3.4.0...v3.5.0) (2019-08-20)


### Features

* add parent id to span ([6497904](https://github.com/informatievlaanderen/datadog-tracing/commit/6497904))

# [3.4.0](https://github.com/informatievlaanderen/datadog-tracing/compare/v3.3.0...v3.4.0) (2019-08-20)


### Features

* add support for parent span id ([44f7033](https://github.com/informatievlaanderen/datadog-tracing/commit/44f7033))

# [3.3.0](https://github.com/informatievlaanderen/datadog-tracing/compare/v3.2.1...v3.3.0) (2019-08-12)


### Bug Fixes

* revert to previous sqlite ([593571b](https://github.com/informatievlaanderen/datadog-tracing/commit/593571b))


### Features

* span exposes traceid ([49ea65c](https://github.com/informatievlaanderen/datadog-tracing/commit/49ea65c))

## [3.2.1](https://github.com/informatievlaanderen/datadog-tracing/compare/v3.2.0...v3.2.1) (2019-04-25)

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
