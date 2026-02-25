using EligibilityScoring.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EligibilityScoring.Application.Interfaces
{
    public interface IScorecardService
    {
        Task<ScorecardDto> GenerateScorecardAsync(ScorecardRequest request);
        Task<ScorecardDto?> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<ScorecardDto>> GetAllAsync();
        Task<bool> DeleteAsync(int id);
    }
}
