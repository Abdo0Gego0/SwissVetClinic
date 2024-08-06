using CmsDataAccess.AutoMapping;
using CmsDataAccess.ModelsDto;
using CmsDataAccess.TimeZoneUtils;
using CmsDataAccess.Utils.FilesUtils;
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
    public class Attachment
    {
        [Key]
        public Guid Id { get; set; }  

        [ForeignKey("MedicalCenter")]
        public Guid MedicalCenterId { get; set; }

        [ForeignKey("BaseClinic")]
        public Guid BaseClinicId { get; set; }

        [ForeignKey("PetOwner")]
        public Guid PetOwnerId { get; set; }

        [ForeignKey("PatientVisit")]
        public Guid PatientVisitId { get; set; }
        public string PatientName()
        {
            return new ApplicationDbContext().PetOwner.Find(PetOwnerId).FullName;
        }

        public string ClinicName()
        {
            BaseClinic BaseClinic_= new ApplicationDbContext().BaseClinic.Include(a => a.BaseClinicTranslation)
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
     


    }
}
