using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmsDataAccess.Utils.FilesUtils;
using Microsoft.EntityFrameworkCore;
using CmsResources;

namespace CmsDataAccess.DbModels
{
    [Table("PetOwner")]

    public class PetOwner : Person
    {

       


        [Display(Name = nameof(Messages.GeneralNumber), ResourceType = typeof(Messages))]
        public int GeneralNumber { set; get; }

        [Display(Name = nameof(Messages.MedicalRecordNumber), ResourceType = typeof(Messages))]
        public string MedicalRecordNumber { get; set; } = "";

        [Display(Name = nameof(Messages.InsuranceId), ResourceType = typeof(Messages))]
        public string? InsuranceId { get; set; } = "";

        [Display(Name = nameof(Messages.InsuranceCompany), ResourceType = typeof(Messages))]
        public string? InsuranceCompany { get; set; } = "";

        [Display(Name = nameof(Messages.BloodType), ResourceType = typeof(Messages))]
        public int? BloodType { get; set; }

        [Display(Name = nameof(Messages.MedicalCenterId), ResourceType = typeof(Messages))]
        public Guid? MedicalCenterId { get; set; }

        public PetOwner GetFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                PetOwner Medic = context.PetOwner
                    .Include(a => a.User)
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
                context.PetOwner.Add(this);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            try
            {
                PetOwner temp = GetFromDb();

                if (context.Appointment.Any(a => a.PetOwnerId == Id) || context.COrder.Any(a => a.PetOwnerId == Id))
                {
                    temp.SoftDelte();
                    return true;
                }

                

                FileHandler.DeleteImageFile(temp.ImageName);
                temp = GetFromDb();
                context.PetOwner.Remove(temp);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SoftDelte()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            try
            {
                PetOwner temp = GetFromDb();
                //FileHandler.DeleteImageFile(temp.ImageName);

                temp.IsDeleted = true;
                context.PetOwner.Attach(temp);
                context.Entry(temp).State = EntityState.Modified;
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }






        [NotMapped]
        public string CenterName
        {
            get
            {
                try
                {

                    MedicalCenter res = new ApplicationDbContext().MedicalCenter.Find(MedicalCenterId).GetModelByLnag("Ar");

                    return res.MedicalCenterTranslation[0].Name;
                }
                catch
                {
                    return "";
                }

            }
        }

    }
}