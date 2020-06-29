using System.Collections.Generic;
using System.Linq;
using UHResidentInformationAPI.V1.Boundary.Responses;
using ResidentInformationResponse = UHResidentInformationAPI.V1.Boundary.Responses.ResidentInformation;
using ResidentInformation = UHResidentInformationAPI.V1.Domain.ResidentInformation;
using Address = UHResidentInformationAPI.V1.Domain.Address;
using AddressResponse = UHResidentInformationAPI.V1.Boundary.Responses.Address;

namespace UHResidentInformationAPI.V1.Factories
{
    public static class ResponseFactory
    {
        public static ResidentInformationResponse ToResponse(this ResidentInformation domain)
        {
            return new ResidentInformationResponse
            {
                FirstName = domain.FirstName,
                LastName = domain.LastName,
                DateOfBirth = domain.DateOfBirth,
                ResidentAddress = domain.ResidentAddress.ToResponse(),
                PhoneNumber = domain.PhoneNumber.Any() ? domain.PhoneNumber.ToResponse() : null
            };
        }
        public static List<ResidentInformationResponse> ToResponse(this IEnumerable<ResidentInformation> people)
        {
            return people.Select(p => p.ToResponse()).ToList();
        }

        private static List<Phone> ToResponse(this List<Domain.Phone> phoneNumbers)
        {
            return phoneNumbers.Select(number => new Phone
            {
                PhoneNumber = number.PhoneNumber,
                PhoneType = number.Type
            }).ToList();
        }

        private static AddressResponse ToResponse(this Address address)
        {
            return new AddressResponse
            {
                PropertyRef = address.PropertyRef,
                AddressLine1 = address.AddressLine1.Trim(),
                PostCode = address.PostCode
            };
        }
    }
}
