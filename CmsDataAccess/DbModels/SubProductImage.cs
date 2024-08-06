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
    public class SubProductImage
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("SubProduct")]
        public Guid SubProductId { get; set; }

        [Display(Name = nameof(Messages.ImageName), ResourceType = typeof(Messages))]
        public string? ImageName { get; set; } = "";
        public string? ImageFullPath
        {
            get
            {
                if (ImageName == "" || ImageName == null)
                {
                    return "/siteimages/userAvatat.svg";
                }
                return new ApplicationDbContext().MySystemConfiguration.FirstOrDefault().ApiUrl + "pImages/" + ImageName;
            }
        }


        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }

}
