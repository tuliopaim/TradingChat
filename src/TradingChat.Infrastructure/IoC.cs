using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TradingChat.Domain.Contracts;
using TradingChat.Domain.UseCases.Base.Pipelines;
using TradingChat.Infrastructure.Context;
using TradingChat.Infrastructure.Repositories;

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

    public static IServiceCollection AddValidators(
        this IServiceCollection services)
    {
        return services
            .AddValidatorsFromAssembly(typeof(Domain.AssemblyReference).Assembly);
    }

    public static IServiceCollection AddMediator(
        this IServiceCollection services)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipeline<,>));

        return services.AddMediatR(
            config =>
            {
                config
                    .RegisterServicesFromAssembly(typeof(Domain.AssemblyReference).Assembly);
            });
    }

    public static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        return services
            .AddScoped<IChatUserRepository, ChatUserRepository>();
    }
}
