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
    public class CertificateTranslation 
    {
        [Key]
        public Guid Id { get; set; } 

        [Required]
        [Display(Name = nameof(Messages.LangCode), ResourceType = typeof(Messages))]

        public string? LangCode { get; set; } = "en-US";

        [ForeignKey("Certificate")]
        public Guid CertificateId { get; set; }

        [Display(Name = nameof(Messages.Name), ResourceType = typeof(Messages))]

        public string? Name { get; set; }


    }
}
