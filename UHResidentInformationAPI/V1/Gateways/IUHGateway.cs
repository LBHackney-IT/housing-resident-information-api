using System.Collections.Generic;
using UHResidentInformationAPI.V1.Domain;

namespace UHResidentInformationAPI.V1.Gateways
{
    public interface IUHGateway
    {
        List<ResidentInformation> GetAllResidents(string houseReference, string residentName, string address);
        ResidentInformation GetResidentById(string houseReference, int personReference);
    }
}
