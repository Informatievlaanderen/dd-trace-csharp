namespace Be.Vlaanderen.Basisregisters.DataDog.Tracing.SqlStreamStore
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using global::SqlStreamStore;
    using global::SqlStreamStore.Streams;
    using global::SqlStreamStore.Subscriptions;

    public class TraceStreamStore : IStreamStore
    {
        private const string DefaultServiceName = "stream-store";
        private const string TypeName = "stream-store";

        private string ServiceName { get; }

        private readonly MsSqlStreamStore _streamStore;
        private readonly ISpanSource _spanSource;

        public event Action OnDispose;

        public TraceStreamStore(MsSqlStreamStore streamStore)
            : this(streamStore, DefaultServiceName, TraceContextSpanSource.Instance) { }

        public TraceStreamStore(MsSqlStreamStore streamStore, string serviceName)
            : this(streamStore, serviceName, TraceContextSpanSource.Instance) { }

        public TraceStreamStore(MsSqlStreamStore streamStore, string serviceName, ISpanSource spanSource)
        {
            _streamStore = streamStore ?? throw new ArgumentException(nameof(streamStore));
            _spanSource = spanSource ?? throw new ArgumentException(nameof(spanSource));

            ServiceName = serviceName ?? DefaultServiceName;

            // TODO: Not sure about this
            _streamStore.OnDispose += OnDispose;
        }

        public void Dispose() => _streamStore.Dispose();

        public async Task<ReadAllPage> ReadAllForwards(
            long fromPositionInclusive,
            int maxCount,
            bool prefetchJsonData = true,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var span = _spanSource.Begin("stream-store." + nameof(ReadAllForwards), ServiceName, "all", TypeName);
            try
            {
                return await _streamStore
                    .ReadAllForwards(
                        fromPositionInclusive,
                        maxCount,
                        prefetchJsonData,
                        cancellationToken);
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

        public async Task<ReadAllPage> ReadAllBackwards(
            long fromPositionInclusive,
            int maxCount,
            bool prefetchJsonData = true,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var span = _spanSource.Begin("stream-store." + nameof(ReadAllBackwards), ServiceName, "all", TypeName);
            try
            {
                return await _streamStore
                    .ReadAllBackwards(
                        fromPositionInclusive,
                        maxCount,
                        prefetchJsonData,
                        cancellationToken);
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

        public async Task<ReadStreamPage> ReadStreamForwards(
            StreamId streamId,
            int fromVersionInclusive,
            int maxCount,
            bool prefetchJsonData = true,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var span = _spanSource.Begin("stream-store." + nameof(ReadStreamForwards), ServiceName, streamId, TypeName);
            try
            {
                return await _streamStore
                    .ReadStreamForwards(
                        streamId,
                        fromVersionInclusive,
                        maxCount,
                        prefetchJsonData,
                        cancellationToken);
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

        public async Task<ReadStreamPage> ReadStreamBackwards(
            StreamId streamId,
            int fromVersionInclusive,
            int maxCount,
            bool prefetchJsonData = true,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var span = _spanSource.Begin("stream-store." + nameof(ReadStreamBackwards), ServiceName, streamId, TypeName);
            try
            {
                return await _streamStore
                    .ReadStreamBackwards(
                        streamId,
                        fromVersionInclusive,
                        maxCount,
                        prefetchJsonData,
                        cancellationToken);
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

        public IStreamSubscription SubscribeToStream(
            StreamId streamId,
            int? continueAfterVersion,
            StreamMessageReceived streamMessageReceived,
            SubscriptionDropped subscriptionDropped = null,
            HasCaughtUp hasCaughtUp = null,
            bool prefetchJsonData = true,
            string name = null)
        {
            return _streamStore
                .SubscribeToStream(
                    streamId,
                    continueAfterVersion,
                    streamMessageReceived,
                    subscriptionDropped,
                    hasCaughtUp,
                    prefetchJsonData,
                    name);
        }

        public IAllStreamSubscription SubscribeToAll(
            long? continueAfterPosition,
            AllStreamMessageReceived streamMessageReceived,
            AllSubscriptionDropped subscriptionDropped = null,
            HasCaughtUp hasCaughtUp = null,
            bool prefetchJsonData = true,
            string name = null)
        {
            return _streamStore
                .SubscribeToAll(
                    continueAfterPosition,
                    streamMessageReceived,
                    subscriptionDropped,
                    hasCaughtUp,
                    prefetchJsonData,
                    name);
        }

        public async Task<long> ReadHeadPosition(CancellationToken cancellationToken = new CancellationToken())
        {
            var span = _spanSource.Begin("stream-store." + nameof(ReadHeadPosition), ServiceName, "head", TypeName);
            try
            {
                return await _streamStore.ReadHeadPosition(cancellationToken);
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

        public async Task<StreamMetadataResult> GetStreamMetadata(
            string streamId,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var span = _spanSource.Begin("stream-store." + nameof(GetStreamMetadata), ServiceName, streamId, TypeName);
            try
            {
                return await _streamStore
                    .GetStreamMetadata(
                        streamId,
                        cancellationToken);
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

        public async Task<AppendResult> AppendToStream(
            StreamId streamId,
            int expectedVersion,
            NewStreamMessage[] messages,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var span = _spanSource.Begin("stream-store." + nameof(AppendToStream), ServiceName, streamId, TypeName);
            try
            {
                return await _streamStore
                    .AppendToStream(
                        streamId,
                        expectedVersion,
                        messages,
                        cancellationToken);
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

        public async Task DeleteStream(
            StreamId streamId,
            int expectedVersion = -2,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var span = _spanSource.Begin("stream-store." + nameof(DeleteStream), ServiceName, streamId, TypeName);
            try
            {
                await _streamStore
                    .DeleteStream(
                        streamId,
                        expectedVersion,
                        cancellationToken);
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

        public async Task DeleteMessage(
            StreamId streamId,
            Guid messageId,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var span = _spanSource.Begin("stream-store." + nameof(DeleteMessage), ServiceName, $"{streamId}/{messageId}", TypeName);
            try
            {
                await _streamStore
                    .DeleteMessage(
                        streamId,
                        messageId,
                        cancellationToken);
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

        public async Task SetStreamMetadata(
            StreamId streamId,
            int expectedStreamMetadataVersion = -2,
            int? maxAge = null,
            int? maxCount = null,
            string metadataJson = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var span = _spanSource.Begin("stream-store." + nameof(SetStreamMetadata), ServiceName, streamId, TypeName);
            try
            {
                await _streamStore
                    .SetStreamMetadata(
                        streamId,
                        expectedStreamMetadataVersion,
                        maxAge,
                        maxCount,
                        metadataJson,
                        cancellationToken);
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
    }
}
