using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TradingChat.Infrastructure.Persistence;

namespace TradingChat.Infrastructure;

public static class IoC
{
    public static IServiceCollection AddDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration
            .GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found.");

        services.AddDbContext<TradingChatDbContext>(
            options => options.UseNpgsql(connectionString));

        return services;
    }
}
