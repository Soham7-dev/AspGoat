using AspGoat.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using AspGoat.Models;
using RazorLight;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);

bool csrfLab = builder.Configuration.GetValue<bool>("csrfLab");

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddSingleton(sp =>
{
    var client = new OllamaApiClient(new Uri("http://localhost:11434"));
    client.SelectedModel = builder.Configuration.GetValue<string>("aiModel") ?? "tinyllama:1.1b-chat"; 
    return client;
});


builder.Services.AddSingleton<IRazorLightEngine>(
    new RazorLightEngineBuilder().UseMemoryCachingProvider().Build());

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";

        if (csrfLab)
        {
            // CSRF lab → requires HTTPS in browsers
            // Vulnerable as SamSiteMode.None allows both auth and anti-csrf cookies to be included in cross site request
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        }
        else
        {
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.None;
        }
    });

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddResponseCaching();
    
var app = builder.Build();

// Seeding fresh data into the database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Delete app.db if it already exists
    db.Database.EnsureDeleted();
    // Create app.db and rebuild the entire Schema from scratch
    db.Database.EnsureCreated();

    // Users
    db.Users.AddRange(
        new User
        {
            // Id will be 1 after reset
            UserName = "soham",
            PasswordHash = "5f4dcc3b5aa765d61d8327deb882cf99", // "password" (MD5)
            Email = "soham@example.com",
            LastLoginIP = "103.54.120.77",
            Role = "user"
        },
        new User
        {
            // Id will be 2
            UserName = "admin",
            PasswordHash = "21232f297a57a5a743894a0e4a801fc3", // "admin" (MD5)
            Email = "admin@example.com",
            LastLoginIP = "127.0.0.1",
            Role = "admin"
        },
        new User
        {
            // Id will be 3
            UserName = "guest",
            PasswordHash = "084e0343a0486ff05530df6c705c8bb4", // "guest" (MD5)
            Email = "guest@example.com",
            LastLoginIP = "45.67.88.99",
            Role = "user"
        }
    );
    db.SaveChanges();

    // EmailIds
    db.EmailIds.Add(new EmailId
    {
        // Id will be 1 after reset
        Email = "abc@user.net"
    });
    db.SaveChanges();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseResponseCaching();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
