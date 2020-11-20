using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using HousingResidentInformationAPI.V1.Boundary.Requests;
using HousingResidentInformationAPI.V1.Factories;
using HousingResidentInformationAPI.V1.Gateways;
using HousingResidentInformationAPI.V1.UseCase;
using ResidentInformation = HousingResidentInformationAPI.V1.Domain.ResidentInformation;

namespace HousingResidentInformationAPI.Tests.V1.UseCase
{
    [TestFixture]
    public class GetAllResidentsUseCaseTest
    {
        private Mock<IHousingGateway> _mockhousingGateway;
        private GetAllResidentsUseCase _classUnderTest;
        private Fixture _fixture = new Fixture();

        [SetUp]
        public void SetUp()
        {
            _mockhousingGateway = new Mock<IHousingGateway>();
            _classUnderTest = new GetAllResidentsUseCase(_mockhousingGateway.Object);
        }

        [Test]
        public void ReturnsResidentInformationList()
        {
            var stubbedResidents = _fixture.CreateMany<ResidentInformation>();

            _mockhousingGateway.Setup(x =>
                    x.GetAllResidents(null, 10, "000011", "ciasom", "tessellate", "1 Montage street", false))
                .Returns(stubbedResidents.ToList());
            var rqp = new ResidentQueryParam
            {
                HouseReference = "000011",
                FirstName = "ciasom",
                LastName = "tessellate",
                Address = "1 Montage street"
            };

            var response = _classUnderTest.Execute(rqp, null, 10);

            response.Should().NotBeNull();
            response.Residents.Should().BeEquivalentTo(stubbedResidents.ToResponse());
        }

        [Test]
        public void IfLimitLessThanTheMinimumWillUseTheMinimumLimit()
        {
            _mockhousingGateway.Setup(x => x.GetAllResidents(null, 10, null, null, null, null, false))
                .Returns(new List<ResidentInformation>()).Verifiable();

            _classUnderTest.Execute(new ResidentQueryParam(), null, 9);

            _mockhousingGateway.Verify();
        }

        [Test]
        public void IfLimitMoreThanTheMaximumWillUseTheMaximumLimit()
        {
            _mockhousingGateway.Setup(x => x.GetAllResidents(null, 100, null, null, null, null, false))
                .Returns(new List<ResidentInformation>()).Verifiable();

            _classUnderTest.Execute(new ResidentQueryParam(), null, 101);

            _mockhousingGateway.Verify();
        }

        [Test]
        public void ReturnsTheNextCursor()
        {
            var stubbedResidents = _fixture
                .CreateMany<ResidentInformation>(10)
                .OrderBy(r => r.HouseReference + r.PersonNumber.ToString());

            var expectedNextCursor = stubbedResidents
                .Max(r => r.HouseReference + r.PersonNumber.ToString());

            _mockhousingGateway.Setup(x =>
                    x.GetAllResidents(null, 10, null, null, null, null, false))
                .Returns(stubbedResidents.ToList());

            var receivedNextCursor = _classUnderTest
                .Execute(new ResidentQueryParam(), null, 10).NextCursor;

            receivedNextCursor.Should().Be(expectedNextCursor);
        }

        [Test]
        public void WhenAtTheEndOfTheResidentListReturnsTheNextCursorAsEmptyString()
        {
            var stubbedResidents = _fixture.CreateMany<ResidentInformation>(7);

            _mockhousingGateway.Setup(x =>
                    x.GetAllResidents(null, 10, null, null, null, null, false))
                .Returns(stubbedResidents.ToList());

            _classUnderTest.Execute(new ResidentQueryParam(), null, 10).NextCursor.Should().Be("");
        }
    }
}
