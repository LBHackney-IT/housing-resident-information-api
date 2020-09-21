using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using HousingResidentInformationAPI.V1.Factories;
using HousingResidentInformationAPI.V1.Infrastructure;
using Address = HousingResidentInformationAPI.V1.Infrastructure.Address;
using ResidentInformation = HousingResidentInformationAPI.V1.Domain.ResidentInformation;

namespace HousingResidentInformationAPI.V1.Gateways
{
    public class HousingGateway : IHousingGateway
    {
        private readonly HousingContext _housingContext;

        public HousingGateway(HousingContext housingContext)
        {
            _housingContext = housingContext;
        }
        public ResidentInformation GetResidentById(string houseReference, int personReference)
        {
            var person = _housingContext
                .Persons
                .FirstOrDefault(p => p.HouseRef.Trim() == houseReference && p.PersonNo.Equals(personReference));

            if (person == null) return null;

            var address =
                _housingContext
                    .Addresses
                    .OrderByDescending(a => a.Dtstamp)
                    .FirstOrDefault(a => a.HouseRef.Trim() == person.HouseRef.Trim());

            var tenancy = _housingContext
                .TenancyAgreements
                .FirstOrDefault(ta => ta.HouseRef.Trim() == person.HouseRef.Trim());

            var contactKey = _housingContext
                .Contacts
                .FirstOrDefault(c => c.TagRef.Trim() == tenancy.TagRef.Trim())?.ContactKey;

            var contactLink = GetContactLinkForPerson(person.HouseRef.Trim(), person.PersonNo);

            var singleRecord = MapDetailsToResidentInformation(person, address, tenancy, contactLink, contactKey);

            return singleRecord;
        }

        public List<ResidentInformation> GetAllResidents(string cursor, int limit, string houseReference = null, string firstName = null, string lastName = null, string address = null)
        {
            var cursorAsInt = string.IsNullOrEmpty(cursor) ? 0 : int.Parse(cursor);

            var addressSearchPattern = GetSearchPattern(address);
            var houseReferenceSearchPattern = GetSearchPattern(houseReference);
            var firstNameSearchPattern = GetSearchPattern(firstName);
            var lastNameSearchPattern = GetSearchPattern(lastName);

            var dbRecords = (
                from person in _housingContext.Persons
                where string.IsNullOrEmpty(houseReference) || EF.Functions.ILike(person.HouseRef.Replace(" ", ""),
                    houseReferenceSearchPattern)
                where string.IsNullOrEmpty(firstName) ||
                      EF.Functions.ILike(person.FirstName, firstNameSearchPattern)
                where string.IsNullOrEmpty(lastName) || EF.Functions.ILike(person.LastName, lastNameSearchPattern)
                orderby person.HouseRef, person.PersonNo
                where cursorAsInt == 0 || Convert.ToInt32(person.HouseRef.Trim() + person.PersonNo.ToString()) > cursorAsInt
                join a in _housingContext.Addresses on person.HouseRef equals a.HouseRef
                where string.IsNullOrEmpty(address) ||
                      EF.Functions.ILike(a.AddressLine1.Replace(" ", ""), addressSearchPattern)
                join ta in _housingContext.TenancyAgreements on person.HouseRef equals ta.HouseRef
                join ck in _housingContext.Contacts on ta.TagRef equals ck.TagRef into cks
                from contacts in cks.DefaultIfEmpty()
                join c in _housingContext.ContactLinks on new { key1 = ta.TagRef, key2 = person.PersonNo.ToString() } equals new { key1 = c.TagRef, key2 = c.PersonNo } into addedContactLink
                from link in addedContactLink.DefaultIfEmpty()
                select new
                {
                    personDetails = person,
                    addressDetails = a,
                    tenancyDetails = ta,
                    contactDetails = link,
                    contactKey = contacts
                }
                ).Take(limit).ToList();

            if (!dbRecords.Any())
                return new List<ResidentInformation>();

            var listRecords = dbRecords.Select(x =>
                    MapDetailsToResidentInformation(x.personDetails, x.addressDetails, x.tenancyDetails,
                        x.contactDetails, x.contactKey?.ContactKey))
                .ToList();

            return listRecords;
        }

        private ResidentInformation MapDetailsToResidentInformation(Person person, Address address,
            TenancyAgreement tenancyAgreement, ContactLink contactLink, int? contactKey)
        {
            var resident = person.ToDomain();
            resident.UPRN = address?.UPRN;
            resident.ResidentAddress = address?.ToDomain();
            resident.TenancyReference = tenancyAgreement?.TagRef;
            resident.ContactKey = contactKey.ToString();

            if (contactLink == null) return resident;

            var telephoneNumberForPerson = _housingContext
                .TelephoneNumbers.Where(t => t.ContactID == contactLink.ContactID).ToList();

            var emailAddressForPerson =
                _housingContext.EmailAddresses.Where(c => c.ContactID == contactLink.ContactID).ToList();

            AttachContactDetailsToPerson(resident, telephoneNumberForPerson, emailAddressForPerson);

            return resident;
        }

        private static void AttachContactDetailsToPerson(ResidentInformation person, List<TelephoneNumber> phoneNumbers,
            List<EmailAddresses> emails)
        {
            person.PhoneNumber = phoneNumbers.Any() ? phoneNumbers.Select(n => n.ToDomain()).ToList() : null;
            person.Email = emails.Any() ? emails.Select(n => n.ToDomain()).ToList() : null;
        }

        private ContactLink GetContactLinkForPerson(string houseReference, int personReference)
        {
            var tagReference = _housingContext.TenancyAgreements.FirstOrDefault(ta => ta.HouseRef.Trim() == houseReference)?.TagRef;
            var contactLinkUsingTagReference = _housingContext.ContactLinks
                .FirstOrDefault(co => (co.TagRef == tagReference) && (co.PersonNo == personReference.ToString(CultureInfo.InvariantCulture)));

            return contactLinkUsingTagReference;
        }

        private static string GetSearchPattern(string str)
        {
            return $"%{str?.Replace(" ", "")}%";
        }
    }
}
