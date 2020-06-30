using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UHResidentInformationAPI.V1.Boundary.Requests;
using UHResidentInformationAPI.V1.Boundary.Responses;
using UHResidentInformationAPI.V1.Domain;
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
        private IGetEntityByIdUseCase _getEntityByIdUseCase;
        public UHController(IGetAllResidentsUseCase getAllResidentsUseCase, IGetEntityByIdUseCase getEntityByIdUseCase)
        {
            _getAllResidentsUseCase = getAllResidentsUseCase;
            _getEntityByIdUseCase = getEntityByIdUseCase;
        }
        /// <summary>
        /// Returns list of contacts who share the query search parameter
        /// </summary>
        /// <response code="200">Success. Returns a list of matching residents information</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(ResidentInformationList), StatusCodes.Status200OK)]

        [HttpGet]
        public IActionResult ListRecords([FromQuery] ResidentQueryParam rqp)
        {
            try
            {
                return Ok(_getAllResidentsUseCase.Execute(rqp));
            }
            catch (InvalidQueryParameterException e)
            {
                return BadRequest(e.Message);
            }
            // return Ok(_getAllResidentsUseCase.Execute(rqp));
        }

        [HttpGet]
        [Route("/people")]
        // Add route for view endpoint
        public IActionResult ViewRecord()
        {
            return null;
        }
    };
}
