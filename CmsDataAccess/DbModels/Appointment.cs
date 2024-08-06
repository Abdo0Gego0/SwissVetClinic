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
    public class  Appointment   
    {
        [Key]
        public Guid Id { get; set; }  
        public string? RoomNumber { get; set; } = string.Empty;

        [ForeignKey("Session Duration")]
                [Display(Name = nameof(Messages.SessionDuration), ResourceType = typeof(Messages))]

        public int SessionDuration { get; set; } = 30;


        [ForeignKey("MedicalCenter")]
                [Display(Name = nameof(Messages.MedicalCenterId), ResourceType = typeof(Messages))]

        public Guid MedicalCenterId { get; set; }

        [ForeignKey("Clinic")]
                [Display(Name = nameof(Messages.ClinicId), ResourceType = typeof(Messages))]

        public Guid BaseClinicId { get; set; }

        [ForeignKey("PetOwner")]
                [Display(Name = nameof(Messages.Patient), ResourceType = typeof(Messages))]

        public Guid PetOwnerId { get; set; }

        [ForeignKey("CenterServices")]
        [Display(Name = nameof(Messages.ServiceType), ResourceType = typeof(Messages))]
        public Guid CenterServicesId { get; set; }

        [Display(Name = nameof(Messages.Price), ResourceType = typeof(Messages))]
        public double? Cost { get; set; }

        [ForeignKey("Pet")]
        [Display(Name = nameof(Messages.PetName), ResourceType = typeof(Messages))]
        public Guid PetId { get; set; }


        [Display(Name = nameof(Messages.Notified), ResourceType = typeof(Messages))]

        public bool Notified { get; set; } = false;
        public bool IsFromMobile { get; set; } = false;


        public string PatientName()
        {
            return new ApplicationDbContext().PetOwner.Find(PetOwnerId).FullName;
        }

        public string ClinicName()
        {
            BaseClinic BaseClinic_= new ApplicationDbContext().BaseClinic.Include(a => a.BaseClinicTranslation)
                .FirstOrDefault(a => a.Id == BaseClinicId);
            try
            {
                return BaseClinic_.BaseClinicTranslation.Where(a => a.LangCode == "ar").ToList()[0].Name;
            }
            catch
            {
                return BaseClinic_.BaseClinicTranslation.ToList()[0].Name;
            }
        }

                [Display(Name = nameof(Messages.Start), ResourceType = typeof(Messages))]

        public DateTime Start { get; set; }
                [Display(Name = nameof(Messages.End), ResourceType = typeof(Messages))]

        public DateTime End { get; set; }
                        [Display(Name = nameof(Messages.AppointmentType), ResourceType = typeof(Messages))]

        public int AppointmentType { get; set; } = 0;   
                        [Display(Name = nameof(Messages.AppointmentStatus), ResourceType = typeof(Messages))]

        public int AppointmentStatus { get; set; } = 0; 
                        [Display(Name = nameof(Messages.Title), ResourceType = typeof(Messages))]

        public string Title { get; set; }
                        [Display(Name = nameof(Messages.IsFinished), ResourceType = typeof(Messages))]

        public bool IsFinished { get; set; } = false;
                        [Display(Name = nameof(Messages.IsStarted), ResourceType = typeof(Messages))]

        public bool IsStarted { get; set; } = false;
                        [Display(Name = nameof(Messages.IsFirstVisit), ResourceType = typeof(Messages))]

        public bool IsFirstVisit{ get; set; } = false;
                        [Display(Name = nameof(Messages.Description), ResourceType = typeof(Messages))]

        public string Description { get; set; }
                        [Display(Name = nameof(Messages.Color), ResourceType = typeof(Messages))]

        
        [NotMapped]        
        public string? Color { get; set; }
                        [Display(Name = nameof(Messages.IsAllDay), ResourceType = typeof(Messages))]


        public bool IsAllDay { get; set; } = false;
                        [Display(Name = nameof(Messages.LangCode), ResourceType = typeof(Messages))]

        public bool IsReadonly { get; set; }=false;
                        [Display(Name = nameof(Messages.LangCode), ResourceType = typeof(Messages))]

        public string? RecurrenceRule { get; set; }
                        [Display(Name = nameof(Messages.LangCode), ResourceType = typeof(Messages))]

        public string? RecurrenceException { get; set; }
                        [Display(Name = nameof(Messages.LangCode), ResourceType = typeof(Messages))]

        public string? RecurrenceID { get; set; }
                        [Display(Name = nameof(Messages.TimeZone), ResourceType = typeof(Messages))]


        public string? Timezone { get; set; }
                        [Display(Name = nameof(Messages.EndTimezone), ResourceType = typeof(Messages))]

        public string? EndTimezone { get; set; }
                        [Display(Name = nameof(Messages.StartTimezone), ResourceType = typeof(Messages))]

        public string? StartTimezone { get; set; }
                        



    }
}
