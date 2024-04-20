using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<IdentityUser> signInManager;

    public LoginModel(SignInManager<IdentityUser> signInManager)
    {
        this.signInManager = signInManager;
    }

    [BindProperty]
    public CredentialViewModel Credential { get; set; } = new CredentialViewModel();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        // sign in
        var result = await signInManager.PasswordSignInAsync(
            this.Credential.Email,
            this.Credential.Password,
            this.Credential.RememberMe,
            false
        );
        if (result.Succeeded)
        {
            return RedirectToPage("/Index");
        }
        else
        {
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("Login", "You are locked out.");
            }
            else
            {
                ModelState.AddModelError("Login", "Failed to log in.");
            }
            return Page();
        }
    }
}


public class CredentialViewModel
{
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; }
}