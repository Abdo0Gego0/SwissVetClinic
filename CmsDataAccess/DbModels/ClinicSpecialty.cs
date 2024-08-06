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
    public class ClinicSpecialty
    {
        [Key]
        public Guid Id { get; set; }
        public List<ClinicSpecialtyTranslation>? ClinicSpecialtyTranslation { get; set; }

        [ForeignKey("MedicalCenter")]
        public Guid MedicalCenterId { get; set; }


        public ClinicSpecialty GetFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                ClinicSpecialty Medic = context.ClinicSpecialty
                    .Include(a => a.ClinicSpecialtyTranslation)
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
                context.ClinicSpecialty.Add(this);
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
                context.ClinicSpecialty.Remove(GetFromDb());
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ClinicSpecialty GetModelByLnag(string langCode = "en-US")
        {
            langCode = langCode.ToLower();

            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                ClinicSpecialty Medic = context.ClinicSpecialty
                    .Include(a => a.ClinicSpecialtyTranslation.Where(a => a.LangCode == langCode))
                    .FirstOrDefault(a => a.Id == Id);

                return Medic;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public override string ToString()
        {
            
            ClinicSpecialty clinicSpecialty = GetFromDb();

            return string.Join(", " ,clinicSpecialty.ClinicSpecialtyTranslation.Select(a=>a.Name));

        }





    }
}
