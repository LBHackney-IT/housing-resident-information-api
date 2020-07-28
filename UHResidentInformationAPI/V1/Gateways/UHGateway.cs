using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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
        public ResidentInformation GetResidentById(string houseReference, int personReference)
        {
            var databaseRecord = _uHContext.Persons.FirstOrDefault(p => p.HouseRef == houseReference && p.PersonNo.Equals(personReference));

            if (databaseRecord == null) return null;

            var addressForPerson =
                _uHContext.Addresses.OrderByDescending(a => a.Dtstamp).FirstOrDefault(a => a.HouseRef == databaseRecord.HouseRef);

            var tenancyForPerson = _uHContext.TenancyAgreements.FirstOrDefault(ta => ta.HouseRef == databaseRecord.HouseRef);

            var contactLinkNoForPerson = GetContactNoFromContactLink(databaseRecord.HouseRef, databaseRecord.PersonNo);

            var telephoneNumberForPerson = _uHContext.TelephoneNumbers.Where(t => t.ContactID == contactLinkNoForPerson).ToList();

            var emailAddressForPerson =
                _uHContext.EmailAddresses.Where(c => c.ContactID == contactLinkNoForPerson).ToList();

            var person = MapPersonWithTenancyAndAddressesToResidentInformation(databaseRecord, addressForPerson, tenancyForPerson);

            AttachContactDetailsToPerson(person, telephoneNumberForPerson, emailAddressForPerson);

            return person;
        }

        private static ResidentInformation MapPersonWithTenancyAndAddressesToResidentInformation(Person person, Address address, TenancyAgreement tenancyAgreement)
        {
            var resident = person.ToDomain();

            resident.ResidentAddress = address?.ToDomain();
            resident.UPRN = address?.UPRN;
            resident.TenancyReference = tenancyAgreement?.TagRef;

            return resident;
        }

        private static void AttachContactDetailsToPerson(ResidentInformation person, List<TelephoneNumber> phoneNumbers,
            List<EmailAddresses> emails)
        {
            person.PhoneNumber = phoneNumbers.Any() ? phoneNumbers.Select(n => n.ToDomain()).ToList() : null;
            person.Email = emails.Any() ? emails.Select(n => n.ToDomain()).ToList() : null;
        }

        private int? GetContactNoFromContactLink(string houseReference, int personReference)
        {
            var tagReference = _uHContext.TenancyAgreements.FirstOrDefault(ta => ta.HouseRef == houseReference)?.TagRef;
            var contactLinkUsingTagReference = _uHContext.ContactLinks
                .FirstOrDefault(co => (co.TagRef == tagReference) && (co.PersonNo == personReference.ToString(CultureInfo.InvariantCulture)))?.ContactID;

            return contactLinkUsingTagReference;
        }

        private static string GetSearchPattern(string str)
        {
            return $"%{str?.Replace(" ", "")}%";
        }

        public List<ResidentInformation> GetAllResidents(string houseReference = null, string firstName = null, string lastName = null, string address = null)
        {
            var addressSearchPattern = GetSearchPattern(address);
            var houseReferenceSearchPattern = GetSearchPattern(houseReference);
            var firstNameSearchPattern = GetSearchPattern(firstName);
            var lastNameSearchPattern = GetSearchPattern(lastName);

            var dbRecords = (from person in _uHContext.Persons
                             where string.IsNullOrEmpty(firstName) ||
                                   EF.Functions.ILike(person.FirstName, firstNameSearchPattern)
                             where string.IsNullOrEmpty(lastName) || EF.Functions.ILike(person.LastName, lastNameSearchPattern)
                             where string.IsNullOrEmpty(houseReference) || EF.Functions.ILike(person.HouseRef.Replace(" ", ""),
                                 houseReferenceSearchPattern)
                             join a in _uHContext.Addresses on person.HouseRef equals a.HouseRef
                             where string.IsNullOrEmpty(address) ||
                                   EF.Functions.ILike(a.AddressLine1.Replace(" ", ""), addressSearchPattern)
                             join ta in _uHContext.TenancyAgreements on person.HouseRef equals ta.HouseRef
                             join c in _uHContext.ContactLinks on new { key1 = ta.TagRef, key2 = person.PersonNo.ToString() } equals new { key1 = c.TagRef, key2 = c.PersonNo } into addedContactLink
                             from link in addedContactLink.DefaultIfEmpty()
                             orderby person.HouseRef, person.PersonNo
                             select new
                             {
                                 personDetails = person,
                                 addressDetails = a,
                                 tenancyDetails = ta,
                                 contactDetails = link
                             }
                ).ToList();

            if (!dbRecords.Any())
                return new List<ResidentInformation>();

            var listRecords = dbRecords.Select(x =>
                    MapDetailsToResidentInformation(x.personDetails, x.addressDetails, x.tenancyDetails,
                        x.contactDetails))
                .ToList();

            return listRecords;
        }

        private ResidentInformation MapDetailsToResidentInformation(Person person, Address address, TenancyAgreement tenancyAgreement, ContactLink contactLink)
        {
            var resident = person.ToDomain();
            resident.UPRN = address?.UPRN;
            resident.ResidentAddress = address?.ToDomain();
            resident.TenancyReference = tenancyAgreement?.TagRef;

            if (contactLink == null) return resident;
            var telephoneNumberForPerson = _uHContext.TelephoneNumbers.Where(t => t.ContactID == contactLink.ContactID).ToList();

            var emailAddressForPerson =
                _uHContext.EmailAddresses.Where(c => c.ContactID == contactLink.ContactID).ToList();

            AttachContactDetailsToPerson(resident, telephoneNumberForPerson, emailAddressForPerson);

            return resident;
        }
    }
}
