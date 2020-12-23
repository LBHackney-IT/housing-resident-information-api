using System.Collections.Generic;
using System.Globalization;
using HousingResidentInformationAPI.Tests.V1.Helper;
using HousingResidentInformationAPI.V1.Boundary.Responses;
using HousingResidentInformationAPI.V1.Infrastructure;
using Address = HousingResidentInformationAPI.V1.Boundary.Responses.Address;

namespace HousingResidentInformationAPI.Tests.V1.E2ETests
{
    public static class E2ETestHelpers
    {
        public static ResidentInformation AddPersonWithRelatedEntitiesToDb(UHContext context, string houseRef = null,
            int? personNo = null, string firstname = null, string lastname = null, string postcode = null,
            string addressLines = null, string tenureTypeId = null, bool isTerminated = false)
        {
            //create person record
            var person = TestHelper.CreateDatabasePersonEntity(firstname, lastname);
            person.HouseRef = houseRef ?? person.HouseRef;
            person.PersonNo = personNo ?? person.PersonNo;

            var addedPerson = context.Persons.Add(person);
            context.SaveChanges();

            //create tenure record
            var tenure = TestHelper.CreateTenureType();
            tenure.UhTenureTypeId = tenureTypeId ?? tenure.UhTenureTypeId;
            context.UhTenure.Add(tenure);
            context.SaveChanges();

            //create address, and tenancy entities with tenuretypeId
            var address = TestHelper.CreateDatabaseAddressForPersonId(addedPerson.Entity.HouseRef, address1: addressLines, postcode: postcode);
            var tenancyAgreement = TestHelper.CreateDatabaseTenancyEntity(addedPerson.Entity.HouseRef, tenure.UhTenureTypeId);
            tenancyAgreement.IsTerminated = isTerminated;
            tenancyAgreement.TagRef = houseRef ?? tenancyAgreement.TagRef;

            //create contact Link record
            var contactLink = TestHelper.CreateDatabaseContactLinkForPerson(tenancyAgreement.TagRef, addedPerson.Entity.PersonNo);
            var addedContact = context.ContactLinks.Add(contactLink);
            context.SaveChanges();
            var phone = TestHelper.CreateDatabaseTelephoneNumberForPersonId(addedContact.Entity.ContactID);
            var email = TestHelper.CreateDatabaseEmailForPerson(addedContact.Entity.ContactID);
            var contact = TestHelper.CreateContactRecordFromTagRef(tenancyAgreement.TagRef);

            context.TenancyAgreements.Add(tenancyAgreement);
            context.Addresses.Add(address);
            context.TelephoneNumbers.Add(phone);
            context.EmailAddresses.Add(email);
            context.Contacts.Add(contact);
            context.SaveChanges();

            return new ResidentInformation
            {
                HouseReference = person.HouseRef,
                PersonNumber = person.PersonNo,
                TenancyReference = tenancyAgreement.TagRef,
                TenureType = $"{tenure.UhTenureTypeId}: {tenure?.Description}",
                FirstName = person.FirstName,
                LastName = person.LastName,
                NiNumber = person.NINumber,
                Uprn = address.UPRN,
                HousingWaitingListContactKey = contact.ContactKey.ToString(),
                PhoneNumbers =
                    new List<Phone>
                    {
                        new Phone
                        {
                            PhoneNumber = phone.Number,
                            PhoneType = phone.Type,
                            LastModified = phone.DateCreated.ToString("O", CultureInfo.InvariantCulture)
                        }
                    },
                EmailAddresses = new List<Email>
                {
                    new Email
                    {
                        EmailAddress = email.EmailAddress,
                        LastModified = email.DateModified.ToString("O", CultureInfo.InvariantCulture)
                    }
                },
                Address = new Address
                {
                    PropertyRef = address.PropertyRef,
                    AddressLine1 = address.AddressLine1,
                    Postcode = address.PostCode
                },
                DateOfBirth = person.DateOfBirth.ToString("O", CultureInfo.InvariantCulture)
            };
        }
    }
}
