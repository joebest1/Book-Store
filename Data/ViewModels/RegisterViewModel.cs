using System.ComponentModel.DataAnnotations;

public class RegisterModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
