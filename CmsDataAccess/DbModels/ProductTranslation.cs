using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CmsResources;

namespace CmsDataAccess.DbModels
{
    public class ProductTranslation
    { 
        [Key]
        public Guid Id { get; set; }


        [Required]
        [Display(Name = nameof(Messages.LangCode), ResourceType = typeof(Messages))]
        public string? LangCode { get; set; } = "en-US";


        [ForeignKey("Product")]
        public Guid ProductId { get; set; }

        
        [Display(Name = nameof(Messages.Name), ResourceType = typeof(Messages))]
        public string Name { get; set; }

 
        [Display(Name = nameof(Messages.SKU), ResourceType = typeof(Messages))]
        public string? SKU { get; set; }

        
        [Display(Name = nameof(Messages.Description), ResourceType = typeof(Messages))]
        public string? Description { get; set; } = string.Empty;


        [Display(Name = nameof(Messages.KeyWords), ResourceType = typeof(Messages))]
        public string? KeyWords { get; set; } = string.Empty;

    }
}
