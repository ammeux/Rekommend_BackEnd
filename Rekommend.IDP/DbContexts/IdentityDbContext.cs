using Microsoft.EntityFrameworkCore;
using Rekommend.IDP.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rekommend.IDP.DbContexts
{
    public class IdentityDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserClaim> UserClaims { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Checks to ensure data integrity

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Subject)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<User>().HasData(
                new User()
                {
                    Id = new Guid("40acecde-ba0f-4936-9f70-a4ef44d65ed9"),
                    Password = "c6bockdo",
                    Subject = "40acecde-ba0f-4936-9f70-a4ef44d65ad3",
                    UserName = "Frank",
                    Active = true
                },
                new User()
                {
                    Id = new Guid("b7539694-97e7-4dfe-84da-b4256e1ff5c7"),
                    Password = "c6bockdo",
                    Subject = "b7539694-97e7-4dfe-84da-b4256e1ff3dr",
                    UserName = "Claire",
                    Active = true
                });

            modelBuilder.Entity<UserClaim>().HasData(
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("40acecde-ba0f-4936-9f70-a4ef44d65ed9"),
                 Type = "given_name",
                 Value = "Frank"
             },
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("40acecde-ba0f-4936-9f70-a4ef44d65ed9"),
                 Type = "family_name",
                 Value = "Underwood"
             },
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("40acecde-ba0f-4936-9f70-a4ef44d65ed9"),
                 Type = "email",
                 Value = "frank@underwood.com"
             },
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("40acecde-ba0f-4936-9f70-a4ef44d65ed9"),
                 Type = "address",
                 Value = "Main Road 1"
             },
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("40acecde-ba0f-4936-9f70-a4ef44d65ed9"),
                 Type = "subscriptionlevel",
                 Value = "FreeUser"
             },
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("b7539694-97e7-4dfe-84da-b4256e1ff5c7"),
                 Type = "given_name",
                 Value = "Claire"
             },
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("b7539694-97e7-4dfe-84da-b4256e1ff5c7"),
                 Type = "family_name",
                 Value = "Underwood"
             },
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("b7539694-97e7-4dfe-84da-b4256e1ff5c7"),
                 Type = "email",
                 Value = "claire@underwood.com"
             },
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("b7539694-97e7-4dfe-84da-b4256e1ff5c7"),
                 Type = "address",
                 Value = "Big Street 2"
             },
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("b7539694-97e7-4dfe-84da-b4256e1ff5c7"),
                 Type = "subscriptionlevel",
                 Value = "PayingUser"
             });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // get updated entries
            var updatedConcurrencyyAwareEntries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified)
                .OfType<IConcurrencyAware>();

            foreach (var entry in updatedConcurrencyyAwareEntries)
            {
                entry.ConcurrencyStamp = Guid.NewGuid().ToString();
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
