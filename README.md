#  Personal Expense Tracker

A simple full-stack MVC web application to track personal income and expenses, built with **ASP.NET Core 8 (MVC)** and **SQL Server** using **Stored Procedures** for database operations.

## Features

- Add, edit, and delete transactions
- Categorize transactions by type (Income / Expense)
- Dynamic category filtering based on transaction type
- View transaction summary with total income, expense, and balance
- Monthly income vs. expense bar chart using Chart.js
- Responsive UI styled with Bootstrap

## Technologies Used

- ASP.NET Core 8 MVC
- SQL Server (with Stored Procedures)
- Bootstrap 5
- jQuery (for dynamic category filtering)
- Chart.js (for data visualization)

## Project Structure

- `Controllers/` – MVC Controllers (TransactionController)
- `Models/` – Transaction, Category, Summary ViewModels
- `Views/Transaction/` – Pages like Index, Create, Summary
- `wwwroot/` – Static files (CSS, JS, etc.)
- `appsettings.json` – Connection strings and configs

## Setup Instructions

1. **Clone the repository**

   ```bash
   git clone https://github.com/yourusername/income-expense-tracker.git
   cd income-expense-tracker

2. **Configure the database**

   - Run the SQL scripts found in the folder:

     - `CreateTables.sql` – creates `Category` and `Transaction` tables
     - `CreateProcedures.sql` – adds stored procedures for CRUD operations and summary reports
     - *(Optional)* `SeedData.sql` – inserts sample data for testing

   - Update your connection string in `appsettings.json`:

     ```json
     "ConnectionStrings": {
       "TrackerDbContext": "Server=YOUR_SERVER;Database=TrackerDB;Trusted_Connection=True;"
     }
     ```

   
