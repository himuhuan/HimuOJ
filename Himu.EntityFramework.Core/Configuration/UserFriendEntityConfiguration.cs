using Himu.EntityFramework.Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Himu.EntityFramework.Core.Configuration
{
    public class UserFriendEntityConfiguration : IEntityTypeConfiguration<UserFriend>
    {
        public void Configure(EntityTypeBuilder<UserFriend> builder)
        {
            builder.HasKey(uf => new { uf.UserId, uf.FriendId });

            builder.HasOne(uf => uf.User)
                   .WithMany(u => u.Friends)
                   .HasForeignKey(u => u.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(uf => uf.Friend)
                   .WithMany()
                   .HasForeignKey(uf => uf.FriendId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
