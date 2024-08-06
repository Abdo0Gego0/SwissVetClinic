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
    public class VisitMeasurement
    {
        [Key]
        public Guid Id { get; set; }
               [Display(Name = nameof(Messages.MeasurementType), ResourceType = typeof(Messages))]

        public string? Type{ get; set; }
                       [Display(Name = nameof(Messages.MeasurementValue), ResourceType = typeof(Messages))]

        public string? Value{ get; set; }
                       [Display(Name = nameof(Messages.ClinicId), ResourceType = typeof(Messages))]


        [ForeignKey("BaseClinic")]
        public Guid BaseClinicId { get; set; }
               [Display(Name = nameof(Messages.Patient), ResourceType = typeof(Messages))]


        [ForeignKey("PetOwner")]
        public Guid PetOwnerId { get; set; }
                       [Display(Name = nameof(Messages.LangCode), ResourceType = typeof(Messages))]


        [ForeignKey("PatientVisit")]
        public Guid PatientVisitId { get; set; }

    }
}
