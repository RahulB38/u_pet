namespace ExpenseTracker.Models.ViewModels;

public class DashboardViewModel
{
    public string UserName { get; set; } = string.Empty;
    public decimal MonthTotal { get; set; }
    public int TransactionCount { get; set; }
    public string TopCategory { get; set; } = "—";
    public decimal AvgDaily { get; set; }
    public ExpenseViewModel NewExpense { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<ExpenseListItem> RecentExpenses { get; set; } = new();
    public List<ReminderItem> Reminders { get; set; } = new();
}

public class ReminderItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ReminderDate { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsOverdue => !IsCompleted && ReminderDate < DateTime.Today;
    public bool IsToday => ReminderDate.Date == DateTime.Today;
}

public class ExpenseListItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryIcon { get; set; } = string.Empty;
    public string CategoryColor { get; set; } = "#6366f1";
    public DateTime ExpenseDate { get; set; }
}
