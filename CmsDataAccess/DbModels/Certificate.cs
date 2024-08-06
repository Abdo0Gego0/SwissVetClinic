using CmsDataAccess.Utils.FilesUtils;
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
    public class Certificate 
    {
        [Key]
        public Guid Id { get; set; }
        public List<CertificateTranslation>? CertificateTranslation { get; set; }



        [ForeignKey("Doctor")]
        [Display(Name = nameof(Messages.Doctor), ResourceType = typeof(Messages))]


        public Guid? DoctorId { get; set; }

        [Display(Name = nameof(Messages.ImageName), ResourceType = typeof(Messages))]

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

        public Certificate GetFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                Certificate Medic = context.Certificate
                    .Include(a => a.CertificateTranslation)
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
                Certificate certificate=this.GetFromDb();

                string uniqueFileName = FileHandler.SaveUploadedFile(ImageFile);

                certificate.ImageName= uniqueFileName;

                context.Certificate.Add(this);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public string SaveCertificateImage()
        {
            if (ImageFile!=null)
            {
                string uniqueFileName = FileHandler.SaveUploadedFile(ImageFile);
                ImageName = uniqueFileName;
                return uniqueFileName;
            }
            return "";
        }


        public bool DeleteFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            try
            {
                Certificate temp= GetFromDb();

                FileHandler.DeleteImageFile(temp.ImageName);

                context.Certificate.Remove(temp);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Address GetModelByLnag(string langCode = "en-US")
        {
            langCode = langCode.ToLower();

            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                Address Medic = context.Address
                    .Include(a => a.AddressTranslation.Where(a=>a.LangCode==langCode))
                    .FirstOrDefault(a => a.Id == Id);

                return Medic;
            }
            catch (Exception ex)
            {
                return null;
            }
        }




    }
}
