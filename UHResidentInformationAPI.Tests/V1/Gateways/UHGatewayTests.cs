using System.Collections.Generic;
using System.Globalization;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using UHResidentInformationAPI.Tests.V1.Helper;
using UHResidentInformationAPI.V1.Domain;
using UHResidentInformationAPI.V1.Enums;
using UHResidentInformationAPI.V1.Gateways;
using UHResidentInformationAPI.V1.Infrastructure;
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
            response.Address.Should().BeEquivalentTo(expectedDomainAddress);
        }

        [Test]
        public void GetResidentByIdReturnsPhoneContactDetails()
        {
            var databasePersonEntity = AddPersonRecordToDatabase();
            var databasePhoneEntity = TestHelper.CreateDatabaseTelephoneNumberForPersonId(databasePersonEntity.PersonNo);

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
            var databaseEmailEntity = TestHelper.CreateDatabaseEmailForPerson(databasePersonEntity.PersonNo);

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


        private Person AddPersonRecordToDatabase(string firstname = null, string lastname = null)
        {
            var databaseEntity = TestHelper.CreateDatabasePersonEntity(firstname, lastname);
            UHContext.Persons.Add(databaseEntity);
            UHContext.SaveChanges();
            return databaseEntity;
        }
    }
}
