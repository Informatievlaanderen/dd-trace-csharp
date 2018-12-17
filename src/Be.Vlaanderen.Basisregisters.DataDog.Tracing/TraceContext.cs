namespace Be.Vlaanderen.Basisregisters.DataDog.Tracing
{
    using System.Threading;

    public static class TraceContext
    {
        private static readonly AsyncLocal<ISpan> CurrentLocal = new AsyncLocal<ISpan>();

        /// <summary>
        /// Gets the current span or <c>null</c> if no span was set up.
        /// </summary>
        public static ISpan Current
        {
            get => CurrentLocal.Value;
            internal set => CurrentLocal.Value = value;
        }

        /// <summary>
        /// Clears the TraceContext from the current scope.
        /// This is useful for protecting things that shouldn't have access to it. e.g. background work.
        /// </summary>
        public static void Reset() => CurrentLocal.Value = null;
    }
}
