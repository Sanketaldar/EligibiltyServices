using EligibilityScoring.Application.Interfaces;
using EligibilityScoring.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EligibilityScoring.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Eligibility> Eligibilities { get; set; }
        public DbSet<Scorecard> Scorecards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Eligibility>(entity =>
            {
                entity.HasKey(e => e.EligibileId);
            });

            modelBuilder.Entity<Scorecard>(entity =>
            {
                entity.HasKey(e => e.ScoreId);
                entity.Property(e => e.MonthlyIncome).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ExistingObligation).HasColumnType("decimal(18,2)");
                entity.Property(e => e.EligibleLoanAmount).HasColumnType("decimal(18,2)");
            });
        }
    }
}
