using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Riffdle.Models;

namespace Riffdle.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class RegisterModel : PageModel
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IUserStore<AppUser> _userStore;
    private readonly IUserEmailStore<AppUser> _emailStore;

    public RegisterModel(
        UserManager<AppUser> userManager,
        IUserStore<AppUser> userStore,
        SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _userStore = userStore;
        _signInManager = signInManager;
        _emailStore = GetEmailStore();
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

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

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (ModelState.IsValid)
        {
            var user = CreateUser();
            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
            user.OIB = Input.OIB;
            user.JMBG = Input.JMBG;

            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

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
