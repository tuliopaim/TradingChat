using TradingChat.Core;

namespace TradingChat.ChatBot.Commands.Contracts;

public interface IStockPriceService
{
    Task<Result<StockDataDto>> GetStockPrice(string stockCode);
}