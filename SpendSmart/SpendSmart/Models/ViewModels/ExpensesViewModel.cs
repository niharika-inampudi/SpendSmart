namespace SpendSmart.Models.ViewModels
{
    public class ExpensesViewModel
    {
        public string selectedMonth { get; set; }
        public int selectedYear { get; set; }

        public List<string> Months { get; set; }

        public List<int> Years { get; set; }
        public List<Expense> Expenses { get; set; }
    }
}
