using Himu.EntityFramework.Core.Entity;
using Himu.EntityFramework.Core.Entity.Limits;
using Himu.EntityFramework.Core.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Himu.EntityFramework.Core.Configuration
{
    public class ChatMessageEntityConfiguration : IEntityTypeConfiguration<ChatMessage>
    {
        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .HasValueGenerator<SnowflakeIdGenerator>()
                   .ValueGeneratedOnAdd();
            
            builder.Property(c => c.Value).HasMaxLength(ChatLimit.MaxMessageTextLength);

            builder.HasOne(cm => cm.Session)
                   .WithMany(cs => cs.Messages)
                   .HasForeignKey(cm => cm.SessionId);

            builder.HasOne(cm => cm.Sender)
                   .WithMany()
                   .HasForeignKey(cm => cm.SenderId);
        }
    }
}
