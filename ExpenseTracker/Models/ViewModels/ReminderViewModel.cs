using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models.ViewModels;

public class ReminderViewModel
{
    [Required, MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required, Display(Name = "Reminder Date"), DataType(DataType.Date)]
    public DateTime ReminderDate { get; set; } = DateTime.Today;
}
