namespace ExpenseTracker.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = "#6366f1";
    public int? UserId { get; set; }
    public User? User { get; set; }

    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
