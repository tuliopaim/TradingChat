using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using TradingChat.ChatCommandBot.Commands.ChatMessageCommands;
using TradingChat.ChatCommandBot.Commands.ChatMessageCommands.StockPrice;
using TradingChat.ChatCommandBot.Commands.Contracts;
using TradingChat.Core.Messaging;
using TradingChat.ExternalService.Stooq;

namespace TradingChat.ChatCommandBot;


