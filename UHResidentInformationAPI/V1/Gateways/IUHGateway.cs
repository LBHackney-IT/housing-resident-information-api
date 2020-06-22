using UHResidentInformationAPI.V1.Domain;

namespace UHResidentInformationAPI.V1.Gateways
{
    public interface IUHGateway
    {
        Entity GetEntityById(int id);
    }
}
