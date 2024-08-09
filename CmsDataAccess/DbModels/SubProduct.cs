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

        public List<SubProductImage>? SubProductImage { get; set; } = new List<SubProductImage>();
        public List<SubproductCharacteristics> SubproductCharacteristics { get; set; } = new List<SubproductCharacteristics>();

        // Parameterless constructor
        public SubProduct() { }

        // Constructor with ApplicationDbContext injection
        public SubProduct(ApplicationDbContext context)
        {
            _context = context;
        }

        // Dependency injection is handled differently in ASP.NET Core; consider using services instead of injecting into models directly
        private readonly ApplicationDbContext _context;

        public string ParentProductName
        {
            get
            {
                if (_context == null) return "Unknown Product";

                var prod = _context.Product
                    .Include(p => p.ProductTranslation)
                    .FirstOrDefault(p => p.Id == ProductId);

                if (prod != null && prod.ProductTranslation.Any())
                {
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
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Insert Error: {ex.Message}");
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
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Delete Error: {ex.Message}");
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
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Soft Delete Error: {ex.Message}");
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
