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
    public class PersonNotification
    {
        [Key]
        public Guid Id { get; set; }
                [Display(Name = nameof(Messages.Date), ResourceType = typeof(Messages))]

        public DateTime CreateDate{ get; set; } 

        [ForeignKey("MedicalCenter")]
                [Display(Name = nameof(Messages.MedicalCenterId), ResourceType = typeof(Messages))]

        public Guid MedicalCenterId { get; set; }

        [ForeignKey("Clinic")]
                        [Display(Name = nameof(Messages.ClinicId), ResourceType = typeof(Messages))]

        public Guid BaseClinicId { get; set; }

        [ForeignKey("Person")]

        public Guid PersonId { get; set; }

                [Display(Name = nameof(Messages.Title), ResourceType = typeof(Messages))]

        public string Title { get; set; }
                [Display(Name = nameof(Messages.Body), ResourceType = typeof(Messages))]

        public string Body { get; set; }

                [Display(Name = nameof(Messages.IsRead), ResourceType = typeof(Messages))]

        public bool IsRead { get; set; } = false;

        public string ClinicName()
        {
            BaseClinic BaseClinic_ = new ApplicationDbContext().BaseClinic.Include(a => a.BaseClinicTranslation)
                .FirstOrDefault(a => a.Id == BaseClinicId);
            try
            {
                return BaseClinic_.BaseClinicTranslation.Where(a => a.LangCode == "ar").ToList()[0].Name;
            }
            catch
            {
                return BaseClinic_.BaseClinicTranslation.ToList()[0].Name;
            }
        }

        public string CenterName()
        {
            MedicalCenter BaseClinic_ = new ApplicationDbContext().MedicalCenter.Include(a => a.MedicalCenterTranslation)
                .FirstOrDefault(a => a.Id == MedicalCenterId);
            try
            {
                return BaseClinic_.MedicalCenterTranslation.Where(a => a.LangCode == "ar").ToList()[0].Name;
            }
            catch
            {
                return BaseClinic_.MedicalCenterTranslation.ToList()[0].Name;
            }
        }
    }
}
