using Himu.EntityFramework.Core.Entity;
using Himu.EntityFramework.Core.Entity.Limits;
using Himu.EntityFramework.Core.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Himu.EntityFramework.Core.Configuration
{
    public class HimuTestPointEntityConfiguration : IEntityTypeConfiguration<HimuTestPoint>
    {
        public void Configure(EntityTypeBuilder<HimuTestPoint> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .HasValueGenerator<SnowflakeIdGenerator>()
                   .ValueGeneratedOnAdd();

            builder.Property(x => x.CaseName)
                   .HasMaxLength(HimuTestPointLimit.MaxCaseName);
            builder.Property(x => x.Expected)
                   .HasMaxLength(HimuTestPointLimit.MaxExpectedLength);
            builder.Property(x => x.Input)
                   .HasMaxLength(HimuTestPointLimit.MaxInputLength);
            
            builder.HasOne(tp => tp.Problem)
                   .WithMany(p => p.TestPoints)
                   .IsRequired();
        }
    }
}