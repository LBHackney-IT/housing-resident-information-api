using System.Threading.Tasks;
using HousingResidentInformationAPI.V1.Boundary.Requests;
using HousingResidentInformationAPI.V1.Boundary.Responses;

namespace HousingResidentInformationAPI.V1.UseCase.Interfaces
{
    public interface IGetAllResidentsUseCase
    {
        Task<ResidentInformationList> Execute(ResidentQueryParam rqp, string cursor, int limit);
    }
}
