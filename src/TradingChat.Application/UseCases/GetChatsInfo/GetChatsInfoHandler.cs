using Microsoft.EntityFrameworkCore;
using TradingChat.Application.Abstractions;
using TradingChat.Domain.Contracts;
using TradingChat.Domain.Shared;

namespace TradingChat.Application.UseCases.GetChatsInfo;

public class GetChatsInfoHandler : IQueryHandler<GetChatsInfoQuery, ChatsInfoResult>
{
    private readonly IChatRoomRepository _chatRoomRepository;

    public GetChatsInfoHandler(IChatRoomRepository chatRoomRepository)
    {
        _chatRoomRepository = chatRoomRepository;
    }

    public async Task<Result<ChatsInfoResult>> Handle(
        GetChatsInfoQuery request,
        CancellationToken cancellationToken)
    {
        var chats = await _chatRoomRepository
            .GetAsNoTracking()
            .Select(x => new ChatInfoModel
            {
                Id = x.Id,
                NumberOfUsers = x.Users.Count(),
                MaxNumberOfUsers = x.MaxNumberOfUsers,
                Name = x.Name,
                Owner = x.Owner!.Name
            }).
            ToListAsync(cancellationToken);

        return new ChatsInfoResult
        {
            Chats = chats
        };
    }
}
