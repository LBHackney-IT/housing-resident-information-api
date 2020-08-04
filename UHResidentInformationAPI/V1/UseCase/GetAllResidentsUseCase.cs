using System.Linq;
using UHResidentInformationAPI.V1.Boundary.Requests;
using UHResidentInformationAPI.V1.Boundary.Responses;
using UHResidentInformationAPI.V1.Factories;
using UHResidentInformationAPI.V1.Gateways;
using UHResidentInformationAPI.V1.UseCase.Interfaces;

namespace UHResidentInformationAPI.V1.UseCase
{
    public class GetAllResidentsUseCase : IGetAllResidentsUseCase
    {
        private IUHGateway _uHGateway;
        public GetAllResidentsUseCase(IUHGateway uHGateway)
        {
            _uHGateway = uHGateway;
        }

        public ResidentInformationList Execute(ResidentQueryParam rqp, string cursor, int limit)
        {
            limit = limit < 10 ? 10 : limit;
            limit = limit > 100 ? 100 : limit;

            var residents = _uHGateway.GetAllResidents(cursor, limit, rqp.HouseReference, rqp.FirstName, rqp.LastName, rqp.Address).ToResponse();
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
