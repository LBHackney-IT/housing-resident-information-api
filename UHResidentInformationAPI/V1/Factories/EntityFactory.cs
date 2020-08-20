using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UHResidentInformationAPI.V1.Domain;
using UHResidentInformationAPI.V1.Enums;
using UHResidentInformationAPI.V1.Infrastructure;
using Address = UHResidentInformationAPI.V1.Domain.Address;
using DbAddress = UHResidentInformationAPI.V1.Infrastructure.Address;

namespace UHResidentInformationAPI.V1.Factories
{
    //Convert Database result objects to Application Domain objects
    public static class EntityFactory
    {
        public static ResidentInformation ToDomain(this Person databaseEntity)
        {
            return new ResidentInformation
            {
                HouseReference = databaseEntity.HouseRef.Trim(),
                PersonNumber = databaseEntity.PersonNo,
                FirstName = databaseEntity.FirstName.Trim(),
                LastName = databaseEntity.LastName.Trim(),
                DateOfBirth = databaseEntity.DateOfBirth.ToString("O", CultureInfo.InvariantCulture),
                NINumber = databaseEntity.NINumber.Trim(),
            };
        }

        public static List<ResidentInformation> ToDomain(this IEnumerable<Person> people)
        {
            return people.Select(p => p.ToDomain()).ToList();
        }

        public static List<Email> ToDomain(this IEnumerable<EmailAddresses> people)
        {
            return people.Select(p => p.ToDomain()).ToList();
        }

        public static List<Phone> ToDomain(this IEnumerable<TelephoneNumber> people)
        {
            return people.Select(p => p.ToDomain()).ToList();
        }

        public static Address ToDomain(this DbAddress databaseEntity)
        {
            return new Address
            {
                PropertyRef = databaseEntity.PropertyRef.Trim(),
                AddressLine1 = databaseEntity.AddressLine1.Trim(),
                PostCode = databaseEntity.PostCode.Trim()
            };
        }

        public static Phone ToDomain(this TelephoneNumber databaseEntity)
        {
            var canParseType = Enum.TryParse<PhoneType>(databaseEntity.Type, out var type);

            return new Phone
            {
                PhoneNumber = databaseEntity.Number,
                Type = canParseType ? type : (PhoneType?) null,
                LastModified = databaseEntity.DateCreated
            };
        }
        public static Email ToDomain(this EmailAddresses databaseEntity)
        {
            return new Email
            {
                EmailAddress = databaseEntity.EmailAddress.Trim(),
                LastModified = databaseEntity.DateModified
            };
        }

    }
}
