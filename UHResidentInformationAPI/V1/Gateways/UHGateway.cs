using System.Collections.Generic;
using  UHResidentInformationAPI.V1.Boundary.Responses;
using UHResidentInformationAPI.V1.Domain;
using UHResidentInformationAPI.V1.Factories;
using UHResidentInformationAPI.V1.Infrastructure;
using ResidentInformation = UHResidentInformationAPI.V1.Domain.ResidentInformation;

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


    }
}
