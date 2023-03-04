using MediatR;
using TradingChat.Domain.Shared;

namespace TradingChat.Application.Abstractions;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
