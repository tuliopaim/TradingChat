namespace TradingChat.Application.UseCases.GetStockPrice;

public record StockDataDto(
    string Symbol,
    DateOnly Date,
    TimeOnly Time,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    decimal Volume);
