using System.Collections.Generic;
using System.Linq;
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
            var listOfPerson = _uHContext.Persons
                            .Where(a => string.IsNullOrEmpty(houseReference)|| a.HouseRef.Contains(houseReference))
                            .Where(a => string.IsNullOrEmpty(residentName)|| a.FirstName.Contains(residentName) || a.LastName.Contains(residentName))
                            // .Where(a => string.IsNullOrEmpty(residentName)|| a.FirstName.Contains(residentName) || a.LastName.Contains(residentName))
                            .ToList();

            _uHContext.Addresses.Where(a => string.IsNullOrEmpty(houseReference) || a.HouseRef.Contains(houseReference))

                       
            return new List<ResidentInformation>();
        }


    }
}
