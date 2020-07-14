using UHResidentInformationAPI.V1.Boundary.Requests;
using UHResidentInformationAPI.V1.Boundary.Responses;
using UHResidentInformationAPI.V1.Domain;
using UHResidentInformationAPI.V1.Factories;
using UHResidentInformationAPI.V1.Gateways;
using UHResidentInformationAPI.V1.UseCase.Interfaces;

namespace UHResidentInformationAPI.V1.UseCase
{
    public class GetAllResidentsUseCase : IGetAllResidentsUseCase
    {
        private IUHGateway _uHGateway;
        private IValidatePostcode _validatePostcode;
        public GetAllResidentsUseCase(IUHGateway uHGateway)
        {
            _uHGateway = uHGateway;
            _validatePostcode = new ValidatePostcode();
        }

        public ResidentInformationList Execute(ResidentQueryParam rqp, int cursor, int limit)
        {
            limit = limit < 10 ? 10 : limit;
            limit = limit > 100 ? 100 : limit;

            var residents = _uHGateway
                .GetAllResidents(cursor, limit, rqp.HouseReference, rqp.FirstName, rqp.LastName, rqp.AddressLine1, rqp.Postcode);

            return new ResidentInformationList
            {
                Residents = residents.ToResponse()
            };
        }

        private void CheckPostcodeValid(string postcode)
        {
            var validPostcode = _validatePostcode.Execute(postcode);
            if (!validPostcode)
                throw new InvalidQueryParameterException("The Postcode given does not have a valid format");
        }
    }
}
