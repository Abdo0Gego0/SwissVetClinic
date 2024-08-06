using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.DbModels
{
    public class DoctorSpeciality
    {
        [Key]
        public Guid Id { get; set; }

        public List<DoctorSpecialityTranslation> DoctorSpecialityTranslation { get; set; }

        [ForeignKey("MedicalCenter")]
        public Guid MedicalCenterId { get; set; }

        public List<Doctor>? Doctor { get; set; }

        public DoctorSpeciality GetFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                DoctorSpeciality Medic = context.DoctorSpeciality
                    .Include(a => a.DoctorSpecialityTranslation)
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
                context.DoctorSpeciality.Add(this);
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
                context.DoctorSpeciality.Remove(GetFromDb());
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
