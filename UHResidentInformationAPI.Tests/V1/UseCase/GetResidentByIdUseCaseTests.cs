using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using UHResidentInformationAPI.V1.Domain;
using UHResidentInformationAPI.V1.Enums;
using UHResidentInformationAPI.V1.Factories;
using UHResidentInformationAPI.V1.Gateways;
using UHResidentInformationAPI.V1.UseCase;
using ResidentInformation = UHResidentInformationAPI.V1.Domain.ResidentInformation;
using ResidentInformationResponse = UHResidentInformationAPI.V1.Boundary.Responses.ResidentInformation;


namespace UHResidentInformationAPI.Tests.V1.UseCase
{
    [TestFixture]
    public class GetResidentByIdUseCaseTests
    {
        private Mock<IUHGateway> _mockUhGateway;
        private GetResidentByIdUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();

        [SetUp]
        public void Setup()
        {
            _mockUhGateway = new Mock<IUHGateway>();
            _classUnderTest = new GetResidentByIdUseCase(_mockUhGateway.Object);
        }

        [Test]
        public void ReturnsAResidentInformationRecordWithAddressForTheSpecifiedID()
        {
            var stubbedResidentInfo = _fixture.Create<ResidentInformation>();
            var houseRef = _fixture.Create<string>();
            var personRef = _fixture.Create<int>();

            _mockUhGateway.Setup(x =>
                    x.GetResidentById(houseRef, personRef))
                .Returns(stubbedResidentInfo);

            var response = _classUnderTest.Execute(houseRef, personRef);
            var expectedResponse = stubbedResidentInfo.ToResponse();

            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(expectedResponse);
        }

        [Test]
        public void ReturnsAResidentInformationRecordWithOutAddressForTheSpecifiedID()
        {
            var stubbedResidentInfo = _fixture.Create<ResidentInformation>();
            var houseRef = _fixture.Create<string>();
            var personRef = _fixture.Create<int>();

            stubbedResidentInfo.ResidentAddress = null;

            _mockUhGateway.Setup(x =>
                    x.GetResidentById(houseRef, personRef))
                .Returns(stubbedResidentInfo);

            var response = _classUnderTest.Execute(houseRef, personRef);
            var expectedResponse = stubbedResidentInfo.ToResponse();

            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(expectedResponse);
        }

        [Test]
        public void ReturnsAResidentInformationRecordWithOutPhoneTypeForTheSpecifiedID()
        {
            var stubbedResidentInfo = _fixture.Create<ResidentInformation>();
            var houseRef = _fixture.Create<string>();
            var personRef = _fixture.Create<int>();

            stubbedResidentInfo.PhoneNumber.FirstOrDefault().Type = null;

            _mockUhGateway.Setup(x =>
                    x.GetResidentById(houseRef, personRef))
                .Returns(stubbedResidentInfo);

            var response = _classUnderTest.Execute(houseRef, personRef);
            var expectedResponse = stubbedResidentInfo.ToResponse();

            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(expectedResponse);
            response.PhoneNumber.FirstOrDefault().PhoneType.Should().BeNullOrEmpty();
        }

        [Test]
        public void ReturnsAResidentInformationRecordWithPhoneTypeForTheSpecifiedID()
        {
            var stubbedResidentInfo = _fixture.Create<ResidentInformation>();
            var houseRef = _fixture.Create<string>();
            var personRef = _fixture.Create<int>();

            stubbedResidentInfo.PhoneNumber.FirstOrDefault().Type = PhoneType.F;

            _mockUhGateway.Setup(x =>
                    x.GetResidentById(houseRef, personRef))
                .Returns(stubbedResidentInfo);

            var response = _classUnderTest.Execute(houseRef, personRef);
            var expectedResponse = stubbedResidentInfo.ToResponse();

            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(expectedResponse);
            response.PhoneNumber.FirstOrDefault().PhoneType.Should().Be("F");
        }

        [Test]
        public void IfGatewayReturnsNullThrowNotFoundException()
        {
            ResidentInformation resultFromGateway = null;

            _mockUhGateway.Setup(x =>
                    x.GetResidentById(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(resultFromGateway);

            Func<ResidentInformationResponse> testDelegate = () => _classUnderTest.Execute("123", 456);
            testDelegate.Should().Throw<ResidentNotFoundException>();
        }
    }
}
