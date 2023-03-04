using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TradingChat.Domain.Entities;

namespace TradingChat.Infrastructure.Context;

public class TradingChatDbContext :
    IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
{
    public TradingChatDbContext(DbContextOptions<TradingChatDbContext> options)
        : base(options)
    {
    }

    public DbSet<ChatRoom> ChatRooms => Set<ChatRoom>();
    public DbSet<ChatUser> ChatUsers => Set<ChatUser>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
