using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using UHResidentInformationAPI.V1.Boundary.Requests;
using UHResidentInformationAPI.V1.Boundary.Responses;
using UHResidentInformationAPI.V1.Controllers;
using UHResidentInformationAPI.V1.UseCase.Interfaces;

namespace UHResidentInformationAPI.Tests.V1.Controllers
{
    [TestFixture]
    public class UHControllerTests
    {
        private UHController _classUnderTest;

        private Mock<IGetAllResidentsUseCase> _mockGetAllResidentsUseCase;
        private Mock<IGetResidentByIdUseCase> _mockGetResidentByIdUseCase;

        [SetUp]
        public void SetUp()
        {
            _mockGetAllResidentsUseCase = new Mock<IGetAllResidentsUseCase>();
            _mockGetResidentByIdUseCase = new Mock<IGetResidentByIdUseCase>();
            _classUnderTest = new UHController(_mockGetAllResidentsUseCase.Object, _mockGetResidentByIdUseCase.Object);
        }

        [Test]
        public void ListRecordsTest()
        {
            var persons = new List<ResidentInformation>()
            {
                new ResidentInformation()
                {
                    FirstName = "test",
                    LastName = "test",
                    DateOfBirth = "01/01/2020"

                }
            };

            var residentInformationList = new ResidentInformationList()
            {
                Residents = persons
            };

            var rqp = new ResidentQueryParam();

            _mockGetAllResidentsUseCase
                .Setup(x => x.Execute(rqp, 0, 20)).Returns(residentInformationList);

            var response = _classUnderTest.ListRecords(rqp) as OkObjectResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(200);
            response.Value.Should().BeEquivalentTo(residentInformationList);
        }

        [Test]
        public void ViewRecordTest()
        {
            var singleResidentInfo = new ResidentInformation()
            {
                FirstName = "test",
                LastName = "test",
                DateOfBirth = "01/01/2020",
                UPRN = "test",
                HouseReference = "testHouseRef",
                PersonNumber = 123,
            };

            _mockGetResidentByIdUseCase.Setup(x => x.Execute("testHouseRef", 123)).Returns(singleResidentInfo);
            var response = _classUnderTest.ViewRecord("testHouseRef", 123) as OkObjectResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(200);
            response.Value.Should().BeEquivalentTo(singleResidentInfo);

        }

    }
}
