using System.Globalization;
using UHResidentInformationAPI.V1.Domain;
using UHResidentInformationAPI.V1.Infrastructure;

namespace UHResidentInformationAPI.V1.Factories
{
    public static class EntityFactory
    {
        public static ResidentInformation ToDomain(this Person databaseEntity)
        {
            return new ResidentInformation
            {
                HouseReference = databaseEntity.houseRef,
                PersonNumber = databaseEntity.personNo,
                FirstName = databaseEntity.FirstName,
                LastName = databaseEntity.LastName,
                DateOfBirth = databaseEntity.DateOfBirth.ToString("O", CultureInfo.InvariantCulture),
                NINumber = databaseEntity.NINumber
            };
        }
    }
}
