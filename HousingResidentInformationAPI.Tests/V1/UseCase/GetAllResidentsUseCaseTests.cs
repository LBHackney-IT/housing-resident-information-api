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
using HousingResidentInformationAPI.V1.UseCase.Interfaces;
using System;
using HousingResidentInformationAPI.V1.Domain;
using HousingResidentInformationAPI.V1.Boundary.Responses;

namespace HousingResidentInformationAPI.Tests.V1.UseCase
{
    [TestFixture]
    public class GetAllResidentsUseCaseTest
    {
        private Mock<IHousingGateway> _mockhousingGateway;
        private GetAllResidentsUseCase _classUnderTest;
        private Fixture _fixture = new Fixture();
        private Mock<IValidatePostcode> _mockPostcodeValidator;

        [SetUp]
        public void SetUp()
        {
            _mockPostcodeValidator = new Mock<IValidatePostcode>();
            _mockPostcodeValidator.Setup(x => x.Execute(It.IsAny<string>())).Returns(true);
            _mockhousingGateway = new Mock<IHousingGateway>();
            _classUnderTest = new GetAllResidentsUseCase(_mockhousingGateway.Object, _mockPostcodeValidator.Object);
        }

        [Test]
        public void ReturnsResidentInformationList()
        {
            var stubbedResidents = _fixture.CreateMany<ResidentInformation>();

            _mockhousingGateway.Setup(x =>
                    x.GetAllResidents(null, 10, "000011", "ciasom", "tessellate", "1 Montage street", "E8 1DY", false))
                .Returns(stubbedResidents.ToList());
            var rqp = new ResidentQueryParam
            {
                HouseReference = "000011",
                FirstName = "ciasom",
                LastName = "tessellate",
                Address = "1 Montage street",
                Postcode = "E8 1DY",
                ActiveTenanciesOnly = false
            };

            var response = _classUnderTest.Execute(rqp, null, 10);

            response.Should().NotBeNull();
            response.Residents.Should().BeEquivalentTo(stubbedResidents.ToResponse());
        }

        [Test]
        public void IfLimitLessThanTheMinimumWillUseTheMinimumLimit()
        {
            _mockhousingGateway.Setup(x => x.GetAllResidents(null, 10, null, null, null, null, null, false))
                .Returns(new List<ResidentInformation>()).Verifiable();

            _classUnderTest.Execute(new ResidentQueryParam(), null, 9);

            _mockhousingGateway.Verify();
        }

        [Test]
        public void IfLimitMoreThanTheMaximumWillUseTheMaximumLimit()
        {
            _mockhousingGateway.Setup(x => x.GetAllResidents(null, 100, null, null, null, null, null, false))
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
                    x.GetAllResidents(null, 10, null, null, null, null, null, false))
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
                    x.GetAllResidents(null, 10, null, null, null, null, null, false))
                .Returns(stubbedResidents.ToList());

            _classUnderTest.Execute(new ResidentQueryParam(), null, 10).NextCursor.Should().Be("");
        }


        [Test]
        public void ExecuteCallsTheGatewayWithPostcodeQueryParameter()
        {
            var postcode = _fixture.Create<string>();
            var stubbedResidents = _fixture.CreateMany<ResidentInformation>();

            _mockhousingGateway.Setup(x =>
                    x.GetAllResidents(null, 10, null, null, null, null, postcode, false))
                .Returns(stubbedResidents.ToList());

            var rqp = new ResidentQueryParam
            {
                Postcode = postcode,
                ActiveTenanciesOnly = false
            };

            _classUnderTest.Execute(rqp, null, 10);
            _mockhousingGateway.Verify();
        }

        [Test]
        public void IfPostcodeQueryIsInvalidExecuteWillReturnBadRequest()
        {
            var postcode = "E8881DY";
            var stubbedResidents = _fixture.CreateMany<ResidentInformation>(7);

            _mockhousingGateway.Setup(x =>
                    x.GetAllResidents(null, 10, null, null, null, null, postcode, false))
                .Returns(stubbedResidents.ToList());
            _mockPostcodeValidator.Setup(x => x.Execute(postcode)).Returns(false);

            var rqp = new ResidentQueryParam
            {
                HouseReference = "000011",
                FirstName = "ciasom",
                LastName = "tessellate",
                Address = "1 Montage street",
                Postcode = postcode,
                ActiveTenanciesOnly = false
            };

            Func<ResidentInformationList> testDelegate = () => _classUnderTest.Execute(rqp, null, 10);
            testDelegate.Should().Throw<InvalidQueryParameterException>()
                .WithMessage("The Postcode given does not have a valid format");
        }
    }
}
