using HousingResidentInformationAPI.V1.Boundary.Responses;

namespace HousingResidentInformationAPI.V1.UseCase.Interfaces
{
    public interface IGetResidentByIdUseCase
    {
        ResidentInformation Execute(string houseRef, int personRef);
    }
}
