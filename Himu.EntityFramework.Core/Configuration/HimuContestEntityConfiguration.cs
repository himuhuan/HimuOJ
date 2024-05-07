using Himu.EntityFramework.Core.Entity;
using Himu.EntityFramework.Core.Entity.Limits;
using Himu.EntityFramework.Core.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Himu.EntityFramework.Core.Configuration
{
    public class HimuContestEntityConfiguration : IEntityTypeConfiguration<HimuContest>
    {
        public void Configure(EntityTypeBuilder<HimuContest> builder)
        {
            builder.Property(x => x.Id).HasValueGenerator<SnowflakeIdGenerator>();

            builder.OwnsOne(x => x.Information, info =>
            {
                info.Property(t => t.LaunchTaskAtOnce).HasDefaultValue(false);
                info.HasIndex(t => t.Title).HasPrefixLength(50);
                info.HasIndex(t => t.DistributeDateTime);
                info.HasIndex(t => t.Code).IsUnique();
                info.Property(t => t.Code).HasColumnName("ContestCode");

                info.Property(t => t.Code).HasMaxLength(HimuContestLimit.MaxCodeLength);
                info.Property(t => t.Title).HasMaxLength(HimuContestLimit.MaxTitleLength);
                info.Property(t => t.Description)
                    .HasMaxLength(HimuContestLimit.MaxDescriptionLength);
                info.Property(t => t.Introduction)
                    .HasMaxLength(HimuContestLimit.MaxIntroductionLength);
            });

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Distributor)
                   .WithMany(u => u.Contests);
        }
    }
}