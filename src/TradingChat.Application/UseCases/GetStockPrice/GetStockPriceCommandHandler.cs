using TradingChat.Application.Abstractions;
using TradingChat.Application.Contracts;
using TradingChat.Domain.Shared;

namespace TradingChat.Application.UseCases.GetStockPrice;

public class GetStockPriceCommandHandler : ICommandHandler<GetStockPriceCommand, StockDataDto>
{
    private readonly IStockPriceService _stockPriceService;

    public GetStockPriceCommandHandler(IStockPriceService stockPriceService)
    {
        _stockPriceService = stockPriceService;
    }

    public async Task<Result<StockDataDto>> Handle(GetStockPriceCommand request, CancellationToken cancellationToken)
    {
        return await _stockPriceService.GetStockPrice(request.StockCode);
    }
}
