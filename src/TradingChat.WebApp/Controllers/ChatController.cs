using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradingChat.Domain.UseCases.CreateChatRoom;
using TradingChat.Domain.UseCases.GetChatsInfo;
using TradingChat.WebApp.ViewModels;

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
        CreateChatRoomModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View();

        var command = new CreateChatRoomCommand(model.Name!, model.MaxNumberOfUsers);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return RedirectToAction("Index");
        }

        foreach(var error in  result.Errors)
        {
            ModelState.AddModelError("", error.Message);
        }
        
        return View();
    }
}
