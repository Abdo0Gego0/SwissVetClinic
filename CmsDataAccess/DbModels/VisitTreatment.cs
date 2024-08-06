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
    public class VisitTreatment
    {
        [Key]
        public Guid Id { get; set; }
                [Display(Name = nameof(Messages.Name), ResourceType = typeof(Messages))]

        public string? Name{ get; set; }
                [Display(Name = nameof(Messages.Note), ResourceType = typeof(Messages))]

        public string? Note{ get; set; }

        [ForeignKey("BaseClinic")]
                [Display(Name = nameof(Messages.ClinicId), ResourceType = typeof(Messages))]

        public Guid BaseClinicId { get; set; }

        [ForeignKey("PetOwner")]
                        [Display(Name = nameof(Messages.Patient), ResourceType = typeof(Messages))]

        public Guid PetOwnerId { get; set; }

        [ForeignKey("PatientVisit")]

        public Guid PatientVisitId { get; set; }

    }
}
