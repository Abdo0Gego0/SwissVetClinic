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
	public class DoctorRating
	{
		[Key]
		public Guid Id { get; set; }

		[ForeignKey("Doctor")]
        [Display(Name = "LangCode")]

        public Guid DoctorId { set; get; }

        [Display(Name = "LangCode")]

        public int Points { get; set; }

        [Display(Name = "LangCode")]

        public string Comment { get; set; }

        [Display(Name = "LangCode")]

        public int RatingOwner { set; get; }


        public bool InsertIntoDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            try
            {
                context.DoctorRating.Add(this);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }



}
