using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using HousingResidentInformationAPI.Tests.V1.Helper;
using HousingResidentInformationAPI.V1.Domain;
using HousingResidentInformationAPI.V1.Enums;
using HousingResidentInformationAPI.V1.Factories;
using HousingResidentInformationAPI.V1.Infrastructure;

namespace HousingResidentInformationAPI.Tests.V1.Factories
{
    [TestFixture]
    public class EntityFactoryTest
    {
        [Test]
        public void MapsAPersonDatabaseEntityIntoResidentInformationObject()
        {
            var personRecord = TestHelper.CreateDatabasePersonEntity();
            var domain = personRecord.ToDomain();

            domain.Should().BeEquivalentTo(new ResidentInformation()
            {
                HouseReference = personRecord.HouseRef,
                PersonNumber = personRecord.PersonNo,
                FirstName = personRecord.FirstName,
                LastName = personRecord.LastName,
                DateOfBirth = personRecord.DateOfBirth.ToString("O", CultureInfo.InvariantCulture),
                NINumber = personRecord.NINumber
            });
        }
    }
}
