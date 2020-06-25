using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UHResidentInformationAPI.V1.Domain;
using UHResidentInformationAPI.V1.Infrastructure;

namespace UHResidentInformationAPI.V1.Factories
{
    //Convert Database result objects to Application Domain objects
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

        public static Domain.Address ToDomain(this Infrastructure.Address databaseEntity)
        {
            return new Domain.Address();
        }

        public static Domain.Phone ToDomain(this Infrastructure.TelephoneNumber databaseEntity)
        {
            return new Domain.Phone();
        }

        public static List<Domain.Phone> ToDomain(this IEnumerable<Infrastructure.TelephoneNumber> phoneList)
        {
            return phoneList.Select(p => p.ToDomain()).ToList();
        }

    }
}
