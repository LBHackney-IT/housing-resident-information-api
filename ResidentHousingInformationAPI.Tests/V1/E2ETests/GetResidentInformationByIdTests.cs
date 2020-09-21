using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using HousingResidentInformationAPI;
using HousingResidentInformationAPI.Tests;
using HousingResidentInformationAPI.Tests.V1.E2ETests;
using HousingResidentInformationAPI.V1.Boundary.Responses;

namespace HousingResidentInformationAPI.Tests.V1.E2ETests
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

            var expectedResponse = E2ETestHelpers.AddPersonWithRelatedEntitiesToDb(HousingContext, houseRef, personNo);

            var uri = new Uri($"api/v1/households/{houseRef}/people/{personNo}", UriKind.Relative);
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
            var requestUri = new Uri($"api/v1/households/0/people/0", UriKind.Relative);
            var response = Client.GetAsync(requestUri);
            var statusCode = response.Result.StatusCode;
            statusCode.Should().Be(404);
        }
    }
}
