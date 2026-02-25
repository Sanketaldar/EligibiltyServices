using EligibilityScoring.Application.DTOs;
using EligibilityScoring.Application.Interfaces;
using EligibilityScoringService.Response;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EligibilityScoringService.Controllers
{
    [ApiController]
    [Route("api/v1/scorecard")]
    public class ScorecardController : ControllerBase
    {
        private readonly IScorecardService _scorecardService;

        public ScorecardController(IScorecardService scorecardService)
        {


            _scorecardService = scorecardService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] ScorecardRequest request)
        {
            if (request == null)
                return BadRequest(ApiResponse<ScorecardDto>.FailureResponse("Request data is required."));

            var result = await _scorecardService.GenerateScorecardAsync(request);
            return Ok(ApiResponse<ScorecardDto>.SuccessResponse(result, "Scorecard generated successfully."));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _scorecardService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<ScorecardDto>>.SuccessResponse(result, "All scorecards retrieved successfully."));
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            var result = await _scorecardService.GetByCustomerIdAsync(customerId);
            if (result == null)
                return NotFound(ApiResponse<ScorecardDto>.FailureResponse("Scorecard not found."));

            return Ok(ApiResponse<ScorecardDto>.SuccessResponse(result));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _scorecardService.DeleteAsync(id);
            if (!success)
                return NotFound(ApiResponse<bool>.FailureResponse("Record not found."));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Record deleted successfully."));
        }
    }
}
