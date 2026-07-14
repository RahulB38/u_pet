using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models.ViewModels;

public class RegisterViewModel
{
    [Required, Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6), DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required, Compare("Password"), DataType(DataType.Password), Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
