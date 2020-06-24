using System;
using AutoFixture;
using UHResidentInformationAPI.V1.Enums;
using UHResidentInformationAPI.V1.Infrastructure;

namespace UHResidentInformationAPI.Tests.V1.Helper
{
    public static class TestHelper
    {
        public static Person CreateDatabasePersonEntity(string firstname = null, string lastname = null, string houseRef = null)
        {
            var faker = new Fixture();
            var fp = faker.Build<Person>()
                .Without(p => p.HouseRef)
                .Create();
            fp.DateOfBirth = new DateTime
                (fp.DateOfBirth.Year, fp.DateOfBirth.Month, fp.DateOfBirth.Day);
            fp.FirstName = firstname ?? fp.FirstName;
            fp.LastName = lastname ?? fp.LastName;
            if (houseRef != null) fp.HouseRef = (string) houseRef;

            return fp;
        }
        public static Address CreateDatabaseAddressForPersonId(string houseRef, string postcode = null, string address1 = null, string address2 = null)
        {
            var faker = new Fixture();

            var fa = faker.Build<Address>()
                .With(add => add.HouseRef, houseRef)
                .Create();

            fa.PostCode = postcode ?? fa.PostCode;
            fa.AddressLine1 = address1 ?? fa.AddressLine1;
            fa.AddressLine2 = address2 ?? fa.AddressLine2;
            return fa;
        }

        public static TelephoneNumber CreateDatabaseTelephoneNumberForPersonId(int contactID)
        {
            var faker = new Fixture();

            return faker.Build<TelephoneNumber>()
                .With(tel => tel.ContactID, contactID)
                .With(tel => tel.Type, PhoneType.Mobile.ToString)
                .Without(tel => tel.ContactID)
                .Create();
        }
    }
}