using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TradingChat.Domain.Entities;

namespace TradingChat.Infrastructure.Mappings;

public class ChatMessageMapping : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("chat_messages", CustomSchemas.TradingChat);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(1000);

        builder
            .HasOne(x => x.ChatUser)
            .WithMany()
            .HasForeignKey(x => x.ChatUserId);
    }
}
