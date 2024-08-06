using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.DbModels
{

    public class ShiftTable
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("MedicalCenter")]
        public Guid MedicalCenterId { set; get; }
        public string ShiftName { get; set; } = "";
        public DayOfWeek DayOfWeek { get; set; }

        [UIHint("time")]
        [Display(Name = "Start Time")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]

        public TimeSpan StartTime { get; set; }

        [UIHint("time")]
        [Display(Name = "End Time")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan EndTime { get; set; }






        public ShiftTable GetFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                ShiftTable Medic = context.ShiftTable
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
                context.ShiftTable.Add(this);
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
                context.ShiftTable.Remove(GetFromDb());
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
