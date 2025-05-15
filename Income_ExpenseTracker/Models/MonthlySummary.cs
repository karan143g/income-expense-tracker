namespace PersonalExpenseTracker.Models
{
    public class MonthlySummary
    {
        public string Month { get; set; }  // e.g., "2025-01"
        public decimal Income { get; set; }
        public decimal Expense { get; set; }

    }
}
