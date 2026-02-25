using System;

namespace EligibilityScoring.Domain.Entities
{
    public class Eligibility : BaseEntity
    {
        public int EligibileId { get; set; }
        public int CustomerId { get; set; }
        public bool IsEligible { get; set; }
        public string RejectionReason { get; set; } = string.Empty;
        public DateTime EvaluatedDate { get; set; } = DateTime.UtcNow;
    }
}
