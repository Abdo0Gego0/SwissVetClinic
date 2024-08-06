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
    public class MySystemConfiguration
    {
        [Key]
        public Guid Id { get; set; }

        [Display(Name = nameof(Messages.ApiUrl), ResourceType = typeof(Messages))]
        public string? ApiUrl { get; set; } = "";
        [Display(Name = nameof(Messages.TimeZone), ResourceType = typeof(Messages))]


        public string? SystemTimeZone {  get; set; }
        [Display(Name = nameof(Messages.UseEmailVerfication), ResourceType = typeof(Messages))]


        public bool UseEmailVerfication{ get; set; }=false;

        [Display(Name = nameof(Messages.UseEmailVerfication), ResourceType = typeof(Messages))]

        public bool UseCashPayementForSubscriber{ get; set; }=true;

        [Display(Name = "Stripe Private Key")]

        public string? StripePrivateKey { get; set; } = "";




        [Display(Name = "WhatsApp")]
        public string? WhatsApp { get; set; } = "";






        [Display(Name = "Stripe Public Key")]

        public string? StripePublicKey { get; set; } = "";

        [Display(Name = nameof(Messages.ImageName), ResourceType = typeof(Messages))]

        public string? ImageName { get; set; } = ""; 
        public string? ImageFullPath
        {
            get
            {
                return new ApplicationDbContext().MySystemConfiguration.FirstOrDefault().ApiUrl + "pImages/" + ImageName;
            }
        }


        [Display(Name = nameof(Messages.EmailHost), ResourceType = typeof(Messages))]

        public string? EmailHost { get; set; } = "";


        [Display(Name = nameof(Messages.EmailPort), ResourceType = typeof(Messages))]

        public string? EmailPort { get; set; } = "";


        [Display(Name = nameof(Messages.UserName), ResourceType = typeof(Messages))]

        public string? EmailUsername { get; set; } = "";


        [Display(Name = nameof(Messages.Password), ResourceType = typeof(Messages))]

        public string? EmailPassword { get; set; } = "";

        [NotMapped]
        public IFormFile? ImageFile { get; set; }



        public MySystemConfiguration GetFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                MySystemConfiguration Medic = context.MySystemConfiguration
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
                ImageName = FileHandler.SaveUploadedFile(ImageFile);
                context.MySystemConfiguration.Add(this);
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
                if (File.Exists(ImageFullPath))
                {
                    try
                    {
                        File.Delete(ImageFullPath); 
                    }
                    catch (Exception ex) { 
                    }
                }
                context.MySystemConfiguration.Remove(GetFromDb());
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }
}
