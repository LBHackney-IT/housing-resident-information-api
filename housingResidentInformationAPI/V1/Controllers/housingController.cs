using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using housingResidentInformationAPI.V1.Boundary.Requests;
using housingResidentInformationAPI.V1.Boundary.Responses;
using housingResidentInformationAPI.V1.Domain;
using housingResidentInformationAPI.V1.UseCase.Interfaces;

namespace housingResidentInformationAPI.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/households")]
    [Produces("application/json")]
    public class housingController : BaseController
    {
        private IGetAllResidentsUseCase _getAllResidentsUseCase;
        private IGetResidentByIdUseCase _getResidentByIdUseCase;
        public housingController(IGetAllResidentsUseCase getAllResidentsUseCase, IGetResidentByIdUseCase getResidentByIdUseCase)
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
        public IActionResult ListRecords([FromQuery] ResidentQueryParam rqp, string cursor = null, int? limit = 20)
        {
            try
            {
                return Ok(_getAllResidentsUseCase.Execute(rqp, cursor, (int) limit));
            }
            catch (InvalidQueryParameterException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("{houseReference}/people/{personReference}")]
        public IActionResult ViewRecord(string houseReference, int personReference)
        {
            try
            {
                var record = _getResidentByIdUseCase.Execute(houseReference, personReference);
                return Ok(record);
            }
            catch (ResidentNotFoundException)
            {
                return NotFound();
            }
        }
    };
}
