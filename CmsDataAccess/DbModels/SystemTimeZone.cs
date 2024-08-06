using CmsResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.DbModels
{
    public class SystemTimeZone
    {
        [Key]
        public Guid Id { get; set; }

        [Display(Name = nameof(Messages.DisplayName), ResourceType = typeof(Messages))]


        public string DisplayName { get; set; }
        [Display(Name = nameof(Messages.StandardName), ResourceType = typeof(Messages))]


        public string StandardName { get; set; }
        public string? IANAID { get; set; }

        //[ForeignKey("MedicalCenter")]
        //public Guid? MedicalCenterId { get; set; }

        public SystemTimeZone GetFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                SystemTimeZone Medic = context.SystemTimeZone
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
                context.SystemTimeZone.Add(this);
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
                context.SystemTimeZone.Remove(GetFromDb());
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
