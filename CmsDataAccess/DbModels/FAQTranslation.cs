using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.DbModels
{
	public class FAQTranslation
	{
		[Key]
		public Guid Id { get; set; }
		[ForeignKey("FAQ")]
		public Guid FAQId { get; set; }
        [ForeignKey("MedicalCenter")]
        public Guid? MedicalCenterId { get; set; }
        public string? LangCode { get; set; } = "en-US";
        public string Question { get; set; }
        public string Answer { get; set; }

    }
}
