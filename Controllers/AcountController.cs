using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private readonly AuthService _authService;
    private readonly string _captchaSecret;

    public AccountController(AuthService authService, IConfiguration config)
    {
        _authService = authService;

        _captchaSecret = config["Captcha:SecretKey"]
            ?? throw new InvalidOperationException(
                "Captcha Secret Key is missing from appsettings.json");
    }

    // ----------------------------
    // Login
    // ----------------------------
    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var role = await _authService.LoginAsync(model);

        if (!string.IsNullOrWhiteSpace(role))
        {
            HttpContext.Session.SetString("UserRole", role);
            HttpContext.Session.SetString("UserEmail", model.Email);

            return RedirectToAction("Index", "Books");
        }

        ModelState.AddModelError("", "Invalid email or password");
        return View(model);
    }

    // ----------------------------
    // Register
    // ----------------------------
    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var token = Request.Form["cf-turnstile-response"].ToString();

        if (string.IsNullOrWhiteSpace(token))
        {
            ModelState.AddModelError("", "Verification failed: no token received.");
            return View(model);
        }

        using var httpClient = new HttpClient();

        var values = new Dictionary<string, string>
        {
            { "secret", _captchaSecret },
            { "response", token }
        };

        var response = await httpClient.PostAsync(
            "https://challenges.cloudflare.com/turnstile/v0/siteverify",
            new FormUrlEncodedContent(values));

        var jsonString = await response.Content.ReadAsStringAsync();

        var result = System.Text.Json.JsonSerializer.Deserialize<CloudFlareViewModel>(
            jsonString,
            new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (result == null || !result.Success)
        {
            ModelState.AddModelError("", "Bot verification failed.");
            return View(model);
        }

        bool success = await _authService.RegisterAsync(model);

        if (success)
            return RedirectToAction(nameof(Login));

        ModelState.AddModelError("", "Registration failed");
        return View(model);
    }

    // ----------------------------
    // Logout
    // ----------------------------
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();

        HttpContext.Session.Clear();

        return RedirectToAction(nameof(Login));
    }

    // ----------------------------
    // Forgot Password
    // ----------------------------
    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var success = await _authService.ForgotPasswordAsync(model);

        if (!success)
        {
            ModelState.AddModelError(
                "",
                "User not found or failed to send reset link.");

            return View(model);
        }

        ViewBag.Message = "Check your email for reset link.";
        return View();
    }

    // ----------------------------
    // Reset Password
    // ----------------------------
    [HttpGet]
    public IActionResult ResetPassword(string email, string token)
    {
        return View(new ResetPasswordModel
        {
            Email = email,
            Token = token
        });
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var success = await _authService.ResetPasswordAsync(model);

        if (!success)
        {
            ModelState.AddModelError("", "Failed to reset password.");
            return View(model);
        }

        TempData["Message"] =
            "Password reset successfully. Please login.";

        return RedirectToAction(nameof(Login));
    }
}