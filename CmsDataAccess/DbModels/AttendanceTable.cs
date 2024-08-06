
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.DbModels
{
	public class AttendanceTable
    {
		[Key]
		public Guid Id { get; set; }

		[ForeignKey("IdentityUser")]
		public Guid IdentityUserId { set; get;}

        [ForeignKey("MedicalCenter")]
        public Guid MedicalCenterId { set; get; }

        [ForeignKey("ShiftTable")]
        public Guid ShiftTableId { set; get; }

        public DateTime EnteringTime { get; set; }
		public DateTime LeavingTime { get; set; }

        public DateTime ShiftEnteringTime { get; set; }
        public DateTime ShiftLeavingTime { get; set; }

        public bool DayOff { get; set; } = false;
        public string ReasonForAbsence { get; set; } = "";

        public string ShiftName { get; set; } = "";


        [NotMapped]
		public DayOfWeek DayWeek {
			get
			{
				return EnteringTime.DayOfWeek;
			}
		}

        [NotMapped]
        public int DayMonth
        {
            get
            {
                return EnteringTime.Day;
            }
        }

        [NotMapped]
        public int Month
        {
            get
            {
                return EnteringTime.Month;
            }
        }

        [NotMapped]
        public int Year
        {
            get
            {
                return EnteringTime.Year;
            }
        }

        [NotMapped]
        public TimeSpan EnteringTime_
        {
            get
            {
                return EnteringTime.TimeOfDay;
            }
        }

        [NotMapped]
        public TimeSpan LeavingTime_
        {
            get
            {
                return LeavingTime.TimeOfDay;
            }
        }

        [NotMapped]
        public double Duration
        {
            get
            {
                return (LeavingTime- EnteringTime).TotalHours;
            }
        }

        public AttendanceTable GetFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                AttendanceTable Medic = context.AttendanceTable
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
                context.AttendanceTable.Add(this);
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
                context.AttendanceTable.Remove(GetFromDb());
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }

}
