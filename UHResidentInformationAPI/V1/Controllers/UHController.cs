using Microsoft.AspNetCore.Mvc;

namespace UHResidentInformationAPI.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/resident-housing-information")]
    [Produces("application/json")]
    public class UHController : BaseController
    {
        // Declare private Use Case fields

        public UHController()
        {
            //Pass in relevant Use Cases as part of the constructor
        }

        [HttpGet]
        public IActionResult ListRecords()
        {
            return null;
        }

        [HttpGet]
        // [Route()]
        // Add route for view endpoint
        public IActionResult ViewRecord()
        {
            return null;
        }
    };
}
