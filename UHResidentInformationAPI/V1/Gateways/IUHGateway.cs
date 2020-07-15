using System.Collections.Generic;
using ResidentInformation = UHResidentInformationAPI.V1.Domain.ResidentInformation;

namespace UHResidentInformationAPI.V1.Gateways
{
    public interface IUHGateway
    {
        List<ResidentInformation> GetAllResidents(int cursor, int limit, string houseReference, string firstName, string lastName, string address);
        ResidentInformation GetResidentById(string houseReference, int personReference);
    }
}
