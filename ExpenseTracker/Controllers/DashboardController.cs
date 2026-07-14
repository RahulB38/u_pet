using System.Security.Claims;
using ExpenseTracker.Data;
using ExpenseTracker.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(int? categoryId)
    {
        var userId = GetUserId();
        var now = DateTime.Today;
        var monthStart = new DateTime(now.Year, now.Month, 1);

        var expenses = await _db.Expenses
            .Include(e => e.Category)
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.ExpenseDate)
            .ThenByDescending(e => e.Id)
            .ToListAsync();

        var monthExpenses = expenses.Where(e => e.ExpenseDate >= monthStart && e.ExpenseDate <= now).ToList();
        var monthTotal = monthExpenses.Sum(e => e.Amount);

        var topCat = monthExpenses
            .GroupBy(e => e.Category.Name)
            .OrderByDescending(g => g.Sum(e => e.Amount))
            .Select(g => g.Key)
            .FirstOrDefault() ?? "—";

        var filtered = categoryId.HasValue
            ? expenses.Where(e => e.CategoryId == categoryId.Value)
            : expenses;

        var vm = new DashboardViewModel
        {
            UserName = User.Identity?.Name ?? "User",
            MonthTotal = monthTotal,
            TransactionCount = expenses.Count,
            TopCategory = topCat,
            AvgDaily = now.Day > 0 ? monthTotal / now.Day : 0,
            Categories = await _db.Categories
                .Where(c => c.UserId == null || c.UserId == userId)
                .OrderBy(c => c.Name)
                .ToListAsync(),
            Reminders = await _db.Reminders
                .Where(r => r.UserId == userId && !r.IsCompleted)
                .OrderBy(r => r.ReminderDate)
                .Select(r => new ReminderItem
                {
                    Id = r.Id,
                    Title = r.Title,
                    Description = r.Description,
                    ReminderDate = r.ReminderDate,
                    IsCompleted = r.IsCompleted
                })
                .ToListAsync(),
            NewExpense = new ExpenseViewModel { ExpenseDate = DateTime.Today },
            RecentExpenses = filtered.Take(50).Select(e => new ExpenseListItem
            {
                Id = e.Id,
                Title = e.Title,
                Amount = e.Amount,
                CategoryName = e.Category.Name,
                CategoryIcon = e.Category.Icon,
                CategoryColor = e.Category.Color,
                ExpenseDate = e.ExpenseDate
            }).ToList()
        };

        ViewBag.SelectedCategory = categoryId;
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> ChartData()
    {
        var userId = GetUserId();
        var now = DateTime.Today;
        var monthStart = new DateTime(now.Year, now.Month, 1);

        var monthExpenses = await _db.Expenses
            .Include(e => e.Category)
            .Where(e => e.UserId == userId && e.ExpenseDate >= monthStart)
            .ToListAsync();

        var byCategory = monthExpenses
            .GroupBy(e => new { e.Category.Name, e.Category.Color })
            .Select(g => new { g.Key.Name, g.Key.Color, Total = g.Sum(e => e.Amount) })
            .ToList();

        var trend = new List<object>();
        for (int i = 5; i >= 0; i--)
        {
            var d = now.AddMonths(-i);
            var start = new DateTime(d.Year, d.Month, 1);
            var end = start.AddMonths(1).AddDays(-1);
            var total = await _db.Expenses
                .Where(e => e.UserId == userId && e.ExpenseDate >= start && e.ExpenseDate <= end)
                .SumAsync(e => (decimal?)e.Amount) ?? 0;
            trend.Add(new { label = start.ToString("MMM"), total });
        }

        return Json(new { categories = byCategory, trend });
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
