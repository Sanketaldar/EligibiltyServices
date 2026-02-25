using EligibilityScoring.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace EligibilityScoring.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Eligibility> Eligibilities { get; set; }
        DbSet<Scorecard> Scorecards { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
