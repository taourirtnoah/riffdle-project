using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Riffdle.Models;

namespace Riffdle.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class ExternalLoginModel : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IUserStore<AppUser> _userStore;
    private readonly IUserEmailStore<AppUser> _emailStore;

    public ExternalLoginModel(
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IUserStore<AppUser> userStore)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ProviderDisplayName { get; set; }
    public string ReturnUrl { get; set; } = "/";

    [TempData]
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(11, MinimumLength = 11)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "OIB smije sadržavati samo brojeve.")]
        [Display(Name = "OIB")]
        public string OIB { get; set; } = string.Empty;

        [Required]
        [StringLength(13, MinimumLength = 13)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "JMBG smije sadržavati samo brojeve.")]
        [Display(Name = "JMBG")]
        public string JMBG { get; set; } = string.Empty;
    }

    public IActionResult OnGet() => RedirectToPage("./Login");

    public IActionResult OnPost(string provider, string? returnUrl = null)
    {
        var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");

        if (remoteError != null)
        {
            ErrorMessage = $"Error from external provider: {remoteError}";
            return RedirectToPage("./Login", new { ReturnUrl });
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            ErrorMessage = "Error loading external login information.";
            return RedirectToPage("./Login", new { ReturnUrl });
        }

        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        if (result.Succeeded)
        {
            return LocalRedirect(ReturnUrl);
        }

        ProviderDisplayName = info.ProviderDisplayName;
        Input.Email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        return Page();
    }

    public async Task<IActionResult> OnPostConfirmationAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            ErrorMessage = "Error loading external login information during confirmation.";
            return RedirectToPage("./Login", new { ReturnUrl });
        }

        if (!ModelState.IsValid)
        {
            ProviderDisplayName = info.ProviderDisplayName;
            return Page();
        }

        var user = CreateUser();
        await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
        user.OIB = Input.OIB;
        user.JMBG = Input.JMBG;

        var result = await _userManager.CreateAsync(user);
        if (result.Succeeded)
        {
            result = await _userManager.AddLoginAsync(user, info);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                return LocalRedirect(ReturnUrl);
            }
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        ProviderDisplayName = info.ProviderDisplayName;
        return Page();
    }

    private AppUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<AppUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(AppUser)}'. Ensure it has a parameterless constructor.");
        }
    }

    private IUserEmailStore<AppUser> GetEmailStore()
    {
        if (_userStore is not IUserEmailStore<AppUser> emailStore)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }

        return emailStore;
    }
}
