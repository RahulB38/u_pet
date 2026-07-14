using System.Data.Common;
using ExpenseTracker.Models;
using ExpenseTracker.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data;

public static class DbInitializer
{
    public static void Seed(AppDbContext context)
    {
        context.Database.EnsureCreated();
        UpgradeSchema(context);

        if (!context.Categories.Any(c => c.UserId == null))
        {
            context.Categories.AddRange(
                new Category { Name = "Food", Icon = "🍔", Color = "#f59e0b" },
                new Category { Name = "Transport", Icon = "🚗", Color = "#3b82f6" },
                new Category { Name = "Shopping", Icon = "🛍️", Color = "#ec4899" },
                new Category { Name = "Bills", Icon = "📄", Color = "#6366f1" },
                new Category { Name = "Entertainment", Icon = "🎬", Color = "#10b981" },
                new Category { Name = "Health", Icon = "💊", Color = "#ef4444" },
                new Category { Name = "Other", Icon = "📦", Color = "#94a3b8" }
            );
            context.SaveChanges();
        }

        if (!context.Users.Any(u => u.Email == "demo@expense.com"))
        {
            var demoUser = new User
            {
                FullName = "Demo User",
                Email = "demo@expense.com",
                PasswordHash = PasswordHelper.Hash("demo123")
            };
            context.Users.Add(demoUser);
            context.SaveChanges();

            var categories = context.Categories.Where(c => c.UserId == null).ToList();
            var today = DateTime.Today;
            var sampleExpenses = new[]
            {
                ("Lunch at cafe", 350m, "Food", -1),
                ("Uber ride", 180m, "Transport", -2),
                ("Grocery shopping", 1200m, "Shopping", -3),
                ("Electricity bill", 850m, "Bills", -5),
                ("Netflix subscription", 649m, "Entertainment", -7),
                ("Pharmacy", 420m, "Health", -4),
                ("Coffee", 120m, "Food", 0),
                ("Bus pass", 500m, "Transport", -10)
            };

            foreach (var (title, amount, catName, daysAgo) in sampleExpenses)
            {
                var cat = categories.First(c => c.Name == catName);
                context.Expenses.Add(new Expense
                {
                    Title = title,
                    Amount = amount,
                    CategoryId = cat.Id,
                    UserId = demoUser.Id,
                    ExpenseDate = today.AddDays(daysAgo)
                });
            }

            context.Reminders.AddRange(
                new Reminder { Title = "Pay rent", Description = "Monthly rent due", ReminderDate = today.AddDays(3), UserId = demoUser.Id },
                new Reminder { Title = "Review budget", Description = "Check monthly spending", ReminderDate = today, UserId = demoUser.Id }
            );

            context.SaveChanges();
        }
    }

    private static void UpgradeSchema(AppDbContext context)
    {
        if (context.Database.IsSqlite())
        {
            var userIdColumnExists = SqliteColumnExists(context, "Categories", "UserId");

            if (!userIdColumnExists)
            {
                context.Database.ExecuteSqlRaw(@"
                    ALTER TABLE Categories ADD COLUMN UserId INTEGER NULL;
                ");
            }

            context.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS Reminders (
                    Id INTEGER NOT NULL CONSTRAINT PK_Reminders PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Description TEXT NULL,
                    ReminderDate TEXT NOT NULL,
                    IsCompleted INTEGER NOT NULL DEFAULT 0,
                    CreatedAt TEXT NOT NULL,
                    UserId INTEGER NOT NULL,
                    CONSTRAINT FK_Reminders_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
                );
            ");

            return;
        }

        context.Database.ExecuteSqlRaw(@"
            IF COL_LENGTH('Categories', 'UserId') IS NULL
                ALTER TABLE Categories ADD UserId INT NULL;
        ");

        context.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Reminders')
            CREATE TABLE Reminders (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                Title NVARCHAR(150) NOT NULL,
                Description NVARCHAR(500) NULL,
                ReminderDate DATE NOT NULL,
                IsCompleted BIT NOT NULL DEFAULT 0,
                CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                UserId INT NOT NULL,
                CONSTRAINT FK_Reminders_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
            );
        ");
    }

    private static bool SqliteColumnExists(AppDbContext context, string tableName, string columnName)
    {
        var connection = context.Database.GetDbConnection();
        var shouldClose = connection.State != System.Data.ConnectionState.Open;

        if (shouldClose)
        {
            connection.Open();
        }

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"PRAGMA table_info('{tableName}')";
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var existingColumnName = reader.GetString(1);
                if (string.Equals(existingColumnName, columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
        finally
        {
            if (shouldClose)
            {
                connection.Close();
            }
        }
    }
}
