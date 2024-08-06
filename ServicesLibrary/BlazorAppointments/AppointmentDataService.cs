using CmsDataAccess.DbModels;
using Microsoft.EntityFrameworkCore;
using ServicesLibrary.MedicalCenterServices;
using ServicesLibrary.UserServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLibrary.BlazorAppointments
{
    public class AppointmentDataService
    {
        private readonly ApplicationDbContext? applicationDbContext;
        private readonly IMedicalCenterService medicalCenterService;
        private readonly IUserService userService ;

        public AppointmentDataService(ApplicationDbContext applicationDbContext_, IMedicalCenterService medicalCenterService_, IUserService userService_)
        {
            medicalCenterService = medicalCenterService_;
            applicationDbContext = applicationDbContext_;
            userService= userService_;
        }

        public async Task<List<Appointment>> Get(DateTime startDate,DateTime endDate)
        {
            Guid guid = (Guid)userService.GetMyCenterIdWeb();

            List<Appointment> res= await applicationDbContext.Appointment.Where(a=> a.Start>=startDate && a.End<= endDate || a.RecurrenceRule!=null && a.MedicalCenterId==guid  )
                .ToListAsync();

            DateTime now = medicalCenterService.ConvertToLocalTime(DateTime.Now);

            foreach (var item in res)
            {
                if (item.End< now)
                {
                    item.Color = "gray";
                }
                else
                {
                    if (item.Start>now)
                    {
                        item.Color = "green";

                    }
                    else
                    {
                        item.Color = "blue";
                    }
                }
            }

            return res;
        }

        public async Task Insert(Appointment appointment)
        {
            appointment.MedicalCenterId = (Guid)userService.GetMyCenterIdWeb();
            appointment.RecurrenceException = "";
            appointment.RecurrenceRule = "";
            await applicationDbContext.Appointment.AddAsync(appointment);
            await applicationDbContext.SaveChangesAsync();

        }
        public async Task Update(Appointment appointment)
        {
            ApplicationDbContext applicationDb = new ApplicationDbContext();
            applicationDb.Appointment.Attach(appointment);
            applicationDb.Entry(appointment).State = EntityState.Modified;
            await applicationDb.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            ApplicationDbContext applicationDb = new ApplicationDbContext();
            var appointment= await applicationDb.Appointment.FindAsync(id);
            applicationDb.Appointment.Remove(appointment);
            await applicationDb.SaveChangesAsync();
        }
    }
}
