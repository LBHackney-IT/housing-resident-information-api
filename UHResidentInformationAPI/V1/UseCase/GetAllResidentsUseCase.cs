using UHResidentInformationAPI.V1.Boundary.Requests;
using UHResidentInformationAPI.V1.Boundary.Responses;
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

        public ResidentInformationList Execute(ResidentQueryParam rqp)
        {
            return new ResidentInformationList();
        }


    }
}
