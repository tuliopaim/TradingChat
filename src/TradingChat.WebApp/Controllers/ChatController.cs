using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TradingChat.Application.Hubs;
using TradingChat.Application.UseCases.CreateChatRoom;
using TradingChat.Application.UseCases.GetChatMessages;
using TradingChat.Application.UseCases.GetChatsInfo;
using TradingChat.Application.UseCases.JoinChatRoom;
using TradingChat.Application.UseCases.SendMessage;
using TradingChat.Application.UseCases.Shared;
using TradingChat.Core;
using TradingChat.WebApp.Extensions;
using TradingChat.WebApp.Hubs;

namespace TradingChat.WebApp.Controllers;

public class ChatController : Controller
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index(
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var chatsInfo = await mediator.Send(new GetChatsInfoQuery(), cancellationToken);

        return View(chatsInfo.Value);
    }

    [HttpGet]
    [Authorize]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(
        [FromServices] IMediator mediator,
        CreateChatRoomCommand command,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View();

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsSuccess) return RedirectToAction(nameof(Index));

        result.ToModelState(ModelState);
        
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> JoinChat(
        [FromServices] IMediator mediator,
        Guid chatId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new JoinChatRoomCommand(chatId), cancellationToken);

        return result.IsSuccess
            ? Ok(chatId)
            : BadRequest(new
            {
                errors = result.Errors.Select(e => e.Message),
            });
    }

    [HttpGet("/Chat/{chatId:Guid}")]
    [Authorize]
    public async Task<IActionResult> Chat(
        [FromServices] IMediator mediator,
        Guid chatId,
        CancellationToken cancellationToken)
    {
        Result<ChatMessagesDto> result = await mediator.Send(
            new GetChatMessagesQuery(chatId),
            cancellationToken);

        return View(result.Value);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> SendMessage(
       [FromServices] IMediator mediator,
       [FromServices] IHubContext<ChatHub, IChatHub> chatHubContext,
       [FromForm]SendMessageCommand command)
    {
        Result<ChatMessageInfoDto> result = await mediator.Send(command);

        if (result.IsSuccess)
        {
            await chatHubContext.Clients
                .Group(command.ChatRoomId.ToString())
                .ReceiveMessage(result.Value);

            return Ok();
        }

        return BadRequest(new
        {
            errors = result.Errors.Select(e => e.Message),
        });
    }
}
