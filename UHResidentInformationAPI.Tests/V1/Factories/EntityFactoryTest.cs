using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using UHResidentInformationAPI.Tests.V1.Helper;
using UHResidentInformationAPI.V1.Domain;
using UHResidentInformationAPI.V1.Enums;
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
