using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CmsDataAccess.DbModels
{
    public class Bill
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime BillDate { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Discount { get; set; } = 0.00m;

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Tax { get; set; } = 0.00m;

        public decimal TotalPrice => TotalAmount - Discount + Tax;

        public List<BillProduct>? BillProducts { get; set; }

        public static async Task<Bill?> GetFromDb(Guid id, ApplicationDbContext context)
        {
            return await context.Bill.FirstOrDefaultAsync(b => b.Id == id);
        }

        public static async Task InsertIntoDb(Bill bill, ApplicationDbContext context)
        {
            context.Bill.Add(bill);
            await context.SaveChangesAsync();
        }

        public static async Task DeleteFromDb(Guid id, ApplicationDbContext context)
        {
            var bill = await context.Bill
                .Include(b => b.BillProducts) // Ensure to include related BillProducts
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bill != null)
            {
                // Remove associated BillProducts
                context.BillProduct.RemoveRange(bill.BillProducts);

                // Remove the bill
                context.Bill.Remove(bill);

                await context.SaveChangesAsync();
            }
        }
    }
}
