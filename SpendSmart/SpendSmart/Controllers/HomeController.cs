using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using MaxMind.GeoIP2;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using SpendSmart.Models;
using SpendSmart.Models.ViewModels;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.Xml;

namespace SpendSmart.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly SpendSmartDbContext _context;
        public HomeController(ILogger<HomeController> logger, SpendSmartDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var expenses = _context.Expenses.ToList();
            
            return View();
        }


        [HttpGet]
        public IActionResult Expenses(int selectedYear, int selectedMonth)
        {
            var allExpenses = _context.Expenses.ToList();
            var totalExpenses = allExpenses.Sum(x => x.Value);

            ExpensesViewModel expensesViewModel = new ExpensesViewModel();
            expensesViewModel.Months = Enumerable.Range(1, 12)
                                    .Select(i => new SelectListItem
                                    {
                                        Value = i.ToString(),
                                        Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i)
                                    }).ToList();
            expensesViewModel.Years = Enumerable.Range(0, 5).Select(i => new SelectListItem
            {
                Value = ((DateTime.Now.Year) - i).ToString(),
                Text = ((DateTime.Now.Year) - i).ToString()
            }).ToList();

            expensesViewModel.SelectedYear = selectedYear;
            expensesViewModel.SelectedMonth = selectedMonth;
            if (expensesViewModel.SelectedYear==0 && expensesViewModel.SelectedMonth==0)
            {
                expensesViewModel.SelectedYear = DateTime.Now.Year;
                expensesViewModel.SelectedMonth = DateTime.Now.Month;
            }
            expensesViewModel.Expenses = allExpenses.Where(e => e.Date.Year == expensesViewModel.SelectedYear && e.Date.Month == expensesViewModel.SelectedMonth).ToList();
           TempData["Expenses"] = JsonConvert.SerializeObject(expensesViewModel.Expenses);
            ViewBag.Total = expensesViewModel.Expenses.Sum(e => e.Value);
            TempData["Count"] = expensesViewModel.Expenses.Count;
            return View(expensesViewModel);
        }

        public IActionResult CreateEditExpense(int? id)
        {
            if (id != null)
            {
                var expenseInDb = _context.Expenses.SingleOrDefault(expense => expense.Id == id);
                return View(expenseInDb);
            }
            else
            {
                Expense exp = new Expense();
                exp.Date = DateOnly.FromDateTime(DateTime.Today);
                return View(exp);
            } 
        }

        public IActionResult DeleteExpense(int? id)
        {
            var expenseInDb = _context.Expenses.SingleOrDefault(expense => expense.Id == id);
            _context.Expenses.Remove(expenseInDb);
            _context.SaveChanges();
            return RedirectToAction("Expenses", new { selectedYear = expenseInDb.Date.Year, selectedMonth = expenseInDb.Date.Month });
            //return RedirectToAction("Expenses");
        }

        public IActionResult CreateEditExpenseFrom(Expense model)
        {
            if (model.Description != null && model.Value != 0)
            {
                if (model.Id == 0)
                {
                    _context.Expenses.Add(model);
                }
                else
                {
                    _context.Expenses.Update(model);
                }
                _context.SaveChanges();
                
            }
            return RedirectToAction("Expenses", new { selectedYear = model.Date.Year, selectedMonth = model.Date.Month });
            //return RedirectToAction("Expenses");model
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Weather()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ViewData["IpAddress"]= ip.ToString();

                    using var reader = new DatabaseReader("GeoLite2-City.mmdb");
                    var city1 = reader.City(ip.ToString());


                }
            }
            return View();
        }

        //public async Task<string> GetCityFromIpAsync(string ip)
        //{
        //    using HttpClient client = new HttpClient();
        //    var json = await client.GetStringAsync($"https://ipinfo.io/{ip}/json");
        //    var result = JsonSerializer.Deserialize<IpInfoResponse>(json);

        //    return result?.City;
        //}






        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ExpenseFilter()
        {
            var expenses = _context.Expenses.ToList();
            ExpensesViewModel expensesViewModel = new ExpensesViewModel();
            expensesViewModel.Expenses = expenses.Where(e => e.Date.Year == expensesViewModel.SelectedYear && e.Date.Month == expensesViewModel.SelectedMonth)
            .ToList();
            return View(expenses);
        }

        public IActionResult DownloadReport(int selectedYear, int selectedMonth)
        {
            ExpensesViewModel expensesViewModel = new ExpensesViewModel();
            var filename = $"{selectedMonth.ToString() +"_"+ selectedYear.ToString()}_Expenses_Report.xlsx";

            var expenses = _context.Expenses.ToList();
            expenses=expenses.Where(e => e.Date.Year == selectedYear && e.Date.Month == selectedMonth).ToList();
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
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                }
            }
        }

        private void CreateExcel()
        {

        }

    }
}
