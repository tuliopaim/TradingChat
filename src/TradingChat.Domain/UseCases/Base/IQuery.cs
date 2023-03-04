using MediatR;
using TradingChat.Domain.Shared;

namespace TradingChat.Domain.UseCases.Base;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
