using FluentValidation;

namespace TradingChat.Application.UseCases.GetStockPrice;

public class GetStockPriceCommandValidator : AbstractValidator<GetStockPriceCommand>
{
    public GetStockPriceCommandValidator()
    {
        RuleFor(x => x.StockCode).NotEmpty();
    }
}
