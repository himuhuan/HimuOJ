using Himu.EntityFramework.Core.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Himu.EntityFramework.Core.Contests
{
    public class HimuIdentityContext
        : IdentityDbContext<HimuHomeUser, HimuHomeRole, long>
    {
        public virtual DbSet<PermissionRecord> PermissionRecords { get; set; } = null!;

        public HimuIdentityContext(DbContextOptions<HimuIdentityContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }

    public class HimuIdentityContextFactory : HimuMySqlContextFactory<HimuIdentityContext>
    {

    }
}
