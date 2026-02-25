using System;

namespace EligibilityScoring.Application.DTOs
{
    public class EligibilityRequest
    {
        public int CustomerId { get; set; }
        public string PanNo { get; set; } = string.Empty;
    }

    public class EligibilityDto
    {
        public int EligibileId { get; set; }
        public int CustomerId { get; set; }
        public bool IsEligible { get; set; }
        public string RejectionReason { get; set; } = string.Empty;
        public DateTime EvaluatedDate { get; set; }
    }

    public class ScorecardRequest
    {
        public int CustomerId { get; set; }
        public decimal MonthlyIncome { get; set; }
        public string EmploymentType { get; set; } = string.Empty;
        public int Age { get; set; }
        public decimal ExistingObligation { get; set; }
    }

    public class ScorecardDto
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
        public string RiskCategory { get; set; } = string.Empty;
        public DateTime GeneratedDate { get; set; }
    }

}
