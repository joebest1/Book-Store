namespace WebApplication8.Data.ViewModels
{
    public class ForgotPasswordModel
    {
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordModel
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
