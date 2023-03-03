using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TradingChat.Domain.Entities;

namespace TradingChat.Infrastructure.Mappings;

public class ChatUserMapping : IEntityTypeConfiguration<ChatUser>
{
    public void Configure(EntityTypeBuilder<ChatUser> builder)
    {
        builder.ToTable("chat_users", CustomSchemas.TradingChat);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(30);

        builder.HasOne(x => x.IdentityUser)
            .WithOne()
            .HasForeignKey<ChatUser>(x => x.IdentityUserId);
    }
}
