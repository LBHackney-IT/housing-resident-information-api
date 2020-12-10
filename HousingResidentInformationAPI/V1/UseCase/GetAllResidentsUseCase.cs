using System.Linq;
using HousingResidentInformationAPI.V1.Boundary.Requests;
using HousingResidentInformationAPI.V1.Boundary.Responses;
using HousingResidentInformationAPI.V1.Domain;
using HousingResidentInformationAPI.V1.Factories;
using HousingResidentInformationAPI.V1.Gateways;
using HousingResidentInformationAPI.V1.UseCase.Interfaces;

namespace HousingResidentInformationAPI.V1.UseCase
{
    public class GetAllResidentsUseCase : IGetAllResidentsUseCase
    {
        private IHousingGateway _housingGateway;
        private readonly IValidatePostcode _validatePostcode;
        public GetAllResidentsUseCase(IHousingGateway housingGateway, IValidatePostcode validatePostcode)
        {
            _housingGateway = housingGateway;
            _validatePostcode = validatePostcode;
        }

        public ResidentInformationList Execute(ResidentQueryParam rqp, string cursor, int limit)
        {
            limit = limit < 10 ? 10 : limit;
            limit = limit > 100 ? 100 : limit;

            CheckPostcodeValid(rqp.Postcode);

            var residents = _housingGateway.GetAllResidents(cursor, limit, rqp.HouseReference, rqp.FirstName,
                rqp.LastName, rqp.Address, rqp.Postcode, rqp.ActiveTenanciesOnly).ToResponse();
            var lastResident = residents.LastOrDefault();
            var nextCursor = residents.Count == limit ? $"{lastResident.HouseReference}{lastResident.PersonNumber}" : "";

            return new ResidentInformationList
            {
                Residents = residents,
                NextCursor = nextCursor
            };
        }

        private void CheckPostcodeValid(string postcode)
        {
            if (string.IsNullOrEmpty(postcode)) return;
            if (!_validatePostcode.Execute(postcode))
                throw new InvalidQueryParameterException("The Postcode given does not have a valid format");
        }

    }
}
