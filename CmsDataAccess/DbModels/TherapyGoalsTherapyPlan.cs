using CmsDataAccess.ModelsDto;
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
    public class TherapyGoalsTherapyPlan
    {
        public Guid TherapyGoalsId { get; set; }
        public Guid TherapyPlanId { get; set; }
        public bool IsDeleted { get; set; } = false;


    }
}
