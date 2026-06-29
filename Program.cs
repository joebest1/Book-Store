using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication8.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
});

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddSingleton(sp =>
{
    var handler = new HttpClientHandler
    {
        CookieContainer = new System.Net.CookieContainer(),
        UseCookies = true
    };

    return new HttpClient(handler)
    {
        BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7265/")
    };
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<CartService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    if (!roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
        roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();

    if (!roleManager.RoleExistsAsync("User").GetAwaiter().GetResult())
        roleManager.CreateAsync(new IdentityRole("User")).GetAwaiter().GetResult();

    var adminEmail = "admin@example.com";
    var adminUser = userManager.FindByEmailAsync(adminEmail).GetAwaiter().GetResult();
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        userManager.CreateAsync(adminUser, "Admin@123").GetAwaiter().GetResult();
        userManager.AddToRoleAsync(adminUser, "Admin").GetAwaiter().GetResult();
    }

    var userEmail = "user@example.com";
    var normalUser = userManager.FindByEmailAsync(userEmail).GetAwaiter().GetResult();
    if (normalUser == null)
    {
        normalUser = new IdentityUser
        {
            UserName = userEmail,
            Email = userEmail,
            EmailConfirmed = true
        };

        userManager.CreateAsync(normalUser, "User@123").GetAwaiter().GetResult();
        userManager.AddToRoleAsync(normalUser, "User").GetAwaiter().GetResult();
    }
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (errorFeature != null)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(errorFeature.Error, "Unhandled exception caught by middleware");

            await context.Response.WriteAsJsonAsync(new
            {
                status = 500,
                message = "Something went wrong, please try again later."
            });
        }
    });
});

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
