using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.DbModels
{
    public class TherapyPlanTranslations
    {
        [Key]
        public Guid Id { get; set; }

        public string? LangCode { get; set; } = "en-US";

        [ForeignKey("TherapyPlan")]
        public Guid TherapyPlanId { get; set; }

        public string Description { get; set; }
    }
}
