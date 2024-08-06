using CmsDataAccess.DbModels;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NoificationManager.MobileModels;
using Quartz;
using ServicesLibrary.MedicalCenterServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLibrary.RepeatedJobs
{
    [DisallowConcurrentExecution]
    public class ProcessAppointmentsJob : IJob
    {
        private readonly ApplicationDbContext cmsContext;
        private readonly ILogger<ProcessAppointmentsJob> _logger;
        private readonly INotificationService notiService;
        private readonly IMedicalCenterService medicalCenterService;

        public ProcessAppointmentsJob(
            IMedicalCenterService medicalCenterService_,
        INotificationService notiService_,

            ILogger<ProcessAppointmentsJob> logger,
            ApplicationDbContext dbContext
            )
        {
            notiService = notiService_;
            medicalCenterService= medicalCenterService_;
            _logger = logger;
            cmsContext = dbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Started");


           List<Appointment> appointments = cmsContext.Appointment
                .Where(a=>!a.IsFinished && !a.IsStarted && !a.Notified && a.Start.AddHours(1)>=DateTime.Now)
                .ToList();

            foreach (Appointment appointment in appointments)
            {

                Person person = cmsContext.Person.Find(appointment.PetOwnerId);   

                if (!person.fcm_token.IsNullOrEmpty())
                {
                    MedicalCenter medicalCenter = cmsContext.MedicalCenter.Find(appointment.MedicalCenterId);

                    DateTime appDateTime = medicalCenterService.ConvertToLocalTime(medicalCenter.Id, appointment.Start);


                    string Body = $"عزيزي المريض، نود أن نذكرك بأن لديك موعد زيارة طبية في {appDateTime}، مع تمنياتنا بالشفاء العاجل.";
                    string Title = "تذكير بموعد";

                    notiService.SendNotification(new NotificationModel { Title= Title, Body= Body, Token = person.fcm_token });
                    
                    DateTime notiDateTime = medicalCenterService.ConvertToLocalTime(medicalCenter.Id, DateTime.Now);

                    cmsContext.PersonNotification.Add(
                        new PersonNotification
                        {
                            BaseClinicId=appointment.BaseClinicId,
                            PersonId=appointment.PetOwnerId,
                            MedicalCenterId=appointment.MedicalCenterId,
                            Body=Body,
                            Title=Title,
                            CreateDate= notiDateTime,
                        }
                        );

                    cmsContext.SaveChanges();

                    appointment.Notified = true;
                    cmsContext.Appointment.Attach(appointment);
                    cmsContext.Entry(appointment).Property(a => a.Notified).IsModified = true;
                    cmsContext.SaveChanges();

                }


    
            }

        }



    }
}
