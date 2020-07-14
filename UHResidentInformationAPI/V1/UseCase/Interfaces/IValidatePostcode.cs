namespace UHResidentInformationAPI.V1.UseCase.Interfaces
{
    public interface IValidatePostcode
    {
        bool Execute(string postcode);
    }
}
