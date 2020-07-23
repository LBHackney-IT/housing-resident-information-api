using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
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

        public List<ResidentInformation> GetAllResidents(string houseReference = null, string firstName = null, string lastName = null, string address = null)
        {
            Console.WriteLine("Inner join on person and address");
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

            Console.WriteLine($"one person: {JsonConvert.SerializeObject(listOfPerson.FirstOrDefault().person)}");

            Console.WriteLine("Query result is empty, return empty list");
            //Query result is empty, return empty list
            if (!listOfPerson.Any())
                return new List<ResidentInformation>();

            Console.WriteLine("Join to get tag_ref from tenagree");
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
            Console.WriteLine($"tenagreeperson: {JsonConvert.SerializeObject(residentWithTagRefs.FirstOrDefault().person)}, address: {JsonConvert.SerializeObject(residentWithTagRefs.FirstOrDefault().address)}, tenancy: {JsonConvert.SerializeObject(residentWithTagRefs.FirstOrDefault().tenancy)}");

            Console.WriteLine("Left Join on tagRef to get contactNo from cccontactLink");
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
                            tenancy = anon.tenancy,
                            contact = contact
                        };

                        return person;
                    }
            );

            Console.WriteLine($"cccontactLinkperson: {JsonConvert.SerializeObject(residentWithContacts.FirstOrDefault().person)}, address: {JsonConvert.SerializeObject(residentWithContacts.FirstOrDefault().address)}, contact: {JsonConvert.SerializeObject(residentWithContacts.FirstOrDefault().contact)}");


            Console.WriteLine("retrieve and project all contactNo's");
            //retrieve and project all contactNo's
            var contacts = residentWithContacts
                .SelectMany(
                    anon => anon.contact
                    );

            Console.WriteLine("join contacts on PhoneNumbers using contactID");
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

            Console.WriteLine("join contacts on Emails using contactID");
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

            Console.WriteLine("create list of residents");
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
                    resident.PhoneNumber = phoneList.Any() ? phoneList : null;
                    resident.Email = emailList.Any() ? emailList : null;
                    resident.ResidentAddress = person.address.ToDomain();
                    resident.UPRN = person.address.UPRN;
                    resident.TenancyReference = person.tenancy?.TagRef;
                    return resident;
                }
                ).ToList();

            return listOfResident;
        }
    }
}
