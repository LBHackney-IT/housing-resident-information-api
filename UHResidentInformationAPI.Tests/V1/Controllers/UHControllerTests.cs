using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using UHResidentInformationAPI.V1.Controllers;

namespace UHResidentInformationAPI.Tests.V1.Controllers
{
    [TestFixture]
    public class UHControllerTests
    {
        private UHController _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _classUnderTest = new UHController();
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
