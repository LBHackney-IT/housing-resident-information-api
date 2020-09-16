using System;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using housingResidentInformationAPI.V1.Domain;

namespace housingResidentInformationAPI.Tests.V1.Domain
{
    [TestFixture]
    public class EntityTests
    {
        private Fixture _fixture;
        private ResidentInformation _entity;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _entity = new ResidentInformation();
        }
        [Test]
        public void EntitiesHaveAnHouseReference()
        {
            var houseRef = _fixture.Create<string>();
            _entity.HouseReference = houseRef;

            _entity.HouseReference.Should().Be(houseRef);
        }

        [Test]
        public void EntitiesHaveAnPersonNumber()
        {
            var personNo = _fixture.Create<int>();
            _entity.PersonNumber = personNo;

            _entity.PersonNumber.Should().Be(personNo);
        }
    }
}
