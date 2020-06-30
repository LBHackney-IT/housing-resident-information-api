using UHResidentInformationAPI.V1.Domain;
using UHResidentInformationAPI.V1.Factories;
using UHResidentInformationAPI.V1.Gateways;
using UHResidentInformationAPI.V1.UseCase.Interfaces;
using ResidentInformation = UHResidentInformationAPI.V1.Boundary.Responses.ResidentInformation;

namespace UHResidentInformationAPI.V1.UseCase
{
    public class GetResidentByIdUseCase : IGetResidentByIdUseCase
    {
        private readonly IUHGateway _uhGateway;

        public GetResidentByIdUseCase(IUHGateway uhGateway)
        {
            _uhGateway = uhGateway;
        }

        public ResidentInformation Execute(string houseRef, int personRef)
        {
            var residentInfo = _uhGateway.GetResidentById(houseRef, personRef);
            if (residentInfo == null) throw new ResidentNotFoundException();
            return residentInfo.ToResponse();
        }
    }
}
