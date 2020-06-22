using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using UHResidentInformationAPI.V1.Controllers;
using UHResidentInformationAPI.V1.UseCase.Interfaces;

namespace UHResidentInformationAPI.Tests.V1.Controllers
{
    [TestFixture]
    public class UHControllerTests
    {
        private UHController _classUnderTest;

        private Mock<IGetAllResidentsUseCase> _mockGetAllResidentsUseCase;
        private Mock<IGetEntityByIdUseCase> _mockGetEntityByIdUseCase;

        [SetUp]
        public void SetUp()
        {
            _mockGetAllResidentsUseCase = new Mock<IGetAllResidentsUseCase>();
            _mockGetEntityByIdUseCase = new Mock<IGetEntityByIdUseCase>();
            _classUnderTest = new UHController(_mockGetAllResidentsUseCase.Object, _mockGetEntityByIdUseCase.Object);
        }

        [Test]
        public void ListRecordsTest()
        {

        }

        [Test]
        public void ViewRecordTest()
        {

        }

    }
}
