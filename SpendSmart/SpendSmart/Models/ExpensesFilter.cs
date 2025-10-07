using Microsoft.AspNetCore.Mvc.Rendering;

namespace SpendSmart.Models
{
    public class ExpensesFilterModel
    {
        public string SelectedCategory { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<Expense> FilteredProducts { get; set; }
    }
}
