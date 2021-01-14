using System;
using System.Globalization;
using AutoFixture;
using AutoFixture.Kernel;
using HousingResidentInformationAPI.V1.Enums;
using HousingResidentInformationAPI.V1.Infrastructure;

namespace HousingResidentInformationAPI.Tests.V1.Helper
{
    public static class TestHelper
    {
        public static Person CreateDatabasePersonEntity(string firstname = null, string lastname = null, string houseRef = null, int? personNo = null)
        {
            //Generate integers for house_ref column 
            var rrng = new RandomRangedNumberGenerator();
            var sc = new SpecimenContext(rrng);
            rrng.Create(new RangedNumberRequest(typeof(int), 100000, 999999), sc);

            var fixture = new Fixture();
            fixture.Customizations.Add(rrng);
            var fp = fixture.Build<Person>()
                .Create();

            fp.DateOfBirth = new DateTime
                (fp.DateOfBirth.Year, fp.DateOfBirth.Month, fp.DateOfBirth.Day);
            fp.FirstName = firstname ?? fp.FirstName;
            fp.LastName = lastname ?? fp.LastName;
            fp.HouseRef = houseRef ?? fixture.Create<int>().ToString();
            fp.PersonNo = personNo ?? fp.PersonNo;

            return fp;
        }

        public static Address CreateDatabaseAddressForPersonId(string houseRef, string postcode = null, string address1 = null)
        {
            var fixture = new Fixture();

            var fa = fixture.Build<Address>()
                .With(add => add.HouseRef, houseRef)
                .Create();

            fa.PostCode = postcode ?? fa.PostCode;
            fa.AddressLine1 = address1 ?? fa.AddressLine1;
            return fa;
        }

        public static TelephoneNumber CreateDatabaseTelephoneNumberForPersonId(int contactNo)
        {
            var fixture = new Fixture();
            var fakePhoneType = PhoneType.M;

            var fakeNumber = fixture.Build<TelephoneNumber>()
                .With(tel => tel.ContactID, contactNo)
                .With(tel => tel.Type, fakePhoneType.ToString)
                .Without(tel => tel.PhoneId)
                .Create();

            fakeNumber.DateCreated = new DateTime
                (fakeNumber.DateCreated.Year, fakeNumber.DateCreated.Month, fakeNumber.DateCreated.Day);

            return fakeNumber;
        }

        public static EmailAddresses CreateDatabaseEmailForPerson(int contactNo)
        {
            var fixture = new Fixture();

            var fakeEmail = fixture.Build<EmailAddresses>()
                .With(email => email.ContactID, contactNo)
                .Without(email => email.ContactID)
                .Create();

            fakeEmail.DateModified = new DateTime
                (fakeEmail.DateModified.Year, fakeEmail.DateModified.Month, fakeEmail.DateModified.Day);

            return fakeEmail;
        }

        public static TenancyAgreement CreateDatabaseTenancyEntity(string houseRef = null, string tenureTypeLookupId = null)
        {
            var fixture = new Fixture();

            var ft = fixture.Build<TenancyAgreement>()
                .With(ta => ta.HouseRef, houseRef)
                .Without(ta => ta.UhTenureType)
                .Create();

            ft.UhTenureTypeId = tenureTypeLookupId ?? ft.UhTenureTypeId;
            return ft;
        }

        public static ContactLink CreateDatabaseContactLinkForPerson(string tagRef, int? personNo)
        {
            var fixture = new Fixture();
            var stringPersonNo = personNo?.ToString();

            var cl = fixture.Build<ContactLink>()
                .With(cl => cl.TagRef, tagRef)
                .With(cl => cl.PersonNo, stringPersonNo)
                .Without(cl => cl.ContactID)
                .Create();

            return cl;
        }

        public static Contact CreateContactRecordFromTagRef(string tagRef)
        {
            var fixture = new Fixture();
            return new Contact { HouseReference = fixture.Create<string>().Substring(0, 6), TagRef = tagRef };
        }

        public static UhTenureType CreateTenureType()
        {
            var fixture = new Fixture();
            var tenureLookup = fixture.Create<UhTenureType>();
            return tenureLookup;
        }
    }
}
