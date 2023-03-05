using CsvHelper;
using System.Globalization;
using TradingChat.ChatCommandBot.Commands.Contracts;
using TradingChat.Core;

namespace TradingChat.ExternalService.Stooq;

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

        var isInvalid = CheckIfIsInvalid(csvResult);

        if (isInvalid) return new Error("Stock price not found!");

        using var stringReader = new StringReader(csvResult.Value);
        using var csv = new CsvReader(stringReader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<StockDataDto>();

        return records.First();
    }

    private static bool CheckIfIsInvalid(Result<string> csvResult)
    {
        using var stringReader = new StringReader(csvResult.Value);
        using var csv = new CsvReader(stringReader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<StockDataRaw>();

        var record = records.First();

        const string noDataStr = "N/D";

        return record is
        {
            Date: noDataStr,
            Time: noDataStr,
            Open: noDataStr,
            High: noDataStr,
            Low: noDataStr,
            Close: noDataStr,
            Volume: noDataStr,
        };
    }

    private record StockDataRaw(
        string Symbol,
        string Date,
        string Time,
        string Open,
        string High,
        string Low,
        string Close,
        string Volume);
}
