using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UHResidentInformationAPI.V1.Boundary.Responses;
using Address = UHResidentInformationAPI.V1.Domain.Address;
using AddressResponse = UHResidentInformationAPI.V1.Boundary.Responses.Address;
using ResidentInformation = UHResidentInformationAPI.V1.Domain.ResidentInformation;
using ResidentInformationResponse = UHResidentInformationAPI.V1.Boundary.Responses.ResidentInformation;

namespace UHResidentInformationAPI.V1.Factories
{
    public static class ResponseFactory
    {
        //Convert application domain objects to response object
        public static ResidentInformationResponse ToResponse(this ResidentInformation domain)
        {
            return new ResidentInformationResponse
            {
                HouseReference = domain.HouseReference,
                PersonNumber = domain.PersonNumber,
                FirstName = domain.FirstName,
                LastName = domain.LastName,
                DateOfBirth = domain.DateOfBirth,
                NINumber = domain.NINumber,
                PhoneNumber = domain.PhoneNumberList.ToResponse(),
                Email = domain.EmailList.ToResponse(),
                Address = domain.Address.ToResponse()
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
                PhoneType = number.Type,
                LastModified = number.LastModified.ToString("O", CultureInfo.InvariantCulture)
            }).ToList();
        }

        private static List<Email> ToResponse(this List<Domain.Email> emailAddresses)
        {
            return emailAddresses.Select(email => new Email()
            {
                EmailAddress = email.EmailAddress,
                LastModified = email.LastModified.ToString("O", CultureInfo.InvariantCulture)
            }).ToList();
        }

        private static AddressResponse ToResponse(this Address address)
        {
            if (address == null) return new AddressResponse();
            return new AddressResponse
            {
                PropertyRef = address.PropertyRef,
                AddressLine1 = address.AddressLine1,
                Postcode = address.Postcode
            };
        }
    }
}
