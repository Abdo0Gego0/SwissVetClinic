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
	public class MedicalCenterTranslation 
    {
		[Key]
		public Guid Id { get; set; }
        [Display(Name = nameof(Messages.ImageName), ResourceType = typeof(Messages))]


        public string? LangCode { get; set; } = "en-US";

        [Display(Name = nameof(Messages.MedicalCenterId), ResourceType = typeof(Messages))]


        public Guid MedicalCenterId { get; set; }
        [Display(Name = nameof(Messages.Name), ResourceType = typeof(Messages))]


        public string  Name { get; set; }
        [Display(Name = nameof(Messages.Description), ResourceType = typeof(Messages))]


        public string  Description { get; set; }

    }
}
