using AutoMapper;
using CmsDataAccess.AutoMapping;
using CmsDataAccess.ModelsDto;
using CmsDataAccess.TimeZoneUtils;
using CmsDataAccess.Utils.FilesUtils;
using CmsResources;
using Microsoft.AspNetCore.Http;
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
    public partial class MedicalCenter    
    {

        [Key]
        public Guid Id { get; set; }
        //[Display(Name = "Last Name", Order = -9, Prompt = "Enter Last Name", Description = "Emp Last Name")]
        [Display(Name = nameof(Messages.CreateDate), ResourceType = typeof(Messages))]


        public DateTime CreateDate { get; set; }= DateTime.Now;
        [Display(Name = nameof(Messages.BlockedByAdmin), ResourceType = typeof(Messages))]

        public bool BlockedByAdmin   { get; set; }=false;


        public bool SubscriptionExpired { get; set; } = true;
        public bool PaidAccountIsActive { get; set; } = false;
        
        [Display(Name = "Subscription Expire Date")]
        public DateTime? SubscriptionExpireDate { get; set; }
        [Display(Name = nameof(Messages.LockMyCenter), ResourceType = typeof(Messages))]

        public bool LockMyCenter { get; set; } = false;


        public bool ShowInMobileApplication { get; set; } = false;
        public bool Deleted { get; set; }=false;
        [Display(Name = nameof(Messages.TimeZone), ResourceType = typeof(Messages))]

        public string? TimeZone { get; set; }=string.Empty;
        [Display(Name = nameof(Messages.KeyWords), ResourceType = typeof(Messages))]

        public string? KeyWords { get; set; }=string.Empty;
        public List<MedicalCenterTranslation>? MedicalCenterTranslation { get; set; }
        public List<OpeningHours>? OpeningHours { get; set; }
        [Display(Name = nameof(Messages.IsTwentyFourHours), ResourceType = typeof(Messages))]

        public bool IsTwentyFourHours { get; set; } = false;
        public List<ContactInfo>? ContactInfo { get; set; }
        public Address? Address { get; set; }

        [Display(Name = nameof(Messages.UseEmailVerfication), ResourceType = typeof(Messages))]

        public bool UseEmailVerfication { get; set; } = false;

        [Display(Name = nameof(Messages.ImageName), ResourceType = typeof(Messages))]

        public string? ImageName { get; set; } = "";
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
            set
            {

                try
                {
                    _LangCode = value.ToLower();
                }
                catch { }
            }
        }

        public MedicalCenterDto ToDto()
        {
            return MappingToDto.Mapper.Map<MedicalCenterDto>(this);
        }
        public MedicalCenter GetFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                MedicalCenter Medic = context.MedicalCenter
                    .Include(a => a.Address).ThenInclude(a => a.AddressTranslation)
                    .Include(a => a.ContactInfo)
                    .Include(a => a.MedicalCenterTranslation)
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
                CreateDate = TimeZoneConverter.getLocalTime(null,TimeZone);
                context.MedicalCenter.Add(this);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool SoftDelete()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            try
            {
                MedicalCenter temp = GetFromDb();
                temp.Deleted = true;
                context.MedicalCenter.Attach(temp);
                context.Entry(temp).State = EntityState.Modified;
                context.SaveChanges() ;
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
                MedicalCenter temp = GetFromDb();
                FileHandler.DeleteImageFile(temp.ImageName);
                context.MedicalCenter.Remove(temp);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool IsCenterActive()
        {
            if (BlockedByAdmin || Deleted || SubscriptionExpired)
            {
                return false;
            }
            return true;
        }
        public string WorkHoursToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (OpeningHours branchTime in (IEnumerable<OpeningHours>)this.OpeningHours)
                stringBuilder.AppendLine(branchTime.HoursOfOperation());
            return stringBuilder.ToString();
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
        

        //public string StartHour
        //{
        //    get
        //    {
        //        DateTime currentDate = DateTime.Now;
        //        foreach (OpeningHours openingHours in OpeningHours)
        //        {
        //            if (currentDate.DayOfWeek == openingHours.DayOfWeek)
        //            {
        //                return openingHours.OpeningTime.ToString("HH:mm");
        //            }
        //        }
        //        return "00:00";
        //    }
        //}

        //public string EndHour
        //{
        //    get
        //    {
        //        DateTime currentDate = DateTime.Now;
        //        foreach (OpeningHours openingHours in OpeningHours)
        //        {
        //            if (currentDate.DayOfWeek == openingHours.DayOfWeek)
        //            {
        //                return openingHours.ClosingTime.ToString("HH:mm");
        //            }
        //        }
        //        return "00:00";
        //    }
        //}

        public MedicalCenter GetModelByLnag(string langCode="en-US")
        {
            langCode=langCode.ToLower();

            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                MedicalCenter Medic = context.MedicalCenter
                    .Include(a => a.Address).ThenInclude(a => a.AddressTranslation.Where(a=>a.LangCode==langCode))
                    .Include(a => a.ContactInfo)
                    .Include(a => a.MedicalCenterTranslation.Where(a => a.LangCode == langCode))
                    .Include(a => a.OpeningHours)
                    .FirstOrDefault(a => a.Id == Id);

                

                return Medic;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

}
