using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp_UnderTheHood.Pages;

public class LogoutModel : PageModel
{
    // we need a way to trigger this function from HTML UI
    public async Task<IActionResult> OnPostAsync()
    {
        await HttpContext.SignOutAsync("MyCookieAuth");
        // provide schemename => so to know which cookie to kill
        // best to create a constant
        return RedirectToPage("/Index");
    }
}