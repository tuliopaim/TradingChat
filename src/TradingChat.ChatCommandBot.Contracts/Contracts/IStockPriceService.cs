using TradingChat.Core;

namespace TradingChat.ChatCommandBot.Commands.Contracts;

public interface IStockPriceService
{
    Task<Result<StockDataDto>> GetStockPrice(string stockCode);
}