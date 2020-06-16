using UHResidentInformationAPI.V1.Domain;

namespace UHResidentInformationAPI.V1.Gateways
{
    public interface IExampleGateway
    {
        Entity GetEntityById(int id);
    }
}
