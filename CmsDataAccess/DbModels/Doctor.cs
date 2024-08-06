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
    [Table("Doctor")]
    public class Doctor : Person
    {

        [ForeignKey("Clinic")]
                [Display(Name = nameof(Messages.ClinicId), ResourceType = typeof(Messages))]

        public Guid? ClinicId { get; set; }

        [ForeignKey("MedicalCenter")]
                [Display(Name = nameof(Messages.MedicalCenterId), ResourceType = typeof(Messages))]

        public Guid? MedicalCenterId { get; set; }

        public List<DoctorSpeciality>? DoctorSpeciality { get; set; }
        
        public List<Certificate>? Certificate { get; set; }


        public List<DoctorTranslation>? DoctorTranslation { get; set; }
        public BaseClinic? BaseClinic()
        {
            try
            {
                return new ApplicationDbContext().BaseClinic.Find(ClinicId);
            }
            catch (Exception)
            {

                return null;
            }
        }

        [NotMapped]
        public string? _LangCode;

        [NotMapped]
        public string? LangCode
        {
            get { return _LangCode; }
            set
            {

                try
                {
                    _LangCode = value.ToLower();
                }
                catch { }
            }
        }

        public double? AverageRating()
        {
            List<DoctorRating> list = new ApplicationDbContext().DoctorRating.Where(a => a.DoctorId == Id).ToList();

            if (list.Count > 0)
            {
                return list.Sum(a => a.Points) / list.Count;
            }

            return null;
        }


        public Doctor GetFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                Doctor Medic = context.Doctor
                    .Include(a => a.User)
                    .Include(a => a.Certificate)
                    .Include(a => a.DoctorTranslation)
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
                context.Doctor.Add(this);
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
                Doctor temp = GetFromDb();
                FileHandler.DeleteImageFile(temp.ImageName);

                foreach (var item in temp.Certificate)
                {
                    item.DeleteFromDb();
                }
                context.IdentityUser.Remove(temp.User);
                temp = GetFromDb();
                context.Doctor.Remove(temp);
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
                Doctor temp = GetFromDb();
                //FileHandler.DeleteImageFile(temp.ImageName);

                temp.IsDeleted=true;
                context.Doctor.Attach(temp);
                context.Entry(temp).State= EntityState.Modified;
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Doctor GetModelByLnag(string langCode = "en-US")
        {
            langCode = langCode.ToLower();

            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                Doctor Medic = context.Doctor
                    .Include(a => a.User)
                    .Include(a => a.Certificate).ThenInclude(a => a.CertificateTranslation.Where(a => a.LangCode == langCode))
                    .Include(a => a.DoctorTranslation.Where(a => a.LangCode == langCode))
                    .FirstOrDefault(a => a.Id == Id);

                return Medic;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public List<Guid>? DoctorSpecialityId
        {
            get
            {
                try
                {
                    return DoctorSpeciality.Select(a => a.Id).ToList();
                }
                catch { return null; }
            }
        }



    }

    public class DocSpecBreakRelation
    {

        [Key]
        public Guid Id { get; set; }

        [ForeignKey("DoctorSpeciality")]

        public Guid DoctorSpecialityId { get; set; }


        [ForeignKey("Doctor")]

        public Guid DoctorId { get; set; }


    }

}
