namespace Be.Vlaanderen.Basisregisters.DataDog.Tracing.Sql
{
    using System;
    using System.Data;
    using Tracing;

    public class TraceDbCommand : IDbCommand
    {
        private const string DefaultServiceName = "sql";
        private const string TypeName = "sql";

        private string ServiceName { get; }

        private readonly IDbCommand _command;
        private readonly ISpanSource _spanSource;

        public IDbCommand InnerCommand => _command;

        public IDataParameterCollection Parameters => _command.Parameters;

        public string CommandText
        {
            get => _command.CommandText;
            set => _command.CommandText = value;
        }

        public int CommandTimeout
        {
            get => _command.CommandTimeout;
            set => _command.CommandTimeout = value;
        }

        public CommandType CommandType
        {
            get => _command.CommandType;
            set => _command.CommandType = value;
        }

        public IDbConnection Connection
        {
            get => _command.Connection;
            set => _command.Connection = value;
        }

        public IDbTransaction Transaction
        {
            get => _command.Transaction;
            set => _command.Transaction = value;
        }

        public UpdateRowSource UpdatedRowSource
        {
            get => _command.UpdatedRowSource;
            set => _command.UpdatedRowSource = value;
        }

        public TraceDbCommand(IDbCommand command)
            : this(command, DefaultServiceName, TraceContextSpanSource.Instance) { }

        public TraceDbCommand(IDbCommand command, string serviceName)
            : this(command, serviceName, TraceContextSpanSource.Instance) { }

        public TraceDbCommand(IDbCommand command, ISpanSource spanSource)
            : this(command, DefaultServiceName, spanSource) { }

        public TraceDbCommand(IDbCommand command, string serviceName, ISpanSource spanSource)
        {
            _command = command ?? throw new ArgumentNullException(nameof(command));
            _spanSource = spanSource ?? throw new ArgumentNullException(nameof(spanSource));

            ServiceName = string.IsNullOrWhiteSpace(serviceName)
                ? DefaultServiceName
                : serviceName;
        }

        public void Dispose() => _command.Dispose();

        public void Cancel() => _command.Cancel();

        public void Prepare() => _command.Prepare();

        public IDbDataParameter CreateParameter() => _command.CreateParameter();

        public int ExecuteNonQuery()
        {
            const string name = "sql." + nameof(ExecuteNonQuery);
            var span = _spanSource.Begin(name, ServiceName, _command.Connection.Database, TypeName);
            try
            {
                var result = _command.ExecuteNonQuery();
                if (span != null)
                {
                    span.SetMeta("sql.RowsAffected", result.ToString());
                    SetMeta(span);
                }
                return result;
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

        public IDataReader ExecuteReader() => ExecuteReader(CommandBehavior.Default);

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            const string name = "sql." + nameof(ExecuteReader);
            var span = _spanSource.Begin(name, ServiceName, _command.Connection.Database, TypeName);
            try
            {
                if (span != null)
                {
                    const string metaKey = "sql." + nameof(CommandBehavior);
                    span.SetMeta(metaKey, behavior.ToString("x"));
                    SetMeta(span);
                }
                return _command.ExecuteReader(behavior);
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

        public object ExecuteScalar()
        {
            const string name = "sql." + nameof(ExecuteScalar);
            var span = _spanSource.Begin(name, ServiceName, _command.Connection.Database, TypeName);
            try
            {
                if (span != null) SetMeta(span);
                return _command.ExecuteScalar();
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


        private void SetMeta(ISpan span)
        {
            span.SetMeta("sql.CommandText", CommandText);
            span.SetMeta("sql.CommandType", CommandType.ToString());
        }
    }
}
