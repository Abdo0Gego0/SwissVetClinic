using CmsDataAccess.DbModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Parser;

namespace CmsDataAccess.Utils.ProductUtil
{
    public static class ProductHanlder
    {
        public static object GetProductById(Guid id, Guid? userId, string lang = "en-US")
        {
            List<CmsDataAccess.DbModels.Product> prods = new List<CmsDataAccess.DbModels.Product>();

            ApplicationDbContext cmsContext = new ApplicationDbContext();

            prods = cmsContext.Product
                .Include(a => a.ProductTranslation.Where(a => a.LangCode == lang))
                .Include(a => a.SubProduct).ThenInclude(a => a.SubproductCharacteristics)
                .Include(a => a.SubProduct).ThenInclude(a => a.SubProductImage)
                .Where(a => a.Id == id)
                .ToList();



            List<object> res = new List<object>();


            foreach (var item in prods)
            {

                List<object> sub = new List<object>();


                foreach (var su in item.SubProduct)
                {

                    List<SubproductCharacteristicsTranslation> translations = new List<SubproductCharacteristicsTranslation>();

                    foreach (var tr in su.SubproductCharacteristics)
                    {
                        translations.AddRange(cmsContext.SubproductCharacteristicsTranslation.Where(a => a.SubproductCharacteristicsId == tr.Id && a.LangCode == lang));
                    }


                    sub.Add(
                        new
                        {
                            Id = su.Id,
                            Images = su.SubProductImage.Select(a => a.ImageFullPath).ToList(),
                            Quantity = su.Quantity,
                            Price = su.Price,
                            Characteristics = translations.Select(a => a.Description),
                            Avialable = su.Quantity > 0 ? true : false,
                            IsInBasket = ProductHanlder.IsSubProductInBasket(su.Id, userId),
                            IsInFavourite = IsInFavourite(item.Id, userId) // Check product favourite status
                        }
                        );
                }

                res.Add(
                    new
                    {
                        Id = item.Id,
                        Name = item.ProductTranslation[0].Name,
                        Description = item.ProductTranslation[0].Description,
                        SKU = item.ProductTranslation[0].SKU,
                        KeyWords = item.ProductTranslation[0].KeyWords,
                        CategoryId = item.ProductCategoriesId,
                        Details = sub,
                        //IsInBasket = IsInBasket(id,userId),
                        //IsInFavourite = IsInFavourite(id,userId),
                        Review = item.GetReview(),

                    }

                    );
            }

            return res.FirstOrDefault();
        }

        public static object GetSubProductById(Guid id, Guid? userId, string lang = "en-US")
        {
            List<CmsDataAccess.DbModels.Product> prods = new List<CmsDataAccess.DbModels.Product>();

            ApplicationDbContext cmsContext = new ApplicationDbContext();

            Guid prodId = cmsContext.SubProduct.Find(id).ProductId;

            prods = cmsContext.Product
                .Include(a => a.ProductTranslation.Where(a => a.LangCode == lang))
                .Include(a => a.SubProduct).ThenInclude(a => a.SubproductCharacteristics)
                .Include(a => a.SubProduct).ThenInclude(a => a.SubProductImage)
                .Where(a => a.Id == prodId)
                .ToList();


            List<object> res = new List<object>();


            foreach (var item in prods)
            {

                List<object> sub = new List<object>();


                foreach (var su in item.SubProduct)
                {

                    List<SubproductCharacteristicsTranslation> translations = new List<SubproductCharacteristicsTranslation>();

                    foreach (var tr in su.SubproductCharacteristics)
                    {
                        translations.AddRange(cmsContext.SubproductCharacteristicsTranslation.Where(a => a.SubproductCharacteristicsId == tr.Id && a.LangCode == lang));
                    }


                    sub.Add(
                        new
                        {
                            Id = su.Id,
                            Images = su.SubProductImage.Select(a => a.ImageFullPath).ToList(),
                            Quantity = su.Quantity,
                            Price = su.Price,
                            Characteristics = translations.Select(a => a.Description),
                            Avialable = su.Quantity > 0 ? true : false,
                            IsInBasket = ProductHanlder.IsSubProductInBasket(su.Id, userId),
                            IsInFavourite = IsInFavourite(item.Id, userId),

                        }
                        );
                }

                res.Add(
                    new
                    {
                        Id = item.Id,
                        Name = item.ProductTranslation[0].Name,
                        Description = item.ProductTranslation[0].Description,
                        SKU = item.ProductTranslation[0].SKU,
                        KeyWords = item.ProductTranslation[0].KeyWords,
                        CategoryId = item.ProductCategoriesId,
                        Details = sub,
                        //IsInBasket = IsInBasket(id, userId),
                        //IsInFavourite = IsInFavourite(id, userId),
                        Review = item.GetReview(),

                    }

                    );
            }

            return res.FirstOrDefault();
        }

        public static double? GetAvailableQuantityOfSubProduct(Guid id, double Qnt)
        {

            ApplicationDbContext context = new ApplicationDbContext();

            double? currentQnt = context.SubProduct.Find(id).Quantity;

            if (currentQnt == null)
            {
                return 1;
            }

            if (currentQnt < Qnt) // 5<7
            {
                return Qnt - currentQnt; // -2
            }
            else
            {
                return 1;
            }


        }



        public static void DecreaseQuantity(Guid id, double Qnt)
        {

            ApplicationDbContext context = new ApplicationDbContext();

            SubProduct currentProduct = context.SubProduct.Find(id);
            if (currentProduct.Quantity == null)
            {
                return;
            }

            currentProduct.Quantity -= Qnt;

            context.SubProduct.Attach(currentProduct);
            context.Entry(currentProduct).Property(a => a.Quantity).IsModified = true;
            context.SaveChanges();


        }


        public static object IsInBasket(Guid id, Guid? userId)
        {
            if (userId == null)
            {
                return false;
            }

            List<CmsDataAccess.DbModels.Product> prods = new List<CmsDataAccess.DbModels.Product>();

            ApplicationDbContext cmsContext = new ApplicationDbContext();

            List<Guid> ids = cmsContext.Product.Include(a => a.SubProduct).FirstOrDefault(a => a.Id == id).SubProduct.Select(a => a.Id).ToList();

            if (cmsContext.Basket.Any(a => a.PetOwnerId == userId && ids.Contains(a.SubProductId)))
            {
                return true;
            }

            return false;
        }

        public static object IsSubProductInBasket(Guid id, Guid? userId)
        {
            if (userId == null)
            {
                return false;
            }

            List<CmsDataAccess.DbModels.Product> prods = new List<CmsDataAccess.DbModels.Product>();

            ApplicationDbContext cmsContext = new ApplicationDbContext();



            if (cmsContext.Basket.Any(a => a.PetOwnerId == userId && a.SubProductId == id))
            {
                return true;
            }

            return false;
        }

        public static object IsInFavourite(Guid id, Guid? userId)
        {
            if (userId == null)
            {
                return false;
            }

            using (ApplicationDbContext cmsContext = new ApplicationDbContext())
            {
                if (cmsContext.Favourite.Any(a => a.PetOwnerId == userId && a.ProductId == id))
                {
                    return true;
                }
            }

            return false;
        }

        public static object IsSubProductInFavourite(Guid id, Guid? userId)
        {
            if (userId == null)
            {
                return false;
            }

            List<CmsDataAccess.DbModels.Product> prods = new List<CmsDataAccess.DbModels.Product>();

            ApplicationDbContext cmsContext = new ApplicationDbContext();

            if (cmsContext.Favourite.Any(a => a.PetOwnerId == userId && a.SubProductId == id))
            {
                return true;
            }

            return false;
        }

    }
}
