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
using Microsoft.AspNetCore.Http;

namespace CmsDataAccess.DbModels
{
    [Table("Pet")]

    public class Pet 
    {
        [Key]

        public Guid Id { get; set; }

        [ForeignKey("PetOwner")]
        public Guid PetOwnerId { get; set; }

        [Display(Name = nameof(Messages.PetName), ResourceType = typeof(Messages))]
        public string PetName { get; set; }

        [Display(Name = nameof(Messages.PetType), ResourceType = typeof(Messages))]
        public string? PetType { get; set; } = "";

        [Display(Name = nameof(Messages.LifeStyle), ResourceType = typeof(Messages))]
        public string? LifeStyle { get; set; }="";

        [Display(Name = nameof(Messages.Breed), ResourceType = typeof(Messages))]
        public string? Breed { get; set; } = "";

        [Display(Name = nameof(Messages.BloodType), ResourceType = typeof(Messages))]
        public string? BloodType { get; set; }
        [Display(Name = nameof(Messages.MedicalCenterId), ResourceType = typeof(Messages))]
        public Guid? MedicalCenterId { get; set; }

        [Display(Name = nameof(Messages.ImageName), ResourceType = typeof(Messages))]
        public string? ImageName { get; set; } = "";


        [Display(Name = nameof(Messages.Notes), ResourceType = typeof(Messages))]
        public string? Notes { get; set; } = "";





        [Display(Name = nameof(Messages.Sex), ResourceType = typeof(Messages))]
        public int? Sex { get; set; }


        [Display(Name = nameof(Messages.BirthDate), ResourceType = typeof(Messages))]
        public DateTime? BirthDate { get; set; }

        public string? ImageFullPath
        {
            get
            {
                if (ImageName == "" || ImageName == null)
                {
                    return "/siteimages/petAvatat.svg";
                }
                return new ApplicationDbContext().MySystemConfiguration.FirstOrDefault().ApiUrl + "/pImages/" + ImageName;
            }
        }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }


        
        public bool IsDeleted { get; set; } = false;


        public List<PatientDiagnosis>? PatientDiagnosis { get; set; }
        public List<PatientAllergy>? PatientAllergy { get; set; }
        public List<PatientFamilyHistory>? PatientFamilyHistory { get; set; }
        public List<PatientMedicalHistory>? PatientMedicalHistory { get; set; }
        public List<PatientMedicineHistory>? PatientMedicineHistory { get; set; }
        public List<PatientSurgicalHistory>? PatientSurgicalHistory { get; set; }
        public List<PatientTherapyGoals>? PatientTherapyGoals { get; set; }


        [NotMapped]
        [Display(Name = nameof(Messages.Age), ResourceType = typeof(Messages))]
        public int? Age
        {
            get
            {
                if (BirthDate==DateTime.MinValue)
                {
                    return null;

                }

                if (BirthDate.HasValue)
                {
                    DateTime today = DateTime.Today;
                    int age = today.Year - BirthDate.Value.Year;

                    // Check if the birthday has occurred this year
                    if (BirthDate.Value.Date > today.AddYears(-age))
                    {
                        age--;
                    }

                    return age;
                }

                return null;
            }
        }

        public Pet GetFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                Pet Medic = context.Pet
                    .Include(a=>a.PatientAllergy)
                    .Include(a=>a.PatientSurgicalHistory)
                    .Include(a=>a.PatientMedicalHistory)
                    .Include(a=>a.PatientMedicineHistory)
                    .Include(a=>a.PatientDiagnosis)
                    .FirstOrDefault(a => a.Id == Id);

                return Medic;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool InsertIntoDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            try
            {
                context.Pet.Add(this);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            try
            {
                Pet temp = GetFromDb();
                FileHandler.DeleteImageFile(temp.ImageName);
                temp = GetFromDb();
                context.Pet.Remove(temp);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SoftFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            try
            {
                Pet temp = context.Pet.Find(Id);
                temp.IsDeleted = true;
                context.Pet.Attach(temp);
                context.Entry(temp).Property(a=>a.IsDeleted).IsModified = true;
                context.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }


        [NotMapped]
        public string CenterName
		{
            get
            {
                try
                {
                    MedicalCenter res = new ApplicationDbContext().MedicalCenter.Find(MedicalCenterId).GetModelByLnag("Ar");
                    return res.MedicalCenterTranslation[0].Name;
                }
                catch
                {
                    return "";
                }

            }
        }

        [NotMapped]
        public string? Image64 { get; set; }

    }


}
