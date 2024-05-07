using Himu.EntityFramework.Core.Entity;
using Himu.EntityFramework.Core.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Himu.EntityFramework.Core.Configuration
{
    public class HimuArticleEntityConfiguration : IEntityTypeConfiguration<HimuArticle>
    {
        public void Configure(EntityTypeBuilder<HimuArticle> builder)
        {
            builder.Property(a => a.Id)
                .HasValueGenerator<SnowflakeIdGenerator>();

            builder.HasOne(a => a.Author)
                   .WithMany(a => a.Articles)
                   .IsRequired();
        }
    }
}