using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NUnit.Framework;
using UHResidentInformationAPI.V1.Infrastructure;

namespace UHResidentInformationAPI.Tests
{
    [TestFixture]
    [NonParallelizable]
    public class DatabaseTests
    {
        private IDbContextTransaction _transaction;

        private DbContextOptionsBuilder _builder;

        protected UHContext UHContext { get; set; }

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            _builder = new DbContextOptionsBuilder();
            _builder.UseNpgsql(ConnectionString.TestDatabase());
        }

        [SetUp]
        public void SetUp()
        {
            UHContext = new UHContext(_builder.Options);
            UHContext.Database.EnsureCreated();
            _transaction = UHContext.Database.BeginTransaction();
        }

        [TearDown]
        public void TearDown()
        {
            _transaction.Rollback();
            _transaction.Dispose();
        }
    }
}
