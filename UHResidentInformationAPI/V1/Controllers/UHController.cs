using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UHResidentInformationAPI.V1.Boundary.Requests;
using UHResidentInformationAPI.V1.Boundary.Responses;
using UHResidentInformationAPI.V1.UseCase.Interfaces;

namespace UHResidentInformationAPI.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/households")]
    [Produces("application/json")]
    public class UHController : BaseController
    {
        private IGetAllResidentsUseCase _getAllResidentsUseCase;
        private IGetResidentByIdUseCase _getResidentByIdUseCase;
        public UHController(IGetAllResidentsUseCase getAllResidentsUseCase, IGetResidentByIdUseCase getResidentByIdUseCase)
        {
            _getAllResidentsUseCase = getAllResidentsUseCase;
            _getResidentByIdUseCase = getResidentByIdUseCase;

        }
        /// <summary>
        /// Returns list of contacts who share the query search parameter
        /// </summary>
        /// <response code="200">Success. Returns a list of matching residents information</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(ResidentInformationList), StatusCodes.Status200OK)]
        [HttpGet]

        [HttpGet]
        public IActionResult ListRecords([FromQuery] ResidentQueryParam rqp)
        {
            return Ok(_getAllResidentsUseCase.Execute(rqp));
        }

        [HttpGet]
        [Route("/households/{houseReference}/people/{personReference}")]
        public IActionResult ViewRecord(string houseReference, int personReference)
        {
            return Ok(_getResidentByIdUseCase.Execute(houseReference, personReference));
        }
    };
}
