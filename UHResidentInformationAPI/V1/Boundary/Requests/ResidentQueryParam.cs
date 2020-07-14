using Microsoft.AspNetCore.Mvc;

namespace UHResidentInformationAPI.V1.Boundary.Requests
{
    public class ResidentQueryParam
    {

        [FromQuery(Name = "housereference")]
        public string HouseReference { get; set; }

        [FromQuery(Name = "firstname")]
        public string FirstName { get; set; }

        [FromQuery(Name = "lastname")]
        public string LastName { get; set; }

        [FromQuery(Name = "addressLine1")]
        public string AddressLine1 { get; set; }

        [FromQuery(Name = "postcode")]
        public string Postcode { get; set; }
    }
}
