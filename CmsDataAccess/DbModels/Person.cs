using CmsDataAccess.Models;
using CmsResources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.DbModels
{
	public class Person 
	{
		[Key]
		public Guid Id { get; set; }

        [Display(Name = nameof(Messages.Title), ResourceType = typeof(Messages))]

        [StringLength(maximumLength: 50, MinimumLength = 2, ErrorMessageResourceName = nameof(Messages.fillField),ErrorMessageResourceType = typeof(Messages))]
        
        public string? Title { get; set; } = string.Empty;

        [Display(Name = nameof(Messages.FirstName), ResourceType = typeof(Messages))]

        [StringLength(maximumLength: 50, MinimumLength = 2, ErrorMessageResourceName = nameof(Messages.fillField), ErrorMessageResourceType = typeof(Messages))]

        [Required(ErrorMessageResourceName = nameof(Messages.fillField),ErrorMessageResourceType = typeof(Messages))]
        public string FirstName { get; set; }

        [Display(Name = nameof(Messages.MiddleName), ResourceType = typeof(Messages))]

        //[StringLength(maximumLength: 50, MinimumLength = 2, ErrorMessageResourceName = nameof(Messages.fillField), ErrorMessageResourceType = typeof(Messages))]

        //[Required(ErrorMessageResourceName = nameof(Messages.fillField), ErrorMessageResourceType = typeof(Messages))]


        public string? MiddleName { get; set; } = "";

        [Display(Name = nameof(Messages.LastName), ResourceType = typeof(Messages))]

        [StringLength(maximumLength: 50, MinimumLength = 2, ErrorMessageResourceName = nameof(Messages.fillField), ErrorMessageResourceType = typeof(Messages))]

        [Required(ErrorMessageResourceName = nameof(Messages.fillField), ErrorMessageResourceType = typeof(Messages))]


        public string LastName { get; set; }



        //[StringLength(maximumLength: 50, MinimumLength = 2, ErrorMessageResourceName = nameof(Messages.fillField), ErrorMessageResourceType = typeof(Messages))]

        [Display(Name = nameof(Messages.MotherName), ResourceType = typeof(Messages))]
        public string? MotherName { get; set; }=string.Empty;


        [UIHint("Date")]
        [Display(Name = nameof(Messages.CreateDate), ResourceType = typeof(Messages))]

        public DateTime? CreateDate { set; get; }

        [Display(Name = nameof(Messages.Gendre), ResourceType = typeof(Messages))]


        [Required(ErrorMessageResourceName = nameof(Messages.fillField), ErrorMessageResourceType = typeof(Messages))]

        public int Gendre { get; set; } = 0;

        [Display(Name = nameof(Messages.BirthDate), ResourceType = typeof(Messages))]

        [UIHint("Date")]
		public DateTime? BirthDate { get; set; }

        [Display(Name = nameof(Messages.Nationality), ResourceType = typeof(Messages))]


        //[Required(ErrorMessageResourceName = nameof(Messages.fillField), ErrorMessageResourceType = typeof(Messages))]


        public string? Nationality { get; set; } = "";

        [Display(Name = nameof(Messages.PassportNumber), ResourceType = typeof(Messages))]

        //[Required(ErrorMessageResourceName = nameof(Messages.fillField), ErrorMessageResourceType = typeof(Messages))]


        public string? PassportNumber { get; set; } = "";

        [Display(Name = nameof(Messages.NationalCardId), ResourceType = typeof(Messages))]

        //[Required(ErrorMessageResourceName = nameof(Messages.fillField), ErrorMessageResourceType = typeof(Messages))]


        public string? NationalCardId { get; set; } = "";

        [Display(Name = nameof(Messages.JobCardNumber), ResourceType = typeof(Messages))]


        public string? JobCardNumber { get; set; } = "";

        public bool BlockedByAdmin { get; set; } = false;
		public bool AccountVerfied { get; set; } = true;
        public DateTime DateTimeCreationTime { get; set; } = DateTime.Now;
        public string? VerificationToken { get; set; } = "";
        public DateTime? VerificationTime { get; set; }




        public string? PreferredLanguage { get; set; } = "en-us";
        public string fcm_token { get; set; } = "";
        [Display(Name = nameof(Messages.Description), ResourceType = typeof(Messages))]

        public string? Description { get; set; } = "";
        public Address? Address { get; set; }
        public IdentityUser? User { get; set; }
		public int Status { set; get; } = 0;
		public int CenterStatus { set; get; } = 0;

        [NotMapped]
        [Display(Name = nameof(Messages.FullName), ResourceType = typeof(Messages))]


        public string FullName
		{
			get
			{
				return FirstName+" "+MiddleName+" "+LastName;
			}
		}

		[NotMapped]
        [Display(Name = nameof(Messages.Age), ResourceType = typeof(Messages))]

        public int? Age
		{
			get
			{
				if (BirthDate.HasValue)
				{
					DateTime today = DateTime.Today;
					int age = today.Year - BirthDate.Value.Year;

					// Check if the birthday has occurred this year
					if (BirthDate.Value.Date > today.AddYears(-age))
					{
						age--;
					}

					return age;
				}

				return null;
			}
		}


        [Display(Name = nameof(Messages.ImageName), ResourceType = typeof(Messages))]
        public string? ImageName { get; set; } = "";

        public string? ImageFullPath
		{
			get
			{
                if (ImageName=="" || ImageName==null)
                {
                    return "/siteimages/userAvatat.svg";
                }
                return new ApplicationDbContext().MySystemConfiguration.FirstOrDefault().ApiUrl+"/pImages/" + ImageName;
			}
		}


		[NotMapped]
        public IFormFile? ImageFile { get; set; }


        public bool IsDeleted { set; get; } = false;



        [Required(ErrorMessage = "You have to enter the employee e-mail")]

        [Display(Name = nameof(Messages.Email), ResourceType = typeof(Messages))]
        [NotMapped]
        [Remote(action: "IsEmailUnique", controller: "Login", areaName: "Auth", HttpMethod = "GET", ErrorMessage = "Email is already in use",AdditionalFields = "PreviousEmail")]
        public string PersonEmail { get; set; }

        [Required(ErrorMessage = "You have to enter the phone number")]
        [Phone]
        [Display(Name = nameof(Messages.Phone), ResourceType = typeof(Messages))]

        [NotMapped]

        public string PersonPhone { get; set; }


        [Phone]
        [Display(Name = nameof(Messages.Phone), ResourceType = typeof(Messages))]

        [NotMapped]

        public string? PersonPhoneFromDashBoard { get; set; }


        [Required(ErrorMessage = "You have to enter the username")]
        [Display(Name = nameof(Messages.UserName), ResourceType = typeof(Messages))]

        [NotMapped]
        [Remote(action: "IsUserNameUnique", controller: "Login", areaName: "Auth", HttpMethod = "GET", ErrorMessage = "Username is already in use", AdditionalFields = "PreviousUsername")]
        public string PersonUserName { get; set; }

        [NotMapped]
        [Display(Name = nameof(Messages.Password), ResourceType = typeof(Messages))]

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_])[A-Za-z\d\W_]{8,}$",
        ErrorMessage = "Password must have at least 8 characters and include at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string? Password { get; set; }


        [Display(Name = nameof(Messages.Passport), ResourceType = typeof(Messages))]

        public string? PassportFileName { get; set; } = "";
        [NotMapped]
        public IFormFile PassportFile { get; set; }
        public string PassportFullPath 
        {
            get
            {
                return new ApplicationDbContext().MySystemConfiguration.FirstOrDefault().ApiUrl + "pImages/" + PassportFileName;
            }
        }


        [Display(Name = nameof(Messages.FamillyBookFile), ResourceType = typeof(Messages))]

        public string? FamilyBookFileName { get; set; } = "";
        [NotMapped]
        public IFormFile FamillyBookFile { get; set; }
        public string FamilyBookFullPath
        {
            get
            {
                return new ApplicationDbContext().MySystemConfiguration.FirstOrDefault().ApiUrl + "pImages/" + FamilyBookFileName;
            }
        }


        [Display(Name = nameof(Messages.LaborCard), ResourceType = typeof(Messages))]

        public string? LaborCardFileName { get; set; } = "";
        [NotMapped]
        public IFormFile LaborCardFile { get; set; }
        public string LaborCardFullPath
        {
            get
            {
                return new ApplicationDbContext().MySystemConfiguration.FirstOrDefault().ApiUrl+ "pImages/" + LaborCardFileName;
            }
        }





    }
}
