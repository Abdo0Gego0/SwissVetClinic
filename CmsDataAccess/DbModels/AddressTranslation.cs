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
    public class AddressTranslation
    {
        [Key]
        public Guid Id { get; set; } 

        [Required]
        [Display(Name = nameof(Messages.LangCode), ResourceType = typeof(Messages))]

        public string? LangCode { get; set; } = "en-US";

        [ForeignKey("Address")]

        public Guid AddressId { get; set; }

        [Display(Name = nameof(Messages.Building), ResourceType = typeof(Messages))]

        public string? Building { get; set; }
        [Display(Name = nameof(Messages.Street), ResourceType = typeof(Messages))]

        public string? Street { get; set; }
        [Display(Name = nameof(Messages.City), ResourceType = typeof(Messages))]

        public string? City { get; set; }
        [Display(Name = nameof(Messages.Country), ResourceType = typeof(Messages))]

        public string? Country { get; set; }
        [Display(Name = nameof(Messages.FullAddress), ResourceType = typeof(Messages))]

        public double lng { get; set; }

        public double lat { get; set; }
        public string? FullAddress { get; set; }
    }
}
