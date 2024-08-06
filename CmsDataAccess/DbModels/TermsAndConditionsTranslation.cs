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
    public class TermsAndConditionsTranslation
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
                [Display(Name = nameof(Messages.LangCode), ResourceType = typeof(Messages))]
        public string? LangCode { get; set; } = "en-US";

        [ForeignKey("TermsAndConditions")]

        public Guid TermsAndConditionsId { get; set; }
                [Display(Name = nameof(Messages.TermsAndConditionsText), ResourceType = typeof(Messages))]

        public string TermsAndConditionsText { get; set; }

    }
}
