using CmsDataAccess.Utils.FilesUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.DbModels
{
    public class SubscriptionApplication
    {
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "الاسم")]
        [StringLength(maximumLength: 50, MinimumLength = 2, ErrorMessage = "يجب  ملأ هذا الحقل")]
        [Required(ErrorMessage = "يجب  ملأ هذاالحقل")]
        public string FirstName { get; set; }

        [Display(Name = "اسم الأب")]
        [StringLength(maximumLength: 50, MinimumLength = 2, ErrorMessage = "يجب  ملأ هذا الحقل")]
        [Required(ErrorMessage = "يجب  ملأ هذا الحقل")]

        public string MiddleName { get; set; } = "";

        [Display(Name = "الكنية")]
        [StringLength(maximumLength: 50, MinimumLength = 2, ErrorMessage = "يجب  ملأ هذا الحقل")]
        [Required(ErrorMessage = "يجب  ملأ هذا الحقل")]
        public string LastName { get; set; }


        [Display(Name = "الجنسية")]
        [Required(ErrorMessage = "يجب  ملأ هذا الحقل")]

        public string? Nationality { get; set; } = string.Empty;

        [Display(Name = "رقم جواز السفر")]
        [Required(ErrorMessage = "يجب  ملأ هذا الحقل")]

        public string? PassportNumber { get; set; } = string.Empty;

        [Display(Name = "رقم الهوية الوطنية")]
        [Required(ErrorMessage = "يجب  ملأ هذا الحقل")]
        public string? NationalCardId { get; set; } = string.Empty;

        [Required(ErrorMessage = "يجب عليك إدخال هاتف المشرف")]
        [Phone]
        [Display(Name = "الهاتف")]
        public string SubscriberPhone { get; set; }

        [Required(ErrorMessage = "يجب عليك إدخال البريد الالكتروني")]
        [EmailAddress(ErrorMessage = "صيغة البريد الالكتروني غير صحيحة")]
        [Display(Name = "البريد الالكتروني")]
        [Remote(action: "IsEmployeeEmailUnique", controller: "Center", areaName: "Admin", HttpMethod = "GET", ErrorMessage = "هذا الإيميل مستخدم مسبقا")]
        public string SubscriberEmail { get; set; }
        public string? VerificationToken { get; set; } = string.Empty;
        public DateTime? VerificationExpireDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool EmailVerfied { get; set; } = false;
        public string? LicenseNumber { get; set; } = string.Empty;
        public DateTime? LicenseExpireDate { get; set; }

        [Display(Name = "LicenseImageName")]
        public string? LicenseImageName { get; set; } = "";
        public string? LicenseImageFullPath
        {
            get
            {
                return new ApplicationDbContext().MySystemConfiguration.FirstOrDefault().ApiUrl + "pImages/" + LicenseImageName;
            }
        }

        [NotMapped]
        public IFormFile? LicenseImageFile { get; set; }

        [Display(Name = "ImageName")]
        public string? PassportImageName { get; set; } = "";

        public string? PassportImageFullPath
        {
            get
            {
                return new ApplicationDbContext().MySystemConfiguration.FirstOrDefault().ApiUrl + "pImages/" + PassportImageName;
            }
        }

        [NotMapped]
        public IFormFile? PassportImageFile { get; set; }
        
        public int Accepted { get; set; } = 0;
        public bool IsSeen { get; set; } = false;
        public string ResponseFromAdmin { get; set; } = string.Empty;

        public string CenterName { get; set; } = string.Empty;
        public string CenterAddress { get; set; } = string.Empty;



        [ForeignKey("CenterAdmin")]
        public Guid? CenterAdminId { set; get; }


        public string FullName
        {
            get
            {
                return FirstName+" "+ MiddleName+" "+LastName;
            }
        }


        public SubscriptionApplication GetFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            try
            {
                SubscriptionApplication Medic = context.SubscriptionApplication
                    .FirstOrDefault(a => a.Id == Id);

                return Medic;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool InsertIntoDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            try
            {
                context.SubscriptionApplication.Add(this);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            try
            {

                SubscriptionApplication temp = GetFromDb();
                
                FileHandler.DeleteImageFile(temp.LicenseImageName);
                FileHandler.DeleteImageFile(temp.PassportImageName);
                
                context.SubscriptionApplication.Remove(temp);
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
