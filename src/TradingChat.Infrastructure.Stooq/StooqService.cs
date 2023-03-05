using CsvHelper;
using System.Globalization;
using TradingChat.Application.Contracts;
using TradingChat.Application.UseCases.GetStockPrice;
using TradingChat.Domain.Shared;

namespace TradingChat.Infrastructure.Stooq;

public class StooqService : IStockPriceService
{
    private readonly StooqClient _stooqClient;

    public StooqService(StooqClient stooqClient)
    {
        _stooqClient = stooqClient;
    }

    public async Task<Result<StockDataDto>> GetStockPrice(string stockCode)
    {
        var csvResult = await _stooqClient.GetStockPriceCsv(stockCode);

        if (!csvResult.IsSuccess)
        {
            return Result.WithErrors<StockDataDto>(csvResult.Errors);
        }

        using var stringReader = new StringReader(csvResult.Value);
        using var csv = new CsvReader(stringReader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<StockDataDto>();

        return records.First();
    }
}
