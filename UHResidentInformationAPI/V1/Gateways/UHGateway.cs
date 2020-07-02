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
                .Where(a => string.IsNullOrEmpty(firstName) || a.FirstName.Trim().ToLower().Contains(firstName.ToLower()))
                .Where(a => string.IsNullOrEmpty(lastName) || a.LastName.Trim().ToLower().Contains(lastName.ToLower()))
                .Join(
                _uHContext.Addresses
                .Where(a => string.IsNullOrEmpty(address) || a.AddressLine1.ToLower().Contains(address.ToLower())),
                person => person.HouseRef,
                address => address.HouseRef,
                (person, address) => new { person, address });

            //Query result is empty, return empty list
            if (!listOfPerson.Any())
                return new List<ResidentInformation>();

            // Join to get tag_ref from tenagree
            var resident = listOfPerson.ToList()
            .Join(
                    _uHContext.TenancyAgreements,
                    anon => anon.person.HouseRef,
                    tenancy => tenancy.HouseRef,
                    (anon, tenancy) =>
                    {
                        var people = new
                        {
                            person = anon.person,
                            address = anon.address,
                            tenancy = tenancy
                        };
                        return people;
                    }
            );
            // Join on tagRef to get contactNo from cccontactLink
            var resident1 = resident
            .GroupJoin(
                    _uHContext.ContactLinks,
                    anon => anon.tenancy.TagRef.Trim(),
                    contact => contact.TagRef.Trim(),
                    (anon, contact) =>
                    {
                        var person = new
                        {
                            person = anon.person,
                            address = anon.address,
                            contact = contact.FirstOrDefault()
                        };
                        return person;
                    }
            );

            //Left join on resident1 and PhoneNumbers using contactID 
            var resident2 = resident1
                .GroupJoin
                (
                    _uHContext.TelephoneNumbers,

                    anon => anon.contact?.ContactID,
                    telephone => telephone.ContactID,
                    (anon, telephone) =>
                    {
                        var resident = new
                        {
                            person = anon.person,
                            address = anon.address,
                            phone = telephone,
                            contact = anon.contact
                        };
                        return resident;
                    }
                );

            //Left join on resident2 and email addresses using contactID from cccontactLink
            var listOfResident = resident2
                .GroupJoin
                (
                    _uHContext.EmailAddresses,
                    anon => anon.contact?.ContactID,
                    email => email.ContactID,
                    (anon, email) =>
                    {
                        var resident = anon.person.ToDomain();
                        resident.Email = email.Any() ? email.Select(e => e.ToDomain()).ToList() : null;
                        resident.ResidentAddress = anon.address.ToDomain();
                        resident.UPRN = anon.address.UPRN;
                        resident.PhoneNumber = anon.phone.Any() ? anon.phone.ToList().ToDomain() : null;
                        return resident;
                    }
                 ).ToList();

            return listOfResident;
        }
    }
}
