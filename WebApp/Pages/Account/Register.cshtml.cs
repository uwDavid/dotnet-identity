using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly UserManager<IdentityUser> userManager;

    public RegisterModel(UserManager<IdentityUser> userManager)
    {
        this.userManager = userManager;
    }

    [BindProperty]
    public RegisterViewModel RegisterViewModel { get; set; } = new RegisterViewModel();


    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        // Validate Email Address (optoinal - b/c it's in configurations)

        // Create User 
        var user = new IdentityUser
        {
            Email = RegisterViewModel.Email,
            UserName = RegisterViewModel.Email
        };
        // Use UserManager to store user in DB
        var result = await userManager.CreateAsync(user, RegisterViewModel.Password);
        if (result.Succeeded)
        {
            return RedirectToPage("/Account/Login");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("Register", error.Description);
            }
            return Page();
        }
    }
}

public class RegisterViewModel
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;
    [Required]
    [DataType(dataType: DataType.Password)]
    public string Password { get; set; } = string.Empty;
}