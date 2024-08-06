using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.MobileViewModels.AuthModels
{
    public class RegisterModel
    {
        [Required]
        public string UserName { get; set; }
        [Required, MinLength(8)]
        public string Password { get; set; }
        [Required, Compare("Password", ErrorMessage = "Password does not match")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string FirstName { set; get; }

        public string? MiddleName { set; get; }

        [Required]
        public string LastName { set; get; }

        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, Phone]
        public string Telephone { get; set; }


    }
}
