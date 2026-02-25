using EligibilityScoring.Application.DTOs;
using EligibilityScoring.Application.Interfaces;
using EligibilityScoring.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EligibilityScoring.Application.Services
{
    public class EligibilityService(ICreditReportingClient creditReportingClient, IApplicationDbContext dbContext, ILogger<EligibilityService> logger) : IEligibilityService
    {
        private readonly ICreditReportingClient _creditReportingClient = creditReportingClient;
        private readonly IApplicationDbContext _dbContext = dbContext;
        private readonly ILogger<EligibilityService> _logger = logger;

        public async Task<EligibilityDto> EvaluateEligibilityAsync(EligibilityRequest request)
        {
            _logger.LogInformation("Evaluating eligibility for customer {CustomerId}", request.CustomerId);
            var cibilScore = await _creditReportingClient.GetCibilScoreAsync(request.CustomerId, request.PanNo);
            return await SaveAndMapEligibility(request.CustomerId, cibilScore);
        }

        public async Task<EligibilityDto> EvaluateByCustomerIdAsync(int customerId)
        {
            _logger.LogInformation("Evaluating eligibility by ID for customer {CustomerId}", customerId);
            var cibilScore = await _creditReportingClient.GetCibilScoreByCustomerIdAsync(customerId);
            return await SaveAndMapEligibility(customerId, cibilScore);
        }

        private async Task<EligibilityDto> SaveAndMapEligibility(int customerId, int cibilScore)
        {
            bool isEligible = cibilScore >= 500;
            var eligibility = new Eligibility
            {
                CustomerId = customerId,
                IsEligible = isEligible,
                RejectionReason = isEligible ? "" : $"CIBIL score {cibilScore} is below 500",
                EvaluatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            };

            await _dbContext.Eligibilities.AddAsync(eligibility);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Eligibility evaluated for {CustomerId}. Eligible: {IsEligible}", customerId, isEligible);

            return MapToDto(eligibility);
        }

        public async Task<EligibilityDto?> GetByCustomerIdAsync(int customerId)
        {
            _logger.LogInformation("Fetching eligibility for customer {CustomerId}", customerId);
            var eligibility = await _dbContext.Eligibilities
                .FirstOrDefaultAsync(e => e.CustomerId == customerId && !e.IsDeleted);
            
            return eligibility == null ? null : MapToDto(eligibility);
        }

        public async Task<IEnumerable<EligibilityDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all eligibility records");
            return await _dbContext.Eligibilities
                .Where(e => !e.IsDeleted)
                .Select(e => MapToDto(e))
                .ToListAsync();
        }

        public async Task<PaginatedList<EligibilityDto>> GetAllPagedAsync(int pageIndex, int pageSize)
        {
            _logger.LogInformation("Fetching paginated eligibilities. Page: {PageIndex}, Size: {PageSize}", pageIndex, pageSize);
            
            var query = _dbContext.Eligibilities.Where(e => !e.IsDeleted);
            var count = await query.CountAsync();
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(e => MapToDto(e))
                .ToListAsync();

            return new PaginatedList<EligibilityDto>(items, count, pageIndex, pageSize);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogWarning("Deleting eligibility record {Id}", id);
            var eligibility = await _dbContext.Eligibilities
                .FirstOrDefaultAsync(e => e.EligibileId == id);
            if (eligibility == null) return false;

            eligibility.IsDeleted = true;
            eligibility.DeletedAt = DateTime.UtcNow;
            eligibility.DeletedBy = "Admin";

            await _dbContext.SaveChangesAsync();
            return true;
        }

        private static EligibilityDto MapToDto(Eligibility eligibility)
        {
            return new EligibilityDto
            {
                EligibileId = eligibility.EligibileId,
                CustomerId = eligibility.CustomerId,
                IsEligible = eligibility.IsEligible,
                RejectionReason = eligibility.RejectionReason,
                EvaluatedDate = eligibility.EvaluatedDate
            };
        }
    }
}
