using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using NUnit.Framework;
using HousingResidentInformationAPI.V1.Infrastructure;

namespace HousingResidentInformationAPI.Tests
{
    [NonParallelizable]
    [TestFixture]
    public class EndToEndTests<TStartup> where TStartup : class
    {
        private HttpClient _client;
        private HousingContext _housingContext;

        private MockWebApplicationFactory<TStartup> _factory;
        private NpgsqlConnection _connection;
        private IDbContextTransaction _transaction;
        private DbContextOptionsBuilder _builder;

        protected HttpClient Client => _client;
        protected HousingContext HousingContext => _housingContext;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _connection = new NpgsqlConnection(ConnectionString.TestDatabase());
            _connection.Open();

            var npgsqlCommand = _connection.CreateCommand();
            npgsqlCommand.CommandText = "SET deadlock_timeout TO 30";
            npgsqlCommand.ExecuteNonQuery();

            _builder = new DbContextOptionsBuilder();
            _builder.UseNpgsql(_connection);
        }

        [SetUp]
        public void BaseSetup()
        {
            _factory = new MockWebApplicationFactory<TStartup>(_connection);
            _client = _factory.CreateClient();
            _housingContext = new HousingContext(_builder.Options);
            _housingContext.Database.EnsureCreated();
            _transaction = HousingContext.Database.BeginTransaction();
        }

        [TearDown]
        public void BaseTearDown()
        {
            _client.Dispose();
            _factory.Dispose();
            _transaction.Rollback();
            _transaction.Dispose();
        }
    }
}
