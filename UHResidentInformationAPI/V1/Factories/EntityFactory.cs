using UHResidentInformationAPI.V1.Domain;
using UHResidentInformationAPI.V1.Infrastructure;

namespace UHResidentInformationAPI.V1.Factories
{
    public static class EntityFactory
    {
        public static Entity ToDomain(this DatabaseEntity databaseEntity)
        {
            return new Entity
            {
                Id = databaseEntity.Id,
                CreatedAt = databaseEntity.CreatedAt,
            };
        }
    }
}
