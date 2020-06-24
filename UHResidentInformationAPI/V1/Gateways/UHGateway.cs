using System.Collections.Generic;
using UHResidentInformationAPI.V1.Domain;
using UHResidentInformationAPI.V1.Infrastructure;

namespace UHResidentInformationAPI.V1.Gateways
{
    public class UHGateway : IUHGateway
    {
        private readonly UHContext _uHContext;

        public UHGateway(UHContext uHContext)
        {
            _uHContext = uHContext;
        }


        public List<ResidentInformation> GetAllResidents(string houseReference, string residentName, string address)
        {
            return new List<ResidentInformation>();
        }

        public ResidentInformation GetResidentById(string houseReference, int personReference)
        {
            return null;
        }
    }
}
