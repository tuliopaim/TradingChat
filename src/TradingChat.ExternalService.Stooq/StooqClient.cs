using Microsoft.Extensions.Configuration;
using TradingChat.Core;

namespace TradingChat.ExternalService.Stooq;

public class StooqClient
{
    private readonly HttpClient _httpClient;
    private readonly string _stooqUrl;
    private readonly string _stockField;

    public StooqClient(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;

        _stooqUrl = configuration["Stooq:Endpoint"]!;
        _stockField = configuration["Stooq:StockField"]!;
    }

    public async Task<Result<string>> GetStockPriceCsv(string stockCode)
    {
        var finalUrl = _stooqUrl.Replace(_stockField, stockCode);

        var request = new HttpRequestMessage(HttpMethod.Get, finalUrl);

        var response = await _httpClient.SendAsync(request);

        var responseStr = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return new Error($"Error while getting the stock price of {stockCode}");
        }

        return responseStr;
    }
}
