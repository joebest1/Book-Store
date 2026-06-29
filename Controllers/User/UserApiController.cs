using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication8.Data.ViewModels;

[ApiController]
[Route("api/[controller]")]
public class UserApiController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService; 

    public UserApiController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        IEmailService emailService) 
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _emailService = emailService; 
    }

    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddClaimsAsync(user, new[]
        {
            new System.Security.Claims.Claim("FirstName", model.FirstName),
            new System.Security.Claims.Claim("LastName", model.LastName)
        });

        var role = string.IsNullOrEmpty(model.Role) ? "User" : model.Role;
        if (!await _roleManager.RoleExistsAsync(role))
            await _roleManager.CreateAsync(new IdentityRole(role));

        await _userManager.AddToRoleAsync(user, role);

        return Ok(new { message = $"User registered successfully with role {role}" });
    }

    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user?.UserName == null)
            return Unauthorized(new { message = "Invalid login attempt" });

        var result = await _signInManager.PasswordSignInAsync(
            user.UserName,
            model.Password,
            isPersistent: true,
            lockoutOnFailure: false
        );

        if (!result.Succeeded)
            return Unauthorized(new { message = "Invalid login attempt" });

        
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "User";

        return Ok(new { role = role, message = "Login successful" });
    }

   
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { message = "Logged out successfully" });
    }

    
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user?.Email == null)
            return BadRequest(new { message = "User not found" });

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

       
        var resetLink = $"https://localhost:7265/Account/ResetPassword?email={user.Email}&token={Uri.EscapeDataString(token)}";



        await _emailService.SendEmailAsync(
            user.Email,
            "Reset Password",
            $"Click here to reset your password: <a href='{resetLink}'>Reset Password</a>"
        );

        return Ok(new { message = "Password reset link has been sent to your email." });
    }



    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return BadRequest(new { message = "User not found" });

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "Password has been reset successfully" });
    }
}
