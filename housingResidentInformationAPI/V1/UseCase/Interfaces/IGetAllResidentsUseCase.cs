using housingResidentInformationAPI.V1.Boundary.Requests;
using housingResidentInformationAPI.V1.Boundary.Responses;

namespace housingResidentInformationAPI.V1.UseCase.Interfaces
{
    public interface IGetAllResidentsUseCase
    {
        ResidentInformationList Execute(ResidentQueryParam rqp, string cursor, int limit);
    }
}
