namespace TradingChat.ChatCommandBot.Commands.Contracts;

public record StockDataDto(
    string Symbol,
    DateOnly Date,
    TimeOnly Time,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    decimal Volume);
