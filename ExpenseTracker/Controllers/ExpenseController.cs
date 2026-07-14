using System.Security.Claims;
using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers;

[Authorize]
public class ExpenseController : Controller
{
    private readonly AppDbContext _db;

    public ExpenseController(AppDbContext db) => _db = db;

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(ExpenseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please fill all required fields correctly.";
            return RedirectToAction("Index", "Dashboard");
        }

        var expense = new Expense
        {
            Title = model.Title.Trim(),
            Amount = model.Amount,
            CategoryId = model.CategoryId,
            ExpenseDate = model.ExpenseDate,
            Notes = model.Notes,
            UserId = GetUserId()
        };

        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Expense added successfully!";
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var expense = await _db.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == GetUserId());
        if (expense != null)
        {
            _db.Expenses.Remove(expense);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Expense deleted.";
        }
        return RedirectToAction("Index", "Dashboard");
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
