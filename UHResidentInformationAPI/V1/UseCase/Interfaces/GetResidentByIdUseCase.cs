using UHResidentInformationAPI.V1.Boundary.Responses;

namespace UHResidentInformationAPI.V1.UseCase.Interfaces
{
    public interface IGetResidentByIdUseCase
    {
        ResidentInformation Execute(string houseRef, int personRef);
    }
}
