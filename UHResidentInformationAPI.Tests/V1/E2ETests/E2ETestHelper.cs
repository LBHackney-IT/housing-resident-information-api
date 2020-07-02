using System;
using System.Collections.Generic;
using System.Globalization;
using UHResidentInformationAPI.Tests.V1.Helper;
using UHResidentInformationAPI.V1.Boundary.Responses;
using UHResidentInformationAPI.V1.Enums;
using UHResidentInformationAPI.V1.Infrastructure;

namespace UHResidentInformationAPI.Tests.V1.E2ETests
{
    public static class E2ETestHelpers
    {
        public static ResidentInformation AddPersonWithRelatedEntitiesToDb(UHContext context, string id = null, string firstname = null, string lastname = null, string addressLines = null)
        {
            var person = TestHelper.CreateDatabasePersonEntity(firstname, lastname, id);
            var addedPerson = context.Persons.Add(person);
            context.SaveChanges();

            var address = TestHelper.CreateDatabaseAddressForPersonId(addedPerson.Entity.HouseRef, address1: addressLines);
            //Save entities for tenancy agreement and contact
            var tenancyAgreement = TestHelper.CreateDatabaseTenancyAgreementForPerson(addedPerson.Entity.HouseRef);
            var contactLink = TestHelper.CreateDatabaseContactLinkForPerson(tenancyAgreement.TagRef);
            var phone = TestHelper.CreateDatabaseTelephoneNumberForPersonId(contactLink.ContactID);
            var email = TestHelper.CreateDatabaseEmailForPerson(contactLink.ContactID);

            context.Addresses.Add(address);
            context.TenancyAgreements.Add(tenancyAgreement);
            context.ContactLinks.Add(contactLink);
            context.TelephoneNumbers.Add(phone);
            context.EmailAddresses.Add(email);
            context.SaveChanges();


            return new ResidentInformation
            {
                FirstName = person.FirstName.Trim(),
                LastName = person.LastName.Trim(),
                PhoneNumber =
                    new List<Phone>
                    {
                        new Phone { PhoneNumber = phone.Number, PhoneType = Enum.Parse<PhoneType>(phone.Type) }
                    },
                DateOfBirth = person.DateOfBirth.ToString("O", CultureInfo.InvariantCulture),
                ResidentAddress = new UHResidentInformationAPI.V1.Boundary.Responses.Address
                {
                    PropertyRef = address.PropertyRef,
                    AddressLine1 = address.AddressLine1,
                    PostCode = address.PostCode
                },
                UPRN = address.UPRN,
                Email =
                    new List<Email>
                    {
                        new Email {EmailAddress = email.EmailAddress, LastModified = email.DateModified.ToString("O")}
                    }
            };

        }
    }
}
