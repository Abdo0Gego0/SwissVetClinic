using CmsDataAccess.ModelsDto;
using CmsResources;
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
    public class PatientTherapyGoals 
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Pet")]
                [Display(Name = nameof(Messages.Patient), ResourceType = typeof(Messages))]

        public Guid PetId { get; set; }


        [ForeignKey("Doctor")]
                [Display(Name = nameof(Messages.Doctor), ResourceType = typeof(Messages))]

        public Guid DoctorId { get; set; }


                [Display(Name = nameof(Messages.TherapyGoal), ResourceType = typeof(Messages))]

        public string TherapyGoal { get; set; }

                [Display(Name = nameof(Messages.IsDone), ResourceType = typeof(Messages))]

        public int IsDone { get; set; } = 0;
    }
}
