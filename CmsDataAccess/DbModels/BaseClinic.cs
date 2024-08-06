using CmsDataAccess.AutoMapping;
using CmsDataAccess.ModelsDto;
using CmsDataAccess.TimeZoneUtils;
using CmsDataAccess.Utils.FilesUtils;
using CmsResources;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.DbModels
{
    public class BaseClinic
    {
        [Key]
        public Guid Id { get; set; }
        [Display(Name = nameof(Messages.RoomNumber), ResourceType = typeof(Messages))] 


        public string? RoomNumber { get; set; } = string.Empty;
        [Display(Name = nameof(Messages.Color), ResourceType = typeof(Messages))]

        public string? Color { get; set; } = "Blue";

        [ForeignKey("MedicalCenter")]
        [Display(Name = nameof(Messages.MedicalCenterId), ResourceType = typeof(Messages))]

        public Guid MedicalCenterId { get; set; }
        public bool SubscriptionExpired { get; set; } = false;
        [Display(Name = nameof(Messages.IsShownOnMobile), ResourceType = typeof(Messages))]

        public bool IsShownOnMobile { get; set; } = false;
        public bool IsDraft { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        [NotMapped]
        public bool IsExpand { get; set; } = false;

        [Display(Name = nameof(Messages.ClinicName), ResourceType = typeof(Messages))]


        public string ClinicName
        {
            get
            {
                try
                {
                    return BaseClinicTranslation[1].Name + " " + BaseClinicTranslation[0].Name;
                }
                catch
                {
                    return BaseClinicTranslation[0].Name;
                }
            }
        }
            
          

        public List<OpeningHours>? OpeningHours { get; set; }
        public List<BaseClinicTranslation>? BaseClinicTranslation { get; set; }
        public string WorkHoursToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (OpeningHours branchTime in (IEnumerable<OpeningHours>)this.OpeningHours)
                stringBuilder.AppendLine(branchTime.HoursOfOperation());
            return stringBuilder.ToString();
        }

        [ForeignKey("ClinicSpecialty")]
        public Guid? ClinicSpecialtyId { get; set; }

        [Display(Name = "ImageName")]
        public string? ImageName { get; set; } = "";

        [Display(Name = "ImageName")]

        public double? VisitCost{ get; set; } 

        public string? ImageFullPath
        {
            get
            {
                return new ApplicationDbContext().MySystemConfiguration.FirstOrDefault().ApiUrl + "pImages/" + ImageName;
            }
        }


        [NotMapped]
        public IFormFile? ImageFile { get; set; } 


        [NotMapped]
        public string? _LangCode;

        [NotMapped]
        public string? LangCode
        {
            get { return _LangCode; }
            set {

                try
                {
                    _LangCode = value.ToLower();
                }
                catch { }
            }
        }


        public ClinicSpecialty? ClinicSpecialty()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            if (LangCode.IsNullOrEmpty())
            { 
            ClinicSpecialty ClinicSpecialty = context.ClinicSpecialty.Include(a => a.ClinicSpecialtyTranslation).FirstOrDefault(a => a.Id == ClinicSpecialtyId);

            return ClinicSpecialty!=null? ClinicSpecialty:null;
            }

            ClinicSpecialty ClinicSpecialty1= context.ClinicSpecialty
                .Include(a => a.ClinicSpecialtyTranslation.Where(a=>a.LangCode== LangCode))
                .FirstOrDefault(a => a.Id == ClinicSpecialtyId);

            return ClinicSpecialty1 != null ? ClinicSpecialty1 : null;
        }

        public object? ClinicSpecialty(string lang)
        {
            ApplicationDbContext context = new ApplicationDbContext();


            var ClinicSpecialty = context.ClinicSpecialty.Include(a => a.ClinicSpecialtyTranslation.Where(a=>a.LangCode==lang))
                .FirstOrDefault(a => a.Id == ClinicSpecialtyId)
                ;

            if (ClinicSpecialty==null)
            {
                return null;
            }

            if (ClinicSpecialty.ClinicSpecialtyTranslation==null || ClinicSpecialty.ClinicSpecialtyTranslation.Count==0)
            {
                return null;

            }

            return ClinicSpecialty.ClinicSpecialtyTranslation[0].Name;

        }

        public BaseClinic GetFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                BaseClinic Medic = context.BaseClinic
                    .Include(a => a.BaseClinicTranslation)
                    .Include(a => a.OpeningHours)
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
                context.BaseClinic.Add(this);
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
                BaseClinic temp = GetFromDb();
                FileHandler.DeleteImageFile(temp.ImageName);
                context.BaseClinic.Remove(temp);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool IsOpen()
        {
            DateTime currentDate = DateTime.Now;
            foreach (OpeningHours openingHours in OpeningHours)
            {
                if (currentDate.DayOfWeek == openingHours.DayOfWeek)
                {
                    if (currentDate.TimeOfDay >= openingHours.OpeningTime && currentDate.TimeOfDay <= openingHours.ClosingTime)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public BaseClinic GetModelByLnag(string langCode = "en-US")
        {
            langCode = langCode.ToLower();

            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                BaseClinic Medic = context.BaseClinic
                    .Include(a => a.BaseClinicTranslation.Where(a=>a.LangCode==langCode))
                    .Include(a => a.OpeningHours)
                    .FirstOrDefault(a => a.Id == Id);


                if (Medic.BaseClinicTranslation == null || Medic.BaseClinicTranslation.Count()==0)
                {
                    Medic = context.BaseClinic
     .Include(a => a.BaseClinicTranslation)
     .Include(a => a.OpeningHours)
     .FirstOrDefault(a => a.Id == Id);
                }



                return Medic;
            }
            catch (Exception ex)
            {
                BaseClinic Medic = context.BaseClinic
     .Include(a => a.BaseClinicTranslation)
     .Include(a => a.OpeningHours)
     .FirstOrDefault(a => a.Id == Id);
                return Medic;

            }
        }
        public override string ToString() 
        {

            StringBuilder stringBuilder = new StringBuilder();

            if (LangCode.IsNullOrEmpty())
            {
                stringBuilder.AppendLine($"Room number: {RoomNumber}");
                stringBuilder.AppendLine($"Opening Hours: {this.WorkHoursToString()}");
                stringBuilder.AppendLine($"Clinic Specialty: {string.Join(", ", this.ClinicSpecialty().ClinicSpecialtyTranslation.Select(a => a.Name))}");
                stringBuilder.AppendLine($"Base Clinic Translation: {string.Join(", ", this.BaseClinicTranslation.Select(a => a.Name))}");
                return stringBuilder.ToString();
    
            }

            stringBuilder.AppendLine($"Room number: {RoomNumber}");
            stringBuilder.AppendLine($"Opening Hours: {this.WorkHoursToString()}");
            stringBuilder.AppendLine($"Clinic Specialty: {string.Join(", ", this.ClinicSpecialty().ClinicSpecialtyTranslation.Select(a => a.Name))}");
            stringBuilder.AppendLine($"Base Clinic Translation: {string.Join(", ", this.BaseClinicTranslation.Where(a=>a.LangCode==LangCode).Select(a => a.Name))}");
            return stringBuilder.ToString();

        }
        public BaseClinicDto ToDto()
        {
            return MappingToDto.Mapper.Map<BaseClinicDto>(this);
        }
    }
}
