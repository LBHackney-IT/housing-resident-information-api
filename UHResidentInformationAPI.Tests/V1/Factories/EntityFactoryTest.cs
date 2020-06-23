using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using UHResidentInformationAPI.V1.Domain;
using UHResidentInformationAPI.V1.Factories;
using UHResidentInformationAPI.V1.Infrastructure;

namespace UHResidentInformationAPI.Tests.V1.Factories
{
    [TestFixture]
    public class EntityFactoryTest
    {
        [Test]
        public void MapsAPersonDatabaseEntityIntoResidentInformationObject()
        {
            // Needs test helper set up
            // var personRecord = TestHelper.CreateDatabasePersonEntity();
            // var domain = personRecord.ToDomain();
            //
            // domain.Should().BeEquivalentTo(new ResidentInformation()
            // {
            //     HouseReference = personRecord.houseRef,
            //     PersonNumber = personRecord.personNo,
            //     FirstName = personRecord.FirstName,
            //     LastName = personRecord.LastName,
            //     DateOfBirth = personRecord.DateOfBirth.ToString("O", CultureInfo.InvariantCulture),
            //     NINumber = personRecord.NINumber
            // });
        }
    }
}
