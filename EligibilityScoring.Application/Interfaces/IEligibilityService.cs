using EligibilityScoring.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EligibilityScoring.Application.Interfaces
{
    public interface IEligibilityService
    {
        Task<EligibilityDto> EvaluateEligibilityAsync(EligibilityRequest request);
        Task<EligibilityDto> EvaluateByCustomerIdAsync(int customerId);
        Task<EligibilityDto?> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<EligibilityDto>> GetAllAsync();
        Task<PaginatedList<EligibilityDto>> GetAllPagedAsync(int pageIndex, int pageSize);
        Task<bool> DeleteAsync(int id);
    }
}
