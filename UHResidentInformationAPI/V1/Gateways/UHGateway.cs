using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
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

        public List<ResidentInformation> GetAllResidents(int cursor, int limit, string houseReference = null, string firstName = null, string lastName = null, string addressLine1 = null, string postcode = null)
        {
            return (List<ResidentInformation>) (
                from person in _uHContext.Persons
                where string.IsNullOrEmpty(houseReference) || EF.Functions.ILike(person.HouseRef, houseReference)
                where string.IsNullOrEmpty(firstName) || EF.Functions.ILike(person.FirstName, firstName)
                where string.IsNullOrEmpty(lastName) || EF.Functions.ILike(person.LastName, lastName)
                join address in _uHContext.Addresses on person.HouseRef equals address.HouseRef
                where string.IsNullOrEmpty(addressLine1) || EF.Functions.ILike(address.AddressLine1.Replace(" ", ""), "%" + addressLine1.Replace(" ", "") + "%")
                where string.IsNullOrEmpty(postcode) || EF.Functions.ILike(address.Postcode.Replace(" ", ""), "%" + postcode.Replace(" ", "") + "%")
                orderby person.HouseRef, person.PersonNo
                select new { person, address }
            ).Skip(cursor).Take(limit).ToList().Select(x =>
            {
                var person = new Person
                {
                    AtRisk = x.person.AtRisk,
                    BankAccType = x.person.BankAccType,
                    DateOfBirth = x.person.DateOfBirth,
                    FirstName = x.person.FirstName,
                    FullEd = x.person.FullEd,
                    HouseRef = x.person.HouseRef,
                    LastName = x.person.LastName,
                    MemberSID = x.person.MemberSID,
                    NINumber = x.person.NINumber,
                    Oap = x.person.Oap,
                    PersonNo = x.person.PersonNo,
                    Responsible = x.person.Responsible,
                    Title = x.person.Title
                };

                var address = new Address
                {
                    PropertyRef = x.address.PropertyRef,
                    AddressLine1 = x.address.AddressLine1,
                    Postcode = x.address.Postcode
                };

                var residentInformation = person.ToDomain();
                residentInformation.Address = address.ToDomain();

                return residentInformation;
            }).ToList();
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

            resident.Address = address.ToDomain();
            resident.UPRN = address.UPRN;

            return resident;
        }

        private static ResidentInformation AttachContactDetailsToPerson(ResidentInformation person, List<TelephoneNumber> phoneNumbers,
            List<EmailAddresses> emails)
        {
            person.PhoneNumberList = phoneNumbers.Any() ? phoneNumbers.Select(n => n.ToDomain()).ToList() : null;
            person.EmailList = emails.Any() ? emails.Select(n => n.ToDomain()).ToList() : null;
            return person;
        }

        private int? GetContactNoFromContactLink(string houseReference, int personReference)
        {
            var tagReference = _uHContext.TenancyAgreements.FirstOrDefault(ta => ta.HouseRef == houseReference)?.TagRef;
            var contactLinkUsingTagReference = _uHContext.ContactLinks
                .FirstOrDefault(co => (co.TagRef == tagReference) && (co.PersonNo == personReference.ToString(CultureInfo.InvariantCulture)))?.ContactID;

            return contactLinkUsingTagReference;
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
            var residentWithTagRefs = listOfPerson.ToList()
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

            //Left Join on tagRef to get contactNo from cccontactLink
            var residentWithContacts = residentWithTagRefs
            .GroupJoin(
                    _uHContext.ContactLinks,
                    //Join on composite key to get list of contactno
                    anon => new { personno = anon.person.PersonNo.ToString(), tagref = anon.tenancy.TagRef.Trim() },
                    contact => new { personno = contact.PersonNo.Trim(), tagref = contact.TagRef.Trim() },
                    (anon, contact) =>
                    {
                        var person = new
                        {
                            person = anon.person,
                            address = anon.address,
                            contact = contact
                        };
                        return person;
                    }
            );

            //retrieve and project all contactNo's
            var contacts = residentWithContacts
                .SelectMany(
                    anon => anon.contact
                    );

            //join contacts on PhoneNumbers using contactID
            var residentWithPhones = contacts
                .Join
                (
                    _uHContext.TelephoneNumbers,
                    anon => anon.ContactID,
                    telephone => telephone.ContactID,
                    (anon, telephone) =>
                    {
                        var telephones = new
                        {
                            contactid = anon.ContactID,
                            phone = telephone,
                        };
                        return telephones;
                    }
                );

            //join contacts on Emails using contactID
            var residentWithEmails = contacts
                .Join
                (
                    _uHContext.EmailAddresses,
                    anon => anon.ContactID,
                    email => email.ContactID,
                    (anon, email) =>
                    {
                        var emails = new
                        {
                            contactid = anon.ContactID,
                            email = email,
                        };
                        return emails;
                    }
                );

            //create list of residents
            var listOfResident = residentWithContacts.Select(
                person =>
                {
                    //Get all the phone numbers
                    var phoneList = residentWithPhones.Where(phone =>
                           person.contact.Any(c => phone.contactid == c.ContactID))
                    .Select(p => p.phone.ToDomain()).ToList();

                    //Get all the email addresses
                    var emailList = residentWithEmails.Where(email =>
                           person.contact.Any(c => email.contactid == c.ContactID))
                    .Select(e => e.email.ToDomain()).ToList();

                    var resident = person.person.ToDomain();
                    resident.PhoneNumberList = phoneList.Any() ? phoneList : null;
                    resident.EmailList = emailList.Any() ? emailList : null;
                    resident.Address = person.address.ToDomain();
                    resident.UPRN = person.address.UPRN;
                    return resident;
                }
                ).ToList();

            return listOfResident;
        }
    }
}
