using MediatR;
using TradingChat.Domain.Shared;

namespace TradingChat.Domain.UseCases.Base;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
