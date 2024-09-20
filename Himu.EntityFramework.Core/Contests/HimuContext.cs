using Himu.EntityFramework.Core.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Himu.EntityFramework.Core.Contests
{
    /// <summary>
    /// The context that contains all the entities in Himu OJ.
    /// </summary>
    [Obsolete("This class is obsolete, use other contexts instead.")]
    public class HimuContext
        : IdentityDbContext<HimuHomeUser, HimuHomeRole, long>
    {
        public virtual DbSet<HimuArticle> Articles { get; set; } = null!;
        public virtual DbSet<HimuContest> Contests { get; set; } = null!;
        public virtual DbSet<HimuProblem> ProblemSet { get; set; } = null!;
        public virtual DbSet<HimuCommit> UserCommits { get; set; } = null!;
        public virtual DbSet<HimuTestPoint> TestPoints { get; set; } = null!;
        public virtual DbSet<TestPointResult> PointResults { get; set; } = null!;
        public virtual DbSet<PermissionRecord> PermissionRecords { get; set; } = null!;
        public virtual DbSet<ContestCreator> ContestCreators { get; set; } = null!;
        public virtual DbSet<CompilerPreset> CompilerPresets { get; set; } = null!;

        public HimuContext(DbContextOptions<HimuContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }

    public class HimuContextFactory : HimuMySqlContextFactory<HimuContext>
    {

    }
}