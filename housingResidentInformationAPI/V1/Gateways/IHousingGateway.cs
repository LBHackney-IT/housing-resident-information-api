using System.Collections.Generic;
using housingResidentInformationAPI.V1.Domain;
using ResidentInformation = housingResidentInformationAPI.V1.Domain.ResidentInformation;

namespace housingResidentInformationAPI.V1.Gateways
{
    public interface IHousingGateway
    {
        List<ResidentInformation> GetAllResidents(string cursor, int limit, string houseReference, string firstName, string lastName, string address);
        ResidentInformation GetResidentById(string houseReference, int personReference);
    }
}
