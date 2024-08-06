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
    public class TherapyPlan
    {
        [Key]
        public Guid Id { get; set; }
        public List<TherapyPlanTranslations> TherapyPlanTranslations { get; set; }


        [ForeignKey("MedicalCenter")]
        public Guid MedicalCenterId { set; get; }
        public List<TherapyGoals> TherapyGoals { get; set; }
        public int? PatientVideoEveryNumberOfSessions { get; set; }

        public bool IsDeleted { get; set; } = false;


    }





}
