﻿using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TradingChat.Application;
using TradingChat.Application.Abstractions;
using TradingChat.Application.Contracts;
using TradingChat.Application.Pipelines;
using TradingChat.Domain.Contracts;
using TradingChat.Infrastructure.Context;
using TradingChat.Infrastructure.Repositories;
using TradingChat.Infrastructure.Stooq;

namespace TradingChat.Infrastructure;

public static class IoC
{
    public static IServiceCollection InjectApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddScoped<ICurrentUser, CurrentUser>()
            .AddDbContext(configuration)
            .AddValidators()
            .AddMediator()
            .AddRepositories();
    }

    public static IServiceCollection InjectBotServices(
        this IServiceCollection services)
    {
        return services
            .AddValidators()
            .AddMediator()
            .AddStockPriceService();
    }

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
            .AddValidatorsFromAssembly(typeof(AssemblyReference).Assembly);
    }

    public static IServiceCollection AddMediator(
        this IServiceCollection services)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipeline<,>));

        return services.AddMediatR(
            config =>
            {
                config
                    .RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly);
            });
    }

    public static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        return services
            .AddScoped<IChatRoomRepository, ChatRoomRepository>()
            .AddScoped<IChatUserRepository, ChatUserRepository>();
    }

    public static IServiceCollection AddStockPriceService(
        this IServiceCollection services)
    {
        services.AddHttpClient<StooqClient>();

        services.AddScoped<IStockPriceService, StooqService>();

        return services;
    }
}
