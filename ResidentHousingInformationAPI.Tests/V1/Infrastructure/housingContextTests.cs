using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using HousingResidentInformationAPI.Tests.V1.Helper;
using HousingResidentInformationAPI.V1.Infrastructure;

namespace HousingResidentInformationAPI.Tests.V1.Infrastructure
{
    [TestFixture]
    public class HousingContextTest : DatabaseTests
    {
        public void CanGetADatabaseEntity()
        {
            var databaseEntity = TestHelper.CreateDatabasePersonEntity();

            HousingContext.Add(databaseEntity);
            HousingContext.SaveChanges();

            var result = HousingContext.Persons.ToList().FirstOrDefault();

            result.Should().BeEquivalentTo(databaseEntity);
        }
    }
}
