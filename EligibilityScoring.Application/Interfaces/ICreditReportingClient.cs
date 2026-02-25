using System.Threading.Tasks;

namespace EligibilityScoring.Application.Interfaces
{
    public interface ICreditReportingClient
    {
        Task<int> GetCibilScoreAsync(int customerId, string panNo);
        Task<int> GetCibilScoreByCustomerIdAsync(int customerId);
    }
}
