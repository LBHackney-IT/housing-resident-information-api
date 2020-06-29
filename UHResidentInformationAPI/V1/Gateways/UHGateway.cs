using System.Collections.Generic;
using System.Linq;
using UHResidentInformationAPI.V1.Boundary.Responses;
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

        public List<ResidentInformation> GetAllResidents(string houseReference, string firstName, string lastName, string address)
        {
            var listOfPerson = _uHContext.Persons
                .Where(a => string.IsNullOrEmpty(houseReference) || a.HouseRef.Contains(houseReference))
                .Where(a => string.IsNullOrEmpty(firstName) || a.FirstName.Contains(firstName))
                .Where(a => string.IsNullOrEmpty(lastName) || a.LastName.Contains(lastName))
                .Join(_uHContext.Addresses,
                person => person.HouseRef,
                address => address.HouseRef,
                (person, address) => new { person, address }
                )
                .GroupJoin(_uHContext.TelephoneNumbers,
                pa => pa.person.PersonNo,
                telephone => telephone.ContactID,
                (pa, t) => new ResidentInformation
                {
                    HouseReference = pa.person.HouseRef,
                    ResidentAddress = new UHResidentInformationAPI.V1.Domain.Address
                    {
                        AddressLine1 = pa.address.AddressLine1
                    },
                    PhoneNumber = t.ToDomain().ToList()
                }
                ).ToList();


            return listOfPerson;
        }

    }
}
