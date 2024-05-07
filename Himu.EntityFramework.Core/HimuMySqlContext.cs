using Himu.EntityFramework.Core.Entity;
using Himu.EntityFramework.Core.Entity.Components;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Himu.EntityFramework.Core
{
    public class HimuMySqlContext
        : IdentityDbContext<HimuHomeUser, HimuHomeRole, long>
    {
        public virtual DbSet<HimuArticle> Articles { get; set; } = null!;
        public virtual DbSet<HimuContest> Contests { get; set; } = null!;
        public virtual DbSet<HimuProblem> ProblemSet { get; set; } = null!;
        public virtual DbSet<HimuCommit> UserCommits { get; set; } = null!;
        public virtual DbSet<HimuTestPoint> TestPoints { get; set; } = null!;
        public virtual DbSet<TestPointResult> PointResults { get; set; } = null!; 
        public virtual DbSet<PermissionRecord> PermissionRecords { get; set; } = null!;

        public HimuMySqlContext(DbContextOptions<HimuMySqlContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }
    }

    public class HimuMySqlContextFactory : IDesignTimeDbContextFactory<HimuMySqlContext>
    {
        public HimuMySqlContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<HimuMySqlContext> builder = new();
            const string connectionString = "server=localhost;uid=root;pwd=liuhuan123;database=himuoj;Character Set=utf8;persist security info=True";

            builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                   .EnableDetailedErrors()
                   .EnableSensitiveDataLogging();

            return new HimuMySqlContext(builder.Options);
        }
    }
}