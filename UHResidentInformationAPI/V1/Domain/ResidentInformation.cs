using System;
using System.Collections.Generic;
using UHResidentInformationAPI.V1.Enums;

namespace UHResidentInformationAPI.V1.Domain
{
    public class ResidentInformation
    {
        public string HouseReference { get; set; }
        public int PersonNumber { get; set; }
        public string TenancyReference { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string NINumber { get; set; }
        public string UPRN { get; set; }
        public List<Phone> PhoneNumber { get; set; }
        public List<Email> Email { get; set; }
        public Address Address { get; set; }
    }

    public class Address
    {
        public string PropertyRef { get; set; }
        public string AddressLine1 { get; set; }
        public string PostCode { get; set; }
    }

    public class Phone
    {
        public string PhoneNumber { get; set; }
        public PhoneType? Type { get; set; }
        public DateTime LastModified { get; set; }
    }

    public class Email
    {
        public string EmailAddress { get; set; }
        public DateTime LastModified { get; set; }
    }
}
