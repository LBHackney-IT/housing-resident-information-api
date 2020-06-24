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
                UPRN = domain.UPRN,
                FirstName = domain.FirstName,
                LastName = domain.LastName,
                DateOfBirth = domain.DateOfBirth,
                ResidentAddress = domain.ResidentAddress.ToResponse(),
                PhoneNumber = domain.PhoneNumber?.ToResponse() ?? null,
                Email = domain.Email?.ToResponse() ?? null
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
                AddressLine1 = address.AddressLine1,
                PostCode = address.PostCode
            };
        }
        private static List<Email> ToResponse(this List<Domain.Email> emailAddresses)
        {
            return emailAddresses.Select(email => new Email
            {
                EmailAddress = email.EmailAddress,
                LastModified = email.LastModified.ToString("O")
            }).ToList();
        }

    }
}
