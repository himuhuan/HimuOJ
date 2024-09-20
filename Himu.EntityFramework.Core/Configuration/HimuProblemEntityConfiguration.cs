using Himu.EntityFramework.Core.Entity;
using Himu.EntityFramework.Core.Entity.Limits;
using Himu.EntityFramework.Core.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Himu.EntityFramework.Core.Configuration
{
    public class HimuProblemEntityConfiguration : IEntityTypeConfiguration<HimuProblem>
    {
        public void Configure(EntityTypeBuilder<HimuProblem> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .HasValueGenerator<SnowflakeIdGenerator>()
                   .ValueGeneratedOnAdd();

            builder.OwnsOne(x => x.Detail, detail =>
            {
                detail.HasIndex(t => t.Code);
                detail.HasIndex(t => t.Title).HasPrefixLength(10);
                detail.Property(t => t.MaxExecuteTimeLimit).HasConversion<long>();

                detail.Property(x => x.Code)
                      .HasMaxLength(HimuProblemLimit.MaxCodeLength);
                detail.Property(x => x.Title)
                      .HasMaxLength(HimuProblemLimit.MaxTitleLength);
                detail.Property(x => x.Content)
                      .HasMaxLength(HimuProblemLimit.MaxContentLength);
            });

            builder.HasOne(x => x.Contest)
                   .WithMany(c => c.Problems)
                   .IsRequired();

            builder.HasOne(x => x.Distributor)
                   .WithMany(u => u.Problems)
                   .IsRequired();
        }
    }
}