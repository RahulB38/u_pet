using System.Security.Claims;
using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers;

[Authorize]
public class CategoryController : Controller
{
    private readonly AppDbContext _db;

    public CategoryController(AppDbContext db) => _db = db;

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(CustomCategoryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please enter a valid category name.";
            return RedirectToAction("Index", "Dashboard");
        }

        var userId = GetUserId();
        var name = model.Name.Trim();

        var exists = await _db.Categories.AnyAsync(c =>
            c.Name.ToLower() == name.ToLower() && (c.UserId == null || c.UserId == userId));

        if (exists)
        {
            TempData["Error"] = "A category with this name already exists.";
            return RedirectToAction("Index", "Dashboard");
        }

        _db.Categories.Add(new Category
        {
            Name = name,
            Icon = string.IsNullOrWhiteSpace(model.Icon) ? "📌" : model.Icon.Trim(),
            Color = string.IsNullOrWhiteSpace(model.Color) ? "#8b5cf6" : model.Color,
            UserId = userId
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Category \"{name}\" added successfully!";
        return RedirectToAction("Index", "Dashboard");
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
