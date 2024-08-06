using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmsResources;

namespace CmsDataAccess.DbModels
{
    public class Banner
    {
        [Key]
        public Guid Id { get; set; }

        public string? ImageName { get; set; } = "";


        [Display(Name = nameof(Messages.StartDate), ResourceType = typeof(Messages))]

        public DateTime StartDate { get; set; }
        [Display(Name = nameof(Messages.EndDate), ResourceType = typeof(Messages))]


        public DateTime EndDate { get; set; }

        public string? ImageFullPath
        {
            get
            {
                return new ApplicationDbContext().MySystemConfiguration.FirstOrDefault().ApiUrl + "pImages/" + ImageName;

            }
        }

        [Display(Name = nameof(Messages.BannerText), ResourceType = typeof(Messages))]

        public string? BannerText { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }


        [NotMapped]
        public string? Image64 { get; set; }
    }
}
