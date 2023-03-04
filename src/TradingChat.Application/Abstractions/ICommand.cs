using MediatR;
using TradingChat.Domain.Shared;

namespace TradingChat.Application.Abstractions;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
