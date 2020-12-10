using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using HousingResidentInformationAPI.Tests.V1.Helper;
using HousingResidentInformationAPI.V1.Domain;
using HousingResidentInformationAPI.V1.Enums;
using HousingResidentInformationAPI.V1.Factories;
using HousingResidentInformationAPI.V1.Gateways;
using HousingResidentInformationAPI.V1.Infrastructure;
using Address = HousingResidentInformationAPI.V1.Infrastructure.Address;
using DomainAddress = HousingResidentInformationAPI.V1.Domain.Address;

namespace HousingResidentInformationAPI.Tests.V1.Gateways
{
    [TestFixture]
    public class HousingGatewayTests : DatabaseTests
    {
        private Fixture _fixture = new Fixture();
        private HousingGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new HousingGateway(UHContext);
            _fixture = new Fixture();
        }

        [Test]
        public void GatewayImplementsBoundaryInterface()
        {
            Assert.NotNull(_classUnderTest is IHousingGateway);
        }

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
            var person = AddPersonRecordToDatabase();
            AddTenancyAgreementToDatabase(person.HouseRef);
            var response = _classUnderTest.GetResidentById(person.HouseRef, person.PersonNo);

            response.HouseReference.Should().Be(person.HouseRef);
            response.FirstName.Should().Be(person.FirstName);
            response.LastName.Should().Be(person.LastName);
            response.NINumber.Should().Be(person.NINumber);
            response.DateOfBirth.Should().Be(person.DateOfBirth.ToString("O", CultureInfo.InvariantCulture));
            response.Should().NotBe(null);
        }

        [Test]
        public void GetResidentByIdReturnsAddressDetails()
        {
            var person = AddPersonRecordToDatabase();
            var address = AddAddressRecordToDatabase(person.HouseRef);
            AddTenancyAgreementToDatabase(person.HouseRef);

            var expectedDomainAddress = new DomainAddress
            {
                PropertyRef = address.PropertyRef,
                AddressLine1 = address.AddressLine1,
                PostCode = address.PostCode
            };

            var response = _classUnderTest.GetResidentById(person.HouseRef, person.PersonNo);

            response.HouseReference.Should().Be(person.HouseRef);
            response.ResidentAddress.Should().BeEquivalentTo(expectedDomainAddress);
        }

        [Test]
        public void GetResidentByIdReturnsPhoneContactDetailsWithPhoneType()
        {
            var person = AddPersonRecordToDatabase();
            var tenancy = AddTenancyAgreementToDatabase(person.HouseRef);
            var contactLink = AddContactLinkForPersonToDatabase(tenancy.TagRef, person.PersonNo);

            var phone = TestHelper.CreateDatabaseTelephoneNumberForPersonId(contactLink.ContactID);

            var type = PhoneType.X;
            phone.Type = type.ToString();

            UHContext.TelephoneNumbers.Add(phone);
            UHContext.SaveChanges();

            var expectedPhoneNumberList = new List<Phone>
            {
                new Phone
                {
                    PhoneNumber = phone.Number,
                    Type = PhoneType.X,
                    LastModified = phone.DateCreated
                }
            };

            var response = _classUnderTest.GetResidentById(person.HouseRef, person.PersonNo);
            response.PhoneNumber.Should().BeEquivalentTo(expectedPhoneNumberList);
        }

        [Test]
        public void GetResidentByIdReturnsPhoneContactDetailsWithOutPhoneType()
        {
            var person = AddPersonRecordToDatabase();
            var tenancy = AddTenancyAgreementToDatabase(person.HouseRef);
            var contactLink = AddContactLinkForPersonToDatabase(tenancy.TagRef, person.PersonNo);

            var databasePhoneEntity = TestHelper.CreateDatabaseTelephoneNumberForPersonId(contactLink.ContactID);

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

            var response = _classUnderTest.GetResidentById(person.HouseRef, person.PersonNo);
            response.PhoneNumber.Should().BeEquivalentTo(expectedPhoneNumberList);
        }

        [Test]
        public void GetResidentByIdReturnsTheUPRNInTheResponse()
        {
            var person = AddPersonRecordToDatabase();
            var address = AddAddressRecordToDatabase(person.HouseRef);
            var tenancy = AddTenancyAgreementToDatabase(person.HouseRef);

            UHContext.Contacts.Add(TestHelper.CreateContactRecordFromTagRef(tenancy.TagRef));
            UHContext.SaveChanges();

            var response = _classUnderTest.GetResidentById(person.HouseRef, person.PersonNo);
            response.UPRN.Should().Be(address.UPRN);
        }

        [Test]
        public void GetResidentByIdReturnsTheTenancyReferenceInTheResponse()
        {
            var databasePersonEntity = AddPersonRecordToDatabase();
            var tenancyDatabaseEntity = AddTenancyAgreementToDatabase(databasePersonEntity.HouseRef);

            var response = _classUnderTest.GetResidentById(databasePersonEntity.HouseRef, databasePersonEntity.PersonNo);
            response.TenancyReference.Should().Be(tenancyDatabaseEntity.TagRef);
        }

        [Test]
        public void GetResidentByIdReturnsTheContactKey()
        {
            var person = AddPersonRecordToDatabase();
            var tenancy = AddTenancyAgreementToDatabase(person.HouseRef);
            var contact = TestHelper.CreateContactRecordFromTagRef(tenancy.TagRef);

            var addedEntity = UHContext.Contacts.Add(contact);
            UHContext.SaveChanges();

            var response = _classUnderTest.GetResidentById(person.HouseRef, person.PersonNo);
            response.ContactKey.Should().Be(addedEntity.Entity.ContactKey.ToString());
        }

        [Test]
        public void GetResidentByIdReturnsTheEmailDetails()
        {
            var person = AddPersonRecordToDatabase();
            var tenancy = AddTenancyAgreementToDatabase(person.HouseRef);
            var contactLink = AddContactLinkForPersonToDatabase(tenancy.TagRef, person.PersonNo);

            var email = AddEmailAddressToDatabase(contactLink.ContactID);

            var expectedEmailAddressList = new List<Email>
            {
                new Email
                {
                    EmailAddress = email.EmailAddress,
                    LastModified = email.DateModified
                }
            };

            var response = _classUnderTest.GetResidentById(person.HouseRef, person.PersonNo);
            response.Email.Should().BeEquivalentTo(expectedEmailAddressList);
        }

        [Test]
        public void GetAllResidentsIfThereAreNoResidentsReturnsAnEmptyList()
        {
            _classUnderTest.GetAllResidents(null, 10, "00011", "bob", "brown", "1 Hillman Street").Should().BeEmpty();
        }

        [Test]
        public void GetAllResidentsWithActiveTenancySetToTrueReturnsOnlyMatchingResidents()
        {
            //Test records 1
            var person1 = AddPersonRecordToDatabase(firstname: "ciasom"); ;
            var address1 = AddAddressRecordToDatabase(person1.HouseRef, address1: "1 Hillman st");
            var tenancy1 = AddTenancyAgreementToDatabase(address1.HouseRef, true);

            //Test records 2
            var person2 = AddPersonRecordToDatabase();
            var address2 = AddAddressRecordToDatabase(person2.HouseRef);
            var tenancy2 = AddTenancyAgreementToDatabase(address2.HouseRef);

            //Test records 3
            var person3 = AddPersonRecordToDatabase(firstname: "ciasom"); ;
            var address3 = AddAddressRecordToDatabase(person3.HouseRef, address1: "2 Hillman st");
            var tenancy3 = AddTenancyAgreementToDatabase(address3.HouseRef);


            var listOfPersons = _classUnderTest.GetAllResidents(null, 10, firstName: "ciasom", activeTenancyOnly: true);
            listOfPersons.Count.Should().Be(1);

            var expectedResponse = MapToExpectedDomain(person3, address3, null, null, tenancy3);
            listOfPersons.Should().ContainEquivalentOf(expectedResponse);
        }

        [Test]
        public void GetAllResidentsActiveTenancyOnlyParameterSetToFalseReturnsAllMatchingResidents()
        {
            //Test records 1
            var person1 = AddPersonRecordToDatabase(firstname: "ciasom"); ;
            var address1 = AddAddressRecordToDatabase(person1.HouseRef, address1: "1 Hillman st");
            var tenancy1 = AddTenancyAgreementToDatabase(address1.HouseRef, true);

            //Test records 2
            var person2 = AddPersonRecordToDatabase();
            var address2 = AddAddressRecordToDatabase(person2.HouseRef);
            var tenancy2 = AddTenancyAgreementToDatabase(address2.HouseRef);

            //Test records 3
            var person3 = AddPersonRecordToDatabase(firstname: "ciasom"); ;
            var address3 = AddAddressRecordToDatabase(person3.HouseRef, address1: "2 Hillman st");
            var tenancy3 = AddTenancyAgreementToDatabase(address3.HouseRef);


            var listOfPersons = _classUnderTest.GetAllResidents(null, 10, firstName: "ciasom", activeTenancyOnly: false);
            listOfPersons.Count.Should().Be(2);

            var expectedResponse1 = MapToExpectedDomain(person1, address1, null, null, tenancy1);
            var expectedResponse3 = MapToExpectedDomain(person3, address3, null, null, tenancy3);
            listOfPersons.Should().ContainEquivalentOf(expectedResponse1);
            listOfPersons.Should().ContainEquivalentOf(expectedResponse3);
        }

        [Test]
        public void GetAllResidentsWithFirstNameQueryParameterReturnsMatchingResident()
        {
            var person1 = AddPersonRecordToDatabase(firstname: "ciasom");
            var address1 = AddAddressRecordToDatabase(person1.HouseRef);
            var tenancy1 = AddTenancyAgreementToDatabase(address1.HouseRef);
            var link1 = AddContactLinkForPersonToDatabase(tenancy1.TagRef, person1.PersonNo);
            var telephone1 = AddTelephoneNumberToDatabase(link1.ContactID);
            var emailAddressList1 = new List<EmailAddresses> { AddEmailAddressToDatabase(link1.ContactID), AddEmailAddressToDatabase(link1.ContactID) };

            var person2 = AddPersonRecordToDatabase(firstname: "shape");
            var address2 = AddAddressRecordToDatabase(person2.HouseRef);
            var tenancy2 = AddTenancyAgreementToDatabase(address2.HouseRef);
            var link2 = AddContactLinkForPersonToDatabase(tenancy2.TagRef, person2.PersonNo);
            var telephone2 = AddTelephoneNumberToDatabase(link1.ContactID);

            var person3 = AddPersonRecordToDatabase(firstname: "Ciasom");
            var address3 = AddAddressRecordToDatabase(person3.HouseRef);
            var tenancy3 = AddTenancyAgreementToDatabase(address3.HouseRef);
            var link3 = AddContactLinkForPersonToDatabase(tenancy3.TagRef, person3.PersonNo);
            var telephone3 = AddTelephoneNumberToDatabase(link3.ContactID);
            var emailAddressList3 = new List<EmailAddresses> { AddEmailAddressToDatabase(link3.ContactID) };

            var domainEntity = MapToExpectedDomain(person1, address1,
                new List<Phone> { telephone1.ToDomain(), telephone2.ToDomain() }, emailAddressList1.ToDomain(), tenancy1);
            var domainEntity2 = MapToExpectedDomain(person3, address3, new List<Phone> { telephone3.ToDomain() }, emailAddressList3.ToDomain(), tenancy3);


            var listOfPersons = _classUnderTest.GetAllResidents(null, 10, firstName: "ciasom");
            listOfPersons.Count.Should().Be(2);
            listOfPersons.Should().ContainEquivalentOf(domainEntity);
            listOfPersons.Should().ContainEquivalentOf(domainEntity2);
        }

        [Test]
        public void GetAllResidentsWithNoEmailWithLastNameQueryParameterReturnsMatchingResidents()
        {
            var person1 = AddPersonRecordToDatabase(lastname: "brown");
            var address1 = AddAddressRecordToDatabase(person1.HouseRef);
            var tenancy1 = AddTenancyAgreementToDatabase(address1.HouseRef);
            var link1 = AddContactLinkForPersonToDatabase(tenancy1.TagRef, person1.PersonNo);
            var telephoneList1 = new List<TelephoneNumber> { AddTelephoneNumberToDatabase(link1.ContactID), AddTelephoneNumberToDatabase(link1.ContactID) };

            var person2 = AddPersonRecordToDatabase(lastname: "tessellate");
            var address2 = AddAddressRecordToDatabase(person2.HouseRef);
            var tenancy2 = AddTenancyAgreementToDatabase(address2.HouseRef);
            var link2 = AddContactLinkForPersonToDatabase(tenancy2.TagRef, person2.PersonNo);

            var person3 = AddPersonRecordToDatabase(lastname: "brown");
            var address3 = AddAddressRecordToDatabase(person3.HouseRef);
            var tenancy3 = AddTenancyAgreementToDatabase(address3.HouseRef);
            var link3 = AddContactLinkForPersonToDatabase(tenancy3.TagRef, person3.PersonNo);
            var telephoneList3 = new List<TelephoneNumber> { AddTelephoneNumberToDatabase(link3.ContactID) };

            var domainEntity = MapToExpectedDomain(person1, address1, telephoneList1.ToDomain(), null, tenancy1);
            var domainEntity2 = MapToExpectedDomain(person3, address3, telephoneList3.ToDomain(), null, tenancy3);

            var listOfPersons = _classUnderTest.GetAllResidents(null, 10, lastName: "brown");
            listOfPersons.Count.Should().Be(2);
            listOfPersons.Should().ContainEquivalentOf(domainEntity);
            listOfPersons.Should().ContainEquivalentOf(domainEntity2);
        }

        [Test]
        public void GetAllResidentsWithNoPhoneWithFirstAndLastNameQueryParameterReturnsMatchingResidents()
        {
            //Test records 1
            var person1 = AddPersonRecordToDatabase(firstname: "ciasom", lastname: "Brown");
            var address1 = AddAddressRecordToDatabase(person1.HouseRef);
            var tenancy1 = AddTenancyAgreementToDatabase(address1.HouseRef);
            var link1 = AddContactLinkForPersonToDatabase(tenancy1.TagRef, person1.PersonNo);
            var emailAddresses1 = new List<EmailAddresses>
            {
                AddEmailAddressToDatabase(link1.ContactID),
                AddEmailAddressToDatabase(link1.ContactID)
            };

            //Test records 2
            var person2 = AddPersonRecordToDatabase(firstname: "ciasom", lastname: "tessellate");
            var address2 = AddAddressRecordToDatabase(person2.HouseRef);
            var tenancy2 = AddTenancyAgreementToDatabase(address2.HouseRef);
            AddContactLinkForPersonToDatabase(tenancy2.TagRef, person2.PersonNo);

            //Test records 3
            var person3 = AddPersonRecordToDatabase(firstname: "Ciasom", lastname: "brown");
            var address3 = AddAddressRecordToDatabase(person3.HouseRef);
            var tenancy3 = AddTenancyAgreementToDatabase(address3.HouseRef);
            var link3 = AddContactLinkForPersonToDatabase(tenancy3.TagRef, person3.PersonNo);
            var emailAddresses2 = new List<EmailAddresses> { AddEmailAddressToDatabase(link3.ContactID) };

            var domainEntity1 = MapToExpectedDomain(person1, address1, null, emailAddresses1.ToDomain(), tenancy1);
            var domainEntity2 = MapToExpectedDomain(person3, address3, null, emailAddresses2.ToDomain(), tenancy3);

            var listOfPersons = _classUnderTest.GetAllResidents(null, 10, firstName: "ciasom", lastName: "brown");
            listOfPersons.Count.Should().Be(2);
            listOfPersons.Should().ContainEquivalentOf(domainEntity1);
            listOfPersons.Should().ContainEquivalentOf(domainEntity2);
        }

        [Test]
        public void GetAllResidentsWithNoContactLinkWithPostcodeQueryParameterReturnsMatchingResidents()
        {
            var person1 = AddPersonRecordToDatabase();
            var address1 = AddAddressRecordToDatabase(person1.HouseRef, postcode: "E8 1DY");
            var tenancy1 = AddTenancyAgreementToDatabase(address1.HouseRef);

            var person2 = AddPersonRecordToDatabase();
            var address2 = AddAddressRecordToDatabase(person2.HouseRef);
            var tenancy2 = AddTenancyAgreementToDatabase(address2.HouseRef);

            var person3 = AddPersonRecordToDatabase();
            var address3 = AddAddressRecordToDatabase(person3.HouseRef, postcode: "E1 8DY");
            var tenancy3 = AddTenancyAgreementToDatabase(address3.HouseRef);

            var expectedResponse = MapToExpectedDomain(person1, address1, null, null, tenancy1);

            var listOfPersons = _classUnderTest.GetAllResidents(null, 10, postcode: "e8 1dy");
            listOfPersons.Count.Should().Be(1);
            listOfPersons.Should().ContainEquivalentOf(expectedResponse);
        }

        [Test]
        public void GetAllResidentsWithNoContactLinkWithAddressQueryParameterReturnsMatchingResidents()
        {
            //Test records 1
            var person1 = AddPersonRecordToDatabase();
            var address1 = AddAddressRecordToDatabase(person1.HouseRef, address1: "1 Hillman st");
            var tenancy1 = AddTenancyAgreementToDatabase(address1.HouseRef);

            //Test records 2
            var person2 = AddPersonRecordToDatabase();
            var address2 = AddAddressRecordToDatabase(person2.HouseRef);
            var tenancy2 = AddTenancyAgreementToDatabase(address2.HouseRef);

            //Test records 3
            var person3 = AddPersonRecordToDatabase();
            var address3 = AddAddressRecordToDatabase(person3.HouseRef, address1: "2 Hillman st");
            var tenancy3 = AddTenancyAgreementToDatabase(address3.HouseRef);

            var expectedResponse = MapToExpectedDomain(person1, address1, null, null, tenancy1);

            var listOfPersons = _classUnderTest.GetAllResidents(null, 10, address: "1 Hillman st");
            listOfPersons.Count.Should().Be(1);
            listOfPersons.Should().ContainEquivalentOf(expectedResponse);
        }

        [Test]
        public void GetAllResidentsWhenAContactLinkRowsHasNoKey1AndKey2DoesNotBreakTheQuery()
        {
            var databaseEntity = AddPersonRecordToDatabase();
            var address = AddAddressRecordToDatabase(databaseEntity.HouseRef, address1: "1 Hillman st");
            var tenancy1 = AddTenancyAgreementToDatabase(address.HouseRef);
            //Create a row in the CCContactLink that has no values in Key1 and Key2. This was found in production.
            var link1 = AddContactLinkForPersonToDatabase(null, null);

            var domainEntity = MapToExpectedDomain(databaseEntity, address, null, null, tenancy1);

            var listOfPersons = _classUnderTest.GetAllResidents(null, 10, address: "1 Hillman st");
            listOfPersons.Count.Should().Be(1);
            listOfPersons.Should().ContainEquivalentOf(domainEntity);
        }

        [Test]
        public void GetAllResidentsWontReturnMoreRecordsThanTheLimit()
        {
            var person1 = AddPersonRecordToDatabase(houseRef: "123", personNo: 1);
            var person2 = AddPersonRecordToDatabase(houseRef: "123", personNo: 2);

            AddAddressRecordToDatabase(person1.HouseRef, address1: "1 Hillman st");
            var tenancy = AddTenancyAgreementToDatabase(person1.HouseRef);
            AddContactLinkForPersonToDatabase(tenancy.TagRef, person1.PersonNo);

            var peopleReturned = _classUnderTest.GetAllResidents(null, 1);

            peopleReturned.Count.Should().Be(1);
            peopleReturned[0].FirstName.Should().Be(person1.FirstName);
            peopleReturned[0].LastName.Should().Be(person1.LastName);
        }

        [Test]
        public void GetAllResidentsWillOffsetRecordsByTheCursor()
        {
            var persons = new List<Person>
            {
                AddPersonRecordToDatabase(houseRef: "123", personNo: 1),
                AddPersonRecordToDatabase(houseRef: "234", personNo: 1),
                AddPersonRecordToDatabase(houseRef: "123", personNo: 2),
                AddPersonRecordToDatabase(houseRef: "123", personNo: 3)
            };

            persons.Take(2).ToList().ForEach(person =>
            {
                AddAddressRecordToDatabase(person.HouseRef);
                var tenancy = AddTenancyAgreementToDatabase(person.HouseRef);
                AddContactLinkForPersonToDatabase(tenancy.TagRef, person.PersonNo);
            });

            var cursor = $"{persons[0].HouseRef}{persons[0].PersonNo}";

            var receivedPersons = _classUnderTest.GetAllResidents(cursor, 3);

            receivedPersons.Count.Should().Be(3);
            receivedPersons[0].FirstName.Should().Be(persons[2].FirstName);
            receivedPersons[1].FirstName.Should().Be(persons[3].FirstName);
            receivedPersons[2].FirstName.Should().Be(persons[1].FirstName);
        }

        [Test]
        public void GetAllResidentsReturnsTheContactKeyForEachResult()
        {
            var person = AddPersonRecordToDatabase();
            AddAddressRecordToDatabase(person.HouseRef);
            var tenancy = AddTenancyAgreementToDatabase(person.HouseRef);
            AddContactLinkForPersonToDatabase(tenancy.TagRef, person.PersonNo);

            var contact = UHContext.Contacts.Add(TestHelper.CreateContactRecordFromTagRef(tenancy.TagRef));
            UHContext.SaveChanges();

            var response = _classUnderTest.GetAllResidents(null, 10);
            response.First().ContactKey.Should().BeEquivalentTo(contact.Entity.ContactKey.ToString());
        }
        private Person AddPersonRecordToDatabase(string firstname = null, string lastname = null, string houseRef = null, int? personNo = null)
        {
            var databaseEntity = TestHelper.CreateDatabasePersonEntity(firstname, lastname, houseRef, personNo);
            UHContext.Persons.Add(databaseEntity);
            UHContext.SaveChanges();
            return databaseEntity;
        }

        private TelephoneNumber AddTelephoneNumberToDatabase(int contactLinkId)
        {
            var telephone = TestHelper.CreateDatabaseTelephoneNumberForPersonId(contactLinkId);
            UHContext.TelephoneNumbers.Add(telephone);
            UHContext.SaveChanges();
            return telephone;
        }

        private Address AddAddressRecordToDatabase(string houseRef, string postcode = null, string address1 = null)
        {
            var address = TestHelper.CreateDatabaseAddressForPersonId(houseRef, postcode, address1);
            UHContext.Addresses.Add(address);
            UHContext.SaveChanges();
            return address;
        }

        private TenancyAgreement AddTenancyAgreementToDatabase(string houseReference, bool isTerminated = false)
        {
            var tenancyDatabaseEntity = TestHelper.CreateDatabaseTenancyAgreementForPerson(houseReference);
            tenancyDatabaseEntity.IsTerminated = isTerminated;

            UHContext.TenancyAgreements.Add(tenancyDatabaseEntity);
            UHContext.SaveChanges();
            return tenancyDatabaseEntity;
        }
        private ContactLink AddContactLinkForPersonToDatabase(string tagRef, int? personNumber)
        {
            var contactLink = TestHelper.CreateDatabaseContactLinkForPerson(tagRef, personNumber);

            UHContext.ContactLinks.Add(contactLink);
            UHContext.SaveChanges();

            return contactLink;
        }

        private EmailAddresses AddEmailAddressToDatabase(int contactLinkId)
        {
            var databaseEmailEntity = TestHelper.CreateDatabaseEmailForPerson(contactLinkId);
            UHContext.EmailAddresses.Add(databaseEmailEntity);
            UHContext.SaveChanges();
            return databaseEmailEntity;
        }

        private static ResidentInformation MapToExpectedDomain(Person databaseEntity, Address address,
            List<Phone> phoneNumber, List<Email> emailAddresses,
            TenancyAgreement tenancy, string contactKey = "")
        {
            var domainEntity = databaseEntity.ToDomain();
            domainEntity.ResidentAddress = address.ToDomain();
            domainEntity.UPRN = address.UPRN;
            domainEntity.PhoneNumber = phoneNumber;
            domainEntity.Email = emailAddresses;
            domainEntity.TenancyReference = tenancy.TagRef;
            domainEntity.ContactKey = contactKey;
            return domainEntity;
        }
    }
}
