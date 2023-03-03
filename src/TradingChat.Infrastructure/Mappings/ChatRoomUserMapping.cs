using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TradingChat.Domain.Entities;

namespace TradingChat.Infrastructure.Mappings;

public class ChatRoomUserMapping : IEntityTypeConfiguration<ChatRoomUser>
{
    public void Configure(EntityTypeBuilder<ChatRoomUser> builder)
    {
        builder.ToTable("chat_room_users", CustomSchemas.TradingChat);

        builder.HasKey(x => new { x.ChatRoomId, x.ChatUserId });

        builder.HasOne(x => x.ChatUser)
            .WithMany(x => x.Chats)
            .HasForeignKey(x => x.ChatUserId);
            
        builder.HasOne(x => x.ChatRoom)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.ChatRoomId);
    }
}
