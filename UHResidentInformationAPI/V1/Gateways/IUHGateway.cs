using System.Collections.Generic;
using UHResidentInformationAPI.V1.Domain;

namespace UHResidentInformationAPI.V1.Gateways
{
    public interface IUHGateway
    {
        List<ResidentInformation> GetAllResidents(int cursor, int limit, string houseReference, string firstName, string lastName, string addressLine1, string postcode);
        ResidentInformation GetResidentById(string houseReference, int personReference);
    }
}
