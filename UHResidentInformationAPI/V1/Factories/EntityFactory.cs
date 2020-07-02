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
    public static class EntityFactory
    {
        public static ResidentInformation ToDomain(this Person databaseEntity)
        {
            return new ResidentInformation
            {
                HouseReference = databaseEntity.HouseRef,
                PersonNumber = databaseEntity.PersonNo,
                FirstName = databaseEntity.FirstName,
                LastName = databaseEntity.LastName,
                DateOfBirth = databaseEntity.DateOfBirth.ToString("O", CultureInfo.InvariantCulture),
                NINumber = databaseEntity.NINumber
            };
        }

        public static List<ResidentInformation> ToDomain(this IEnumerable<Person> people)
        {
            return people.Select(p => p.ToDomain()).ToList();
        }

        public static Address ToDomain(this DbAddress databaseEntity)
        {
            return new Address
            {
                PropertyRef = databaseEntity.PropertyRef,
                AddressLine1 = databaseEntity.AddressLine1.Trim(),
                PostCode = databaseEntity.PostCode
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
                EmailAddress = databaseEntity.EmailAddress,
                LastModified = databaseEntity.DateModified
            };
        }
    }
}
