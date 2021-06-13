using Microsoft.EntityFrameworkCore;
using Rekommend_BackEnd.Entities;
using Rekommend_BackEnd.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rekommend_BackEnd.DbContexts
{
    public class ApplicationContext : DbContext
    {
        private readonly IUserInfoService _userInfoService;

        public ApplicationContext(DbContextOptions<ApplicationContext> options, IUserInfoService userInfoService) : base(options)
        {
            // userInfoService is a required argument
            _userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<TechJobOpening> TechJobOpenings { get; set; }
        public DbSet<ExtendedUser> Recruiters { get; set; }
        public DbSet<Rekommendation> Rekommendations { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }

        // Not useful

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);

        //    builder.Entity<Rekommendation>(x => x.HasKey(aa => new { aa.AppUserId, aa.TechJobOpeningId }));

        //    builder.Entity<Rekommendation>()
        //        .HasOne(u => u.AppUser)
        //        .WithMany(a => a.Rekommendations)
        //        .HasForeignKey(aa => aa.AppUserId);

        //    builder.Entity<Rekommendation>()
        //        .HasOne(u => u.TechJobOpening)
        //        .WithMany(a => a.Rekommendations)
        //        .HasForeignKey(aa => aa.TechJobOpeningId);
        //}

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // get added or updated entries
            var addedOrUpdatedEntries = ChangeTracker.Entries().Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified));

            // fill out the audit fields
            foreach (var entry in addedOrUpdatedEntries)
            {
                var entity = entry.Entity as AuditableEntity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedBy = _userInfoService.UserId;
                    entity.CreatedOn = DateTime.UtcNow;
                }

                entity.UpdatedBy = _userInfoService.UserId;
                entity.UpdatedOn = DateTime.UtcNow;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
