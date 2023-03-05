using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TradingChat.Domain.Contracts;
using TradingChat.Domain.Entities;

namespace TradingChat.Infrastructure.StartupServices;

public class AdminUserCreator
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public AdminUserCreator(
        IConfiguration configuration,
        IServiceProvider serviceProvider)
	{
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public async Task Create()
    {
        using var scope = _serviceProvider.CreateScope();

        var environment = _serviceProvider.GetRequiredService<IHostingEnvironment>();

        if (environment.EnvironmentName is not ("Internal" or "Development"))
        {
            return;
        }

        var chatUserRepository = scope.ServiceProvider.GetRequiredService<IChatUserRepository>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser<Guid>>>();
        
        var adminUser = ChatUser.AdminChatUser();
        if (await chatUserRepository.GetAsNoTracking().AnyAsync(u => u.Id == adminUser.Id))
        {
            return;
        }

        var adminEmail = _configuration["AdminUser:Email"]!;
        var adminPass = _configuration["AdminUser:Password"]!;

        var identityUser = new IdentityUser<Guid>
        {
            Id = adminUser.Id,
            UserName = adminEmail,
            Email = adminEmail,
        };

        var result = await userManager.CreateAsync(identityUser, adminPass);

        chatUserRepository.Add(adminUser);

        await chatUserRepository.SaveChangesAsync(default);
    }

}
