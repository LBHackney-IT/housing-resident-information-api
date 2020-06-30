using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using UHResidentInformationAPI;
using UHResidentInformationAPI.Tests;
using UHResidentInformationAPI.Tests.V1.E2ETests;
using UHResidentInformationAPI.V1.Boundary.Responses;

namespace UHResidentInformationApi.Tests.V1.E2ETests
{
    [TestFixture]
    public class GetResidentInformationById : EndToEndTests<Startup>
    {
        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        [Test]
        public async Task GetResidentByIdReturnsTheCorrectInformation()
        {
            var personNo = 11;
            var houseRef = "XXXtestRef";

            var expectedResponse = E2ETestHelpers.AddPersonWithRelatedEntitiesToDb(UHContext, houseRef, personNo);

            var uri = new Uri($"/households/{houseRef}/people/{personNo}", UriKind.Relative);
            var response = Client.GetAsync(uri);
            var statusCode = response.Result.StatusCode;
            statusCode.Should().Be(200);

            var content = response.Result.Content;
            var stringContent = await content.ReadAsStringAsync().ConfigureAwait(true);
            var convertedResponse = JsonConvert.DeserializeObject<ResidentInformation>(stringContent);

            convertedResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [Test]
        public void GetHousingInformationByIdReturns404NotFound()
        {
            var requestUri = new Uri($"/this/url/will/not/work", UriKind.Relative);
            var response = Client.GetAsync(requestUri);
            var statusCode = response.Result.StatusCode;
            statusCode.Should().Be(404);
        }
    }
}
