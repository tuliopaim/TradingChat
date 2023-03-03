#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TradingChat.Domain.UseCases.CreateUser;

namespace TradingChat.WebApp.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly ICreateChatUserHandler _createUserHandler;
    private readonly SignInManager<IdentityUser<Guid>> _signInManager;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(
        ICreateChatUserHandler createUserHandler,
        SignInManager<IdentityUser<Guid>> signInManager,
        ILogger<RegisterModel> logger)
    {
        _createUserHandler = createUserHandler;
        _signInManager = signInManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }


    public async Task OnGetAsync(string returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var command = new CreateUserCommand(Input.Email, Input.Name, Input.Password);

        var result = await _createUserHandler.Handle(command, default);

        if (result.IsSuccess)
        {
            _logger.LogInformation("User created a new account with password.");

            return LocalRedirect(returnUrl);
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Message);
        }

        return Page();
    }
}
