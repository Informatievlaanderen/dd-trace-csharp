namespace Be.Vlaanderen.Basisregisters.DataDog.Tracing.Sql.Tests
{
    using NUnit.Framework;
    using SQLitePCL;

    [SetUpFixture]
    class SetUp
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            raw.SetProvider(new SQLite3Provider_e_sqlite3());
        }
    }
}
