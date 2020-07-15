using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using UHResidentInformationAPI.V1.Boundary.Responses;

namespace UHResidentInformationAPI.Tests.V1.E2ETests
{
    [TestFixture]
    public class ListResidentsReturnsAndQueriesAListOfAllResidents : EndToEndTests<Startup>
    {
        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        [Test]
        public async Task IfNoQueryParametersReturnsAllResidentRecordsFromUH()
        {
            var expectedResidentResponseOne = E2ETestHelpers.AddPersonWithRelatedEntitiesToDb(UHContext);
            var expectedResidentResponseTwo = E2ETestHelpers.AddPersonWithRelatedEntitiesToDb(UHContext);
            var expectedResidentResponseThree = E2ETestHelpers.AddPersonWithRelatedEntitiesToDb(UHContext);

            var uri = new Uri("api/v1/households", UriKind.Relative);
            var response = Client.GetAsync(uri);

            var statusCode = response.Result.StatusCode;
            statusCode.Should().Be(200);

            var content = response.Result.Content;
            var stringContent = await content.ReadAsStringAsync().ConfigureAwait(true);
            var convertedResponse = JsonConvert.DeserializeObject<ResidentInformationList>(stringContent);

            convertedResponse.Residents.Should().ContainEquivalentOf(expectedResidentResponseOne);
            convertedResponse.Residents.Should().ContainEquivalentOf(expectedResidentResponseTwo);
            convertedResponse.Residents.Should().ContainEquivalentOf(expectedResidentResponseThree);
        }


        [Test]
        public async Task FirstNameLastNameQueryParametersReturnsMatchingResidentRecordsFromUH()
        {
            var expectedResidentResponseOne = E2ETestHelpers.AddPersonWithRelatedEntitiesToDb(UHContext, firstname: "ciasom", lastname: "tessellate");
            var expectedResidentResponseTwo = E2ETestHelpers.AddPersonWithRelatedEntitiesToDb(UHContext, firstname: "ciasom", lastname: "shape");
            var expectedResidentResponseThree = E2ETestHelpers.AddPersonWithRelatedEntitiesToDb(UHContext);

            var uri = new Uri("api/v1/households?firstname=ciasom&lastname=tessellate", UriKind.Relative);
            var response = Client.GetAsync(uri);

            var statusCode = response.Result.StatusCode;
            statusCode.Should().Be(200);

            var content = response.Result.Content;
            var stringContent = await content.ReadAsStringAsync().ConfigureAwait(true);
            var convertedResponse = JsonConvert.DeserializeObject<ResidentInformationList>(stringContent);

            convertedResponse.Residents.Count.Should().Be(1);
            convertedResponse.Residents.Should().ContainEquivalentOf(expectedResidentResponseOne);
        }

        [Test]
        public async Task AddressQueryParametersReturnsMatchingResidentsRecordsFromUH()
        {
            var matchingResidentOne = E2ETestHelpers.AddPersonWithRelatedEntitiesToDb(UHContext, addressLines: "1 Seasame street, Hackney, LDN");
            var matchingResidentTwo = E2ETestHelpers.AddPersonWithRelatedEntitiesToDb(UHContext, addressLines: "1 Seasame street");
            var nonMatchingResident1 = E2ETestHelpers.AddPersonWithRelatedEntitiesToDb(UHContext);
            var nonMatchingResident2 = E2ETestHelpers.AddPersonWithRelatedEntitiesToDb(UHContext, addressLines: "2 Seasame street, Hackney, LDN");
            var nonMatchingResident3 = E2ETestHelpers.AddPersonWithRelatedEntitiesToDb(UHContext);

            var uri = new Uri("api/v1/households?address=1 Seasame street", UriKind.Relative);
            var response = Client.GetAsync(uri);

            var statusCode = response.Result.StatusCode;
            statusCode.Should().Be(200);

            var content = response.Result.Content;
            var stringContent = await content.ReadAsStringAsync().ConfigureAwait(true);
            var convertedResponse = JsonConvert.DeserializeObject<ResidentInformationList>(stringContent);

            convertedResponse.Residents.Count.Should().Be(2);
            convertedResponse.Residents.Should().ContainEquivalentOf(matchingResidentOne);
            convertedResponse.Residents.Should().ContainEquivalentOf(matchingResidentTwo);
        }

        [Test]
        public async Task UsingAllQueryParametersReturnsMatchingResidentsRecordsFromUH()
        {
            var matchingResidentOne = E2ETestHelpers.AddPersonWithRelatedEntitiesToDb(UHContext,
                addressLines: "1 Seasame street, Hackney, LDN", firstname: "ciasom", lastname: "shape");
            var nonmatchingResidentTwo = E2ETestHelpers.AddPersonWithRelatedEntitiesToDb(UHContext, addressLines: "1 Seasame street", lastname: "shap");
            var nonMatchingResident1 = E2ETestHelpers.AddPersonWithRelatedEntitiesToDb(UHContext, firstname: "ciasom");
            var nonMatchingResident2 = E2ETestHelpers.AddPersonWithRelatedEntitiesToDb(UHContext, addressLines: "1 Seasame street, Hackney, LDN");
            var nonMatchingResident3 = E2ETestHelpers.AddPersonWithRelatedEntitiesToDb(UHContext);

            var uri = new Uri("api/v1/households?address=1 Seasame street&firstname=ciasom&lastname=shape", UriKind.Relative);
            var response = Client.GetAsync(uri);

            var statusCode = response.Result.StatusCode;
            statusCode.Should().Be(200);

            var content = response.Result.Content;
            var stringContent = await content.ReadAsStringAsync().ConfigureAwait(true);
            var convertedResponse = JsonConvert.DeserializeObject<ResidentInformationList>(stringContent);

            convertedResponse.Residents.Count.Should().Be(1);
            convertedResponse.Residents.Should().ContainEquivalentOf(matchingResidentOne);
        }

        [Test]
        public async Task UsingQueryParametersReturnsAPaginatedResponse()
        {
            var matchingResidentOne = E2ETestHelpers.AddPersonWithRelatedEntitiesToDb(UHContext, firstname: "ciasom", lastname: "shape");
            var nonmatchingResidentTwo = E2ETestHelpers.AddPersonWithRelatedEntitiesToDb(UHContext, lastname: "shap");
            var matchingResident5 = E2ETestHelpers.AddPersonWithRelatedEntitiesToDb(UHContext);
            var matchingResident4 = E2ETestHelpers.AddPersonWithRelatedEntitiesToDb(UHContext);
            var nonMatchingResident3 = E2ETestHelpers.AddPersonWithRelatedEntitiesToDb(UHContext);

            var uri = new Uri("api/v1/households?cursor=2&limit=2", UriKind.Relative);
            var response = Client.GetAsync(uri);

            var statusCode = response.Result.StatusCode;
            statusCode.Should().Be(200);

            var content = response.Result.Content;
            var stringContent = await content.ReadAsStringAsync().ConfigureAwait(true);
            var convertedResponse = JsonConvert.DeserializeObject<ResidentInformationList>(stringContent);

            convertedResponse.Residents.Count.Should().Be(2);
            convertedResponse.Residents.Should().ContainEquivalentOf(matchingResident5);
            convertedResponse.Residents.Should().ContainEquivalentOf(matchingResident4);
        }
    }
}
