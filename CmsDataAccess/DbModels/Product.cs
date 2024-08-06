using CmsDataAccess.ModelsDto;
using CmsResources;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CmsDataAccess.DbModels
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("ProductCategories")]
        [Display(Name = nameof(Messages.Category), ResourceType = typeof(Messages))]
        public Guid ProductCategoriesId { get; set; }

        [Display(Name = nameof(Messages.IsDeleted), ResourceType = typeof(Messages))]
        public bool IsDeleted { get; set; } = false;

        [Display(Name = nameof(Messages.Active), ResourceType = typeof(Messages))]
        public bool IsVisible { get; set; } = true;

        [Display(Name = nameof(Messages.CreateDate), ResourceType = typeof(Messages))]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        public DateOnly? ExpireDate { get; set; }

        public string? Type { get; set; }

        public int? MainCategory { get; set; }

        public List<ProductTranslation>? ProductTranslation { get; set; }
        public List<SubProduct>? SubProduct { get; set; }
        public ICollection<ReceiptProduct> ReceiptProduct { get; set; }
        public List<BillProduct>? BillProducts { get; set; }

        public Product GetFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            try
            {
                Product Medic = context.Product
                    .Include(a => a.ProductTranslation)
                    .Include(a => a.SubProduct).ThenInclude(a => a.SubproductCharacteristics).ThenInclude(a => a.SubproductCharacteristicsTranslation)
                    .Include(a => a.SubProduct).ThenInclude(a => a.SubProductImage)
                    .FirstOrDefault(a => a.Id == Id);
                return Medic;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public double? GetReview()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            if (!context.ProductReview.Any(a => a.ProductId == Id))
            {
                return null;
            }

            return context.ProductReview.Where(a => a.ProductId == Id).Average(a => a.Value);

        }

        [Display(Name = nameof(Messages.Review), ResourceType = typeof(Messages))]

        public double? Review
        {

            get
            {
                ApplicationDbContext context = new ApplicationDbContext();

                if (!context.ProductReview.Any(a => a.ProductId == Id))
                {
                    return null;
                }

                return context.ProductReview.Where(a => a.ProductId == Id).Average(a => a.Value);
            }


        }


        public bool InsertIntoDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            try
            {
                context.Product.Add(this);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
        public bool DeleteFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            try
            {
                context.Product.Remove(GetFromDb());
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool SoftDelete()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            try
            {
                Product prod = GetFromDb();
                prod.IsDeleted = true;
                context.Product.Attach(prod);
                context.Entry(prod).Property(a => a.IsDeleted).IsModified = true;
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public Product GetModelByLnag(string langCode = "en-US")
        {
            langCode = langCode.ToLower();

            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                Product Medic = context.Product
                  .Include(a => a.ProductTranslation.Where(b => b.LangCode == langCode))
                  .Include(a => a.SubProduct).ThenInclude(a => a.SubproductCharacteristics).ThenInclude(a => a.SubproductCharacteristicsTranslation.Where(b => b.LangCode == langCode))
                  .Include(a => a.SubProduct).ThenInclude(a => a.SubProductImage)
                  .FirstOrDefault(a => a.Id == Id);

                return Medic;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public string GetProductName(string langCode = "en-US")
        {
            return GetModelByLnag(langCode).ProductTranslation[0].Name;
        }

    }
}
