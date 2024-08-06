using CmsDataAccess.DbModels;
using CmsResources;
using Kendo.Mvc.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLibrary.AppointmentSRVC
{
    public class AppointmentViewModel : ISchedulerEvent  
    {
        public Guid Id { get; set; }
        //[Display(Name = nameof(Messages.Title), ResourceType = typeof(Messages))]


        public string Title { get; set; }

        //[Display(Name = nameof(Messages.Description), ResourceType = typeof(Messages))]


        public string Description { get; set; }

        //[Display(Name = nameof(Messages.SessionDuration), ResourceType = typeof(Messages))]

        public int SessionDuration { get; set; } = 30;

        public double? Cost { get; set; } 

        [NotMapped]
        //[Display(Name = nameof(Messages.Color), ResourceType = typeof(Messages))]

        public string? Color
        {
            get
            {
                if (IsStarted && !IsFinished)
                {
                    return "Coral";
                }

                if (start<DateTime.Now || IsFinished)
                {
                    return "Gray";
                }
                else
                {
                    if (start>DateTime.Now)
                    {
                        return "rgb(74,201,197)";
                    }
                    else
                    {
                        return "Coral";
                    }
                }
            }
        }


        //[Display(Name = nameof(Messages.Start), ResourceType = typeof(Messages))]



        private DateTime start;

        //[Display(Name = nameof(Messages.Start), ResourceType = typeof(Messages))]


        public DateTime Start
        {
            get
            {
                return start;
            }
            set
            {
                start = value.ToUniversalTime();
            }
        }

        public string StartTimezone { get; set; }

        //[Display(Name = nameof(Messages.End), ResourceType = typeof(Messages))]


        private DateTime end;

        //[Display(Name = nameof(Messages.End), ResourceType = typeof(Messages))]


        public DateTime End
        {
            get
            {
                return end;
            }
            set
            {
                end = value.ToUniversalTime();
            }
        }

        public string? EndTimezone { get; set; }

        //public string? Title_ { get; set; }
        //public string? Description_ { get; set; }
        //public DateTime? Start_ { get; set; }
        //public DateTime? End_ { get; set; }


        public string RecurrenceRule { get; set; }
        public string? RecurrenceID { get; set; }
        public string RecurrenceException { get; set; }

        //[Display(Name = nameof(Messages.IsAllDay), ResourceType = typeof(Messages))]


        public bool IsAllDay { get; set; }


        //[Display(Name = nameof(Messages.TimeZone), ResourceType = typeof(Messages))]


        public string Timezone { get; set; }

        //[Display(Name = nameof(Messages.TimeZone), ResourceType = typeof(Messages))]


        public Guid BaseClinicId { get; set; }

        //[Display(Name = nameof(Messages.Patient), ResourceType = typeof(Messages))]

        //[Required(ErrorMessageResourceName = nameof(Messages.requiredMsg), ErrorMessageResourceType = typeof(Messages))]

        public Guid PetOwnerId { get; set; }

        //[Required(ErrorMessageResourceName = nameof(Messages.requiredMsg), ErrorMessageResourceType = typeof(Messages))]

        public Guid PetId { get; set; }

        //[Required(ErrorMessageResourceName = nameof(Messages.requiredMsg), ErrorMessageResourceType = typeof(Messages))]

        public Guid CenterServicesId { get; set; }

        //[Display(Name = nameof(Messages.MedicalCenterId), ResourceType = typeof(Messages))]

        public Guid MedicalCenterId { get; set; }

        //[Display(Name = nameof(Messages.IsFinished), ResourceType = typeof(Messages))]

        public bool IsFinished { get; set; } = false;

        //[Display(Name = nameof(Messages.IsStarted), ResourceType = typeof(Messages))]

        public bool IsStarted { get; set; } = false;

        //[Display(Name = nameof(Messages.IsFirstVisit), ResourceType = typeof(Messages))]

        public bool IsFirstVisit { get; set; } = true;

        //[Display(Name = nameof(Messages.FullName), ResourceType = typeof(Messages))]

        public string PatientName
        {
            get
            {
                try
                {

                    return new ApplicationDbContext().PetOwner.Find(PetOwnerId).FullName;
                }
                catch
                {
                    return "";
                }
            }
        }


        public string ServiceName
        {
            get
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

        public string PetName_
        {
            get
            {
                try
                {

                    return new ApplicationDbContext().Pet.Find(PetId).PetName;
                }
                catch
                {
                    return "";
                }
            }
        }

        public string PetName()
        {
            //get
            {
                return new ApplicationDbContext().Pet.Find(PetId).PetName;
            }
        }

        public string ClinicName()
        {
            BaseClinic BaseClinic_ = new ApplicationDbContext().BaseClinic.Include(a => a.BaseClinicTranslation)
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


        public Appointment ToEntity()
        {
            var meeting = new Appointment
            {
                Id = Id,
                Title = Title,
                Start = Start.ToLocalTime(),
                End = End.ToLocalTime(),
                Description = Description,
                IsAllDay = IsAllDay,
                RecurrenceRule = RecurrenceRule,
                RecurrenceException = RecurrenceException,
                RecurrenceID = RecurrenceID,
                BaseClinicId= BaseClinicId,
                PetOwnerId = PetOwnerId,
                MedicalCenterId= MedicalCenterId,
                Timezone= Timezone,
                StartTimezone= StartTimezone,
                EndTimezone= EndTimezone,
                IsFinished= IsFinished,
                IsStarted= IsStarted,
                IsFirstVisit= IsFirstVisit,
                SessionDuration= SessionDuration,
                PetId=PetId,
                CenterServicesId= CenterServicesId,
                Cost = Cost,

                
    };

            return meeting;
        }

        public AppointmentViewModel CopyToViewModel()
        {
            var meeting = new AppointmentViewModel
            {
                End = this.End,
                Description = this.Description,
                IsAllDay = this.IsAllDay,
                BaseClinicId = this.BaseClinicId,
                EndTimezone = this.EndTimezone,
                IsFinished = this.IsFinished,
                IsStarted = this.IsStarted,
                IsFirstVisit = this.IsFirstVisit,
                MedicalCenterId = this.MedicalCenterId,
                PetOwnerId = this.PetOwnerId,
                RecurrenceException = this.RecurrenceException,
                RecurrenceID = this.RecurrenceID,
                RecurrenceRule = this.RecurrenceRule,
                SessionDuration = this.SessionDuration,
                Start = this.Start,
                StartTimezone = this.StartTimezone,
                Timezone = this.Timezone,
                Title = this.Title,
                PetId = this.PetId,
                CenterServicesId = this.CenterServicesId,
                Cost=this.Cost,
                



    };

            return meeting;
        }

    }
}
