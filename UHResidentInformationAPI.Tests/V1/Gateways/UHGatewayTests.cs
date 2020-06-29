using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using UHResidentInformationAPI.Tests.V1.Helper;
using UHResidentInformationAPI.V1.Domain;
using UHResidentInformationAPI.V1.Gateways;
using UHResidentInformationAPI.V1.Factories;
using System.Collections.Generic;
using UHResidentInformationAPI.V1.Infrastructure;

namespace UHResidentInformationAPI.Tests.V1.Gateways
{
    [TestFixture]
    public class UHGatewayTests : DatabaseTests
    {
        //private Fixture _fixture = new Fixture();
        private UHGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new UHGateway(UHContext);
        }

        [Test]
        public void GatewayImplementsBoundaryInterface()
        {
            Assert.NotNull(_classUnderTest is IUHGateway);
        }

        [Test]
        public void GetAllResidentsIfThereAreNoResidentsReturnsAnEmptyList()
        {
            _classUnderTest.GetAllResidents("00011", "bob", "brown", "1 Hillman Street").Should().BeEmpty();
        }

        //[Test]
        //public void GetAllClaimantsIfThereAreClaimantWillReturnThemWithAddresses()
        //{
        //    var databaseEntity = AddPersonRecordToDatabase();

        //    var address = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity.ClaimId, databaseEntity.HouseId);
        //    AcademyContext.Addresses.Add(address);
        //    AcademyContext.SaveChanges();

        //    var domainEntity = databaseEntity.ToDomain();
        //    domainEntity.ClaimantAddress = address.ToDomain();

        //    var listOfPersons = _classUnderTest.GetAllClaimants(0, 20, postcode: address.PostCode);

        //    listOfPersons.Should().ContainEquivalentOf(domainEntity);

        //    listOfPersons
        //        .First(p => p.ClaimId == databaseEntity.ClaimId)
        //        .ClaimantAddress
        //        .Should().BeEquivalentTo(address.ToDomain());
        //}



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

            UHContext.Persons.AddRange(personslist);
            UHContext.SaveChanges();

            var address = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity.HouseRef);
            UHContext.Addresses.Add(address);
            UHContext.SaveChanges();

            var address1 = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity1.HouseRef);
            UHContext.Addresses.Add(address1);
            UHContext.SaveChanges();

            var address2 = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity2.HouseRef);
            UHContext.Addresses.Add(address2);
            UHContext.SaveChanges();


            var telephone = TestHelper.CreateDatabaseTelephoneNumberForPersonId(databaseEntity.PersonNo);
            UHContext.TelephoneNumbers.Add(telephone);
            UHContext.SaveChanges();

            var telephone1 = TestHelper.CreateDatabaseTelephoneNumberForPersonId(databaseEntity2.PersonNo);
            UHContext.TelephoneNumbers.Add(telephone1);
            UHContext.SaveChanges();

            var domainEntity = databaseEntity.ToDomain();
            domainEntity.ResidentAddress = address.ToDomain();

            var domainEntity2 = databaseEntity2.ToDomain();
            domainEntity2.ResidentAddress = address2.ToDomain();

            var listOfPersons = _classUnderTest.GetAllResidents(firstName: "ciasom");
            listOfPersons.Count.Should().Be(2);
            listOfPersons.Should().ContainEquivalentOf(domainEntity);
            listOfPersons.Should().ContainEquivalentOf(domainEntity2);

        }


    }
}
