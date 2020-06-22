using System.Collections.Generic;

namespace UHResidentInformationAPI.V1.Boundary.Responses
{

    public class ResidentInformation
    {
        public int UPRN {get; set;}

        public string FirstName {get; set;}

        public string LastName {get; set;}

        public string DateOfBirth { get; set; }
        public List<Phone> PhoneNumber { get; set; }
        public List<Address> AddressList { get; set; }
    }
}