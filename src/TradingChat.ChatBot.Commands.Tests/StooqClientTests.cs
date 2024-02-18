using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using TradingChat.ChatBot.External.Stooq;

namespace TradingChat.ExternalService.Stooq.Tests;

public class StooqClientTests
{
    [Fact]
    public async Task Should_Get_CSVString()
    {
        //arrange
        const string stockField = "aapl.us";
        const string stockCode = "^SPX";
        const string stooqEndpoint =
            "https://stooq.com/q/l/?s=aapl.us&f=sd2t2ohlcv&h&e=csv";

        var configurationMoq = new Mock<IConfiguration>();

        configurationMoq
            .Setup(c => c["Stooq:Endpoint"])
            .Returns(stooqEndpoint);
        configurationMoq
            .Setup(c => c["Stooq:StockField"])
            .Returns(stockField);

        var stooqClient = new StooqClient(
            new HttpClient(),
            configurationMoq.Object);

        //act
        var stockResult = await stooqClient.GetStockPriceCsv(stockCode);

        //assert
        stockResult.IsSuccess.Should().BeTrue();
        stockResult.Value.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Should_Return_StockData_If_StockCode_IsValid()
    {
        //arrange
        const string stockField = "aapl.us";
        const string stockCode = "^SPX";
        const string stooqEndpoint =
            "https://stooq.com/q/l/?s=aapl.us&f=sd2t2ohlcv&h&e=csv";

        var configurationMoq = new Mock<IConfiguration>();

        configurationMoq
            .Setup(c => c["Stooq:Endpoint"])
            .Returns(stooqEndpoint);
        configurationMoq
            .Setup(c => c["Stooq:StockField"])
            .Returns(stockField);

        var stooqClient = new StooqClient(
            new HttpClient(),
            configurationMoq.Object);

        var stooqService = new StooqService(stooqClient);

        //act
        var stockResult = await stooqService.GetStockPrice(stockCode);

        //assert
        stockResult.IsSuccess.Should().BeTrue();
        stockResult.Value.Symbol.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Should_Not_Return_StockData_If_StockCode_IsInvalid()
    {
        //arrange
        const string stockField = "aapl.us";
        const string stockCode = "Invalid";
        const string stooqEndpoint =
            "https://stooq.com/q/l/?s=aapl.us&f=sd2t2ohlcv&h&e=csv";

        var configurationMoq = new Mock<IConfiguration>();

        configurationMoq
            .Setup(c => c["Stooq:Endpoint"])
            .Returns(stooqEndpoint);
        configurationMoq
            .Setup(c => c["Stooq:StockField"])
            .Returns(stockField);

        var stooqClient = new StooqClient(
            new HttpClient(),
            configurationMoq.Object);

        var stooqService = new StooqService(stooqClient);

        //act
        var stockResult = await stooqService.GetStockPrice(stockCode);

        //assert
        stockResult.IsSuccess.Should().BeFalse();
    }
}

