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

            var contactLinkNoForPerson = GetContactNoFromContactLink(databaseRecord.HouseRef, databaseRecord.PersonNo);

            var telephoneNumberForPerson = _uHContext.TelephoneNumbers.Where(t => t.ContactID == contactLinkNoForPerson).ToList();

            var emailAddressForPerson =
                _uHContext.EmailAddresses.Where(c => c.ContactID == contactLinkNoForPerson).ToList();

            var person = MapPersonAndAddressesToResidentInformation(databaseRecord, addressForPerson);

            AttachContactDetailsToPerson(person, telephoneNumberForPerson, emailAddressForPerson);

            return person;
        }

        private static ResidentInformation MapPersonAndAddressesToResidentInformation(Person person, Address address)
        {
            var resident = person.ToDomain();

            if (address == null) return resident;

            resident.ResidentAddress = address.ToDomain();
            resident.UPRN = address.UPRN;

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

        public List<ResidentInformation> GetAllResidents(int cursor, int limit, string houseReference = null, string firstName = null, string lastName = null, string address = null)
        {
            var houseRefQuery = "%" + houseReference?.Replace(" ", "") + "%";
            var firstNameQuery = "%" + firstName?.Replace(" ", "") + "%";
            var lastNameQuery = "%" + lastName?.Replace(" ", "") + "%";
            // var addressQuery = "%" + address?.Replace(" ", "") + "%";

            return _uHContext.Persons
                .Where(p => string.IsNullOrEmpty(houseReference) || EF.Functions.ILike(p.HouseRef.Replace(" ", ""), houseRefQuery))
                .Where(p => string.IsNullOrEmpty(firstName) || EF.Functions.ILike(p.FirstName.Replace(" ", ""), firstNameQuery))
                .Where(p => string.IsNullOrEmpty(lastName) || EF.Functions.ILike(p.LastName.Replace(" ", ""), lastNameQuery))
                .Join(
                    _uHContext.Addresses
                        .Where(a => string.IsNullOrEmpty(address) || EF.Functions.ILike(address.Replace(" ", ""), "%" + address.Replace(" ", "") + "%")),
                        person => person.HouseRef,
                        address => address.HouseRef,
                        (person, address) => new { person, address })
                .OrderBy(x => x.person.HouseRef + " " + x.person.PersonNo)
                .ToList()
                .Select(x => x.person.ToDomain())
                .ToList();

            // //Inner join on person and address
            // var listOfPerson = _uHContext.Persons
            //     .Where(p => string.IsNullOrEmpty(houseReference) || EF.Functions.ILike(p.HouseRef.Replace(" ", ""), houseRefQuery))
            //     .Where(p => string.IsNullOrEmpty(firstName) || EF.Functions.ILike(p.FirstName.Replace(" ", ""), firstNameQuery))
            //     .Where(p => string.IsNullOrEmpty(lastName) || EF.Functions.ILike(p.LastName.Replace(" ", ""), lastNameQuery))
            //     .Join(
            //         _uHContext.Addresses
            //             .Where(a => string.IsNullOrEmpty(address) || EF.Functions.ILike(address.Replace(" ", ""), addressQuery)),
            //             person => person.HouseRef,
            //             address => address.HouseRef,
            //             (person, address) => new { person, address })
            //     .OrderBy(x => x.person.HouseRef + " " + x.person.PersonNo);
            //     // .Skip(cursor)
            //     // .Take(limit);

            // //Query result is empty, return empty list
            // if (!listOfPerson.Any())
            //     return new List<ResidentInformation>();

            // // Join to get tag_ref from tenagree
            // var residentWithTagRefs = listOfPerson
            //     .ToList()
            //     .Join(_uHContext.TenancyAgreements,
            //         anon => anon.person.HouseRef,
            //         tenancy => tenancy.HouseRef,
            //         (anon, tenancy) =>
            //         {
            //             var people = new
            //             {
            //                 person = anon.person,
            //                 address = anon.address,
            //                 tenancy = tenancy
            //             };
            //             return people;
            //         });

            // //Left Join on tagRef to get contactNo from cccontactLink
            // var residentWithContacts = residentWithTagRefs
            //     .GroupJoin(
            //         _uHContext.ContactLinks,
            //         //Join on composite key to get list of contactno
            //         anon => new { personno = anon.person.PersonNo.ToString(), tagref = anon.tenancy.TagRef.Trim() },
            //         contact => new { personno = contact.PersonNo.Trim(), tagref = contact.TagRef.Trim() },
            //         (anon, contact) =>
            //         {
            //             var person = new
            //             {
            //                 person = anon.person,
            //                 address = anon.address,
            //                 contact = contact
            //             };
            //             return person;
            //         }
            //     );

            // //retrieve and project all contactNo's
            // var contacts = residentWithContacts
            //     .SelectMany(anon => anon.contact);

            // //join contacts on PhoneNumbers using contactID
            // var residentWithPhones = contacts
            //     .Join
            //     (
            //         _uHContext.TelephoneNumbers,
            //         anon => anon.ContactID,
            //         telephone => telephone.ContactID,
            //         (anon, telephone) =>
            //         {
            //             var telephones = new
            //             {
            //                 contactid = anon.ContactID,
            //                 phone = telephone,
            //             };
            //             return telephones;
            //         }
            //     );

            // //join contacts on Emails using contactID
            // var residentWithEmails = contacts
            //     .Join
            //     (
            //         _uHContext.EmailAddresses,
            //         anon => anon.ContactID,
            //         email => email.ContactID,
            //         (anon, email) =>
            //         {
            //             var emails = new
            //             {
            //                 contactid = anon.ContactID,
            //                 email = email,
            //             };
            //             return emails;
            //         }
            //     );

            // //create list of residents
            // var listOfResidents = residentWithContacts
            //     .Select(person =>
            //     {
            //         //Get all the phone numbers
            //         var phoneList = residentWithPhones.Where(phone =>
            //                person.contact.Any(c => phone.contactid == c.ContactID))
            //         .Select(p => p.phone.ToDomain()).ToList();

            //         //Get all the email addresses
            //         var emailList = residentWithEmails.Where(email =>
            //                person.contact.Any(c => email.contactid == c.ContactID))
            //         .Select(e => e.email.ToDomain()).ToList();

            //         var resident = person.person.ToDomain();
            //         resident.PhoneNumber = phoneList.Any() ? phoneList : null;
            //         resident.Email = emailList.Any() ? emailList : null;
            //         resident.ResidentAddress = person.address.ToDomain();
            //         resident.UPRN = person.address.UPRN;

            //         return resident;
            //     }).ToList();

            // return listOfResidents;
        }
    }
}
