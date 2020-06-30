using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace UHResidentInformationAPI.V1.Boundary.Requests
{
    public class ResidentQueryParam
    {

        [FromQuery(Name = "housereference")]
        public string HouseReference { get; set; }

        [FromQuery(Name = "residentName")]
        public string ResidentName { get; set; }

        [FromQuery(Name = "firstname")]
        public string FirstName { get; set; }

        [FromQuery(Name = "lastname")]
        public string LastName { get; set; }

        [FromQuery(Name = "address")]
        public string Address { get; set; }
    }
}
