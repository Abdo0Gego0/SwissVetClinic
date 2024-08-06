using CmsDataAccess.ModelsDto;
using CmsResources;
using Microsoft.AspNetCore.Http;
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
    public class SubproductCharacteristics
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("SubProduct")]
        public Guid SubProductId { get; set; }

        public bool IsDeleted { get; set; } = false;


        public List<SubproductCharacteristicsTranslation>? SubproductCharacteristicsTranslation { get; set;}

        public SubproductCharacteristics GetFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            try
            {
                SubproductCharacteristics Medic = context.SubproductCharacteristics
                    .Include(a => a.SubproductCharacteristicsTranslation)
                    .FirstOrDefault(a => a.Id == Id);
                return Medic;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool DeleteFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            try
            {
                context.SubproductCharacteristics.Remove(GetFromDb());
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
                SubproductCharacteristics prod = GetFromDb();
                prod.IsDeleted = true;
                context.SubproductCharacteristics.Attach(prod);
                context.Entry(prod).Property(a => a.IsDeleted).IsModified = true;
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



    }


    public class SubproductCharacteristicsTranslationViewModel
    {
        public string DescEn  { get; set; }
        public string DescAr  { get; set; }
    }

}
