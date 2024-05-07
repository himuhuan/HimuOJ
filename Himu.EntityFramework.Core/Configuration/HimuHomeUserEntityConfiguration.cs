using Himu.EntityFramework.Core.Entity;
using Himu.EntityFramework.Core.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Himu.EntityFramework.Core.Configuration
{
    public class HimuHomeUserEntityConfiguration : IEntityTypeConfiguration<HimuHomeUser>
    {
        public void Configure(EntityTypeBuilder<HimuHomeUser> builder)
        {
            builder.Property(x => x.Id).HasValueGenerator<SnowflakeIdGenerator>();
            builder.Property(x => x.Avatar).HasMaxLength(256).IsRequired();
            builder.Property(x => x.Background).HasMaxLength(256).IsRequired();
        }
    }
}