using EligibilityScoring.Application.DTOs;
using EligibilityScoring.Application.Interfaces;
using EligibilityScoringService.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EligibilityScoringService.Controllers
{
    [ApiController]
    [Route("api/v1/eligibility")]
    public class EligibilityController : ControllerBase
    {
        private readonly IEligibilityService _eligibilityService;

        public EligibilityController(IEligibilityService eligibilityService)
        {
            _eligibilityService = eligibilityService;
        }

        [HttpPost("evaluate/{customerId}")]
        public async Task<IActionResult> EvaluateByCustomerId(int customerId)
        {
            var result = await _eligibilityService.EvaluateByCustomerIdAsync(customerId);
            return Ok(ApiResponse<EligibilityDto>.SuccessResponse(result, "Eligibility evaluated successfully using Customer ID."));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var result = await _eligibilityService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<EligibilityDto>>.SuccessResponse(result, "All eligibility records retrieved successfully."));
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            var result = await _eligibilityService.GetByCustomerIdAsync(customerId);
            if (result == null)
                return NotFound(ApiResponse<EligibilityDto>.FailureResponse("Eligibility record not found for this customer."));

            return Ok(ApiResponse<EligibilityDto>.SuccessResponse(result));
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _eligibilityService.DeleteAsync(id);
            if (!success)
                return NotFound(ApiResponse<bool>.FailureResponse("Record not found."));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Record deleted successfully."));
        }
    }
}
