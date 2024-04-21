using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;

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
            // generate token 
            var confirmToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
            Console.WriteLine($"token: {confirmToken}");

            var confirmLink = Url.PageLink("/Account/ConfirmEmail",
                values: new { userId = user.Id, token = confirmToken });

            Console.WriteLine($"link: {confirmLink}");

            // email message
            var message = new MailMessage("fromTest@example.com",
                user.Email,
                "Email Confirmation", // subject
                $"Link to confirm email: {confirmLink}" // body
            );

            // email client + send email
            using (var emailClient = new SmtpClient(host: "localhost", port: 1025))
            {
                // emailClient.Crednetials = new NetworkCredential(email, password);
                await emailClient.SendMailAsync(message);
            }

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