using Himu.EntityFramework.Core.Entity;
using Himu.EntityFramework.Core.Entity.Components;
using Himu.EntityFramework.Core.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Himu.EntityFramework.Core.Configuration
{
    public class PermissionRecordEntityConfiguration : IEntityTypeConfiguration<PermissionRecord>
    {
        public void Configure(EntityTypeBuilder<PermissionRecord> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasValueGenerator<SnowflakeIdGenerator>();

            builder.HasOne(x => x.User).WithMany().IsRequired();
            builder.HasOne(x => x.Role).WithMany().IsRequired();

            builder.Property(x => x.Operation)
                .HasConversion(
                    x => x.ToString(), 
                    x => (PermissionOperation) System.Enum.Parse(typeof(PermissionOperation), x))
                .HasMaxLength(16);
        }
    }
}
