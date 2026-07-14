using System.Security.Claims;
using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers;

[Authorize]
public class ReminderController : Controller
{
    private readonly AppDbContext _db;

    public ReminderController(AppDbContext db) => _db = db;

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(ReminderViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please fill in the reminder details.";
            return RedirectToAction("Index", "Dashboard");
        }

        _db.Reminders.Add(new Reminder
        {
            Title = model.Title.Trim(),
            Description = model.Description?.Trim(),
            ReminderDate = model.ReminderDate,
            UserId = GetUserId()
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Reminder added successfully!";
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int id)
    {
        var reminder = await _db.Reminders.FirstOrDefaultAsync(r => r.Id == id && r.UserId == GetUserId());
        if (reminder != null)
        {
            reminder.IsCompleted = !reminder.IsCompleted;
            await _db.SaveChangesAsync();
            TempData["Success"] = reminder.IsCompleted ? "Reminder marked as done." : "Reminder marked as pending.";
        }
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var reminder = await _db.Reminders.FirstOrDefaultAsync(r => r.Id == id && r.UserId == GetUserId());
        if (reminder != null)
        {
            _db.Reminders.Remove(reminder);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Reminder deleted.";
        }
        return RedirectToAction("Index", "Dashboard");
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
