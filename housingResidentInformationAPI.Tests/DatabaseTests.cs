using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NUnit.Framework;
using housingResidentInformationAPI.V1.Infrastructure;

namespace housingResidentInformationAPI.Tests
{
    [TestFixture]
    [NonParallelizable]
    public class DatabaseTests
    {
        private IDbContextTransaction _transaction;
        protected HousingContext HousingContext { get; private set; }

        private DbContextOptionsBuilder _builder;

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            _builder = new DbContextOptionsBuilder();
            _builder.UseNpgsql(ConnectionString.TestDatabase());
        }

        [SetUp]
        public void SetUp()
        {
            HousingContext = new HousingContext(_builder.Options);
            HousingContext.Database.EnsureCreated();
            _transaction = HousingContext.Database.BeginTransaction();
        }

        [TearDown]
        public void TearDown()
        {
            _transaction.Rollback();
            _transaction.Dispose();
        }
    }
}
