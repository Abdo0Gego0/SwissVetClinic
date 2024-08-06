using CmsDataAccess.DbModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.ModelsDto
{
    public class SubscriptionPlanDto
    {
        [Key]
        public Guid Id { get; set; }


        [Required]
        public string PriceRecuencyInterval { get; set; }

        [Required]
        public long PriceAmount { get; set; }


        public int FreeDays { get; set; } = 0;

        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }


    }
}
