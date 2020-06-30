using System;
using AutoFixture;
using UHResidentInformationAPI.V1.Enums;
using UHResidentInformationAPI.V1.Infrastructure;

namespace UHResidentInformationAPI.Tests.V1.Helper
{
    public static class TestHelper
    {
        public static Person CreateDatabasePersonEntity(string firstname = null, string lastname = null)
        {
            var faker = new Fixture();
            var fp = faker.Build<Person>()
                .Create();
            fp.DateOfBirth = new DateTime
                (fp.DateOfBirth.Year, fp.DateOfBirth.Month, fp.DateOfBirth.Day);
            fp.FirstName = firstname ?? fp.FirstName;
            fp.LastName = lastname ?? fp.LastName;

            return fp;
        }

        public static Address CreateDatabaseAddressForPersonId(string houseRef, string postcode = null, string address1 = null)
        {
            var faker = new Fixture();

            var fa = faker.Build<Address>()
                .With(add => add.HouseRef, houseRef)
                .Create();

            fa.PostCode = postcode ?? fa.PostCode;
            fa.AddressLine1 = address1 ?? fa.AddressLine1;
            return fa;
        }

        public static TelephoneNumber CreateDatabaseTelephoneNumberForPersonId(int personNo)
        {
            var faker = new Fixture();
            var fakePhoneType = (int) PhoneType.Mobile;

            var fakeNumber = faker.Build<TelephoneNumber>()
                .With(tel => tel.ContactID, personNo)
                .With(tel => tel.Type, fakePhoneType.ToString)
                .Create();

            fakeNumber.DateCreated = new DateTime
                (fakeNumber.DateCreated.Year, fakeNumber.DateCreated.Month, fakeNumber.DateCreated.Day);

            return fakeNumber;
        }

        public static EmailAddresses CreateDatabaseEmailForPerson(int personNo)
        {
            var faker = new Fixture();

            var fakeEmail = faker.Build<EmailAddresses>()
                .With(tel => tel.ContactID, personNo)
                .Create();

            fakeEmail.DateModified = new DateTime
                (fakeEmail.DateModified.Year, fakeEmail.DateModified.Month, fakeEmail.DateModified.Day);

            return fakeEmail;
        }
    }
}
