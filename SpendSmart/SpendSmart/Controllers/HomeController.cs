using Microsoft.AspNetCore.Mvc;
using SpendSmart.Models;
using SpendSmart.Models.ViewModels;
using System.Diagnostics;
using System.Globalization;
using ClosedXML.Excel;

namespace SpendSmart.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly SpendSmartDbContext _context;

        public HomeController(ILogger<HomeController> logger,SpendSmartDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Expenses()
        {
          
            var allExpenses = _context.Expenses.ToList();
            var totalExpenses = allExpenses.Sum(x => x.Value);
            ViewBag.Expenses = totalExpenses; 

            ExpensesViewModel expensesViewModel = new ExpensesViewModel();
            expensesViewModel.Months = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames.Take(12).ToList();
            expensesViewModel.Years  = Enumerable.Range(0, 5).Select(i => (DateTime.Now.Year) - i).ToList();

            expensesViewModel.selectedMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month);
            expensesViewModel.selectedYear = DateTime.Now.Year;
            expensesViewModel.Expenses = allExpenses.Where(e => e.Date.Year == expensesViewModel.selectedYear && e.Date.Month == DateTime.ParseExact(expensesViewModel.selectedMonth, "MMMM", CultureInfo.CurrentCulture).Month)
            .ToList();

            return View(expensesViewModel);
        }

        public IActionResult CreateEditExpense(int? id)
        {
            return View();
        }

        public IActionResult DeleteExpense(int? id)
        {
            var expenseInDb = _context.Expenses.SingleOrDefault(expense => expense.Id == id);
            _context.Expenses.Remove(expenseInDb);
            _context.SaveChanges();
            return RedirectToAction("Expenses");
        }
        public IActionResult EditExpense(int? id)
        {
            var expenseInDb = _context.Expenses.SingleOrDefault(expense => expense.Id == id);
            return View(expenseInDb);
        }
        public IActionResult CreateEditExpenseFrom(Expense model)
        {
            if (model.Id==0)
            {
                _context.Expenses.Add(model);
            }
            else
            {
                _context.Expenses.Update(model);
            }
                _context.SaveChanges();
            return RedirectToAction("Expenses");
        }

        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ExpenseFilter()
        {
            var expenses = _context.Expenses.ToList();
            ExpensesViewModel expensesViewModel = new ExpensesViewModel();
            expensesViewModel.Expenses = expenses.Where(e => e.Date.Year == expensesViewModel.selectedYear && e.Date.Month == DateTime.ParseExact(expensesViewModel.selectedMonth, "MMMM", CultureInfo.CurrentCulture).Month)
            .ToList();

            return View(expenses);
        }


        public IActionResult DownloadReport()
        {

            var expenses = _context.Expenses.ToList();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Expenses Report");

                // Add headers
                worksheet.Cell(1, 1).Value = "Date";
                worksheet.Cell(1, 2).Value = "Value";
                worksheet.Cell(1, 3).Value = "Description";

                // Add data rows
                int row = 2;
                foreach (var expense in expenses)
                {
                    worksheet.Cell(row, 1).Value = expense.Date.ToString();
                    worksheet.Cell(row, 2).Value = expense.Value;
                    worksheet.Cell(row, 3).Value = expense.Description;
                    row++;
                }

                // Adjust column widths to fit content
                worksheet.Columns().AdjustToContents();
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    // Return the file
                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Expenses_Report.xlsx"
                    );
                }
            }

        }
        private void CreateExcel()
        {
           
        }

    }
}
