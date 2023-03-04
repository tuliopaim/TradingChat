using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TradingChat.Domain.Entities;

namespace TradingChat.Infrastructure.Mappings;

public class ChatRoomMapping : IEntityTypeConfiguration<ChatRoom>
{
    public void Configure(EntityTypeBuilder<ChatRoom> builder)
    {
        builder.ToTable("chat_rooms", CustomSchemas.TradingChat);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(30);

        builder
            .HasOne(x => x.Owner)
            .WithMany()
            .HasForeignKey(x => x.OwnerId);

        builder
            .HasMany(x => x.Messages)
            .WithOne(x => x.ChatRoom)
            .HasForeignKey(x => x.ChatRoomId);
    }
}
