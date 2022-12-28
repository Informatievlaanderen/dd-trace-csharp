namespace Be.Vlaanderen.Basisregisters.DataDog.Tracing.SqlStreamStore.Microsoft
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
            => await Trace(
                nameof(ReadAllForwards),
                "all",
                () => _streamStore.ReadAllForwards(fromPositionInclusive, maxCount, prefetchJsonData, cancellationToken));

        public async Task<ReadAllPage> ReadAllBackwards(
            long fromPositionInclusive,
            int maxCount,
            bool prefetchJsonData = true,
            CancellationToken cancellationToken = new CancellationToken())
            => await Trace(
                nameof(ReadAllBackwards),
                "all",
                () => _streamStore.ReadAllBackwards(fromPositionInclusive, maxCount, prefetchJsonData, cancellationToken));

        public async Task<ReadStreamPage> ReadStreamForwards(
            StreamId streamId,
            int fromVersionInclusive,
            int maxCount,
            bool prefetchJsonData = true,
            CancellationToken cancellationToken = new CancellationToken())
            => await Trace(
                nameof(ReadStreamForwards),
                streamId,
                () => _streamStore.ReadStreamForwards(streamId, fromVersionInclusive, maxCount, prefetchJsonData, cancellationToken));

        public async Task<ReadStreamPage> ReadStreamBackwards(
            StreamId streamId,
            int fromVersionInclusive,
            int maxCount,
            bool prefetchJsonData = true,
            CancellationToken cancellationToken = new CancellationToken())
            => await Trace(
                nameof(ReadStreamBackwards),
                streamId,
                () => _streamStore.ReadStreamBackwards(streamId, fromVersionInclusive, maxCount, prefetchJsonData, cancellationToken));

        public IStreamSubscription SubscribeToStream(
            StreamId streamId,
            int? continueAfterVersion,
            StreamMessageReceived streamMessageReceived,
            SubscriptionDropped subscriptionDropped = null,
            HasCaughtUp hasCaughtUp = null,
            bool prefetchJsonData = true,
            string name = null)
            => Trace(
                nameof(SubscribeToStream),
                streamId,
                () => _streamStore.SubscribeToStream(
                    streamId,
                    continueAfterVersion,
                    streamMessageReceived,
                    subscriptionDropped,
                    hasCaughtUp,
                    prefetchJsonData,
                    name));

        public IAllStreamSubscription SubscribeToAll(
            long? continueAfterPosition,
            AllStreamMessageReceived streamMessageReceived,
            AllSubscriptionDropped subscriptionDropped = null,
            HasCaughtUp hasCaughtUp = null,
            bool prefetchJsonData = true,
            string name = null)
            => Trace(
                nameof(SubscribeToAll),
                "all",
                () => _streamStore.SubscribeToAll(
                    continueAfterPosition,
                    streamMessageReceived,
                    subscriptionDropped,
                    hasCaughtUp,
                    prefetchJsonData,
                    name));

        public async Task<long> ReadHeadPosition(CancellationToken cancellationToken = new CancellationToken())
            => await Trace(
                nameof(ReadHeadPosition),
                "head",
                () => _streamStore.ReadHeadPosition(cancellationToken));

        public async Task<long> ReadStreamHeadPosition(
            StreamId streamId,
            CancellationToken cancellationToken = new CancellationToken())
            => await Trace(
                nameof(ReadStreamHeadPosition),
                streamId,
                () => _streamStore.ReadStreamHeadPosition(streamId, cancellationToken));

        public async Task<int> ReadStreamHeadVersion(
            StreamId streamId,
            CancellationToken cancellationToken = new CancellationToken())
            => await Trace(
                nameof(ReadStreamHeadVersion),
                streamId,
                () => _streamStore.ReadStreamHeadVersion(streamId, cancellationToken));

        public async Task<StreamMetadataResult> GetStreamMetadata(
            string streamId,
            CancellationToken cancellationToken = new CancellationToken())
            => await Trace(
                nameof(GetStreamMetadata),
                streamId,
                () => _streamStore.GetStreamMetadata(streamId, cancellationToken));

        public async Task<ListStreamsPage> ListStreams(
            int maxCount = 100,
            string? continuationToken = null,
            CancellationToken cancellationToken = new CancellationToken())
            => await Trace(
                nameof(ListStreams),
                "streams",
                () => _streamStore.ListStreams(maxCount, continuationToken, cancellationToken));


        public async Task<ListStreamsPage> ListStreams(
            Pattern pattern,
            int maxCount = 100,
            string continuationToken = null,
            CancellationToken cancellationToken = new CancellationToken())
			=> await Trace(
                nameof(ListStreams),
                "streams",
                () => _streamStore.ListStreams(pattern, maxCount, continuationToken, cancellationToken));

        public async Task<AppendResult> AppendToStream(
            StreamId streamId,
            int expectedVersion,
            NewStreamMessage[] messages,
            CancellationToken cancellationToken = new CancellationToken())
            => await Trace(
                nameof(AppendToStream),
                streamId,
                () => _streamStore.AppendToStream(streamId, expectedVersion, messages, cancellationToken));

        public async Task DeleteStream(
            StreamId streamId,
            int expectedVersion = -2,
            CancellationToken cancellationToken = new CancellationToken())
            => await Trace(
                nameof(DeleteStream),
                streamId,
                () => _streamStore.DeleteStream(streamId, expectedVersion, cancellationToken));

        public async Task DeleteMessage(
            StreamId streamId,
            Guid messageId,
            CancellationToken cancellationToken = new CancellationToken())
            => await Trace(
                nameof(DeleteMessage),
                $"{streamId}/{messageId}",
                () => _streamStore.DeleteMessage(streamId, messageId, cancellationToken));

        public async Task SetStreamMetadata(
            StreamId streamId,
            int expectedStreamMetadataVersion = -2,
            int? maxAge = null,
            int? maxCount = null,
            string metadataJson = null,
            CancellationToken cancellationToken = new CancellationToken())
            => await Trace(
                nameof(SetStreamMetadata),
                streamId,
                () => _streamStore.SetStreamMetadata(streamId, expectedStreamMetadataVersion, maxAge, maxCount, metadataJson, cancellationToken));

        private T Trace<T>(string actionName, string resource, Func<T> action)
            => Trace(actionName, resource, () => Task.Run(action)).GetAwaiter().GetResult();

        private async Task Trace(string actionName, string resource, Func<Task> action)
            => await Trace<object?>(actionName, resource, async () => { await action(); return null; });

        private async Task<T> Trace<T>(string actionName, string resource, Func<Task<T>> action)
        {
            var span = _spanSource.Begin($"stream-store.{actionName}", ServiceName, resource, TypeName);
            try
            {
                return await action();
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
