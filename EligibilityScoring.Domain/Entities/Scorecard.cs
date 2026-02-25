using System;

namespace EligibilityScoring.Domain.Entities
{
    public class Scorecard : BaseEntity
    {
        public int ScoreId { get; set; }
        public int CustomerId { get; set; }
        public int CibilScore { get; set; }
        public decimal MonthlyIncome { get; set; }
        public string EmploymentType { get; set; } = string.Empty;
        public int Age { get; set; }
        public decimal ExistingObligation { get; set; }
       
        public int CalculationScore { get; set; }
        public decimal EligibleLoanAmount { get; set; }
        public string RiskCategory { get; set; } = string.Empty; // low, medium, high
        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;
    }
}
