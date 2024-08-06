using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore; // Ensure this is included

namespace CmsDataAccess.DbModels
{
    public class Receipt
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime ReceiptDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPrice { get; set; }
        public string? CompanyName { get; set; }
        public string? ReceiptPhotoPath { get; set; }
        public string? PhotoName { get; set; }

        // Navigation properties    
        public List<ReceiptProduct>? ReceiptProducts { get; set; }

        [NotMapped]
        public IFormFile? ReceiptPhoto { get; set; }

        public static async Task<Receipt> GetFromDb(Guid id, ApplicationDbContext context)
        {
            return await context.Receipt.FirstOrDefaultAsync(r => r.Id == id);
        }

        public static async Task InsertIntoDb(Receipt receipt, ApplicationDbContext context)
        {
            context.Receipt.Add(receipt);
            await context.SaveChangesAsync();
        }

        public static async Task DeleteFromDb(Guid id, ApplicationDbContext context)
        {
            var receipt = await GetFromDb(id, context);
            if (receipt != null)
            {
                context.Receipt.Remove(receipt);
                await context.SaveChangesAsync();
            }
        }
    }
}
