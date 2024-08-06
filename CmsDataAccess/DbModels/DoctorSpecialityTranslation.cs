using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmsResources;

namespace CmsDataAccess.DbModels
{
    public class DoctorSpecialityTranslation
    {
        [Key]
        public Guid Id { get; set; }
        [Display(Name = nameof(Messages.LangCode), ResourceType = typeof(Messages))]

        public string? LangCode { get; set; } = "en-US";


        [ForeignKey("DoctorSpeciality")]
        public Guid DoctorSpecialityId { get; set; }

        [Display(Name = nameof(Messages.Description), ResourceType = typeof(Messages))]
        public string Description { get; set; }
    }
}
