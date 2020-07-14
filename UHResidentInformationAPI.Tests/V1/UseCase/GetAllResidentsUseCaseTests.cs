using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using UHResidentInformationAPI.V1.Boundary.Requests;
using UHResidentInformationAPI.V1.Domain;
using UHResidentInformationAPI.V1.Factories;
using UHResidentInformationAPI.V1.Gateways;
using UHResidentInformationAPI.V1.UseCase;
using ResidentInformationList = UHResidentInformationAPI.V1.Boundary.Responses.ResidentInformationList;

namespace UHResidentInformationAPI.Tests.V1.UseCase
{
    [TestFixture]
    public class GetAllResidentsUseCaseTests
    {
        private Mock<IUHGateway> _mockUhGateway;
        private GetAllResidentsUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();

        [SetUp]
        public void Setup()
        {
            _mockUhGateway = new Mock<IUHGateway>();
            _classUnderTest = new GetAllResidentsUseCase(_mockUhGateway.Object);
        }

        [Test]
        public void ReturnsAResidentInformationListBuiltFromGatewayResult()
        {

            var gatewayResult = new List<ResidentInformation>
            {
                _fixture.Create<ResidentInformation>()
            };

            _mockUhGateway.Setup(x =>
                    x.GetAllResidents(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(gatewayResult);

            var rqp = new ResidentQueryParam
            {
                HouseReference = "123",
                FirstName = "First",
                LastName = "Last",
                AddressLine1 = "123 Main Street",
                Postcode = "E1 2BC"
            };

            var response = _classUnderTest.Execute(rqp, cursor: 0, limit: 20);

            var expectedResponse = new ResidentInformationList{
                Residents = gatewayResult.ToResponse()
            };

            response.Should().BeEquivalentTo(expectedResponse);
        }
    }
}
