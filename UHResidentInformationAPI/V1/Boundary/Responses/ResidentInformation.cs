using System.Collections.Generic;

namespace UHResidentInformationAPI.V1.Boundary.Responses
{
    public class ResidentInformation
    {
        public string HouseReference { get; set; }
        public int PersonNumber { get; set; }
        public string TenancyReference { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string NiNumber { get; set; }
        public string Uprn { get; set; }
        public List<Phone> PhoneNumbers { get; set; }
        public List<Email> EmailAddresses { get; set; }
        public Address Address { get; set; }
    }
}
