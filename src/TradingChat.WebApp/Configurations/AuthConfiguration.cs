using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using TradingChat.Infrastructure.Context;

namespace TradingChat.WebApp.Configurations;

public static class AuthConfiguration
{
    public static IServiceCollection AddAuth(
        this IServiceCollection services)
    {
        services.AddIdentity();

        services.Configure<CookieAuthenticationOptions>(
            IdentityConstants.ApplicationScheme, options =>
            {
                options.Cookie.Name = "TradingClientAuth";
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
            });

        return services;
    }

    private static IServiceCollection AddIdentity(
        this IServiceCollection services)
    {
        services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>(config =>
        {
            config.User.RequireUniqueEmail = false;
            config.Password.RequiredLength = 6;
            config.Password.RequireDigit = false;
            config.Password.RequireUppercase = false;
            config.Password.RequireLowercase = false;
            config.Password.RequireNonAlphanumeric = false;
            config.SignIn.RequireConfirmedEmail = false;
        })
        .AddEntityFrameworkStores<TradingChatDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }


}

