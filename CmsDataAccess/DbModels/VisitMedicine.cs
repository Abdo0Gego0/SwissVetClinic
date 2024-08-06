using CmsDataAccess.AutoMapping;
using CmsDataAccess.ModelsDto;
using CmsDataAccess.TimeZoneUtils;
using CmsDataAccess.Utils.FilesUtils;
using CmsResources;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.DbModels
{
    public class VisitMedicine
    {
        [Key]
        public Guid Id { get; set; }


        [Display(Name = nameof(Messages.MedicnieName), ResourceType = typeof(Messages))]
        public string MedicineName { get; set; }


        [Display(Name = nameof(Messages.MedicineUnit), ResourceType = typeof(Messages))]
        public string MedicineUnit { get; set; }


        [Display(Name = nameof(Messages.Duration), ResourceType = typeof(Messages))]
        public int Duration { get; set; }   
        
        
        
        [Display(Name = nameof(Messages.Price), ResourceType = typeof(Messages))]
        public double Cost { get; set; }


        

        [ForeignKey("Doctor")]
        [Display(Name = nameof(Messages.Doctor), ResourceType = typeof(Messages))]
        public Guid DoctorId { get; set; }


        [ForeignKey("BaseClinic")]
        [Display(Name = nameof(Messages.ClinicId), ResourceType = typeof(Messages))]
        public Guid BaseClinicId { get; set; }

        [ForeignKey("PetOwner")]

        [Display(Name = nameof(Messages.Patient), ResourceType = typeof(Messages))]

        public Guid PetOwnerId { get; set; }

        [ForeignKey("PatientVisit")]

        public Guid PatientVisitId { get; set; }

    }


    public class VisitMedicineViewModel
    {
        [Display(Name = nameof(Messages.MedicineName), ResourceType = typeof(Messages))]

        [ForeignKey("CenterMedicineList")]
        public Guid CenterMedicineListId { set; get; }

        [Display(Name = nameof(Messages.Concentration), ResourceType = typeof(Messages))]

        [ForeignKey("CenterMedicineUnit")]
        public Guid CenterMedicineUnitId { set; get; }
        [Display(Name = nameof(Messages.Duration), ResourceType = typeof(Messages))]

        public int Duration {  set; get; }

    }

    public class CenterMedicineUnitViewModel
    {
        public Guid Id { set; get; }
        public string Name { set; get; }    

        public Guid CenterMedicineListId { set; get; }

    }

}
