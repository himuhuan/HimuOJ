using Himu.EntityFramework.Core.Entity;
using Himu.EntityFramework.Core.Entity.Limits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Himu.EntityFramework.Core.Configuration
{
    public class HimuCommitEntityConfiguration : IEntityTypeConfiguration<HimuCommit>
    {
        public void Configure(EntityTypeBuilder<HimuCommit> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedNever();

            builder.OwnsOne(c => c.CompilerInformation, info =>
            {
                info.HasIndex(i => i.CompilerName);
                info.Property(i => i.CompilerName)
                    .HasMaxLength(HimuCommitLimit.MaxCompilerNameLength);
                info.Property(i => i.MessageFromCompiler)
                    .HasMaxLength(HimuCommitLimit.MaxCompilerMessageLength);
            });

            builder.Property(c => c.SourceUri)
                   .HasMaxLength(HimuCommitLimit.MaxSourceUriLength);

            builder.HasIndex(c => c.Status);

            builder.Property(c => c.Status)
                   .HasConversion(
                       v => v.ToString(),
                       v => (ExecutionStatus)Enum.Parse(typeof(ExecutionStatus), v))
                   .HasMaxLength(HimuCommitLimit.MaxCommitStatusStrLength);

            builder.HasOne(c => c.Problem)
                   .WithMany(p => p.UserCommits)
                   .HasForeignKey(c => c.ProblemId)
                   .IsRequired();

            builder.HasOne(c => c.User)
                   .WithMany(u => u.MyCommits)
                   .HasForeignKey(c => c.UserId)
                   .IsRequired();
        }
    }
}