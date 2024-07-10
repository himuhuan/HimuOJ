using Himu.EntityFramework.Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Himu.EntityFramework.Core.Configuration
{
    public class UserChatSessionEntityConfiguration : IEntityTypeConfiguration<UserChatSession>
    {
        public void Configure(EntityTypeBuilder<UserChatSession> builder)
        {
            builder.HasKey(ucs => ucs.Id);

            builder.HasOne(ucs => ucs.User)
                .WithMany(user => user.Sessions)
                .HasForeignKey(ucs => ucs.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ucs => ucs.Friend)
                .WithMany()
                .HasForeignKey(ucs => ucs.FriendId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(ucs => ucs.Messages)
                   .WithOne(message => message.Session)
                   .HasForeignKey(message => message.SessionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("UserChatSessions");
        }
    }
}
