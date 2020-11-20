using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HousingResidentInformationAPI.V1.Boundary.Requests
{
    public class ResidentQueryParam
    {

        [FromQuery(Name = "house_reference")]
        public string HouseReference { get; set; }

        [FromQuery(Name = "first_name")]
        public string FirstName { get; set; }

        [FromQuery(Name = "last_name")]
        public string LastName { get; set; }

        [FromQuery(Name = "address")]
        public string Address { get; set; }

        [FromQuery(Name = "active_tenancies_only")]
        public bool ActiveTenanciesOnly { get; set; }
    }
}
