using Microsoft.AspNetCore.Mvc.Rendering;

namespace SpendSmart.Models.ViewModels
{
    public class ExpensesViewModel
    {
        public int SelectedMonth { get; set; }
        public int SelectedYear { get; set; }

        public List<SelectListItem> Months { get; set; }

        public List<SelectListItem> Years { get; set; }
        public List<Expense> Expenses { get; set; }
    }
}
