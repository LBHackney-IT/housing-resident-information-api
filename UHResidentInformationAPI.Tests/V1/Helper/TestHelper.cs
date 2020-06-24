using System;
using AutoFixture;
using UHResidentInformationAPI.V1.Enums;
using UHResidentInformationAPI.V1.Infrastructure;

namespace UHResidentInformationAPI.Tests.V1.Helper
{
    public static class TestHelper
    {
        public static Person CreateDatabasePersonEntity(string firstname = null, string lastname = null, string houseRef = null, int? personNo = null)
        {
            var faker = new Fixture();
            var fp = faker.Build<Person>()
                .Create();
            fp.DateOfBirth = new DateTime
                (fp.DateOfBirth.Year, fp.DateOfBirth.Month, fp.DateOfBirth.Day);
            fp.FirstName = firstname ?? fp.FirstName;
            fp.LastName = lastname ?? fp.LastName;
            if (houseRef != null) fp.HouseRef = (string) houseRef;
            fp.PersonNo = personNo ?? fp.PersonNo;
            return fp;
        }
        public static Address CreateDatabaseAddressForPersonId(string houseRef, string address1 = null)
        {
            var faker = new Fixture();

            var fa = faker.Build<Address>()
                .With(add => add.HouseRef, houseRef)
                .Create();

            fa.AddressLine1 = address1 ?? fa.AddressLine1;
            return fa;
        }

        public static TelephoneNumber CreateDatabaseTelephoneNumberForPersonId(int contactID)
        {
            var faker = new Fixture();

            return faker.Build<TelephoneNumber>()
                .With(tel => tel.ContactID, contactID)
                .With(tel => tel.Type, PhoneType.Mobile.ToString)
                .Create();
        }
    }
}
