using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using HousingResidentInformationAPI.V1.Domain;
using HousingResidentInformationAPI.V1.Enums;
using HousingResidentInformationAPI.V1.Factories;
using HousingResidentInformationAPI.V1.Gateways;
using HousingResidentInformationAPI.V1.UseCase;
using ResidentInformation = HousingResidentInformationAPI.V1.Domain.ResidentInformation;
using ResidentInformationResponse = HousingResidentInformationAPI.V1.Boundary.Responses.ResidentInformation;


namespace HousingResidentInformationAPI.Tests.V1.UseCase
{
    [TestFixture]
    public class GetResidentByIdUseCaseTests
    {
        private Mock<IHousingGateway> _mockhousingGateway;
        private GetResidentByIdUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();

        [SetUp]
        public void Setup()
        {
            _mockhousingGateway = new Mock<IHousingGateway>();
            _classUnderTest = new GetResidentByIdUseCase(_mockhousingGateway.Object);
        }

        [Test]
        public void ReturnsAResidentInformationRecordWithAddressForTheSpecifiedID()
        {
            var stubbedResidentInfo = _fixture.Create<ResidentInformation>();
            var houseRef = _fixture.Create<string>();
            var personRef = _fixture.Create<int>();

            _mockhousingGateway.Setup(x =>
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

            _mockhousingGateway.Setup(x =>
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

            _mockhousingGateway.Setup(x =>
                    x.GetResidentById(houseRef, personRef))
                .Returns(stubbedResidentInfo);

            var response = _classUnderTest.Execute(houseRef, personRef);
            var expectedResponse = stubbedResidentInfo.ToResponse();

            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(expectedResponse);
            response.PhoneNumbers.FirstOrDefault().PhoneType.Should().BeNullOrEmpty();
        }

        [Test]
        public void ReturnsAResidentInformationRecordWithPhoneTypeForTheSpecifiedID()
        {
            var stubbedResidentInfo = _fixture.Create<ResidentInformation>();
            var houseRef = _fixture.Create<string>();
            var personRef = _fixture.Create<int>();

            stubbedResidentInfo.PhoneNumber.FirstOrDefault().Type = PhoneType.F;

            _mockhousingGateway.Setup(x =>
                    x.GetResidentById(houseRef, personRef))
                .Returns(stubbedResidentInfo);

            var response = _classUnderTest.Execute(houseRef, personRef);
            var expectedResponse = stubbedResidentInfo.ToResponse();

            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(expectedResponse);
            response.PhoneNumbers.FirstOrDefault().PhoneType.Should().Be("F");
        }

        [Test]
        public void IfGatewayReturnsNullThrowNotFoundException()
        {
            ResidentInformation resultFromGateway = null;

            _mockhousingGateway.Setup(x =>
                    x.GetResidentById(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(resultFromGateway);

            Func<ResidentInformationResponse> testDelegate = () => _classUnderTest.Execute("123", 456);
            testDelegate.Should().Throw<ResidentNotFoundException>();
        }
    }
}
