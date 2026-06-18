using BookStore.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class AuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // تسجيل الدخول وإرجاع الدور مباشرة
    public async Task<string?> LoginAsync(LoginViewModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/UserApi/login", model);
        if (!response.IsSuccessStatusCode)
            return null;

        var result = await response.Content.ReadFromJsonAsync<UserRoleResponse>();
        Console.WriteLine($"Login returned role: {result?.Role}");
        return result?.Role.Trim('"'); // تشيل أي اقتباسات زائدة
    }

    public async Task<bool> RegisterAsync(RegisterViewModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/UserApi/register", model);
        return response.IsSuccessStatusCode;
    }

    public async Task LogoutAsync()
    {
        await _httpClient.PostAsync("api/UserApi/logout", null);
    }

    // ------------------------------
    // Forgot Password
    // ------------------------------
    public async Task<bool> ForgotPasswordAsync(ForgotPasswordModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/UserApi/forgot-password", model);
        return response.IsSuccessStatusCode;
    }

    // ------------------------------
    // Reset Password
    // ------------------------------
    public async Task<bool> ResetPasswordAsync(ResetPasswordModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/UserApi/reset-password", model);
        return response.IsSuccessStatusCode;
    }
}

// موديل لتلقي الدور من API 
public class UserRoleResponse
{
    public string Role { get; set; } = "";
}
