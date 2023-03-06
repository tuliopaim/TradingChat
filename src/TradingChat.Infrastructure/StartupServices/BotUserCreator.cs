using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TradingChat.Domain.Contracts;
using TradingChat.Domain.Entities;

namespace TradingChat.Infrastructure.StartupServices;

public class BotUserCreator
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public BotUserCreator(
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
        
        var botUser = ChatUser.ChatBotUser();
        if (await chatUserRepository.GetAsNoTracking().AnyAsync(u => u.Id == botUser.Id))
        {
            return;
        }

        var botUserEmail = _configuration["BotUser:Email"]!;
        var botUserPass = _configuration["BotUser:Password"]!;

        var identityUser = new IdentityUser<Guid>
        {
            Id = botUser.Id,
            UserName = botUserEmail,
            Email = botUserEmail,
        };

        var result = await userManager.CreateAsync(identityUser, botUserPass);

        chatUserRepository.Add(botUser);

        await chatUserRepository.SaveChangesAsync(default);
    }

}
