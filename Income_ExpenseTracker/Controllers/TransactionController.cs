using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using PersonalExpenseTracker.Models;
using Microsoft.Extensions.Configuration;

namespace PersonalExpenseTracker.Controllers
{
    public class TransactionController : Controller
    {
        private readonly string _connectionString;

        public TransactionController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TrackerDbContext");
        }
        public IActionResult Index()
        {
            List<Transaction> transactions = new List<Transaction>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetAllTransactions_prc", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    transactions.Add(new Transaction
                    {
                        TransactionId = (int)reader["TransactionId"],
                        Amount = (decimal)reader["Amount"],
                        Type = reader["Type"].ToString(),
                        CategoryId = (int)reader["CategoryId"],
                        CategoryName = reader["CategoryName"].ToString(),
                        Description = reader["Description"].ToString(),
                        TransactionDate = (DateTime)reader["TransactionDate"]
                    });
                }
            }

            return View(transactions);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = GetCategories();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = GetCategories();
                return View(transaction);
            }

            string categoryType = GetCategoryTypeById(transaction.CategoryId);

            if (categoryType == null)
            {
                ModelState.AddModelError("CategoryId", "Invalid category.");
            }
            else if (categoryType != transaction.Type)
            {
                ModelState.AddModelError("CategoryId", $"Selected category is not a {transaction.Type} category.");
            }

            // Insert via stored procedure
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("AddTransaction_prc", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Amount", transaction.Amount);
                cmd.Parameters.AddWithValue("@Type", transaction.Type);
                cmd.Parameters.AddWithValue("@CategoryId", transaction.CategoryId);
                cmd.Parameters.AddWithValue("@Description", transaction.Description ?? string.Empty);
                cmd.Parameters.AddWithValue("@TransactionDate", transaction.TransactionDate);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        private List<Category> GetCategories()
        {
            List<Category> categories = new List<Category>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetAllCategories_prc", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    categories.Add(new Category
                    {
                        CategoryId = (int)reader["CategoryId"],
                        Name = reader["Name"].ToString(),
                        Type = reader["Type"].ToString()
                    });
                }
            }

            return categories;
        }

        private string GetCategoryTypeById(int categoryId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT Type FROM Categories WHERE CategoryId = @CategoryId", conn);
                cmd.Parameters.AddWithValue("@CategoryId", categoryId);

                conn.Open();
                var result = cmd.ExecuteScalar();
                return result?.ToString();
            }
        }

        [HttpGet]
        public JsonResult GetCategoriesByType(string type)
        {
            List<Category> filteredCategories = new List<Category>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetAllCategories_prc", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["Type"].ToString() == type)
                    {
                        filteredCategories.Add(new Category
                        {
                            CategoryId = (int)reader["CategoryId"],
                            Name = reader["Name"].ToString(),
                            Type = reader["Type"].ToString()
                        });
                    }
                }
            }

            return Json(filteredCategories);
        }

        public IActionResult Summary()
        {
            decimal totalIncome = 0, totalExpense = 0;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetTransactionSummary_prc", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string type = reader["Type"].ToString();
                    decimal total = (decimal)reader["Total"];

                    if (type == "Income") totalIncome = total;
                    else if (type == "Expense") totalExpense = total;

                   
                }
                reader.Close();
            }

            var summary = new TransactionSummaryViewModel
            {
                TotalIncome = totalIncome,
                TotalExpense = totalExpense
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd2 = new SqlCommand("GetMonthlySummary_prc", conn);
                cmd2.CommandType = System.Data.CommandType.StoredProcedure;

                conn.Open();
                SqlDataReader reader2 = cmd2.ExecuteReader();
                var temp = new Dictionary<string, MonthlySummary>();

                while (reader2.Read())
                {
                    string month = reader2.GetString(0);
                    string type = reader2.GetString(1);
                    decimal total = reader2.GetDecimal(2);

                    if (!temp.ContainsKey(month))
                    {
                        temp[month] = new MonthlySummary
                        {
                            Month = month,
                            Income = 0,
                            Expense = 0
                        };
                    }

                    if (type == "Income")
                        temp[month].Income = total;
                    else if (type == "Expense")
                        temp[month].Expense = total;
                }

                var monthlyData = temp.Values.ToList();
                ViewBag.MonthlyLabels = monthlyData.Select(m => m.Month).ToList();
                ViewBag.MonthlyIncome = monthlyData.Select(m => m.Income).ToList();
                ViewBag.MonthlyExpense = monthlyData.Select(m => m.Expense).ToList();
            }

            return View(summary);
        }

    }
}
