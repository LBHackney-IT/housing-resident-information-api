using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using HousingResidentInformationAPI.V1.Boundary.Requests;
using HousingResidentInformationAPI.V1.Boundary.Responses;
using HousingResidentInformationAPI.V1.Controllers;
using HousingResidentInformationAPI.V1.UseCase.Interfaces;

namespace HousingResidentInformationAPI.Tests.V1.Controllers
{
    [TestFixture]
    public class HousingControllerTests
    {
        private HousingController _classUnderTest;

        private Mock<IGetAllResidentsUseCase> _mockGetAllResidentsUseCase;
        private Mock<IGetResidentByIdUseCase> _mockGetResidentByIdUseCase;

        [SetUp]
        public void SetUp()
        {
            _mockGetAllResidentsUseCase = new Mock<IGetAllResidentsUseCase>();
            _mockGetResidentByIdUseCase = new Mock<IGetResidentByIdUseCase>();
            _classUnderTest = new HousingController(_mockGetAllResidentsUseCase.Object, _mockGetResidentByIdUseCase.Object);
        }

        [Test]
        public void ListRecordsTest()
        {
            var residentInfo = new List<ResidentInformation>()
            {
                new ResidentInformation()
                {
                    Uprn = "123456789",
                    FirstName = "test",
                    LastName = "test",
                    DateOfBirth = "01/01/2020"
                }
            };

            var residentInformationList = new ResidentInformationList()
            {
                Residents = residentInfo
            };

            var rqp = new ResidentQueryParam
            {
                FirstName = "Ciasom",
                LastName = "Tessellate",
            };

            _mockGetAllResidentsUseCase.Setup(x => x.Execute(rqp, null, 10)).Returns(residentInformationList);
            var response = _classUnderTest.ListRecords(rqp, null, 10) as OkObjectResult;

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
                Uprn = "test",
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
