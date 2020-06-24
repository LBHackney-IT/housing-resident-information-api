using System.Collections.Generic;
using UHResidentInformationAPI.V1.Boundary.Responses;
using UHResidentInformationAPI.V1.Domain;
using ResidentInformation = UHResidentInformationAPI.V1.Domain.ResidentInformation;

namespace UHResidentInformationAPI.V1.Gateways
{
    public interface IUHGateway
    {
        List<ResidentInformation> GetAllResidents(string houseReference, string residentName, string address);
    }
}
