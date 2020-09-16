using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using housingResidentInformationAPI;
using housingResidentInformationAPI.V1.Infrastructure;

namespace housingResidentInformationAPI.Tests
{
    public class MockWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly DbConnection _connection;

        public MockWebApplicationFactory(DbConnection connection)
        {
            _connection = connection;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(b => b.AddEnvironmentVariables())
                .UseStartup<Startup>();
            builder.ConfigureServices(services =>
            {
                var dbBuilder = new DbContextOptionsBuilder();
                dbBuilder.UseNpgsql(_connection);

                var context = new HousingContext(dbBuilder.Options);
                services.AddSingleton(context);

                var serviceProvider = services.BuildServiceProvider();
                var dbContext = serviceProvider.GetRequiredService<HousingContext>();

                dbContext.Database.EnsureCreated();
            });
        }
    }
}
