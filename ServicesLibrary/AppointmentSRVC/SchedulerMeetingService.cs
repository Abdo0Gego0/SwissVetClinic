using CmsDataAccess.DbModels;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServicesLibrary.Services.AppointmentSRVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.WebPages;

namespace ServicesLibrary.AppointmentSRVC
{
    public class SchedulerMeetingService : BaseService, ISchedulerEventService<AppointmentViewModel>
    {
        private static bool UpdateDatabase = true;
        private ISession _session;

        public ISession Session { get { return _session; } }

        public SchedulerMeetingService(IHttpContextAccessor httpContextAccessor)
        {
            //_session = httpContextAccessor.HttpContext.Session;
        }

        public virtual IQueryable<AppointmentViewModel> GetAll()
        {
            return null;
        }
        public virtual IQueryable<AppointmentViewModel> GetAll(int? TypeFilter, Guid CenterId, Guid? ClinicId, string? view, string? date, Guid? PetOwnerId)
        {
            return GetAllMeetings(TypeFilter,CenterId, ClinicId, view, date, PetOwnerId).AsQueryable();
        }

        public virtual IQueryable<AppointmentViewModel> GetAll( Guid? ClinicId, string? view, string? date, Guid? PetOwnerId)
        {
            return GetAllSystemMeetings( ClinicId, view, date, PetOwnerId).AsQueryable();
        }

        

        public virtual IQueryable<PatientViewModel> GetAllPatient(Guid guid)
        {
            return GetAllPatients( guid).AsQueryable();
        }

        public virtual IQueryable<PatientViewModel> GetAllPatient()
        {
            return GetAllPatients().AsQueryable();
        }

        public virtual IQueryable<ClinicViewModel> GetAllClinic(Guid guid)
        {
            return GetAllClinics(guid).AsQueryable();
        }

        public virtual IQueryable<ClinicViewModel> GetAllClinic()
        {
            return GetAllClinics().AsQueryable();
        }

        public virtual IList<PatientViewModel> GetAllPatients(Guid guid)
        {
            using (var db = GetContext())
            {
                return db.PetOwner.Where(a => !a.IsDeleted && a.MedicalCenterId==guid)
                    
                    .Select(a=> new PatientViewModel { Value=a.Id,Text=a.FirstName+" "+a.LastName}).ToList();
            }
        }

        public virtual IList<PatientViewModel> GetAllPatients()
        {
            using (var db = GetContext())
            {
                return db.PetOwner.Where(a => !a.IsDeleted )

                    .Select(a => new PatientViewModel { Value = a.Id, Text = a.FirstName + " " + a.LastName }).ToList();
            }
        }

        public virtual IList<ClinicViewModel> GetAllClinics(Guid guid)
        {
            using (var db = GetContext())
            {
                return db.BaseClinic
                    .Include(a=>a.BaseClinicTranslation)
                    .Where(a => !a.IsDeleted && a.MedicalCenterId == guid)
                    .Select(a=> new ClinicViewModel { Value = a.Id, Text = a.BaseClinicTranslation[0].Name+" "+ a.BaseClinicTranslation[1].Name })
                    .ToList();
            }
        }

        public virtual IList<ClinicViewModel> GetAllClinics()
        {
            using (var db = GetContext())
            {
                return db.BaseClinic
                    .Include(a => a.BaseClinicTranslation)
                    .Where(a => !a.IsDeleted)
                    .Select(a => new ClinicViewModel { Value = a.Id, Text = a.BaseClinicTranslation[0].Name + " " + a.BaseClinicTranslation[1].Name })
                    .ToList();
            }
        }

        public virtual IList<AppointmentViewModel> GetAllMeetings(int? TypeFilter, Guid guid, Guid? ClinicId, string? view, string? date, Guid? PetOwnerId)
        {
            DateTime myDayStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,0,0,0);
            DateTime myDayEnd   = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,23,59,59);

            Expression<Func<Appointment, bool>> combinedExpression = a => (a.IsFromMobile || !a.IsFromMobile) ;

            if (TypeFilter!=null)
            {
                if ((int)TypeFilter == 0)
                {
                    combinedExpression = a => a.IsFromMobile == false;

                }
                else
                {
                    combinedExpression = a => a.IsFromMobile == true;

                }

            }
            using (var db = GetContext())
            {

                if (ClinicId != null)
                {

                    if (PetOwnerId !=null)
                    {
                        if (view.IsNullOrEmpty())
                        {

                            var result = db.Appointment
                           .Where(a => a.MedicalCenterId == guid && a.BaseClinicId == (Guid)ClinicId && a.PetOwnerId == (Guid)PetOwnerId && a.Start>= myDayStart && a.Start<= myDayEnd)
                           .Where(combinedExpression)
                           .ToList()
                           .Select(meeting => new AppointmentViewModel
                           {
                               Id = meeting.Id,
                               Title = meeting.Title,
                               Start = meeting.Start,
                               End = meeting.End,
                               Description = meeting.Description,
                               IsAllDay = meeting.IsAllDay,
                               BaseClinicId = meeting.BaseClinicId,
                               PetOwnerId = meeting.PetOwnerId,
                               MedicalCenterId = meeting.MedicalCenterId,
                               RecurrenceRule = meeting.RecurrenceRule,
                               RecurrenceException = meeting.RecurrenceException,
                               RecurrenceID = meeting.RecurrenceID,
                               Timezone = meeting.Timezone,
                               StartTimezone = meeting.StartTimezone,
                               EndTimezone = meeting.EndTimezone,
                               IsFinished = meeting.IsFinished,
                               IsStarted = meeting.IsStarted,
                               IsFirstVisit = meeting.IsFirstVisit,
                               PetId = meeting.PetId,
                               CenterServicesId = meeting.CenterServicesId,
                               Cost=meeting.Cost,

                           }).ToList();

                            return result;
                        }
                        else
                        {
                            DateTime inputDate = DateTime.Parse(date);
                            DateTime startDate = new DateTime(inputDate.Year, inputDate.Month, inputDate.Day, 0, 0, 0);
                            DateTime endDate = new DateTime(inputDate.Year, inputDate.Month, inputDate.Day, 23, 59, 59);
                            if (view == "week")
                            {
                                startDate = inputDate.AddDays(-(int)inputDate.DayOfWeek);
                                endDate = startDate.AddDays(6);
                            }

                            if (view == "workWeek")
                            {
                                startDate = inputDate.AddDays(-(int)inputDate.DayOfWeek + (int)DayOfWeek.Monday);
                                endDate = startDate.AddDays(4); // Friday
                            }

                            if (view == "month")
                            {
                                startDate = new DateTime(inputDate.Year, inputDate.Month, 1);
                                endDate = startDate.AddMonths(1).AddDays(-1);
                            }

                            if (view == "year")
                            {
                                startDate = new DateTime(inputDate.Year, 1, 1);
                                endDate = new DateTime(inputDate.Year, 12, 31);
                            }

                            var result = db.Appointment
                           .Where(a => a.MedicalCenterId == guid && a.BaseClinicId == (Guid)ClinicId && a.Start >= startDate && a.End <= endDate && a.PetOwnerId == (Guid)PetOwnerId)
                           //.Where(combinedExpression)
                           .ToList()
                           .Select(meeting => new AppointmentViewModel
                           {
                               Id = meeting.Id,
                               Title = meeting.Title,
                               Start = meeting.Start,
                               End = meeting.End,
                               Description = meeting.Description,
                               IsAllDay = meeting.IsAllDay,
                               BaseClinicId = meeting.BaseClinicId,
                               PetOwnerId = meeting.PetOwnerId,
                               MedicalCenterId = meeting.MedicalCenterId,
                               RecurrenceRule = meeting.RecurrenceRule,
                               RecurrenceException = meeting.RecurrenceException,
                               RecurrenceID = meeting.RecurrenceID,
                               Timezone = meeting.Timezone,
                               StartTimezone = meeting.StartTimezone,
                               EndTimezone = meeting.EndTimezone,
                               IsFinished = meeting.IsFinished,
                               IsStarted = meeting.IsStarted,
                               IsFirstVisit = meeting.IsFirstVisit,
                               PetId = meeting.PetId,
                               CenterServicesId = meeting.CenterServicesId,
                               Cost = meeting.Cost,

                           }).ToList();

                            return result;

                        }
                    }

                    if (view.IsNullOrEmpty())
                    {

                        var result = db.Appointment
                       .Where(a => a.MedicalCenterId == guid && a.BaseClinicId == (Guid)ClinicId && a.Start >= myDayStart && a.Start <= myDayEnd)
                       //.Where(combinedExpression)
                       .ToList()
                       .Select(meeting => new AppointmentViewModel
                       {
                           Id = meeting.Id,
                           Title = meeting.Title,
                           Start = meeting.Start,
                           End = meeting.End,
                           Description = meeting.Description,
                           IsAllDay = meeting.IsAllDay,
                           BaseClinicId = meeting.BaseClinicId,
                           PetOwnerId = meeting.PetOwnerId,
                           MedicalCenterId = meeting.MedicalCenterId,
                           RecurrenceRule = meeting.RecurrenceRule,
                           RecurrenceException = meeting.RecurrenceException,
                           RecurrenceID = meeting.RecurrenceID,
                           Timezone = meeting.Timezone,
                           StartTimezone = meeting.StartTimezone,
                           EndTimezone = meeting.EndTimezone,
                           IsFinished = meeting.IsFinished,
                           IsStarted = meeting.IsStarted,
                           IsFirstVisit = meeting.IsFirstVisit,
                           PetId = meeting.PetId,
                           CenterServicesId = meeting.CenterServicesId,
                           Cost = meeting.Cost,

                       }).ToList();

                        return result;
                    }
                    else
                    {
                        DateTime inputDate = DateTime.Parse(date);
                        DateTime startDate = new DateTime(inputDate.Year, inputDate.Month, inputDate.Day,0,0,0);
                        DateTime endDate = new DateTime(inputDate.Year, inputDate.Month, inputDate.Day, 23, 59, 59);
                        if (view=="week")
                        {
                            startDate = inputDate.AddDays(-(int)inputDate.DayOfWeek);
                            endDate  = startDate.AddDays(6);
                        }

                        if (view== "workWeek")
                        {
                            startDate = inputDate.AddDays(-(int)inputDate.DayOfWeek + (int)DayOfWeek.Monday);
                            endDate = startDate.AddDays(4); // Friday
                        }

                        if (view == "month")
                        {
                            startDate = new DateTime(inputDate.Year, inputDate.Month, 1);
                            endDate = startDate.AddMonths(1).AddDays(-1);
                        }

                        if (view == "year")
                        {
                            startDate = new DateTime(inputDate.Year, 1, 1);
                            endDate = new DateTime(inputDate.Year, 12, 31);
                        }

                        var result = db.Appointment
                       .Where(a => a.MedicalCenterId == guid && a.BaseClinicId == (Guid)ClinicId && a.Start >= startDate && a.End <= endDate)
                       //.Where(combinedExpression)
                       .ToList()
                       .Select(meeting => new AppointmentViewModel
                       {
                           Id = meeting.Id,
                           Title = meeting.Title,
                           Start = meeting.Start,
                           End = meeting.End,
                           Description = meeting.Description,
                           IsAllDay = meeting.IsAllDay,
                           BaseClinicId = meeting.BaseClinicId,
                           PetOwnerId = meeting.PetOwnerId,
                           MedicalCenterId = meeting.MedicalCenterId,
                           RecurrenceRule = meeting.RecurrenceRule,
                           RecurrenceException = meeting.RecurrenceException,
                           RecurrenceID = meeting.RecurrenceID,
                           Timezone = meeting.Timezone,
                           StartTimezone = meeting.StartTimezone,
                           EndTimezone = meeting.EndTimezone,
                           IsFinished = meeting.IsFinished,
                           IsStarted = meeting.IsStarted,
                           IsFirstVisit = meeting.IsFirstVisit,
                           PetId = meeting.PetId,
                           CenterServicesId = meeting.CenterServicesId,
                           Cost = meeting.Cost,

                       }).ToList();

                        return result;

                    }

                    
                }
                else
                {

                    if (PetOwnerId!=null)
                    {

                        if (view.IsNullOrEmpty())
                        {

                            var result = db.Appointment
                           .Where(a => a.MedicalCenterId == guid && a.PetOwnerId == (Guid)PetOwnerId && a.Start >= myDayStart && a.Start <= myDayEnd)
                           //.Where(combinedExpression)
                           .ToList()
                           .Select(meeting => new AppointmentViewModel
                           {
                               Id = meeting.Id,
                               Title = meeting.Title,
                               Start = meeting.Start,
                               End = meeting.End,
                               Description = meeting.Description,
                               IsAllDay = meeting.IsAllDay,
                               BaseClinicId = meeting.BaseClinicId,
                               PetOwnerId = meeting.PetOwnerId,
                               MedicalCenterId = meeting.MedicalCenterId,
                               RecurrenceRule = meeting.RecurrenceRule,
                               RecurrenceException = meeting.RecurrenceException,
                               RecurrenceID = meeting.RecurrenceID,
                               Timezone = meeting.Timezone,
                               StartTimezone = meeting.StartTimezone,
                               EndTimezone = meeting.EndTimezone,
                               IsFinished = meeting.IsFinished,
                               IsStarted = meeting.IsStarted,
                               IsFirstVisit = meeting.IsFirstVisit,
                               PetId = meeting.PetId,
                               CenterServicesId = meeting.CenterServicesId,
                               Cost = meeting.Cost,

                           }).ToList();

                            return result;
                        }
                        else
                        {
                            DateTime inputDate = DateTime.Parse(date);
                            DateTime startDate = new DateTime(inputDate.Year, inputDate.Month, inputDate.Day, 0, 0, 0);
                            DateTime endDate = new DateTime(inputDate.Year, inputDate.Month, inputDate.Day, 23, 59, 59);
                            if (view == "week")
                            {
                                startDate = inputDate.AddDays(-(int)inputDate.DayOfWeek);
                                endDate = startDate.AddDays(6);
                            }

                            if (view == "workWeek")
                            {
                                startDate = inputDate.AddDays(-(int)inputDate.DayOfWeek + (int)DayOfWeek.Monday);
                                endDate = startDate.AddDays(4); // Friday
                            }

                            if (view == "month")
                            {
                                startDate = new DateTime(inputDate.Year, inputDate.Month, 1);
                                endDate = startDate.AddMonths(1).AddDays(-1);
                            }

                            if (view == "year")
                            {
                                startDate = new DateTime(inputDate.Year, 1, 1);
                                endDate = new DateTime(inputDate.Year, 12, 31);
                            }

                            var result = db.Appointment
                           .Where(a => a.MedicalCenterId == guid && a.Start >= startDate && a.End <= endDate && a.PetOwnerId == (Guid)PetOwnerId)
                           //.Where(combinedExpression)
                           .ToList()
                           .Select(meeting => new AppointmentViewModel
                           {
                               Id = meeting.Id,
                               Title = meeting.Title,
                               Start = meeting.Start,
                               End = meeting.End,
                               Description = meeting.Description,
                               IsAllDay = meeting.IsAllDay,
                               BaseClinicId = meeting.BaseClinicId,
                               PetOwnerId = meeting.PetOwnerId,
                               MedicalCenterId = meeting.MedicalCenterId,
                               RecurrenceRule = meeting.RecurrenceRule,
                               RecurrenceException = meeting.RecurrenceException,
                               RecurrenceID = meeting.RecurrenceID,
                               Timezone = meeting.Timezone,
                               StartTimezone = meeting.StartTimezone,
                               EndTimezone = meeting.EndTimezone,
                               IsFinished = meeting.IsFinished,
                               IsStarted = meeting.IsStarted,
                               IsFirstVisit = meeting.IsFirstVisit,
                               PetId = meeting.PetId,
                               CenterServicesId = meeting.CenterServicesId,
                               Cost = meeting.Cost,

                           }).ToList();

                            return result;

                        }
                    }
                    else
                    {

                        if (view.IsNullOrEmpty())
                        {

                            var result = db.Appointment
                           .Where(a => a.MedicalCenterId == guid && a.Start >= myDayStart && a.Start <= myDayEnd)
                           //.Where(combinedExpression)
                           .ToList()
                           .Select(meeting => new AppointmentViewModel
                           {
                               Id = meeting.Id,
                               Title = meeting.Title,
                               Start = meeting.Start,
                               End = meeting.End,
                               Description = meeting.Description,
                               IsAllDay = meeting.IsAllDay,
                               BaseClinicId = meeting.BaseClinicId,
                               PetOwnerId = meeting.PetOwnerId,
                               MedicalCenterId = meeting.MedicalCenterId,
                               RecurrenceRule = meeting.RecurrenceRule,
                               RecurrenceException = meeting.RecurrenceException,
                               RecurrenceID = meeting.RecurrenceID,
                               Timezone = meeting.Timezone,
                               StartTimezone = meeting.StartTimezone,
                               EndTimezone = meeting.EndTimezone,
                               IsFinished = meeting.IsFinished,
                               IsStarted = meeting.IsStarted,
                               IsFirstVisit = meeting.IsFirstVisit,
                               PetId = meeting.PetId,
                               CenterServicesId = meeting.CenterServicesId,
                               Cost = meeting.Cost,

                           }).ToList();

                            return result;
                        }
                        else
                        {
                            DateTime inputDate = DateTime.Parse(date);
                            DateTime startDate = new DateTime(inputDate.Year, inputDate.Month, inputDate.Day, 0, 0, 0);
                            DateTime endDate = new DateTime(inputDate.Year, inputDate.Month, inputDate.Day, 23, 59, 59);
                            if (view == "week")
                            {
                                startDate = inputDate.AddDays(-(int)inputDate.DayOfWeek);
                                endDate = startDate.AddDays(6);
                            }

                            if (view == "workWeek")
                            {
                                startDate = inputDate.AddDays(-(int)inputDate.DayOfWeek + (int)DayOfWeek.Monday);
                                endDate = startDate.AddDays(4); // Friday
                            }

                            if (view == "month")
                            {
                                startDate = new DateTime(inputDate.Year, inputDate.Month, 1);
                                endDate = startDate.AddMonths(1).AddDays(-1);
                            }

                            if (view == "year")
                            {
                                startDate = new DateTime(inputDate.Year, 1, 1);
                                endDate = new DateTime(inputDate.Year, 12, 31);
                            }

                            var result = db.Appointment
                           .Where(a => a.MedicalCenterId == guid && a.Start >= startDate && a.End <= endDate)
                           //.Where(combinedExpression)
                           .ToList()
                           .Select(meeting => new AppointmentViewModel
                           {
                               Id = meeting.Id,
                               Title = meeting.Title,
                               Start = meeting.Start,
                               End = meeting.End,
                               Description = meeting.Description,
                               IsAllDay = meeting.IsAllDay,
                               BaseClinicId = meeting.BaseClinicId,
                               PetOwnerId = meeting.PetOwnerId,
                               MedicalCenterId = meeting.MedicalCenterId,
                               RecurrenceRule = meeting.RecurrenceRule,
                               RecurrenceException = meeting.RecurrenceException,
                               RecurrenceID = meeting.RecurrenceID,
                               Timezone = meeting.Timezone,
                               StartTimezone = meeting.StartTimezone,
                               EndTimezone = meeting.EndTimezone,
                               IsFinished = meeting.IsFinished,
                               IsStarted = meeting.IsStarted,
                               IsFirstVisit = meeting.IsFirstVisit,
                               PetId = meeting.PetId,
                               CenterServicesId = meeting.CenterServicesId,
                               Cost = meeting.Cost,

                           }).ToList();

                            return result;

                        }
                    }





                }
            }
        }

        public virtual IList<AppointmentViewModel> GetAllSystemMeetings( Guid? ClinicId, string? view, string? date, Guid? PetOwnerId)
        {
            DateTime myDayStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime myDayEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

            using (var db = GetContext())
            {

                if (ClinicId != null)
                {

                    if (PetOwnerId != null)
                    {
                        if (view.IsNullOrEmpty())
                        {

                            var result = db.Appointment
                           .Where(a =>  a.BaseClinicId == (Guid)ClinicId && a.PetOwnerId == (Guid)PetOwnerId && a.Start >= myDayStart && a.Start <= myDayEnd)
                           .ToList()
                           .Select(meeting => new AppointmentViewModel
                           {
                               Id = meeting.Id,
                               Title = meeting.Title,
                               Start = meeting.Start,
                               End = meeting.End,
                               Description = meeting.Description,
                               IsAllDay = meeting.IsAllDay,
                               BaseClinicId = meeting.BaseClinicId,
                               PetOwnerId = meeting.PetOwnerId,
                               MedicalCenterId = meeting.MedicalCenterId,
                               RecurrenceRule = meeting.RecurrenceRule,
                               RecurrenceException = meeting.RecurrenceException,
                               RecurrenceID = meeting.RecurrenceID,
                               Timezone = meeting.Timezone,
                               StartTimezone = meeting.StartTimezone,
                               EndTimezone = meeting.EndTimezone,
                               IsFinished = meeting.IsFinished,
                               IsStarted = meeting.IsStarted,
                               IsFirstVisit = meeting.IsFirstVisit,
                               Cost = meeting.Cost,

                           }).ToList();

                            return result;
                        }
                        else
                        {
                            DateTime inputDate = DateTime.Parse(date);
                            DateTime startDate = new DateTime(inputDate.Year, inputDate.Month, inputDate.Day, 0, 0, 0);
                            DateTime endDate = new DateTime(inputDate.Year, inputDate.Month, inputDate.Day, 23, 59, 59);
                            if (view == "week")
                            {
                                startDate = inputDate.AddDays(-(int)inputDate.DayOfWeek);
                                endDate = startDate.AddDays(6);
                            }

                            if (view == "workWeek")
                            {
                                startDate = inputDate.AddDays(-(int)inputDate.DayOfWeek + (int)DayOfWeek.Monday);
                                endDate = startDate.AddDays(4); // Friday
                            }

                            if (view == "month")
                            {
                                startDate = new DateTime(inputDate.Year, inputDate.Month, 1);
                                endDate = startDate.AddMonths(1).AddDays(-1);
                            }

                            if (view == "year")
                            {
                                startDate = new DateTime(inputDate.Year, 1, 1);
                                endDate = new DateTime(inputDate.Year, 12, 31);
                            }

                            var result = db.Appointment
                           .Where(a =>  a.BaseClinicId == (Guid)ClinicId && a.Start >= startDate && a.End <= endDate && a.PetOwnerId == (Guid)PetOwnerId)
                           .ToList()
                           .Select(meeting => new AppointmentViewModel
                           {
                               Id = meeting.Id,
                               Title = meeting.Title,
                               Start = meeting.Start,
                               End = meeting.End,
                               Description = meeting.Description,
                               IsAllDay = meeting.IsAllDay,
                               BaseClinicId = meeting.BaseClinicId,
                               PetOwnerId = meeting.PetOwnerId,
                               MedicalCenterId = meeting.MedicalCenterId,
                               RecurrenceRule = meeting.RecurrenceRule,
                               RecurrenceException = meeting.RecurrenceException,
                               RecurrenceID = meeting.RecurrenceID,
                               Timezone = meeting.Timezone,
                               StartTimezone = meeting.StartTimezone,
                               EndTimezone = meeting.EndTimezone,
                               IsFinished = meeting.IsFinished,
                               IsStarted = meeting.IsStarted,
                               IsFirstVisit = meeting.IsFirstVisit,
                                                              Cost=meeting.Cost,

                           }).ToList();

                            return result;

                        }
                    }

                    if (view.IsNullOrEmpty())
                    {

                        var result = db.Appointment
                       .Where(a =>  a.BaseClinicId == (Guid)ClinicId && a.Start >= myDayStart && a.Start <= myDayEnd)
                       .ToList()
                       .Select(meeting => new AppointmentViewModel
                       {
                           Id = meeting.Id,
                           Title = meeting.Title,
                           Start = meeting.Start,
                           End = meeting.End,
                           Description = meeting.Description,
                           IsAllDay = meeting.IsAllDay,
                           BaseClinicId = meeting.BaseClinicId,
                           PetOwnerId = meeting.PetOwnerId,
                           MedicalCenterId = meeting.MedicalCenterId,
                           RecurrenceRule = meeting.RecurrenceRule,
                           RecurrenceException = meeting.RecurrenceException,
                           RecurrenceID = meeting.RecurrenceID,
                           Timezone = meeting.Timezone,
                           StartTimezone = meeting.StartTimezone,
                           EndTimezone = meeting.EndTimezone,
                           IsFinished = meeting.IsFinished,
                           IsStarted = meeting.IsStarted,
                           IsFirstVisit = meeting.IsFirstVisit,
                                                          Cost=meeting.Cost,

                       }).ToList();

                        return result;
                    }
                    else
                    {
                        DateTime inputDate = DateTime.Parse(date);
                        DateTime startDate = new DateTime(inputDate.Year, inputDate.Month, inputDate.Day, 0, 0, 0);
                        DateTime endDate = new DateTime(inputDate.Year, inputDate.Month, inputDate.Day, 23, 59, 59);
                        if (view == "week")
                        {
                            startDate = inputDate.AddDays(-(int)inputDate.DayOfWeek);
                            endDate = startDate.AddDays(6);
                        }

                        if (view == "workWeek")
                        {
                            startDate = inputDate.AddDays(-(int)inputDate.DayOfWeek + (int)DayOfWeek.Monday);
                            endDate = startDate.AddDays(4); // Friday
                        }

                        if (view == "month")
                        {
                            startDate = new DateTime(inputDate.Year, inputDate.Month, 1);
                            endDate = startDate.AddMonths(1).AddDays(-1);
                        }

                        if (view == "year")
                        {
                            startDate = new DateTime(inputDate.Year, 1, 1);
                            endDate = new DateTime(inputDate.Year, 12, 31);
                        }

                        var result = db.Appointment
                       .Where(a =>  a.BaseClinicId == (Guid)ClinicId && a.Start >= startDate && a.End <= endDate)
                       .ToList()
                       .Select(meeting => new AppointmentViewModel
                       {
                           Id = meeting.Id,
                           Title = meeting.Title,
                           Start = meeting.Start,
                           End = meeting.End,
                           Description = meeting.Description,
                           IsAllDay = meeting.IsAllDay,
                           BaseClinicId = meeting.BaseClinicId,
                           PetOwnerId = meeting.PetOwnerId,
                           MedicalCenterId = meeting.MedicalCenterId,
                           RecurrenceRule = meeting.RecurrenceRule,
                           RecurrenceException = meeting.RecurrenceException,
                           RecurrenceID = meeting.RecurrenceID,
                           Timezone = meeting.Timezone,
                           StartTimezone = meeting.StartTimezone,
                           EndTimezone = meeting.EndTimezone,
                           IsFinished = meeting.IsFinished,
                           IsStarted = meeting.IsStarted,
                           IsFirstVisit = meeting.IsFirstVisit,
                                                          Cost=meeting.Cost,

                       }).ToList();

                        return result;

                    }


                }
                else
                {

                    if (PetOwnerId != null)
                    {

                        if (view.IsNullOrEmpty())
                        {

                            var result = db.Appointment
                           .Where(a =>  a.PetOwnerId == (Guid)PetOwnerId && a.Start >= myDayStart && a.Start <= myDayEnd)
                           .ToList()
                           .Select(meeting => new AppointmentViewModel
                           {
                               Id = meeting.Id,
                               Title = meeting.Title,
                               Start = meeting.Start,
                               End = meeting.End,
                               Description = meeting.Description,
                               IsAllDay = meeting.IsAllDay,
                               BaseClinicId = meeting.BaseClinicId,
                               PetOwnerId = meeting.PetOwnerId,
                               MedicalCenterId = meeting.MedicalCenterId,
                               RecurrenceRule = meeting.RecurrenceRule,
                               RecurrenceException = meeting.RecurrenceException,
                               RecurrenceID = meeting.RecurrenceID,
                               Timezone = meeting.Timezone,
                               StartTimezone = meeting.StartTimezone,
                               EndTimezone = meeting.EndTimezone,
                               IsFinished = meeting.IsFinished,
                               IsStarted = meeting.IsStarted,
                               IsFirstVisit = meeting.IsFirstVisit,
                                                              Cost=meeting.Cost,

                           }).ToList();

                            return result;
                        }
                        else
                        {
                            DateTime inputDate = DateTime.Parse(date);
                            DateTime startDate = new DateTime(inputDate.Year, inputDate.Month, inputDate.Day, 0, 0, 0);
                            DateTime endDate = new DateTime(inputDate.Year, inputDate.Month, inputDate.Day, 23, 59, 59);
                            if (view == "week")
                            {
                                startDate = inputDate.AddDays(-(int)inputDate.DayOfWeek);
                                endDate = startDate.AddDays(6);
                            }

                            if (view == "workWeek")
                            {
                                startDate = inputDate.AddDays(-(int)inputDate.DayOfWeek + (int)DayOfWeek.Monday);
                                endDate = startDate.AddDays(4); // Friday
                            }

                            if (view == "month")
                            {
                                startDate = new DateTime(inputDate.Year, inputDate.Month, 1);
                                endDate = startDate.AddMonths(1).AddDays(-1);
                            }

                            if (view == "year")
                            {
                                startDate = new DateTime(inputDate.Year, 1, 1);
                                endDate = new DateTime(inputDate.Year, 12, 31);
                            }

                            var result = db.Appointment
                           .Where(a => a.Start >= startDate && a.End <= endDate && a.PetOwnerId == (Guid)PetOwnerId)
                           .ToList()
                           .Select(meeting => new AppointmentViewModel
                           {
                               Id = meeting.Id,
                               Title = meeting.Title,
                               Start = meeting.Start,
                               End = meeting.End,
                               Description = meeting.Description,
                               IsAllDay = meeting.IsAllDay,
                               BaseClinicId = meeting.BaseClinicId,
                               PetOwnerId = meeting.PetOwnerId,
                               MedicalCenterId = meeting.MedicalCenterId,
                               RecurrenceRule = meeting.RecurrenceRule,
                               RecurrenceException = meeting.RecurrenceException,
                               RecurrenceID = meeting.RecurrenceID,
                               Timezone = meeting.Timezone,
                               StartTimezone = meeting.StartTimezone,
                               EndTimezone = meeting.EndTimezone,
                               IsFinished = meeting.IsFinished,
                               IsStarted = meeting.IsStarted,
                               IsFirstVisit = meeting.IsFirstVisit,
                                                              Cost=meeting.Cost,

                           }).ToList();

                            return result;

                        }
                    }
                    else
                    {

                        if (view.IsNullOrEmpty())
                        {

                            var result = db.Appointment
                           .Where(a =>  a.Start >= myDayStart && a.Start <= myDayEnd)
                           .ToList()
                           .Select(meeting => new AppointmentViewModel
                           {
                               Id = meeting.Id,
                               Title = meeting.Title,
                               Start = meeting.Start,
                               End = meeting.End,
                               Description = meeting.Description,
                               IsAllDay = meeting.IsAllDay,
                               BaseClinicId = meeting.BaseClinicId,
                               PetOwnerId = meeting.PetOwnerId,
                               MedicalCenterId = meeting.MedicalCenterId,
                               RecurrenceRule = meeting.RecurrenceRule,
                               RecurrenceException = meeting.RecurrenceException,
                               RecurrenceID = meeting.RecurrenceID,
                               Timezone = meeting.Timezone,
                               StartTimezone = meeting.StartTimezone,
                               EndTimezone = meeting.EndTimezone,
                               IsFinished = meeting.IsFinished,
                               IsStarted = meeting.IsStarted,
                               IsFirstVisit = meeting.IsFirstVisit,
                                                              Cost=meeting.Cost,

                           }).ToList();

                            return result;
                        }
                        else
                        {
                            DateTime inputDate = DateTime.Parse(date);
                            DateTime startDate = new DateTime(inputDate.Year, inputDate.Month, inputDate.Day, 0, 0, 0);
                            DateTime endDate = new DateTime(inputDate.Year, inputDate.Month, inputDate.Day, 23, 59, 59);
                            if (view == "week")
                            {
                                startDate = inputDate.AddDays(-(int)inputDate.DayOfWeek);
                                endDate = startDate.AddDays(6);
                            }

                            if (view == "workWeek")
                            {
                                startDate = inputDate.AddDays(-(int)inputDate.DayOfWeek + (int)DayOfWeek.Monday);
                                endDate = startDate.AddDays(4); // Friday
                            }

                            if (view == "month")
                            {
                                startDate = new DateTime(inputDate.Year, inputDate.Month, 1);
                                endDate = startDate.AddMonths(1).AddDays(-1);
                            }

                            if (view == "year")
                            {
                                startDate = new DateTime(inputDate.Year, 1, 1);
                                endDate = new DateTime(inputDate.Year, 12, 31);
                            }

                            var result = db.Appointment
                           .Where(a =>  a.Start >= startDate && a.End <= endDate)
                           .ToList()
                           .Select(meeting => new AppointmentViewModel
                           {
                               Id = meeting.Id,
                               Title = meeting.Title,
                               Start = meeting.Start,
                               End = meeting.End,
                               Description = meeting.Description,
                               IsAllDay = meeting.IsAllDay,
                               BaseClinicId = meeting.BaseClinicId,
                               PetOwnerId = meeting.PetOwnerId,
                               MedicalCenterId = meeting.MedicalCenterId,
                               RecurrenceRule = meeting.RecurrenceRule,
                               RecurrenceException = meeting.RecurrenceException,
                               RecurrenceID = meeting.RecurrenceID,
                               Timezone = meeting.Timezone,
                               StartTimezone = meeting.StartTimezone,
                               EndTimezone = meeting.EndTimezone,
                               IsFinished = meeting.IsFinished,
                               IsStarted = meeting.IsStarted,
                               IsFirstVisit = meeting.IsFirstVisit,
                                                              Cost=meeting.Cost,

                           }).ToList();

                            return result;

                        }
                    }





                }
            }
        }

        public virtual void Insert(AppointmentViewModel meeting, Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState)
        {

                using (var db = GetContext())
                {
  

                    if (string.IsNullOrEmpty(meeting.Title))
                    {
                        meeting.Title = "";
                    }

                    var entity = meeting.ToEntity();


                    db.Appointment.Add(entity);
                    db.SaveChanges();

                    meeting.Id = entity.Id;
                }
        }

        public virtual void Update(AppointmentViewModel meeting, ModelStateDictionary modelState)
        {

                    using (var db = GetContext())
                    {
                        if (string.IsNullOrEmpty(meeting.Title))
                        {
                            meeting.Title = "";
                        }

                        var entity = meeting.ToEntity();
                        db.Appointment.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;

                        db.SaveChanges();
                    }
        }

        public virtual void Delete(AppointmentViewModel meeting, ModelStateDictionary modelState)
        {

                using (var db = GetContext())
                {


                    var entity = meeting.ToEntity();

                    db.Appointment.Attach(entity);


                    var recurrenceExceptions = db.Appointment.Where(m => m.Id == entity.Id);

                    foreach (var recurrenceException in recurrenceExceptions)
                    {
                        db.Appointment.Remove(recurrenceException);
                    }

                    db.Entry(entity).State = EntityState.Deleted;
                    db.Appointment.Remove(entity);
                    db.SaveChanges();
                }
        }

        private bool ValidateModel(AppointmentViewModel appointment, ModelStateDictionary modelState)
        {
            if (appointment.Start > appointment.End)
            {
                modelState.AddModelError("errors", "End date must be greater or equal to Start date.");
                return false;
            }

            return true;
        }



        public void Update(AppointmentViewModel appointment, Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState)
        {
            using (var db = GetContext()) 
            {

                if (db.Appointment.Find(appointment.Id).IsFinished || db.Appointment.Find(appointment.Id).IsStarted)
                {
                    return;
                }
                db.ChangeTracker.Clear();

                if (string.IsNullOrEmpty(appointment.Title))
                {
                    appointment.Title = "";
                }

                var entity = appointment.ToEntity();
                db.Appointment.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public void Delete(AppointmentViewModel appointment, Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState)
        {

            if (appointment.IsStarted || appointment.IsFinished)
            {
                return;
            }

            using (var db = GetContext())
            {
                var entity = db.Appointment.Find(appointment.Id);
                db.Appointment.Remove(entity);
                db.SaveChanges();
            }
        }
    }


    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            var parameter = Expression.Parameter(typeof(T));

            var body = Expression.AndAlso(
                Expression.Invoke(first, parameter),
                Expression.Invoke(second, parameter)
            );

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }

}
