using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using WebApp.Data;
using WebApp.Data.Account;
using WebApp.Services;
using WebApp.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var DbUrl = builder.Configuration.GetConnectionString("Default") ?? string.Empty;
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(DbUrl);
});

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // additional Identity config
    options.Password.RequiredLength = 8;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true; // user must confirm email
})
    //specifies that Identity pkg is using ApplicationDbContext
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    // additional cookie config
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.Configure<SmtpSetting>(builder.Configuration.GetSection("SMTP"));
// for any service that requires the SmtpSetting class 
// builder creates the class on demand + load data from SMTP section

builder.Services.AddSingleton<IEmailService, EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();