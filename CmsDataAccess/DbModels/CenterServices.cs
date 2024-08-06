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

namespace CmsDataAccess.DbModels
{
    public class CenterServices
    {
        [Key]
        public Guid Id { get; set; }
        public List<CenterServicesTranslation>? CenterServicesTranslation { get; set; }

        [ForeignKey("MedicalCenter")]
        public Guid MedicalCenterId { get; set; }


       [Display(Name = nameof(Messages.Price), ResourceType = typeof(Messages))]
        public float Price{ get; set; }
        public bool IsDeleted { get; set; } = false;




        public string? ImageName { get; set; } = "";


        public string? ImageFullPath
        {
            get
            {
                return new ApplicationDbContext().MySystemConfiguration.FirstOrDefault().ApiUrl + "pImages/" + ImageName;

            }
        }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }


        [NotMapped]
        public string? Image64 { get; set; }



        public CenterServices GetFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                CenterServices Medic = context.CenterServices
                    .Include(a => a.CenterServicesTranslation)
                    .FirstOrDefault(a => a.Id == Id);

                return Medic;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool InsertIntoDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            try
            {
                context.CenterServices.Add(this);
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
                context.CenterServices.Remove(GetFromDb());
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
                CenterServices item = GetFromDb();
                item.IsDeleted = true;
                context.CenterServices.Attach(item);
                context.Entry(item).State = EntityState.Modified;
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public CenterServices GetModelByLnag(string langCode = "en-US")
        {
            langCode = langCode.ToLower();

            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                CenterServices Medic = context.CenterServices
                    .Include(a => a.CenterServicesTranslation.Where(a => a.LangCode == langCode))
                    .FirstOrDefault(a => a.Id == Id);

                return Medic;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public override string ToString()
        {
            
            CenterServices CenterServices = GetFromDb();

            return string.Join(" " ,CenterServices.CenterServicesTranslation.Select(a=>a.Name));

        }





    }
}
