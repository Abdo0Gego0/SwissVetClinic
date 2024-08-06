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
	public class OpeningHours 
	{
		[Key]
		public Guid Id { get; set; }
        [Display(Name = nameof(Messages.DayOfWeek), ResourceType = typeof(Messages))]

        public DayOfWeek DayOfWeek { get; set; }

        [UIHint("time")]

        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]

        [Display(Name = nameof(Messages.OpeningTime), ResourceType = typeof(Messages))]

        public TimeSpan OpeningTime { get; set; }

        [UIHint("time")]

        [Display(Name = nameof(Messages.ClosingTime), ResourceType = typeof(Messages))]


        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan ClosingTime { get; set; }

		[ForeignKey("Clinic")]
        [Display(Name = nameof(Messages.ClinicName), ResourceType = typeof(Messages))]


        public Guid ClinicId { get; set; }


        [Display(Name = nameof(Messages.IsTwentyFourHours), ResourceType = typeof(Messages))]


        public bool IsTwentyFourHours { get; set; } = false;


        [NotMapped]

		public bool IsDeleted { get; set; }

		[NotMapped]

		public string Display { get; set; } = "";

        public string HoursOfOperation() => string.Format("{0} : {1} to {2}", (object)this.DayOfWeek, (object)this.OpeningTime, (object)this.ClosingTime);
	}
}
