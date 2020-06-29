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
        public static ResidentInformation AddPersonWithRelatedEntitiesToDb(UHContext context, string id = null, string firstname = null, string lastname = null, string postcode = null, string addressLines = null)
        {
            var person = TestHelper.CreateDatabasePersonEntity(firstname, lastname, id);
            var addedPerson = context.Persons.Add(person);
            context.SaveChanges();

            var address = TestHelper.CreateDatabaseAddressForPersonId(addedPerson.Entity.HouseRef, address1: addressLines, postcode: postcode);
            var phone = TestHelper.CreateDatabaseTelephoneNumberForPersonId(addedPerson.Entity.PersonNo);

            context.Addresses.Add(address);
            context.TelephoneNumbers.Add(phone);
            context.SaveChanges();

            return new ResidentInformation
            {
                FirstName = person.FirstName,
                LastName = person.LastName,
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
                }

            };
        }
    }
}
