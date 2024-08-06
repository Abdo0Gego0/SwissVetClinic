using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CmsDataAccess.DbModels
{
    public class SubProduct
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Product")]
        [Display(Name = "Product")]
        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        [Display(Name = "Is Deleted")]
        public bool IsDeleted { get; set; } = false;

        [Display(Name = "Price")]
        public double? Price { get; set; }

        [Display(Name = "Quantity")]
        public double? Quantity { get; set; }

        public List<SubProductImage>? SubProductImage { get; set; }
        public List<SubproductCharacteristics> SubproductCharacteristics { get; set; } = new List<SubproductCharacteristics>();

        // Constructor accepting a DbContext to handle dependency injection
        private readonly ApplicationDbContext _context;

        public SubProduct(ApplicationDbContext context)
        {
            _context = context;
        }

        public string ParentProductName
        {
            get
            {
                var prod = _context.Product
                    .Include(p => p.ProductTranslation) // Corrected property name
                    .FirstOrDefault(p => p.Id == ProductId);

                if (prod != null && prod.ProductTranslation.Any())
                {
                    // Assumes you want the first translation as default or concatenates all available translations
                    return string.Join(" ", prod.ProductTranslation.Select(pt => pt.Name));
                }
                return "Unknown Product";
            }
        }

        public async Task<SubProduct?> GetFromDbAsync(Guid id)
        {
            return await _context.SubProduct
                .Include(sp => sp.SubProductImage)
                .Include(sp => sp.SubproductCharacteristics)
                    .ThenInclude(sc => sc.SubproductCharacteristicsTranslation)
                .FirstOrDefaultAsync(sp => sp.Id == id);
        }

        public async Task<bool> InsertIntoDbAsync()
        {
            try
            {
                _context.SubProduct.Add(this);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteFromDbAsync()
        {
            try
            {
                var entity = await GetFromDbAsync(Id);
                if (entity != null)
                {
                    _context.SubProduct.Remove(entity);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SoftDeleteAsync()
        {
            try
            {
                var entity = await GetFromDbAsync(Id);
                if (entity != null)
                {
                    entity.IsDeleted = true;
                    _context.SubProduct.Update(entity);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<SubProduct?> GetModelByLangAsync(string langCode = "en-US")
        {
            langCode = langCode.ToLower();

            return await _context.SubProduct
                .Include(sp => sp.SubproductCharacteristics)
                    .ThenInclude(sc => sc.SubproductCharacteristicsTranslation
                        .Where(sct => sct.LangCode == langCode))
                .Include(sp => sp.SubProductImage)
                .FirstOrDefaultAsync(sp => sp.Id == Id);
        }
    }
}
