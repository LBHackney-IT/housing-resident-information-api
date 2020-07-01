using System.Collections.Generic;
using System.Linq;
using UHResidentInformationAPI.V1.Factories;
using UHResidentInformationAPI.V1.Infrastructure;
using Address = UHResidentInformationAPI.V1.Infrastructure.Address;
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

        public ResidentInformation GetResidentById(string houseReference, int personReference)
        {
            var databaseRecord = _uHContext.Persons.FirstOrDefault(p => p.HouseRef == houseReference && p.PersonNo.Equals(personReference));

            if (databaseRecord == null) return null;

            var addressForPerson =
                _uHContext.Addresses.OrderByDescending(a => a.Dtstamp).FirstOrDefault(a => a.HouseRef == databaseRecord.HouseRef);

            var retrievedTagReference =
                _uHContext.TenancyAgreements.FirstOrDefault(ta => ta.HouseRef == databaseRecord.HouseRef)?.TagRef;

            var retrievedContactLinkNo =
                _uHContext.ContactLinks.FirstOrDefault(co => co.TagRef == retrievedTagReference)?.ContactID;

            var telephoneNumberForPerson = _uHContext.TelephoneNumbers.Where(t => t.ContactID == retrievedContactLinkNo).ToList();

            var emailAddressForPerson =
                _uHContext.EmailAddresses.Where(c => c.ContactID == retrievedContactLinkNo).ToList();


            var person = MapPersonAndAddressesToResidentInformation(databaseRecord, addressForPerson);

            AttachContactDetailsToPerson(person, telephoneNumberForPerson, emailAddressForPerson);

            return person;
        }

        private static ResidentInformation MapPersonAndAddressesToResidentInformation(Person person, Address address)
        {
            var resident = person.ToDomain();

            if (address == null) return resident;

            resident.Address = address.ToDomain();
            resident.UPRN = address.UPRN;

            return resident;
        }

        private static void AttachContactDetailsToPerson(ResidentInformation person, List<TelephoneNumber> phoneNumbers,
            List<EmailAddresses> emails)
        {
            person.PhoneNumber = phoneNumbers.Any() ? phoneNumbers.Select(n => n.ToDomain()).ToList() : null;
            person.Email = emails.Any() ? emails.Select(n => n.ToDomain()).ToList() : null;
        }

    }
}
