using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmsDataAccess.Utils.FilesUtils;
using Microsoft.EntityFrameworkCore;
using CmsResources;

namespace CmsDataAccess.DbModels
{


    public class PatientDiagnosis 
    {
        [Key]
        public Guid Id { get; set; }
        
        [ForeignKey("PetOwner")]
        [Display(Name = nameof(Messages.Patient), ResourceType = typeof(Messages))]


        public Guid? PetOwnerId { get; set; }
        [Display(Name = nameof(Messages.Diagnosis), ResourceType = typeof(Messages))]


        public string? value { get; set; } = "";
        [Display(Name = nameof(Messages.Date), ResourceType = typeof(Messages))]


        public DateTime? DiagnosisDate { get; set; }

        [ForeignKey("Pet")]
        [Display(Name = nameof(Messages.PetName), ResourceType = typeof(Messages))]

        public Guid? PetId { get; set; }

    }

    public class PatientAllergy
    {

        [Key]
        public Guid Id { get; set; }

        [ForeignKey("PetOwner")]
        [Display(Name = nameof(Messages.Patient), ResourceType = typeof(Messages))]

        public Guid? PetOwnerId { get; set; }

        [Display(Name = nameof(Messages.Allergy), ResourceType = typeof(Messages))]
        public string? value { get; set; } = "";

        [ForeignKey("Pet")]
        [Display(Name = nameof(Messages.PetName), ResourceType = typeof(Messages))]

        public Guid? PetId { get; set; }
    }


    public class PatientFamilyHistory
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("PetOwner")]
        [Display(Name = nameof(Messages.Patient), ResourceType = typeof(Messages))]

        public Guid? PetOwnerId { get; set; }
        [Display(Name = nameof(Messages.FamilyHistory), ResourceType = typeof(Messages))]


        public string? value { get; set; } = "";
        [ForeignKey("Pet")]
        [Display(Name = nameof(Messages.PetName), ResourceType = typeof(Messages))]

        public Guid? PetId { get; set; }
    }

    public class PatientMedicalHistory
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("PetOwner")]
        [Display(Name = nameof(Messages.Patient), ResourceType = typeof(Messages))]

        public Guid? PetOwnerId { get; set; }
        [Display(Name = nameof(Messages.MedicalHistory), ResourceType = typeof(Messages))]

        [ForeignKey("Pet")]

        public string? value { get; set; } = "";
        [Display(Name = nameof(Messages.PetName), ResourceType = typeof(Messages))]

        public Guid? PetId { get; set; }
    }

    public class PatientMedicineHistory
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("PetOwner")]
        [Display(Name = nameof(Messages.Patient), ResourceType = typeof(Messages))]

        public Guid? PetOwnerId { get; set; }
        [Display(Name = nameof(Messages.MedicineHistory), ResourceType = typeof(Messages))]

        public string? value { get; set; } = "";
        [ForeignKey("Pet")]
        [Display(Name = nameof(Messages.PetName), ResourceType = typeof(Messages))]

        public Guid? PetId { get; set; }
    }

    public class PatientSurgicalHistory
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("PetOwner")]
        [Display(Name = nameof(Messages.Patient), ResourceType = typeof(Messages))]

        public Guid? PetOwnerId { get; set; }
        [Display(Name = nameof(Messages.SurgicalHistory), ResourceType = typeof(Messages))]


        public string? value { get; set; } = "";

        [ForeignKey("Pet")]
        [Display(Name = nameof(Messages.PetName), ResourceType = typeof(Messages))]

        public Guid? PetId { get; set; }

    }

}
