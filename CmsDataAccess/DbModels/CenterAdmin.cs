using CmsResources;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    [Table("CenterAdmin")]
    public class CenterAdmin : Person 
    {
        public string? StipeId { set; get; } = string.Empty;
        public string? StripeSubcriptionId { set; get; } = string.Empty;
        public string? StripePaymentMethodId { set; get; } = string.Empty;
        public DateTime? PaidAt { get; set; }
        public bool SetUpFeesIsPayed { get; set; } = false;
        public bool SubscriptionExpired { get; set; } = true;
        public bool PaidAccountIsActive { get; set; } = false;
        public DateTime? SubscriptionExpireDate { get; set; }
        
        [ForeignKey("SubscriptionPlan")]
        public Guid? SubscriptionPlanId { set; get; }
        public bool BlockedByAdmin { get; set; } = false;


        public int Type { get; set; } = 0;
        public string SAddress { get; set; } = "";
        


        [ForeignKey("MedicalCenter")]
        [Display(Name = nameof(Messages.MedicalCenterId), ResourceType = typeof(Messages))]

        public Guid? MedicalCenterId { set; get; }

        [Required(ErrorMessageResourceName = nameof(Messages.fillField), ErrorMessageResourceType = typeof(Messages))]

        [EmailAddress(ErrorMessageResourceName = nameof(Messages.emialError), ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = nameof(Messages.Email), ResourceType = typeof(Messages))]

        [NotMapped]
        [Remote(action: "IsEmployeeEmailUnique", controller: "Center", areaName: "Admin", HttpMethod = "GET", ErrorMessage = "هذا الإيميل مستخدم مسبقا")]
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
        [Remote(action: "IsEmployeeUserNameUnique", controller: "Center", areaName: "Admin", HttpMethod = "GET", ErrorMessage = "اسم المستخدم موجود مسبقا")]

        public string AdminUserName { get; set; }

        [NotMapped]
        [Display(Name = nameof(Messages.Password), ResourceType = typeof(Messages))]

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_])[A-Za-z\d\W_]{8,}$",
       ErrorMessageResourceName = nameof(Messages.passwordError), ErrorMessageResourceType = typeof(Messages))]
        public string Password { get; set; }



    }
}
