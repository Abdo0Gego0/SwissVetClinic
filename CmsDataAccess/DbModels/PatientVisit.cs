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
using System.Web.Razor.Tokenizer.Symbols;

namespace CmsDataAccess.DbModels
{
    public class PatientVisit
    {
        [Key]
        public Guid Id { get; set; }
        public string? RoomNumber { get; set; } = string.Empty;

        [ForeignKey("MedicalCenter")]
        [Display(Name = nameof(Messages.MedicalCenterId), ResourceType = typeof(Messages))]

        public Guid MedicalCenterId { get; set; }

        [ForeignKey("BaseClinic")]
        [Display(Name = nameof(Messages.ClinicId), ResourceType = typeof(Messages))]

        public Guid BaseClinicId { get; set; }


        [ForeignKey("PetOwner")]
        [Display(Name = nameof(Messages.Patient), ResourceType = typeof(Messages))]
        public Guid PetOwnerId { get; set; }


        [ForeignKey("Pet")]
        [Display(Name = nameof(Messages.PetName), ResourceType = typeof(Messages))]
        public Guid PetId { get; set; }


        [ForeignKey("Doctor")]
        [Display(Name = nameof(Messages.Doctor), ResourceType = typeof(Messages))]
        public Guid DoctorId { get; set; }


        [ForeignKey("Appointment")]
        public Guid AppointmentId { get; set; }


        [Display(Name = nameof(Messages.Price), ResourceType = typeof(Messages))]
        public double? Cost { get; set; } = 0;

        [Display(Name = nameof(Messages.IsFirstVisit), ResourceType = typeof(Messages))]
        public bool IsFirstVisit { get; set; } = false;


        [Display(Name = nameof(Messages.IsFinished), ResourceType = typeof(Messages))]
        public bool IsFinished { get; set; } = false;


        [Display(Name = nameof(Messages.Date), ResourceType = typeof(Messages))]
        public DateTime VisistDate { get; set; }


        [Display(Name = nameof(Messages.SessionDuration), ResourceType = typeof(Messages))]
        public double Duration { get; set; } = 0;


        [Display(Name = nameof(Messages.SessionDuration), ResourceType = typeof(Messages))]
        public string Duration_
        {
            get
            {
                return ((int)Duration).ToString() + " Min";
            }
        }


        [Display(Name = nameof(Messages.BloodPressure), ResourceType = typeof(Messages))]
        public string? BloodPressure { get; set; }


        [Display(Name = nameof(Messages.Glucose), ResourceType = typeof(Messages))]
        public string? Glucose { get; set; }


        [Display(Name = nameof(Messages.HearBeat), ResourceType = typeof(Messages))]
        public string? HearBeat { get; set; }

        [Display(Name = nameof(Messages.Height), ResourceType = typeof(Messages))]

        public string? Height { get; set; }

        [Display(Name = nameof(Messages.Weight), ResourceType = typeof(Messages))]

        public int? Weight { get; set; }

        [Display(Name = nameof(Messages.Notes), ResourceType = typeof(Messages))]

        public string? Notes { get; set; }

        [Display(Name = nameof(Messages.Age), ResourceType = typeof(Messages))]



        public string? Age { get; set; }

        [Display(Name = nameof(Messages.Diagnosis), ResourceType = typeof(Messages))]

        public string? Diagnosis { get; set; }

        [Display(Name = nameof(Messages.Body_Condition), ResourceType = typeof(Messages))]

        public int? BodyCondition { get; set; }


        [Display(Name = nameof(Messages.Vaccinated), ResourceType = typeof(Messages))]

        public int? Vaccinated { get; set; }


        [Display(Name = nameof(Messages.Behavior), ResourceType = typeof(Messages))]

        public int? Behavior { get; set; }


        [Display(Name = nameof(Messages.Prognosis), ResourceType = typeof(Messages))]

        public int? Prognosis { get; set; }


        [Display(Name = nameof(Messages.Temperature), ResourceType = typeof(Messages))]

        public int? Temperature { get; set; }

        [Display(Name = nameof(Messages.Abdomen), ResourceType = typeof(Messages))]

        public int? Abdomen { get; set; }


        [Display(Name = nameof(Messages.SKIN), ResourceType = typeof(Messages))]

        public int? SKIN { get; set; }


        [Display(Name = nameof(Messages.HeartRate), ResourceType = typeof(Messages))]

        public int? HeartRate { get; set; }


        [Display(Name = nameof(Messages.MuscSkel), ResourceType = typeof(Messages))]

        public int? MuscSkel { get; set; }


        [Display(Name = nameof(Messages.Ears), ResourceType = typeof(Messages))]

        public int? Ears { get; set; }



        [Display(Name = nameof(Messages.VisitVideo), ResourceType = typeof(Messages))]

        public string? VisitVideo { set; get; }

        public List<VisitMeasurement>? VisitMeasurement { get; set; }
        public List<VisitTreatment>? VisitTreatment { get; set; }
        public List<VisitMedicine>? VisitMedicine { get; set; }

        [ForeignKey("CenterServices")]
        [Display(Name = nameof(Messages.ServiceType), ResourceType = typeof(Messages))]
        public Guid CenterServicesId { get; set; }
        public PetOwner PetOwner
        {
            get
            {
                return new ApplicationDbContext().PetOwner.Find(PetOwnerId);
            }
        }





        public Pet Pet
        {
            get
            {
                return new ApplicationDbContext().Pet.Find(PetId);
            }
        }

        public string ServiceName()
        {
            //get
            {
                try
                {

                    return new ApplicationDbContext().CenterServices.Find(CenterServicesId).ToString();
                }
                catch
                {
                    return "";
                }
            }
        }

        [Display(Name = nameof(Messages.LangCode), ResourceType = typeof(Messages))]

        public string ClinicName(string? lang = "en-US")
        {
            BaseClinic BaseClinic_ = new ApplicationDbContext().BaseClinic.Include(a => a.BaseClinicTranslation)
                .FirstOrDefault(a => a.Id == BaseClinicId);
            try
            {
                return BaseClinic_.BaseClinicTranslation.Where(a => a.LangCode == lang).ToList()[0].Name;
            }
            catch
            {
                return BaseClinic_.BaseClinicTranslation.ToList()[0].Name;
            }
        }

        [Display(Name = nameof(Messages.LangCode), ResourceType = typeof(Messages))]

        public string DoctorName(string? lang = "en-US")
        {
            Doctor doctor = new ApplicationDbContext().Doctor
                .Include(a => a.DoctorTranslation)
                .FirstOrDefault(a => a.Id == DoctorId);
            try
            {
                return doctor.DoctorTranslation.Where(a => a.LangCode == lang).ToList()[0].Name;
            }
            catch
            {
                return doctor.DoctorTranslation.ToList()[0].Name;
            }
        }


        public PatientVisit GetFromDb()
        {
            return new ApplicationDbContext().PatientVisit
                .Include(a => a.VisitTreatment)
                .Include(a => a.VisitMeasurement)
                .FirstOrDefault();

        }


    }
}
