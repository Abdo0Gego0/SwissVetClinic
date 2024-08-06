using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CmsDataAccess.DbModels
{
    public class ProductCategories
    {
        [Key]
        public Guid Id { get; set; }

        public List<ProductCategoriesTranslation>? ProductCategoriesTranslation { get; set; }

        [ForeignKey("MedicalCenter")]
        public Guid MedicalCenterId { get; set; }

        public decimal MainCategory { get; set; }

        public string? ImageName { get; set; } = string.Empty;

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        [NotMapped]
        public string? Image64 { get; set; }

        [NotMapped]
        public string? ImageFullPath
        {
            get
            {
                var apiUrl = "http://example.com/api/"; // Replace with actual URL
                return $"{apiUrl}pImages/{ImageName}";
            }
        }

        private readonly ApplicationDbContext _context;

        // Parameterless constructor required by some frameworks (e.g., Kendo UI)
        public ProductCategories()
        {
        }

        // Constructor to use dependency injection to initialize ApplicationDbContext
        public ProductCategories(ApplicationDbContext context)
        {
            _context = context;
        }

        public ProductCategories GetFromDb()
        {
            try
            {
                ProductCategories Medic = _context.ProductCategories
                    .Include(a => a.ProductCategoriesTranslation)
                    .FirstOrDefault(a => a.Id == Id);

                return Medic;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool InsertIntoDb()
        {
            try
            {
                _context.ProductCategories.Add(this);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteFromDb()
        {
            try
            {
                var entity = GetFromDb();
                if (entity != null)
                {
                    _context.ProductCategories.Remove(entity);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public ProductCategories GetModelByLang(string langCode = "en-US")
        {
            langCode = langCode.ToLower();

            try
            {
                ProductCategories Medic = _context.ProductCategories
                    .Include(a => a.ProductCategoriesTranslation.Where(a => a.LangCode == langCode))
                    .FirstOrDefault(a => a.Id == Id);

                return Medic;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override string ToString()
        {
            ProductCategories clinicSpecialty = GetFromDb();
            return string.Join(", ", clinicSpecialty.ProductCategoriesTranslation.Select(a => a.Name));
        }
    }
}
