using Himu.EntityFramework.Core.Entity;
using Microsoft.EntityFrameworkCore;

namespace Himu.EntityFramework.Core.Contests
{
    public class HimuOnlineJudgeContext : DbContext
    {
        public virtual DbSet<HimuContest> Contests { get; set; } = null!;
        public virtual DbSet<HimuProblem> ProblemSet { get; set; } = null!;
        public virtual DbSet<HimuCommit> UserCommits { get; set; } = null!;
        public virtual DbSet<HimuTestPoint> TestPoints { get; set; } = null!;
        public virtual DbSet<TestPointResult> PointResults { get; set; } = null!;
        public virtual DbSet<ContestCreator> ContestCreators { get; set; } = null!;
        public virtual DbSet<CompilerPreset> CompilerPresets { get; set; } = null!;

        public HimuOnlineJudgeContext(DbContextOptions<HimuOnlineJudgeContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }

    }

    public class HimuOnlineJudgeContextFactory : HimuMySqlContextFactory<HimuOnlineJudgeContext>
    {
    }
}
