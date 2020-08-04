using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using UHResidentInformationAPI.Tests.V1.Helper;
using UHResidentInformationAPI.V1.Domain;
using UHResidentInformationAPI.V1.Enums;
using UHResidentInformationAPI.V1.Factories;
using UHResidentInformationAPI.V1.Gateways;
using UHResidentInformationAPI.V1.Infrastructure;
using Address = UHResidentInformationAPI.V1.Infrastructure.Address;
using DomainAddress = UHResidentInformationAPI.V1.Domain.Address;

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
                PostCode = databaseAddressEntity.PostCode
            };

            var response = _classUnderTest.GetResidentById(databasePersonEntity.HouseRef, databasePersonEntity.PersonNo);

            response.HouseReference.Should().Be(databasePersonEntity.HouseRef);
            response.ResidentAddress.Should().BeEquivalentTo(expectedDomainAddress);
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
            response.PhoneNumber.Should().BeEquivalentTo(expectedPhoneNumberList);
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
            response.PhoneNumber.Should().BeEquivalentTo(expectedPhoneNumberList);
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
        public void GetResidentByIdReturnsTheTenancyReferenceInTheResponse()
        {
            var databasePersonEntity = AddPersonRecordToDatabase();
            var tenancyDatabaseEntity = TestHelper.CreateDatabaseTenancyAgreementForPerson(databasePersonEntity.HouseRef);

            UHContext.TenancyAgreements.Add(tenancyDatabaseEntity);
            UHContext.SaveChanges();

            var response = _classUnderTest.GetResidentById(databasePersonEntity.HouseRef, databasePersonEntity.PersonNo);
            response.TenancyReference.Should().Be(tenancyDatabaseEntity.TagRef);
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
            response.Email.Should().BeEquivalentTo(expectedEmailAddressList);
        }

        public void GetAllResidentsIfThereAreNoResidentsReturnsAnEmptyList()
        {
            _classUnderTest.GetAllResidents(null, 10, "00011", "bob", "brown", "1 Hillman Street").Should().BeEmpty();
        }

        [Test]
        public void GetAllResidentsWithFirstNameQueryParameterReturnsMatchingResident()
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

            var addresslist = new List<Address>
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
            domainEntity.ResidentAddress = address.ToDomain();
            domainEntity.UPRN = address.UPRN;
            domainEntity.PhoneNumber = new List<Phone> { telephone.ToDomain(), telephone1.ToDomain() };
            domainEntity.Email = new List<Email> { emailAddress.ToDomain(), emailAddress1.ToDomain() };
            domainEntity.TenancyReference = tenancy1.TagRef;

            var domainEntity2 = databaseEntity2.ToDomain();
            domainEntity2.ResidentAddress = address2.ToDomain();
            domainEntity2.UPRN = address2.UPRN;
            domainEntity2.PhoneNumber = new List<Phone> { telephone2.ToDomain() };
            domainEntity2.Email = new List<Email> { emailAddress2.ToDomain() };
            domainEntity2.TenancyReference = tenancy3.TagRef;

            var listOfPersons = _classUnderTest.GetAllResidents(null, 10, firstName: "ciasom");
            listOfPersons.Count.Should().Be(2);
            listOfPersons.Should().ContainEquivalentOf(domainEntity);
            listOfPersons.Should().ContainEquivalentOf(domainEntity2);

        }


        [Test]
        public void GetAllResidentsWithNoEmailWithLastNameQueryParameterReturnsMatchingResidents()
        {
            var databaseEntity = TestHelper.CreateDatabasePersonEntity(lastname: "brown");
            var databaseEntity1 = TestHelper.CreateDatabasePersonEntity(lastname: "tessellate");
            var databaseEntity2 = TestHelper.CreateDatabasePersonEntity(lastname: "brown");

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

            var addresslist = new List<Address>
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


            var domainEntity = databaseEntity.ToDomain();
            domainEntity.ResidentAddress = address.ToDomain();
            domainEntity.UPRN = address.UPRN;
            domainEntity.PhoneNumber = new List<Phone> { telephone.ToDomain(), telephone1.ToDomain() };
            domainEntity.TenancyReference = tenancy1.TagRef;

            var domainEntity2 = databaseEntity2.ToDomain();
            domainEntity2.ResidentAddress = address2.ToDomain();
            domainEntity2.UPRN = address2.UPRN;
            domainEntity2.PhoneNumber = new List<Phone> { telephone2.ToDomain() };
            domainEntity2.TenancyReference = tenancy3.TagRef;

            var listOfPersons = _classUnderTest.GetAllResidents(null, 10, lastName: "brown");
            listOfPersons.Count.Should().Be(2);
            listOfPersons.Should().ContainEquivalentOf(domainEntity);
            listOfPersons.Should().ContainEquivalentOf(domainEntity2);

        }

        [Test]
        public void GetAllResidentsWithNoPhoneWithFirstAndLastNameQueryParameterReturnsMatchingResidents()
        {
            var databaseEntity = TestHelper.CreateDatabasePersonEntity(firstname: "ciasom", lastname: "Brown");
            var databaseEntity1 = TestHelper.CreateDatabasePersonEntity(firstname: "ciasom", lastname: "tessellate");
            var databaseEntity2 = TestHelper.CreateDatabasePersonEntity(firstname: "Ciasom", lastname: "brown");

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

            var addresslist = new List<Address>
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
            domainEntity.ResidentAddress = address.ToDomain();
            domainEntity.UPRN = address.UPRN;
            domainEntity.Email = new List<Email> { emailAddress.ToDomain(), emailAddress1.ToDomain() };
            domainEntity.TenancyReference = tenancy1.TagRef;

            var domainEntity2 = databaseEntity2.ToDomain();
            domainEntity2.ResidentAddress = address2.ToDomain();
            domainEntity2.UPRN = address2.UPRN;
            domainEntity2.Email = new List<Email> { emailAddress2.ToDomain() };
            domainEntity2.TenancyReference = tenancy3.TagRef;

            var listOfPersons = _classUnderTest.GetAllResidents(null, 10, firstName: "ciasom", lastName: "brown");
            listOfPersons.Count.Should().Be(2);
            listOfPersons.Should().ContainEquivalentOf(domainEntity);
            listOfPersons.Should().ContainEquivalentOf(domainEntity2);

        }

        [Test]
        public void GetAllResidentsWithNoContactlinkWithAddressQueryParameterReturnsMatchingResidents()
        {
            var databaseEntity = TestHelper.CreateDatabasePersonEntity();
            var databaseEntity1 = TestHelper.CreateDatabasePersonEntity();
            var databaseEntity2 = TestHelper.CreateDatabasePersonEntity();

            var personslist = new List<Person>
            {
                databaseEntity,
                databaseEntity1,
                databaseEntity2
            };
            //Add person entities to test database
            UHContext.Persons.AddRange(personslist);
            UHContext.SaveChanges();

            var address = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity.HouseRef, address1: "1 Hillman st");
            var address1 = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity1.HouseRef);
            var address2 = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity2.HouseRef, address1: "2 Hillman st");

            var addresslist = new List<Address>
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

            var domainEntity = databaseEntity.ToDomain();
            domainEntity.ResidentAddress = address.ToDomain();
            domainEntity.UPRN = address.UPRN;
            domainEntity.TenancyReference = tenancy1.TagRef;

            var listOfPersons = _classUnderTest.GetAllResidents(null, 10, address: "1 Hillman st");
            listOfPersons.Count.Should().Be(1);
            listOfPersons.Should().ContainEquivalentOf(domainEntity);
        }

        [Test]
        public void GetAllResidentsWhenAContactlinkRowsHasNoKey1AndKey2DoesNotBreakTheQuery()
        {
            var databaseEntity = TestHelper.CreateDatabasePersonEntity();
            UHContext.Persons.Add(databaseEntity);
            UHContext.SaveChanges();

            var address = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity.HouseRef, address1: "1 Hillman st");
            UHContext.Addresses.Add(address);
            UHContext.SaveChanges();

            var tenancy1 = TestHelper.CreateDatabaseTenancyAgreementForPerson(address.HouseRef);
            UHContext.TenancyAgreements.Add(tenancy1);
            UHContext.SaveChanges();

            //Create a row in the CCContactLink that has no values in Key1 and Key2. This was found in production.
            var link1 = TestHelper.CreateDatabaseContactLinkForPerson(tenancy1.TagRef, databaseEntity.PersonNo);
            link1.TagRef = null; //Key1
            link1.PersonNo = null; //Key2

            UHContext.ContactLinks.Add(link1);
            UHContext.SaveChanges();

            var domainEntity = databaseEntity.ToDomain();
            domainEntity.ResidentAddress = address.ToDomain();
            domainEntity.UPRN = address.UPRN;
            domainEntity.TenancyReference = tenancy1.TagRef;

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
            AddContactLinkForPersonToDatabase(person1.HouseRef, person1.PersonNo);

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
                AddContactLinkForPersonToDatabase(person.HouseRef, person.PersonNo);
            });

            var cursor = $"{persons[0].HouseRef}{persons[0].PersonNo}";

            var expectedPersons = persons
                .OrderBy(p => p.HouseRef)
                .ThenBy(p => p.PersonNo)
                .ToList()
                .TakeLast(2);

            var receivedPersons = _classUnderTest.GetAllResidents(cursor, 3);

            receivedPersons.Count.Should().Be(3);
            receivedPersons[0].FirstName.Should().Be(persons[2].FirstName);
            receivedPersons[1].FirstName.Should().Be(persons[3].FirstName);
            receivedPersons[2].FirstName.Should().Be(persons[1].FirstName);
        }

        private Person AddPersonRecordToDatabase(string firstname = null, string lastname = null, string houseRef = null, int? personNo = null)
        {
            var databaseEntity = TestHelper.CreateDatabasePersonEntity(firstname, lastname, houseRef, personNo);
            UHContext.Persons.Add(databaseEntity);
            UHContext.SaveChanges();
            return databaseEntity;
        }

        private Address AddAddressRecordToDatabase(string houseRef, string postcode = null, string address1 = null)
        {
            var address = TestHelper.CreateDatabaseAddressForPersonId(houseRef, postcode, address1);
            UHContext.Addresses.Add(address);
            UHContext.SaveChanges();
            return address;
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
    }
}
