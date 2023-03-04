using MediatR;
using TradingChat.Domain.Shared;

namespace TradingChat.Domain.UseCases.Base;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
