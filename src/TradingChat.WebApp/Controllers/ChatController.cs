using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradingChat.Application.UseCases.CreateChatRoom;
using TradingChat.Application.UseCases.GetChatsInfo;
using TradingChat.WebApp.Extensions;

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

        if (result.IsSuccess)
        {
            return RedirectToAction(nameof(Index));
        }

        result.ToModelState(ModelState);
        
        return View();
    }
}
