using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.DbModels
{
	public class ClinicRating
	{
		[Key]
		public Guid Id { get; set; }

		[ForeignKey("IdentityUser")]
        [Display(Name = "LangCode")]

        public Guid IdentityUserId { set; get;} 




		[ForeignKey("BaseClinic")]
        [Display(Name = "LangCode")]

        public Guid BaseClinicId { set; get; }

        [Display(Name = "LangCode")]

        public int Points { get; set; }

        [Display(Name = "LangCode")]

        public string Comment { get; set; }


		public int RatingOwner { set; get; }

	}

}
