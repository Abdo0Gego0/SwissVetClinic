using CmsResources;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.DbModels
{
    [Table("SysAdmin")]
    public class SysAdmin : Person
    {

        public int Type { get; set; } = 0; 
        public string SAddress { get; set; } = "";


        [Required(ErrorMessageResourceName = nameof(Messages.fillField), ErrorMessageResourceType = typeof(Messages))]

        [EmailAddress(ErrorMessageResourceName = nameof(Messages.emialError), ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = nameof(Messages.Email), ResourceType = typeof(Messages))]

        [NotMapped]
        public string AdminEmail { get; set; }

        [Required(ErrorMessageResourceName = nameof(Messages.fillField), ErrorMessageResourceType = typeof(Messages))]

        [Phone]
        [Display(Name = nameof(Messages.Phone), ResourceType = typeof(Messages))]

        [NotMapped]

        public string AdminPhone { get; set; }


        [Required(ErrorMessageResourceName = nameof(Messages.fillField), ErrorMessageResourceType = typeof(Messages))]

        [Phone]
        [Display(Name = nameof(Messages.UserName), ResourceType = typeof(Messages))]

        [NotMapped]
        public string AdminUserName { get; set; }

        [NotMapped]
        [Display(Name = nameof(Messages.Password), ResourceType = typeof(Messages))]

        public string Password { get; set; }

    }
}
