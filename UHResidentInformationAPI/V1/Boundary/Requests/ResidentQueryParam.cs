using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace UHResidentInformationAPI.V1.Boundary.Requests
{
    public class ResidentQueryParam
    {

        [FromQuery(Name = "houseReference")]
        public string HouseReference { get; set; }

        [FromQuery(Name = "residentName")]
        public string ResidentName { get; set; }

        [FromQuery(Name = "firstName")]
        public string FirstName { get; set; }

        [FromQuery(Name = "lastName")]  
        public string LastName { get; set; }
        
        [FromQuery(Name = "address")]
        public string Address { get; set; }
    }
}
