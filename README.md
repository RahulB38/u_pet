# ExpenseTracker

Personal expense tracking web application built with **ASP.NET Core MVC**, **C#**, **SQL Server**, **HTML5**, **CSS**, **JavaScript**, and **Bootstrap 5**.

## Features

- User registration & secure login (cookie-based auth, hashed passwords)
- Add, view, filter & delete expenses
- **Custom categories** — users can create their own expense categories
- **Reminders** — add financial reminders shown on the dashboard
- Navbar quick access: Custom Category, Reminder, Recent Transactions
- 7 default expense categories with color-coded badges
- Dashboard with monthly summary cards
- Interactive charts (category breakdown + 6-month trend) via Chart.js
- Demo account with sample data pre-loaded

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core 8 MVC, C# |
| Database | SQL Server (LocalDB) + Entity Framework Core |
| Frontend | HTML5, CSS, JavaScript, Bootstrap 5 |
| Charts | Chart.js |

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (comes with Visual Studio)
- Visual Studio 2022 **or** VS Code

## How to Run

### Option 1 — Visual Studio
1. Open `ExpenseTracker.sln`
2. Press **F5** to run
3. Browser opens at `https://localhost:7001`

### Option 2 — Command Line
```bash
cd ExpenseTracker
dotnet restore
dotnet run
```


