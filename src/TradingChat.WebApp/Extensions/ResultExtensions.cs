using Microsoft.AspNetCore.Mvc.ModelBinding;
using TradingChat.Core;

namespace TradingChat.WebApp.Extensions;

public static class ResultExtensions
{
    public static void ToModelState(this Result result, ModelStateDictionary modelState)
    {
        foreach(var error in  result.Errors)
        {
            modelState.AddModelError("", error.Message);
        }
    }
}
