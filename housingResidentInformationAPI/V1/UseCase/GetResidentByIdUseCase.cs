using housingResidentInformationAPI.V1.Domain;
using housingResidentInformationAPI.V1.Factories;
using housingResidentInformationAPI.V1.Gateways;
using housingResidentInformationAPI.V1.UseCase.Interfaces;
using ResidentInformation = housingResidentInformationAPI.V1.Boundary.Responses.ResidentInformation;

namespace housingResidentInformationAPI.V1.UseCase
{
    public class GetResidentByIdUseCase : IGetResidentByIdUseCase
    {
        private readonly IHousingGateway _housingGateway;

        public GetResidentByIdUseCase(IHousingGateway housingGateway)
        {
            _housingGateway = housingGateway;
        }

        public ResidentInformation Execute(string houseRef, int personRef)
        {
            var residentInfo = _housingGateway.GetResidentById(houseRef, personRef);
            if (residentInfo == null) throw new ResidentNotFoundException();
            return residentInfo.ToResponse();
        }
    }
}
