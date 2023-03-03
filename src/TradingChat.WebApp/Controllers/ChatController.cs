using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TradingChat.WebApp.Controllers;

public class ChatController : Controller
{
    [Authorize]
    public IActionResult Index()
    {
        return View();
    }
}
