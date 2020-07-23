using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using UHResidentInformationAPI.V1.Boundary.Requests;
using UHResidentInformationAPI.V1.Factories;
using UHResidentInformationAPI.V1.Gateways;
using UHResidentInformationAPI.V1.UseCase;
using ResidentInformation = UHResidentInformationAPI.V1.Domain.ResidentInformation;

namespace UHResidentInformationAPI.Tests.V1.UseCase
{
    [TestFixture]
    public class GetAllResidentsUseCaseTest
    {
        private Mock<IUHGateway> _mockUHGateway;
        private GetAllResidentsUseCase _classUnderTest;
        private Fixture _fixture = new Fixture();

        [SetUp]
        public void SetUp()
        {
            _mockUHGateway = new Mock<IUHGateway>();
            _classUnderTest = new GetAllResidentsUseCase(_mockUHGateway.Object);
        }

        [Test]
        public void ReturnsResidentInformationList()
        {
            var stubbedResidents = _fixture.CreateMany<ResidentInformation>();

            _mockUHGateway.Setup(x =>
                    x.GetAllResidents("000011", "ciasom", "tessellate", "1 Montage street"))
                .Returns(stubbedResidents.ToList());
            var rqp = new ResidentQueryParam
            {
                HouseReference = "000011",
                FirstName = "ciasom",
                LastName = "tessellate",
                Address = "1 Montage street"
            };

            var response = _classUnderTest.Execute(rqp);

            response.Should().NotBeNull();
            response.Residents.Should().BeEquivalentTo(stubbedResidents.ToResponse());
        }

        [Test]
        [Ignore("TO DO")]
        public void IfLimitLessThanTheMinimumWillUseTheMinimumLimit()
        {
            _mockUHGateway.Setup(x => x.GetAllResidents(null, null, null, null))
                .Returns(new List<ResidentInformation>()).Verifiable();

            _classUnderTest.Execute(new ResidentQueryParam());

            _mockUHGateway.Verify();
        }

        [Test]
        [Ignore("TO DO")]
        public void IfLimitMoreThanTheMaximumWillUseTheMaximumLimit()
        {
            _mockUHGateway.Setup(x => x.GetAllResidents(null, null, null, null))
                .Returns(new List<ResidentInformation>()).Verifiable();

            _classUnderTest.Execute(new ResidentQueryParam());

            _mockUHGateway.Verify();
        }

        [Test]
        [Ignore("TO DO")]

        public void ReturnsTheNextCursor()
        {
            var stubbedResidents = _fixture.CreateMany<ResidentInformation>(10);

            var expectedNextCursor = stubbedResidents.Max(r => r.UPRN);

            _mockUHGateway.Setup(x =>
                    x.GetAllResidents(null, null, null, null))
                .Returns(stubbedResidents.ToList());

            // _classUnderTest.Execute(new ResidentQueryParam(), 0, 10).NextCursor.Should().Be(expectedNextCursor);
        }

        [Test]
        [Ignore("TO DO")]
        public void WhenAtTheEndOfTheResidentListReturnsTheNextCursorAsEmptyString()
        {
            var stubbedResidents = _fixture.CreateMany<ResidentInformation>(7);

            _mockUHGateway.Setup(x =>
                    x.GetAllResidents(null, null, null, null))
                .Returns(stubbedResidents.ToList());

            // _classUnderTest.Execute(new ResidentQueryParam(), 0, 10).NextCursor.Should().Be("");
        }
    }
}
