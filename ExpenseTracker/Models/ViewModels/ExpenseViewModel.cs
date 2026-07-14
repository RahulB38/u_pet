using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models.ViewModels;

public class ExpenseViewModel
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required, Range(0.01, 9999999)]
    public decimal Amount { get; set; }

    [Required, Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Required, Display(Name = "Date"), DataType(DataType.Date)]
    public DateTime ExpenseDate { get; set; } = DateTime.Today;

    public string? Notes { get; set; }
}
