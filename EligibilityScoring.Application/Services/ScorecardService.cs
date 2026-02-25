using EligibilityScoring.Application.DTOs;
using EligibilityScoring.Application.Interfaces;
using EligibilityScoring.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EligibilityScoring.Application.Services
{
    public class ScorecardService(IApplicationDbContext dbContext, ICreditReportingClient creditReportingClient) : IScorecardService
    {
        private readonly IApplicationDbContext _dbContext = dbContext;
        private readonly ICreditReportingClient _creditReportingClient = creditReportingClient;

        public async Task<ScorecardDto> GenerateScorecardAsync(ScorecardRequest request)
        {
            int cibilScore = await _creditReportingClient.GetCibilScoreByCustomerIdAsync(request.CustomerId);

            int calculationScore = CalculateScore(cibilScore, request.MonthlyIncome, request.EmploymentType);
            decimal eligibleAmount = CalculateEligibleAmount(request.MonthlyIncome, request.ExistingObligation);
            string riskCategory = GetRiskCategory(calculationScore);

            var scorecard = new Scorecard
            {
                CustomerId = request.CustomerId,
                CibilScore = cibilScore,
                MonthlyIncome = request.MonthlyIncome,
                EmploymentType = request.EmploymentType,
                Age = request.Age,
                ExistingObligation = request.ExistingObligation,
              
                CalculationScore = calculationScore,
                EligibleLoanAmount = eligibleAmount,
                RiskCategory = riskCategory,
                GeneratedDate = DateTime.UtcNow,
                CreatedBy = "System"
            };

            await _dbContext.Scorecards.AddAsync(scorecard);
            await _dbContext.SaveChangesAsync();

            return MapToDto(scorecard);
        }

        public async Task<ScorecardDto?> GetByCustomerIdAsync(int customerId)
        {
            var scorecard = await _dbContext.Scorecards
                .FirstOrDefaultAsync(s => s.CustomerId == customerId && !s.IsDeleted);
            
            return scorecard == null ? null : MapToDto(scorecard);
        }

        public async Task<IEnumerable<ScorecardDto>> GetAllAsync()
        {
            return await _dbContext.Scorecards
                .Where(s => !s.IsDeleted)
                .Select(s => MapToDto(s))
                .ToListAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var scorecard = await _dbContext.Scorecards
                .FirstOrDefaultAsync(s => s.ScoreId == id);
            if (scorecard == null) return false;

            scorecard.IsDeleted = true;
            scorecard.DeletedAt = DateTime.UtcNow;
            scorecard.DeletedBy = "Admin";

            await _dbContext.SaveChangesAsync();
            return true;
        }

        private static int CalculateScore(int cibil, decimal income, string empType)
        {
            int score = (cibil / 10);
            if (income > 50000) score += 10;
            if (empType?.ToLower() == "salaried") score += 5;
            return score;
        }

        private static decimal CalculateEligibleAmount(decimal income, decimal obligation)
        {
            decimal available = (income * 0.5m) - obligation;
            return available > 0 ? available * 60 : 0;
        }

        private static string GetRiskCategory(int score)
        {
            if (score > 80) return "Low";
            if (score > 50) return "Medium";
            return "High";
        }

        private static ScorecardDto MapToDto(Scorecard scorecard)
        {
            return new ScorecardDto
            {
                ScoreId = scorecard.ScoreId,
                CustomerId = scorecard.CustomerId,
                CibilScore = scorecard.CibilScore,
                MonthlyIncome = scorecard.MonthlyIncome,
                EmploymentType = scorecard.EmploymentType,
                Age = scorecard.Age,
                ExistingObligation = scorecard.ExistingObligation,
               
                CalculationScore = scorecard.CalculationScore,
                EligibleLoanAmount = scorecard.EligibleLoanAmount,
                RiskCategory = scorecard.RiskCategory,
                GeneratedDate = scorecard.GeneratedDate
            };
        }
    }
}
