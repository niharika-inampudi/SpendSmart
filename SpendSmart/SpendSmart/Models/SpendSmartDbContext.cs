using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SpendSmart.Models
{
    public class SpendSmartDbContext:DbContext
    {
        public DbSet<Expense> Expenses { get; set; }

        public SpendSmartDbContext(DbContextOptions<SpendSmartDbContext> options): base(options)
        {
        }
    }

    public class Expense
    {
        public int Id { get; set; }

        [Required]
        [Range(0.01, 1000000, ErrorMessage = "Value must be between 0.01 and 1,000,000.")]
        public decimal Value { get; set; }


        [Required]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Description must be between 5 and 200 characters.")]
        public string Description { get; set; }


        [Required]
        public DateOnly Date { get; set; }
    }
}
