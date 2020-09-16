using housingResidentInformationAPI.V1.Boundary.Responses;

namespace housingResidentInformationAPI.V1.UseCase.Interfaces
{
    public interface IGetResidentByIdUseCase
    {
        ResidentInformation Execute(string houseRef, int personRef);
    }
}
