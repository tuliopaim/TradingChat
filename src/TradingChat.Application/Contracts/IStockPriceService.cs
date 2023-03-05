using TradingChat.Application.UseCases.GetStockPrice;
using TradingChat.Domain.Shared;

namespace TradingChat.Application.Contracts;

public interface IStockPriceService
{
    Task<Result<StockDataDto>> GetStockPrice(string stockCode);
}
