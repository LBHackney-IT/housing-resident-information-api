using System;
using System.Collections.Generic;
using System.Globalization;
using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using UHResidentInformationAPI.Tests.V1.Helper;
using UHResidentInformationAPI.V1.Domain;
using UHResidentInformationAPI.V1.Enums;
using UHResidentInformationAPI.V1.Factories;
using UHResidentInformationAPI.V1.Gateways;
using UHResidentInformationAPI.V1.Infrastructure;
using DomainAddress = UHResidentInformationAPI.V1.Domain.Address;
using DatabaseAddress = UHResidentInformationAPI.V1.Infrastructure.Address;

namespace UHResidentInformationAPI.Tests.V1.Gateways
{
    [TestFixture]
    public class UHGatewayTests : DatabaseTests
    {
        private Fixture _fixture = new Fixture();
        private UHGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new UHGateway(UHContext);
            _fixture = new Fixture();
        }

        [Test]
        public void GatewayImplementsBoundaryInterface()
        {
            Assert.NotNull(_classUnderTest is IUHGateway);
        }

        [Test]
        public void GetAllResidentsReturnsEmptyArrayWhenNoRecordMatches()
        {
            var houseRef = _fixture.Create<string>();
            var response = _classUnderTest.GetAllResidents(cursor: 0, limit: 20, houseReference: houseRef);

            response.Should().BeEmpty();
        }

        // [Test]
        // public void GetAllResidentsReturnsAllDetailsWhenHouseRefMatchFound()
        // {
        //     var databasePersonMatch = AddPersonRecordToDatabase();
        //     var databasePersonNonMatch = AddPersonRecordToDatabase();

        //     var addressMatch = AddAddressRecordToDatabase(databasePersonMatch.HouseRef);
        //     AddAddressRecordToDatabase(databasePersonNonMatch.HouseRef);

        //     var contactMatch = AddContactLinkForPersonToDatabase(databasePersonMatch.HouseRef, databasePersonMatch.PersonNo);
        //     AddContactLinkForPersonToDatabase(databasePersonNonMatch.HouseRef, databasePersonNonMatch.PersonNo);

        //     var telephoneMatch = TestHelper.CreateDatabaseTelephoneNumberForPersonId(contactMatch.ContactID);
        //     UHContext.TelephoneNumbers.Add(telephoneMatch);
        //     UHContext.SaveChanges();

        //     Console.WriteLine("> telephoneMatch:");
        //     Console.WriteLine(JsonConvert.SerializeObject(telephoneMatch));

        //     var emailAddress = TestHelper.CreateDatabaseEmailForPerson(contactMatch.ContactID);
        //     UHContext.EmailAddresses.Add(emailAddress);
        //     UHContext.SaveChanges();

        //     var houseRefQuery = databasePersonMatch.HouseRef;
        //     var results = _classUnderTest
        //         .GetAllResidents(cursor: 0, limit: 20, houseReference: houseRefQuery);

        //     results.Count.Should().Be(1);
        //     results[0].HouseReference.Should().Be(databasePersonMatch.HouseRef);
        //     results[0].Address.PropertyRef.Should().Be(addressMatch.PropertyRef);
        //     results[0].PhoneNumberList[0].Should().NotBeNull();
        //     // results[0].PhoneNumberList[0].PhoneNumber.Should().Be(telephoneMatch.Number);

        //     //TODO
        //     // - can find multiple phone numbers?
        //     // - can fine multiple email addresses?
        //     // - what if multiple person matches?
        // }

        // [Test]
        // public void GetAllResidentsReturnsResultsWhenFirstNameMatchFound()
        // {
        //     var databasePersonMatch = AddPersonRecordToDatabase();
        //     var databasePersonNonMatch = AddPersonRecordToDatabase();
        //     var addressMatch = AddAddressRecordToDatabase(databasePersonMatch.HouseRef);
        //     AddAddressRecordToDatabase(databasePersonNonMatch.HouseRef);

        //     var firstName = databasePersonMatch.FirstName;
        //     var results = _classUnderTest
        //         .GetAllResidents(cursor: 0, limit: 20, firstName: firstName);

        //     results.Count.Should().Be(1);
        //     results[0].HouseReference.Should().Be(databasePersonMatch.HouseRef);
        //     results[0].Address.AddressLine1.Should().Be(addressMatch.AddressLine1);
        // }

        // [Test]
        // public void GetAllResidentsReturnsResultsWhenLastNameMatchFound()
        // {
        //     var databasePersonMatch = AddPersonRecordToDatabase();
        //     var databasePersonNonMatch = AddPersonRecordToDatabase();
        //     var addressMatch = AddAddressRecordToDatabase(databasePersonMatch.HouseRef);
        //     AddAddressRecordToDatabase(databasePersonNonMatch.HouseRef);

        //     var lastName = databasePersonMatch.LastName;
        //     var results = _classUnderTest
        //         .GetAllResidents(cursor: 0, limit: 20, lastName: lastName);

        //     results.Count.Should().Be(1);
        //     results[0].HouseReference.Should().Be(databasePersonMatch.HouseRef);
        //     results[0].Address.AddressLine1.Should().Be(addressMatch.AddressLine1);
        // }

        // [Test]
        // public void GetAllResidentsReturnsResultsWhenResidentAddressLine1MatchFound()
        // {
        //     var databasePersonMatch = AddPersonRecordToDatabase();
        //     var databasePersonNonMatch = AddPersonRecordToDatabase();
        //     var addressMatch = AddAddressRecordToDatabase(databasePersonMatch.HouseRef);
        //     AddAddressRecordToDatabase(databasePersonNonMatch.HouseRef);

        //     var addressLine1 = addressMatch.AddressLine1;
        //     var results = _classUnderTest
        //         .GetAllResidents(cursor: 0, limit: 20, addressLine1: addressLine1);

        //     results.Count.Should().Be(1);
        //     results[0].HouseReference.Should().Be(databasePersonMatch.HouseRef);
        //     results[0].Address.AddressLine1.Should().Be(addressMatch.AddressLine1);
        // }

        // [Test]
        // public void GetAllResidentsReturnsResultsWhenResidentPostcodeMatchFound()
        // {
        //     var databasePersonMatch = AddPersonRecordToDatabase();
        //     var databasePersonNonMatch = AddPersonRecordToDatabase();
        //     var addressMatch = AddAddressRecordToDatabase(databasePersonMatch.HouseRef);
        //     AddAddressRecordToDatabase(databasePersonNonMatch.HouseRef);

        //     var postCode = addressMatch.Postcode;
        //     var results = _classUnderTest
        //         .GetAllResidents(cursor: 0, limit: 20, postcode: postCode);

        //     results.Count.Should().Be(1);
        //     results[0].HouseReference.Should().Be(databasePersonMatch.HouseRef);
        //     results[0].Address.AddressLine1.Should().Be(addressMatch.AddressLine1);
        // }

        // [Test]
        // public void GetAllResidentsWontReturnMoreRecordsThanTheLimit()
        // {
        //     var persons = new List<Person>
        //     {
        //         AddPersonRecordToDatabase(),
        //         AddPersonRecordToDatabase(),
        //         AddPersonRecordToDatabase()
        //     };

        //     persons.ForEach(p => AddAddressRecordToDatabase(p.HouseRef));

        //     var results = _classUnderTest.GetAllResidents(0, 2);

        //     results.Count.Should().Be(2);
        // }

        // [Test]
        // public void GetAllResidentsWillOffsetRecordsByTheCursor()
        // {
        //     var persons = new List<Person>
        //     {
        //         AddPersonRecordToDatabase(houseRef: "1"),
        //         AddPersonRecordToDatabase(houseRef: "2"),
        //         AddPersonRecordToDatabase(houseRef: "3")
        //     };

        //     persons.ForEach(p => AddAddressRecordToDatabase(p.HouseRef));

        //     var results = _classUnderTest.GetAllResidents(1, 2);

        //     results.Count.Should().Be(2);
        //     results[0].HouseReference.Replace(" ", "").Should().Be(persons[1].HouseRef);
        //     results[1].HouseReference.Replace(" ", "").Should().Be(persons[2].HouseRef);
        // }

        [Test]
        public void GetResidentByIdReturnsEmptyArrayWhenNoRecordMatches()
        {
            var houseRef = _fixture.Create<string>();
            var personNo = _fixture.Create<int>();

            var response = _classUnderTest.GetResidentById(houseRef, personNo);

            response.Should().BeNull();
        }

        [Test]
        public void GetResidentByIdReturnsPersonalDetails()
        {
            var databasePersonEntity = AddPersonRecordToDatabase();
            var response = _classUnderTest.GetResidentById(databasePersonEntity.HouseRef, databasePersonEntity.PersonNo);

            response.HouseReference.Should().Be(databasePersonEntity.HouseRef);
            response.FirstName.Should().Be(databasePersonEntity.FirstName);
            response.LastName.Should().Be(databasePersonEntity.LastName);
            response.NINumber.Should().Be(databasePersonEntity.NINumber);
            response.DateOfBirth.Should().Be(databasePersonEntity.DateOfBirth.ToString("O", CultureInfo.InvariantCulture));
            response.Should().NotBe(null);
        }

        [Test]
        public void GetResidentByIdReturnsAddressDetails()
        {
            var databasePersonEntity = AddPersonRecordToDatabase();
            var databaseAddressEntity = TestHelper.CreateDatabaseAddressForPersonId(databasePersonEntity.HouseRef);

            UHContext.Addresses.Add(databaseAddressEntity);
            UHContext.SaveChanges();

            var expectedDomainAddress = new DomainAddress
            {
                PropertyRef = databaseAddressEntity.PropertyRef,
                AddressLine1 = databaseAddressEntity.AddressLine1,
                Postcode = databaseAddressEntity.Postcode
            };

            var response = _classUnderTest.GetResidentById(databasePersonEntity.HouseRef, databasePersonEntity.PersonNo);

            response.HouseReference.Should().Be(databasePersonEntity.HouseRef);
            response.Address.Should().BeEquivalentTo(expectedDomainAddress);
        }

        [Test]
        public void GetResidentByIdReturnsPhoneContactDetailsWithPhoneType()
        {
            var databasePersonEntity = AddPersonRecordToDatabase();
            var databaseContactLink = AddContactLinkForPersonToDatabase(databasePersonEntity.HouseRef, databasePersonEntity.PersonNo);

            var databasePhoneEntity = TestHelper.CreateDatabaseTelephoneNumberForPersonId(databaseContactLink.ContactID);

            var type = (int) PhoneType.Primary;
            databasePhoneEntity.Type = type.ToString();

            UHContext.TelephoneNumbers.Add(databasePhoneEntity);
            UHContext.SaveChanges();

            var expectedPhoneNumberList = new List<Phone>
            {
                new Phone
                {
                    PhoneNumber = databasePhoneEntity.Number,
                    Type = PhoneType.Primary,
                    LastModified = databasePhoneEntity.DateCreated
                }
            };

            var response = _classUnderTest.GetResidentById(databasePersonEntity.HouseRef, databasePersonEntity.PersonNo);
            response.PhoneNumberList.Should().BeEquivalentTo(expectedPhoneNumberList);
        }

        [Test]
        public void GetResidentByIdReturnsPhoneContactDetailsWithOutPhoneType()
        {
            var databasePersonEntity = AddPersonRecordToDatabase();
            var databaseContactLink = AddContactLinkForPersonToDatabase(databasePersonEntity.HouseRef, databasePersonEntity.PersonNo);

            var databasePhoneEntity = TestHelper.CreateDatabaseTelephoneNumberForPersonId(databaseContactLink.ContactID);

            databasePhoneEntity.Type = null;

            UHContext.TelephoneNumbers.Add(databasePhoneEntity);
            UHContext.SaveChanges();

            var expectedPhoneNumberList = new List<Phone>
            {
                new Phone
                {
                    PhoneNumber = databasePhoneEntity.Number,
                    Type = null,
                    LastModified = databasePhoneEntity.DateCreated
                }
            };

            var response = _classUnderTest.GetResidentById(databasePersonEntity.HouseRef, databasePersonEntity.PersonNo);
            response.PhoneNumberList.Should().BeEquivalentTo(expectedPhoneNumberList);
        }

        [Test]
        public void GetResidentByIdReturnsTheUPRNInTheResponse()
        {
            var databasePersonEntity = AddPersonRecordToDatabase();
            var databaseAddressEntity = TestHelper.CreateDatabaseAddressForPersonId(databasePersonEntity.HouseRef);

            UHContext.Addresses.Add(databaseAddressEntity);
            UHContext.SaveChanges();

            var response = _classUnderTest.GetResidentById(databasePersonEntity.HouseRef, databasePersonEntity.PersonNo);
            response.UPRN.Should().Be(databaseAddressEntity.UPRN);
        }

        [Test]
        public void GetResidentByIdReturnsTheEmailDetails()
        {
            var databasePersonEntity = AddPersonRecordToDatabase();
            var databaseContactLink = AddContactLinkForPersonToDatabase(databasePersonEntity.HouseRef, databasePersonEntity.PersonNo);

            var databaseEmailEntity = TestHelper.CreateDatabaseEmailForPerson(databaseContactLink.ContactID);

            UHContext.EmailAddresses.Add(databaseEmailEntity);
            UHContext.SaveChanges();

            var expectedEmailAddressList = new List<Email>
            {
                new Email
                {
                    EmailAddress = databaseEmailEntity.EmailAddress,
                    LastModified = databaseEmailEntity.DateModified
                }
            };

            var response = _classUnderTest.GetResidentById(databasePersonEntity.HouseRef, databasePersonEntity.PersonNo);
            response.EmailList.Should().BeEquivalentTo(expectedEmailAddressList);
        }


        private Person AddPersonRecordToDatabase(string firstname = null, string lastname = null, string houseRef = null)
        {
            var databaseEntity = TestHelper.CreateDatabasePersonEntity(firstname, lastname, houseRef);
            UHContext.Persons.Add(databaseEntity);
            UHContext.SaveChanges();
            return databaseEntity;
        }

        private DatabaseAddress AddAddressRecordToDatabase(string houseRef)
        {
            var databaseEntity = TestHelper.CreateDatabaseAddressForPersonId(houseRef);
            UHContext.Addresses.Add(databaseEntity);
            UHContext.SaveChanges();
            return databaseEntity;
        }

        private ContactLink AddContactLinkForPersonToDatabase(string houseReference, int personNumber)
        {

            var tenancyDatabaseEntity = TestHelper.CreateDatabaseTenancyAgreementForPerson(houseReference);
            var contactLinkDatabaseEntity = TestHelper.CreateDatabaseContactLinkForPerson(tenancyDatabaseEntity.TagRef, personNumber);

            UHContext.TenancyAgreements.Add(tenancyDatabaseEntity);
            UHContext.ContactLinks.Add(contactLinkDatabaseEntity);
            UHContext.SaveChanges();

            return contactLinkDatabaseEntity;
        }

        // public void GetAllResidentsIfThereAreNoResidentsReturnsAnEmptyList()
        // {
        //     _classUnderTest.GetAllResidents("00011", "bob", "brown", "1 Hillman Street").Should().BeEmpty();
        // }

        [Test]
        public void GetAllResidentsWithFirstNameQueryParameterReturnsMatchingResidents()
        {
            var databaseEntity = TestHelper.CreateDatabasePersonEntity(firstname: "ciasom");
            var databaseEntity1 = TestHelper.CreateDatabasePersonEntity(firstname: "shape");
            var databaseEntity2 = TestHelper.CreateDatabasePersonEntity(firstname: "Ciasom");

            var personslist = new List<Person>
            {
                databaseEntity,
                databaseEntity1,
                databaseEntity2
            };

            //Add person entities to test database
            UHContext.Persons.AddRange(personslist);
            UHContext.SaveChanges();

            var address = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity.HouseRef);
            var address1 = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity1.HouseRef);
            var address2 = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity2.HouseRef);

            var addresslist = new List<DatabaseAddress>
            {
                address,
                address1,
                address2
            };

            //Add address enitites to test database
            UHContext.Addresses.AddRange(addresslist);
            UHContext.SaveChanges();

            var tenancy1 = TestHelper.CreateDatabaseTenancyAgreementForPerson(address.HouseRef);
            var tenancy2 = TestHelper.CreateDatabaseTenancyAgreementForPerson(address1.HouseRef);
            var tenancy3 = TestHelper.CreateDatabaseTenancyAgreementForPerson(address2.HouseRef);

            var tenancyList = new List<TenancyAgreement>
            {
                tenancy1,
                tenancy2,
                tenancy3
            };

            UHContext.TenancyAgreements.AddRange(tenancyList);
            UHContext.SaveChanges();

            var link1 = TestHelper.CreateDatabaseContactLinkForPerson(tenancy1.TagRef, databaseEntity.PersonNo);
            var link2 = TestHelper.CreateDatabaseContactLinkForPerson(tenancy3.TagRef, databaseEntity2.PersonNo);
            var link3 = TestHelper.CreateDatabaseContactLinkForPerson(tenancy2.TagRef, databaseEntity1.PersonNo);

            var contactLinkList = new List<ContactLink>
            {
                link1,
                link2,
                link3
            };

            UHContext.ContactLinks.AddRange(contactLinkList);
            UHContext.SaveChanges();

            var telephone = TestHelper.CreateDatabaseTelephoneNumberForPersonId(link1.ContactID);
            UHContext.TelephoneNumbers.Add(telephone);
            UHContext.SaveChanges();

            var telephone1 = TestHelper.CreateDatabaseTelephoneNumberForPersonId(link1.ContactID);
            UHContext.TelephoneNumbers.Add(telephone1);
            UHContext.SaveChanges();

            var telephone2 = TestHelper.CreateDatabaseTelephoneNumberForPersonId(link2.ContactID);
            UHContext.TelephoneNumbers.Add(telephone2);
            UHContext.SaveChanges();

            var emailAddress = TestHelper.CreateDatabaseEmailForPerson(link1.ContactID);
            UHContext.EmailAddresses.Add(emailAddress);
            UHContext.SaveChanges();

            var emailAddress1 = TestHelper.CreateDatabaseEmailForPerson(link1.ContactID);
            UHContext.EmailAddresses.Add(emailAddress1);
            UHContext.SaveChanges();

            var emailAddress2 = TestHelper.CreateDatabaseEmailForPerson(link2.ContactID);
            UHContext.EmailAddresses.Add(emailAddress2);
            UHContext.SaveChanges();

            var domainEntity = databaseEntity.ToDomain();
            domainEntity.Address = address.ToDomain();
            domainEntity.UPRN = address.UPRN;
            domainEntity.PhoneNumberList = new List<Phone> { telephone.ToDomain(), telephone1.ToDomain() };
            domainEntity.EmailList = new List<Email> { emailAddress.ToDomain(), emailAddress1.ToDomain() };

            var domainEntity2 = databaseEntity2.ToDomain();
            domainEntity2.Address = address2.ToDomain();
            domainEntity2.UPRN = address2.UPRN;
            domainEntity2.PhoneNumberList = new List<Phone> { telephone2.ToDomain() };
            domainEntity2.EmailList = new List<Email> { emailAddress2.ToDomain() };

            var listOfPersons = _classUnderTest.GetAllResidents(0, 20, firstName: "ciasom");
            listOfPersons.Count.Should().Be(2);
            listOfPersons.Should().ContainEquivalentOf(domainEntity);
            listOfPersons.Should().ContainEquivalentOf(domainEntity2);
        }


        // [Test]
        // public void GetAllResidentsWithNoEmailWithLastNameQueryParameterReturnsMatchingResidents()
        // {
        //     var databaseEntity = TestHelper.CreateDatabasePersonEntity(lastname: "brown");
        //     var databaseEntity1 = TestHelper.CreateDatabasePersonEntity(lastname: "tessellate");
        //     var databaseEntity2 = TestHelper.CreateDatabasePersonEntity(lastname: "brown");

        //     var personslist = new List<Person>
        //     {
        //         databaseEntity,
        //         databaseEntity1,
        //         databaseEntity2
        //     };
        //     //Add person entities to test database
        //     UHContext.Persons.AddRange(personslist);
        //     UHContext.SaveChanges();

        //     var address = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity.HouseRef);
        //     var address1 = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity1.HouseRef);
        //     var address2 = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity2.HouseRef);

        //     var addresslist = new List<DatabaseAddress>
        //     {
        //         address,
        //         address1,
        //         address2
        //     };
        //     //Add address enitites to test database
        //     UHContext.Addresses.AddRange(addresslist);
        //     UHContext.SaveChanges();

        //     var tenancy1 = TestHelper.CreateDatabaseTenancyAgreementForPerson(address.HouseRef);
        //     var tenancy2 = TestHelper.CreateDatabaseTenancyAgreementForPerson(address1.HouseRef);
        //     var tenancy3 = TestHelper.CreateDatabaseTenancyAgreementForPerson(address2.HouseRef);

        //     var tenancyList = new List<TenancyAgreement>
        //     {
        //         tenancy1,
        //         tenancy2,
        //         tenancy3
        //     };

        //     UHContext.TenancyAgreements.AddRange(tenancyList);
        //     UHContext.SaveChanges();

        //     var link1 = TestHelper.CreateDatabaseContactLinkForPerson(tenancy1.TagRef, databaseEntity.PersonNo);
        //     var link2 = TestHelper.CreateDatabaseContactLinkForPerson(tenancy3.TagRef, databaseEntity2.PersonNo);
        //     var link3 = TestHelper.CreateDatabaseContactLinkForPerson(tenancy2.TagRef, databaseEntity1.PersonNo);

        //     var contactLinkList = new List<ContactLink>
        //     {
        //         link1,
        //         link2,
        //         link3
        //     };

        //     UHContext.ContactLinks.AddRange(contactLinkList);
        //     UHContext.SaveChanges();

        //     var telephone = TestHelper.CreateDatabaseTelephoneNumberForPersonId(link1.ContactID);
        //     UHContext.TelephoneNumbers.Add(telephone);
        //     UHContext.SaveChanges();

        //     var telephone1 = TestHelper.CreateDatabaseTelephoneNumberForPersonId(link1.ContactID);
        //     UHContext.TelephoneNumbers.Add(telephone1);
        //     UHContext.SaveChanges();

        //     var telephone2 = TestHelper.CreateDatabaseTelephoneNumberForPersonId(link2.ContactID);
        //     UHContext.TelephoneNumbers.Add(telephone2);
        //     UHContext.SaveChanges();


        //     var domainEntity = databaseEntity.ToDomain();
        //     domainEntity.Address = address.ToDomain();
        //     domainEntity.UPRN = address.UPRN;
        //     domainEntity.PhoneNumberList = new List<Phone> { telephone.ToDomain(), telephone1.ToDomain() };

        //     var domainEntity2 = databaseEntity2.ToDomain();
        //     domainEntity2.Address = address2.ToDomain();
        //     domainEntity2.UPRN = address2.UPRN;
        //     domainEntity2.PhoneNumberList = new List<Phone> { telephone2.ToDomain() };

        //     var listOfPersons = _classUnderTest.GetAllResidents(lastName: "brown");
        //     listOfPersons.Count.Should().Be(2);
        //     listOfPersons.Should().ContainEquivalentOf(domainEntity);
        //     listOfPersons.Should().ContainEquivalentOf(domainEntity2);

        // }

        // [Test]
        // public void GetAllResidentsWithNoPhoneWithFirstAndLastNameQueryParameterReturnsMatchingResidents()
        // {
        //     var databaseEntity = TestHelper.CreateDatabasePersonEntity(firstname: "ciasom", lastname: "Brown");
        //     var databaseEntity1 = TestHelper.CreateDatabasePersonEntity(firstname: "ciasom", lastname: "tessellate");
        //     var databaseEntity2 = TestHelper.CreateDatabasePersonEntity(firstname: "Ciasom", lastname: "brown");

        //     var personslist = new List<Person>
        //     {
        //         databaseEntity,
        //         databaseEntity1,
        //         databaseEntity2
        //     };
        //     //Add person entities to test database
        //     UHContext.Persons.AddRange(personslist);
        //     UHContext.SaveChanges();

        //     var address = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity.HouseRef);
        //     var address1 = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity1.HouseRef);
        //     var address2 = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity2.HouseRef);

        //     var addresslist = new List<DatabaseAddress>
        //     {
        //         address,
        //         address1,
        //         address2
        //     };
        //     //Add address enitites to test database
        //     UHContext.Addresses.AddRange(addresslist);
        //     UHContext.SaveChanges();

        //     var tenancy1 = TestHelper.CreateDatabaseTenancyAgreementForPerson(address.HouseRef);
        //     var tenancy2 = TestHelper.CreateDatabaseTenancyAgreementForPerson(address1.HouseRef);
        //     var tenancy3 = TestHelper.CreateDatabaseTenancyAgreementForPerson(address2.HouseRef);

        //     var tenancyList = new List<TenancyAgreement>
        //     {
        //         tenancy1,
        //         tenancy2,
        //         tenancy3
        //     };

        //     UHContext.TenancyAgreements.AddRange(tenancyList);
        //     UHContext.SaveChanges();

        //     var link1 = TestHelper.CreateDatabaseContactLinkForPerson(tenancy1.TagRef, databaseEntity.PersonNo);
        //     var link2 = TestHelper.CreateDatabaseContactLinkForPerson(tenancy3.TagRef, databaseEntity2.PersonNo);
        //     var link3 = TestHelper.CreateDatabaseContactLinkForPerson(tenancy2.TagRef, databaseEntity1.PersonNo);

        //     var contactLinkList = new List<ContactLink>
        //     {
        //         link1,
        //         link2,
        //         link3
        //     };

        //     UHContext.ContactLinks.AddRange(contactLinkList);
        //     UHContext.SaveChanges();

        //     var emailAddress = TestHelper.CreateDatabaseEmailForPerson(link1.ContactID);
        //     UHContext.EmailAddresses.Add(emailAddress);
        //     UHContext.SaveChanges();

        //     var emailAddress1 = TestHelper.CreateDatabaseEmailForPerson(link1.ContactID);
        //     UHContext.EmailAddresses.Add(emailAddress1);
        //     UHContext.SaveChanges();

        //     var emailAddress2 = TestHelper.CreateDatabaseEmailForPerson(link2.ContactID);
        //     UHContext.EmailAddresses.Add(emailAddress2);
        //     UHContext.SaveChanges();

        //     var domainEntity = databaseEntity.ToDomain();
        //     domainEntity.Address = address.ToDomain();
        //     domainEntity.UPRN = address.UPRN;
        //     domainEntity.EmailList = new List<Email> { emailAddress.ToDomain(), emailAddress1.ToDomain() };

        //     var domainEntity2 = databaseEntity2.ToDomain();
        //     domainEntity2.Address = address2.ToDomain();
        //     domainEntity2.UPRN = address2.UPRN;
        //     domainEntity2.EmailList = new List<Email> { emailAddress2.ToDomain() };

        //     var listOfPersons = _classUnderTest.GetAllResidents(firstName: "ciasom", lastName: "brown");
        //     listOfPersons.Count.Should().Be(2);
        //     listOfPersons.Should().ContainEquivalentOf(domainEntity);
        //     listOfPersons.Should().ContainEquivalentOf(domainEntity2);

        // }

        // [Test]
        // public void GetAllResidentsWithNoContactlinkWithAddressQueryParameterReturnsMatchingResidents()
        // {
        //     var databaseEntity = TestHelper.CreateDatabasePersonEntity();
        //     var databaseEntity1 = TestHelper.CreateDatabasePersonEntity();
        //     var databaseEntity2 = TestHelper.CreateDatabasePersonEntity();

        //     var personslist = new List<Person>
        //     {
        //         databaseEntity,
        //         databaseEntity1,
        //         databaseEntity2
        //     };
        //     //Add person entities to test database
        //     UHContext.Persons.AddRange(personslist);
        //     UHContext.SaveChanges();

        //     var address = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity.HouseRef, address1: "1 Hillman st");
        //     var address1 = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity1.HouseRef);
        //     var address2 = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity2.HouseRef, address1: "2 Hillman st");

        //     var addresslist = new List<DatabaseAddress>
        //     {
        //         address,
        //         address1,
        //         address2
        //     };
        //     //Add address enitites to test database
        //     UHContext.Addresses.AddRange(addresslist);
        //     UHContext.SaveChanges();

        //     var tenancy1 = TestHelper.CreateDatabaseTenancyAgreementForPerson(address.HouseRef);
        //     var tenancy2 = TestHelper.CreateDatabaseTenancyAgreementForPerson(address1.HouseRef);
        //     var tenancy3 = TestHelper.CreateDatabaseTenancyAgreementForPerson(address2.HouseRef);

        //     var tenancyList = new List<TenancyAgreement>
        //     {
        //         tenancy1,
        //         tenancy2,
        //         tenancy3
        //     };

        //     UHContext.TenancyAgreements.AddRange(tenancyList);
        //     UHContext.SaveChanges();

        //     var domainEntity = databaseEntity.ToDomain();
        //     domainEntity.Address = address.ToDomain();
        //     domainEntity.UPRN = address.UPRN;

        //     var listOfPersons = _classUnderTest.GetAllResidents(address: "1 Hillman st");
        //     listOfPersons.Count.Should().Be(1);
        //     listOfPersons.Should().ContainEquivalentOf(domainEntity);
        // }


    }
}
