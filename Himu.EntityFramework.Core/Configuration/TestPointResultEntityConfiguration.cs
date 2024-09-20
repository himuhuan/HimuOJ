using Himu.EntityFramework.Core.Entity;
using Himu.EntityFramework.Core.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Himu.EntityFramework.Core.Configuration
{
    public class TestPointResultEntityConfiguration : IEntityTypeConfiguration<TestPointResult>
    {
        public void Configure(EntityTypeBuilder<TestPointResult> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                   .HasValueGenerator<SnowflakeIdGenerator>()
                   .ValueGeneratedOnAdd();

            builder.Property(c => c.TestStatus)
                   .HasConversion(v => v.ToString(), v => (ExecutionStatus) Enum.Parse(typeof(ExecutionStatus), v))
                   .HasMaxLength(32);

            builder.OwnsOne(t => t.Usage, usage =>
            {
                usage.Property(u => u.TimeUsed)
                     .HasConversion<TimespanMillisecondsConverter>();
            });

            builder.OwnsOne(t => t.Difference);

            builder.HasOne(t => t.TestPoint)
                   .WithMany()
                   .HasForeignKey(t => t.TestPointId)
                   .IsRequired();

            builder.HasOne(t => t.Commit)
                   .WithMany(c => c.TestPointResults)
                   .HasForeignKey(t => t.CommitId)
                   .IsRequired();
        }
    }
}