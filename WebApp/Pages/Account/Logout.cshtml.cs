using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using WebApp.Data.Account;

namespace WebApp.Pages.Account;

public class LogoutModel : PageModel
{
    private readonly Microsoft.AspNetCore.Identity.SignInManager<User> _signInManager;

    public LogoutModel(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
    }

    // we need a way to trigger this function from HTML UI
    public async Task<IActionResult> OnPostAsync()
    {
        await _signInManager.SignOutAsync();

        //await HttpContext.SignOutAsync("MyCookieAuth");
        // provide schemename => so to know which cookie to kill
        // best to create a constant
        return RedirectToPage("/Account/Login");
    }
}

