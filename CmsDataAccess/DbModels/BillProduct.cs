using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CmsDataAccess.DbModels
{
    public class BillProduct
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid BillID { get; set; }
        public Bill Bill { get; set; } = null!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Discount { get; set; } = 0.00m;

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Tax { get; set; } = 0.00m;

        public async Task<string> GetProductNameAsync(ApplicationDbContext context)
        {
            var translation = await context.ProductTranslation
                .FirstOrDefaultAsync(pt => pt.ProductId == ProductId && pt.LangCode == "en-US");
            return translation?.Name ?? string.Empty;
        }
        public static async Task<BillProduct?> GetFromDb(Guid id, ApplicationDbContext context)
        {
            return await context.BillProduct.FirstOrDefaultAsync(bp => bp.Id == id);
        }

        public static async Task InsertIntoDb(BillProduct billProduct, ApplicationDbContext context)
        {
            context.BillProduct.Add(billProduct);
            await context.SaveChangesAsync();
        }

        public static async Task DeleteFromDb(Guid id, ApplicationDbContext context)
        {
            var billProduct = await GetFromDb(id, context);
            if (billProduct != null)
            {
                context.BillProduct.Remove(billProduct);
                await context.SaveChangesAsync();
            }
        }
    }
}
