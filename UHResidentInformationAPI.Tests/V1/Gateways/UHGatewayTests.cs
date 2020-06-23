using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using UHResidentInformationAPI.Tests.V1.Helper;
using UHResidentInformationAPI.V1.Domain;
using UHResidentInformationAPI.V1.Gateways;

namespace UHResidentInformationAPI.Tests.V1.Gateways
{
    [TestFixture]
    public class UHGatewayTests : DatabaseTests
    {
        //private Fixture _fixture = new Fixture();
        private UHGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new UHGateway(UHContext);
        }

        // [Test]
        // public void GatewayImplementsBoundaryInterface()
        // {
        //     Assert.NotNull(_classUnderTest is IUHGateway);
        // }

        // [Test]
        // public void GetEntityByIdReturnsEmptyArray()
        // {
        //     var response = _classUnderTest.GetEntityById(123);

        //     response.Should().BeNull();
        // }

        // [Test]
        // public void GetEntityByIdReturnsCorrectResponse()
        // {
        //     var entity = _fixture.Create<Entity>();
        //     var databaseEntity = DatabaseEntityHelper.CreateDatabaseEntityFrom(entity);

        //     DatabaseContext.DatabaseEntities.Add(databaseEntity);
        //     DatabaseContext.SaveChanges();

        //     var response = _classUnderTest.GetEntityById(databaseEntity.Id);

        //     databaseEntity.Id.Should().Be(response.Id);
        //     databaseEntity.CreatedAt.Should().BeSameDateAs(response.CreatedAt);
        // }
    }
}
