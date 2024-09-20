using Himu.EntityFramework.Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Himu.EntityFramework.Core.Configuration
{
    public class CompilerPresetEntityConfiguration : IEntityTypeConfiguration<CompilerPreset>
    {
        public void Configure(EntityTypeBuilder<CompilerPreset> builder)
        {
            builder.HasKey(e => e.Name);
            builder.Property(e => e.Name).HasMaxLength(256).IsRequired();
            builder.Property(e => e.Language).HasMaxLength(32).IsRequired();
            builder.Property(e => e.Command).HasMaxLength(512).IsRequired();
            builder.Property(e => e.Shared).IsRequired().HasDefaultValue(false);
        }
    }
}
