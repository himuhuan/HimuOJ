using Himu.EntityFramework.Core.Entity;
using Himu.EntityFramework.Core.Entity.Components;
using Himu.EntityFramework.Core.Entity.Results;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MySqlConnector;

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
        public virtual DbSet<ContestCreator> ContestCreators { get; set; } = null!;
        public virtual DbSet<ChatMessage> ChatMessages { get; set; }
        public virtual DbSet<UserChatSession> UserChatSessions { get; set; }
        public virtual DbSet<UserFriend> UserFriends { get; set; } = null!;

        public virtual DbSet<ProblemAccuracy> ProblemAccuracies { get; set; }
        public virtual DbSet<UserSuccessRateResult> UserSuccessRates { get; set; }

        public HimuMySqlContext(DbContextOptions<HimuMySqlContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }

        public IQueryable<ProblemAccuracy> CalculateProblemAccuracy()
        {
            return ProblemAccuracies.FromSqlRaw("CALL CalculateProblemAccuracy()");
        }

        public IQueryable<UserSuccessRateResult> CalculateUserSuccessRate(long userId)
        {
            return UserSuccessRates.FromSqlRaw($"CALL CalculateUserSuccessRate({userId})");
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