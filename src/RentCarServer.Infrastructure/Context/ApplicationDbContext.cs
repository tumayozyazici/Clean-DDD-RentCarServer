using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RentCarServer.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RentCarServer.Infrastructure.Context
{
    internal sealed class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyGlobalFilters();
            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<Entity>();

            HttpContextAccessor httpContextAccessor = new();
            string userIdString =
                httpContextAccessor
                .HttpContext!
                .User
                .Claims
                .First(p => p.Type == ClaimTypes.NameIdentifier)
                .Value;

            Guid userId = Guid.Parse(userIdString);
            IdentityId identityId = new(userId);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property(p => p.CreatedAt)
                        .CurrentValue = DateTimeOffset.Now;
                    entry.Property(p => p.CreatedBy)
                        .CurrentValue = identityId;
                }

                if (entry.State == EntityState.Modified)
                {
                    if (entry.Property(p => p.IsDeleted).CurrentValue == true)
                    {
                        entry.Property(p => p.DeletedAt)
                        .CurrentValue = DateTimeOffset.Now;
                        entry.Property(p => p.DeletedBy)
                        .CurrentValue = identityId;
                    }
                    else
                    {
                        entry.Property(p => p.UpdatedAt)
                            .CurrentValue = DateTimeOffset.Now;
                        entry.Property(p => p.UpdatedBy)
                        .CurrentValue = identityId;
                    }
                }

                if (entry.State == EntityState.Deleted)
                {
                    throw new ArgumentException("Db'den direkt silme işlemi yapamazsınız");
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
