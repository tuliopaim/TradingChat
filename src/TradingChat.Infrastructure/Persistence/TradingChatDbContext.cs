using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TradingChat.Infrastructure.Persistence;

public class TradingChatDbContext :
    IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
{
    public TradingChatDbContext(DbContextOptions<TradingChatDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
