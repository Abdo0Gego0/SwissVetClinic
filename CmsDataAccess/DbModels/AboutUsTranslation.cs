using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CmsResources;

namespace CmsDataAccess.DbModels
{
    public class AboutUsTranslation
    { 
        [Key]
        public Guid Id { get; set; }

        [Required]
                [Display(Name = nameof(Messages.LangCode), ResourceType = typeof(Messages))]
        public string? LangCode { get; set; } = "en-US";

        [ForeignKey("AboutUs")]

        public Guid AboutUsId { get; set; }
                [Display(Name = nameof(Messages.AboutUsText), ResourceType = typeof(Messages))]

        public string? AboutUsText { get; set; }



    }
}
