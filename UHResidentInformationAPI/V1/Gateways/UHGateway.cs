using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
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

        public List<ResidentInformation> GetAllResidents(string houseReference = null, string firstName = null, string lastName = null, string address = null)
        {
            //Inner join on person and address
            var listOfPerson = _uHContext.Persons
                .Where(a => string.IsNullOrEmpty(houseReference) || a.HouseRef.Contains(houseReference))
                .Where(a => string.IsNullOrEmpty(firstName) || a.FirstName.ToLower().Contains(firstName.ToLower()))
                .Where(a => string.IsNullOrEmpty(lastName) || a.LastName.ToLower().Contains(lastName.ToLower()))
                .Join(
                _uHContext.Addresses,
                person => person.HouseRef,
                address => address.HouseRef,
                (person, address) => new { person, address });

            //Left join on listOfPerson and PhoneNumbers
            var listOfResident = listOfPerson.ToList()
                .GroupJoin
                (
                    _uHContext.TelephoneNumbers,
                    anon => anon.person.PersonNo,
                    telephone => telephone.ContactID,
                    (anon, telephone) =>
                    {
                        var resident = anon.person.ToDomain();
                        resident.ResidentAddress = anon.address.ToDomain();
                        resident.PhoneNumber = telephone.Any() ? telephone.ToList().ToDomain() : null;
                        return resident;
                    }
                ).ToList();

            return listOfResident;
        }
    }
}
