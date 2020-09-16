using System.Linq;
using housingResidentInformationAPI.V1.Boundary.Requests;
using housingResidentInformationAPI.V1.Boundary.Responses;
using housingResidentInformationAPI.V1.Factories;
using housingResidentInformationAPI.V1.Gateways;
using housingResidentInformationAPI.V1.UseCase.Interfaces;

namespace housingResidentInformationAPI.V1.UseCase
{
    public class GetAllResidentsUseCase : IGetAllResidentsUseCase
    {
        private IHousingGateway _housingGateway;
        public GetAllResidentsUseCase(IHousingGateway housingGateway)
        {
            _housingGateway = housingGateway;
        }

        public ResidentInformationList Execute(ResidentQueryParam rqp, string cursor, int limit)
        {
            limit = limit < 10 ? 10 : limit;
            limit = limit > 100 ? 100 : limit;

            var residents = _housingGateway.GetAllResidents(cursor, limit, rqp.HouseReference, rqp.FirstName, rqp.LastName, rqp.Address).ToResponse();
            var lastResident = residents.LastOrDefault();
            var nextCursor = residents.Count == limit ? $"{lastResident.HouseReference}{lastResident.PersonNumber}" : "";

            return new ResidentInformationList
            {
                Residents = residents,
                NextCursor = nextCursor
            };
        }

    }
}
