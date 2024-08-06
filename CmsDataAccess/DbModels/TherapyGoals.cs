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
    public class TherapyGoals
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("MedicalCenter")]
        public Guid MedicalCenterId { set; get; }
        public bool IsDeleted { get; set; }=false;
        
        [NotMapped]
        public bool IsSelected { get; set; }=false;
        
        public List<TherapyGoalsTranslations> TherapyGoalsTranslations { get; set; }
        public List<TherapyPlan>? TherapyPlan { get; set; }

    }
}
