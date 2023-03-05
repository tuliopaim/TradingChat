using MediatR;
using TradingChat.Core;

namespace TradingChat.Application.Abstractions;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
