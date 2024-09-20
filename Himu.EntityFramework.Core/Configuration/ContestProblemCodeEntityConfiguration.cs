using Himu.EntityFramework.Core.Entity;
using Himu.EntityFramework.Core.Entity.Limits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Himu.EntityFramework.Core.Configuration
{
    public class ContestProblemCodeEntityConfiguration : IEntityTypeConfiguration<ContestProblemCode>
    {
        public void Configure(EntityTypeBuilder<ContestProblemCode> builder)
        {
            builder.HasKey(c => new { c.ProblemId, c.ContestId });
            builder.Property(c => c.ProblemCode)
                   .HasMaxLength(HimuProblemLimit.MaxCodeLength)
                   .IsRequired();
        }
    }
}