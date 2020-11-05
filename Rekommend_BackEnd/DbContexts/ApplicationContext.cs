using Microsoft.EntityFrameworkCore;
using Rekommend_BackEnd.Entities;

namespace Rekommend_BackEnd.DbContexts
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<TechJobOpening> TechJobOpenings { get; set; }
        public DbSet<Recruiter> Recruiters { get; set; }
        public DbSet<RekoHistory> RekoHistories { get; set; }
        public DbSet<Rekommendation> Rekommendations { get; set; }
        public DbSet<Rekommender> Rekommenders { get; set; }
    }
}
