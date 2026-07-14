using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models.ViewModels;

public class CustomCategoryViewModel
{
    [Required, MaxLength(50), Display(Name = "Category Name")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(10), Display(Name = "Icon (emoji)")]
    public string Icon { get; set; } = "📌";

    [Display(Name = "Color")]
    public string Color { get; set; } = "#8b5cf6";
}
