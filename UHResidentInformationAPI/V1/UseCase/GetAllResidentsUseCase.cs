using UHResidentInformationAPI.V1.Boundary.Responses;
using UHResidentInformationAPI.V1.Gateways;
using UHResidentInformationAPI.V1.UseCase.Interfaces;

namespace UHResidentInformationAPI.V1.UseCase
{
    public class GetAllResidentsUseCase : IGetAllResidentsUseCase
    {
        private IUHGateway _UHGateway;
        public GetAllResidentsUseCase(IUHGateway UHGateway)
        {
            _UHGateway = UHGateway;
        }

        
       
    }
}