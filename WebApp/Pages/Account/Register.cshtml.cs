using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;

using WebApp.Data.Account;
using WebApp.Services;

namespace WebApp.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;

    public RegisterModel(UserManager<User> userManager, IEmailService emailService)
    {
        _userManager = userManager;
        _emailService = emailService;
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
        var user = new User
        {
            Email = RegisterViewModel.Email,
            UserName = RegisterViewModel.Email,
            Department = RegisterViewModel.Department,
            Position = RegisterViewModel.Position
        };
        // Use UserManager to store user in DB
        var result = await _userManager.CreateAsync(user, RegisterViewModel.Password);
        if (result.Succeeded)
        {
            // generate token 
            var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            // return Redirect(Url.PageLink(pageName: "/Account/ConfirmEmail",
            //     values: new { userId = user.Id, token = confirmToken }) ?? "");

            var confirmLink = Url.PageLink("/Account/ConfirmEmail",
                values: new { userId = user.Id, token = confirmToken });
            Console.WriteLine($"link: {confirmLink}");

            await _emailService.SendAsync("fromTest@example.com",
                user.Email,
                "Account Confirmation",
                $"Linked to confirm email: {confirmLink}");

            /* refactored to email service
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
            */

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
    [Required]
    public string Department { get; set; } = string.Empty;
    [Required]
    public string Position { get; set; } = string.Empty;
}