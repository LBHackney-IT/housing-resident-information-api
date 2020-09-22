using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using HousingResidentInformationAPI.Tests.V1.Helper;
using HousingResidentInformationAPI.V1.Infrastructure;

namespace HousingResidentInformationAPI.Tests.V1.Infrastructure
{
    [TestFixture]
    public class UHContextTest : DatabaseTests
    {
        public void CanGetADatabaseEntity()
        {
            var databaseEntity = TestHelper.CreateDatabasePersonEntity();

            UHContext.Add(databaseEntity);
            UHContext.SaveChanges();

            var result = UHContext.Persons.ToList().FirstOrDefault();

            result.Should().BeEquivalentTo(databaseEntity);
        }
    }
}
