namespace Be.Vlaanderen.Basisregisters.DataDog.Tracing.Sql.Tests
{
    using System.Data;

    public class Customer
    {
        public Customer(IDataRecord record)
        {
            Id = record.GetInt32(0);
            FirstName = record.GetString(1);
            LastName = record.GetString(2);
        }

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
