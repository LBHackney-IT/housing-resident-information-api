using System;
using UHResidentInformationAPI.V1.Boundary.Requests;
using UHResidentInformationAPI.V1.Boundary.Responses;
using UHResidentInformationAPI.V1.Factories;
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
            Console.WriteLine("Entering List Endpoint UseCase");
            var residents = _uHGateway.GetAllResidents(rqp.HouseReference, rqp.FirstName, rqp.LastName, rqp.Address).ToResponse();

            Console.WriteLine("Leaving List Endpoint UseCase");
            return new ResidentInformationList
            {
                Residents = residents
            };
        }

    }
}
