using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CmsDataAccess.ModelsDto;

namespace CmsDataAccess.DbModels
{
    public class SubscriptionPlan
    {
        [Key]
        public Guid Id { get; set; }


        public string PriceCurrency { get; set; } = "AED";

        [Required]
        public string PriceRecuencyInterval { get; set; }

        [Required]
        public long PriceAmount { get; set; }
        
        public string PriceId { get; set; }

        [Required]
        public long SetUpCost { get; set; }

        [Required]
        public string StripeId { get; set; }

        public int FreeDays { get; set; } = 0;

        public bool IsDeleted { get; set; } = false;


        public List<SubscriptionPlanTranslation> SubscriptionPlanTranslation { get; set; }

        public SubscriptionPlanDto ToDto()
        {

            SubscriptionPlanDto res = new SubscriptionPlanDto
            {
                Id = Id,
                NameEn = this.SubscriptionPlanTranslation.FirstOrDefault(a => a.LangCode == "en-us").Name,
                NameAr = this.SubscriptionPlanTranslation.FirstOrDefault(a => a.LangCode == "ar").Name,
                DescriptionEn = this.SubscriptionPlanTranslation.FirstOrDefault(a => a.LangCode == "en-us").Description,
                DescriptionAr = this.SubscriptionPlanTranslation.FirstOrDefault(a => a.LangCode == "ar").Description,
                PriceRecuencyInterval = this.PriceRecuencyInterval,
                PriceAmount = this.PriceAmount,
                FreeDays = this.FreeDays
            };

            return res;
        }


    }

}
