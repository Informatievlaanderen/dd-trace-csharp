namespace Be.Vlaanderen.Basisregisters.DataDog.Tracing.Sql
{
    using System;
    using System.Data;
    using Tracing;

    public class TraceDbConnection : IDbConnection
    {
        private const string DefaultServiceName = "sql";
        private const string TypeName = "sql";

        private string ServiceName { get; }

        private readonly ISpanSource _spanSource;
        private readonly IDbConnection _connection;

        public IDbConnection InnerConnection => _connection;

        public int ConnectionTimeout => _connection.ConnectionTimeout;

        public string Database => _connection.Database;

        public ConnectionState State => _connection.State;

        public string ConnectionString
        {
            get => _connection.ConnectionString;
            set => _connection.ConnectionString = value;
        }

        public TraceDbConnection(IDbConnection connection)
            : this(connection, TraceContextSpanSource.Instance) { }

        public TraceDbConnection(IDbConnection connection, string serviceName)
            : this(connection, serviceName, TraceContextSpanSource.Instance) { }

        public TraceDbConnection(IDbConnection connection, ISpanSource spanSource)
            : this(connection, DefaultServiceName, spanSource) { }

        public TraceDbConnection(IDbConnection connection, string serviceName, ISpanSource spanSource)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _spanSource = spanSource ?? throw new ArgumentNullException(nameof(spanSource));

            ServiceName = string.IsNullOrWhiteSpace(serviceName)
                ? DefaultServiceName
                : serviceName;
        }

        public void Dispose() => _connection.Dispose();

        // todo - span around transactions
        public IDbTransaction BeginTransaction() => _connection.BeginTransaction();

        public IDbTransaction BeginTransaction(IsolationLevel il) => _connection.BeginTransaction(il);

        public void ChangeDatabase(string databaseName) => _connection.ChangeDatabase(databaseName);

        public void Close() => _connection.Close();

        public IDbCommand CreateCommand() => new TraceDbCommand(_connection.CreateCommand(), ServiceName, _spanSource);

        public void Open()
        {
            var span = _spanSource.Begin("sql.connect", ServiceName, _connection.Database, TypeName);
            try
            {
                _connection.Open();
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

    /// <summary>
    /// Typed version which allows to have multiple TraceDbConnection registered for dependency injection
    /// </summary>
    /// <typeparam name="T">Type used as a differentiator between the registered TraceDbConnections</typeparam>
    public class TraceDbConnection<T> : TraceDbConnection
    {
        public TraceDbConnection(IDbConnection connection)
            : base(connection) {}

        public TraceDbConnection(IDbConnection connection, string serviceName)
            : base(connection, serviceName) {}

        public TraceDbConnection(IDbConnection connection, ISpanSource spanSource)
            : base(connection, spanSource) {}

        public TraceDbConnection(IDbConnection connection, string serviceName, ISpanSource spanSource)
            : base(connection, serviceName, spanSource) {}
    }
}
