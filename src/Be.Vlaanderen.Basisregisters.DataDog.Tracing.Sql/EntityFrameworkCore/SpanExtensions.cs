namespace Be.Vlaanderen.Basisregisters.DataDog.Tracing.Sql.EntityFrameworkCore
{
    using System.Data;
    using System.Data.Common;

    public static class SpanExtensions
    { 
        public static void SetDatabaseName(this ISpan span, IDbConnection connection)
        {
            if (span == null)
                return;

            if (connection == null)
                return;

            if (string.IsNullOrWhiteSpace(connection.Database))
                return;

            span.Resource = connection.Database;
            span.SetMeta("db.name", connection.Database);
        }

        public static void SetDatabaseName(this ISpan span, DbTransaction transaction)
        {
            if (transaction == null)
                return;

            span?.SetDatabaseName(transaction.Connection);
        }

        public static void SetDatabaseName(this ISpan span, DbCommand command)
        {
            if (command == null)
                return;

            span?.SetDatabaseName(command.Connection);
        }

        public static void SetMeta(this ISpan span, DbCommand command)
        {
            if (command == null)
                return;

            span?.SetDatabaseName(command);
            span?.SetMeta("sql.CommandText", command.CommandText);
            span?.SetMeta("sql.CommandType", command.CommandType.ToString());
        }
    }
}
