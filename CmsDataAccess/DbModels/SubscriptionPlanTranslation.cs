using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CmsDataAccess.DbModels
{
    public class SubscriptionPlanTranslation
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "LangCode")]
        public string? LangCode { get; set; } = "en-US";

        [ForeignKey("SubscriptionPlan")]
        public Guid SubscriptionPlanId { get; set; }

        [Required]
        public string Name { get; set; }

        [AllowHtml]
        public string Description { get; set; }



    }
}
