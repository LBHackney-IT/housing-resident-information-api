using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using UHResidentInformationAPI.Tests.V1.Helper;
using UHResidentInformationAPI.V1.Infrastructure;

namespace UHResidentInformationAPI.Tests.V1.Infrastructure
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
