using Kendo.Mvc.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Drawing;
using System.Security.Claims;
using CmsDataAccess;
using CmsResources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Azure.Core;
using CmsDataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using CmsDataAccess.DbModels;
using ServicesLibrary.PasswordsAndClaims;
using ServicesLibrary.UserServices;
using ServicesLibrary.MedicalCenterServices;
using CmsDataAccess.Utils.FilesUtils;
using Mono.TextTemplating;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Stripe;
using Microsoft.IdentityModel.Tokens;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using ServicesLibrary.PersonServices;
using System.Reflection;

namespace CmsWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DoctorController : Controller
    {
        private readonly IStringLocalizer<CmsResources.Messages> _localizer;
        private readonly ILogger<DoctorController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;

        private readonly IEmailSender _emailSender;
        private readonly IMedicalCenterService medicalCenterService;
        private readonly IPersonService personService ;

        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext cmsContext;

        public DoctorController(
            IPersonService personService_,
            IMedicalCenterService medicalCenterService_,
            IStringLocalizer<CmsResources.Messages> localizer,
            IConfiguration config,
            IUserService userService,
            ApplicationDbContext _cMDbContext,

            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,

            RoleManager<IdentityRole> roleManager,
            ILogger<DoctorController> logger)
        {
            personService = personService_;
            medicalCenterService = medicalCenterService_;
            _localizer = localizer;

            _logger = logger;
            _config = config;
            _userService = userService;
            cmsContext = _cMDbContext;

            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;

            _emailSender = emailSender;

            _roleManager = roleManager;
        }

        public async Task<IActionResult> IndexSpeicality()
        {

            ViewBag.DispalyName = _localizer["Speicality"];
            ViewBag.PreviousActionDispalyName = _localizer["Doctors"];
            ViewBag.PreviousAction = "IndexSpeicality";

            return View("Admin/_DoctorSpeicality");
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request, string preferredCulture = "en-US")
        {
            var result = cmsContext.DoctorSpeciality
                .Include(a=>a.DoctorSpecialityTranslation)
                .ToList().ToDataSourceResult(request);
            return Json(result);
        }

        public async Task<IActionResult> Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<DoctorSpeciality> ContactInfos)
        {

            //if (ModelState.IsValid)
            {
                foreach (var item in ContactInfos)
                {
                    cmsContext.DoctorSpeciality.Add(
                        new DoctorSpeciality
                        {
                            DoctorSpecialityTranslation= new List<DoctorSpecialityTranslation>
                            {
                                new DoctorSpecialityTranslation
                                {
                                    Description=item.DoctorSpecialityTranslation[0].Description,
                                    LangCode="en-us"
                                },
                                 new DoctorSpecialityTranslation
                                {
                                    Description=item.DoctorSpecialityTranslation[1].Description,
                                    LangCode="ar"
                                }
                            }
                        }
                        );
                }
                cmsContext.SaveChanges();
                return Json(_localizer["Success"]);
            }



        }


        [AcceptVerbs("Post")]
        public async Task<IActionResult> Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<DoctorSpeciality> clinicSpecialties)
        {
            {
                foreach (var updatedItem in clinicSpecialties)
                {
                    cmsContext.DoctorSpecialityTranslation.Attach(updatedItem.DoctorSpecialityTranslation[0]);
                    cmsContext.Entry(updatedItem.DoctorSpecialityTranslation[0]).State=EntityState.Modified;

                    cmsContext.DoctorSpecialityTranslation.Attach(updatedItem.DoctorSpecialityTranslation[1]);
                    cmsContext.Entry(updatedItem.DoctorSpecialityTranslation[1]).State = EntityState.Modified;

                    cmsContext.SaveChanges();
                }
                return Json(_localizer["Success"]);

            }
        }

        public async Task<IActionResult> Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<DoctorSpeciality> ContactInfos)
        {
            {
                foreach (var item in ContactInfos)
                {
                    item.DeleteFromDb();
                }
                return Json(_localizer["Success"]);

            }


        }



        public async Task<IActionResult> IndexDoctors()
        {

            ViewBag.DispalyName = _localizer["Center Doctors"];
            ViewBag.PreviousActionDispalyName = _localizer["Doctors"];
            ViewBag.PreviousAction = "IndexDoctors";

            return View("Admin/_Doctor");
        }

        public ActionResult Read_Doctor([DataSourceRequest] DataSourceRequest request, string preferredCulture = "en-US")
        {
            var result = cmsContext.Doctor
                .Where(a => !a.IsDeleted)
                .Include(a => a.DoctorTranslation)
                .Include(a => a.User)
                .ToList().ToDataSourceResult(request);
            return Json(result);
        }

        public async Task<IActionResult> Create_Doctor()
        {

            ViewBag.DispalyName = _localizer["New Doctor"];
            ViewBag.PreviousActionDispalyName = _localizer["Doctors"];
            ViewBag.PreviousAction = "IndexDoctors";

            ViewBag.Password = PasswordUtil.GenerateRandomPassword();
            ViewBag.ErrorMessage = "";

            return View("Admin/_Doctor_Create");
        }


        [HttpPost]
        public async Task<IActionResult> Create_Doctor(Doctor model)
        {


            ViewBag.DispalyName = _localizer["New Doctor"];
            ViewBag.PreviousActionDispalyName = _localizer["Doctors"];
            ViewBag.PreviousAction = "IndexDoctors";


            Guid docId=Guid.NewGuid();


            string res = personService.AddNewDoctor(model);


            if (res.IsNullOrEmpty())
            {
                Doctor doc = cmsContext.Doctor.Include(a => a.User).FirstOrDefault(a => a.User.UserName == model.PersonUserName);
                return RedirectToAction("Certificate_Doctor", new { id = doc.Id });

            }
            else
            {
                ViewBag.ErrorMessage = res;

                return View("Admin/_Doctor_Create",model);
            }

        }






        public async Task<IActionResult> Edit_Doctor(Guid id)
        {


            ViewBag.DispalyName = _localizer["Edit Doctor"];
            ViewBag.PreviousActionDispalyName = _localizer["Doctors"];
            ViewBag.PreviousAction = "IndexDoctors";



            return View("Admin/_Doctor_Edit", personService.GetDoctorById(id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit_Doctor(Doctor model)
        {


            ViewBag.DispalyName = _localizer["Edit Doctor"];
            ViewBag.PreviousActionDispalyName = _localizer["Doctors"];
            ViewBag.PreviousAction = "IndexDoctors";


            if (model.ImageFile != null)
            {
                model.ImageName = FileHandler.UpdateImageFile(model.ImageName, model.ImageFile);
            }



            Doctor centerTutor = cmsContext.Doctor
    .Include(a => a.User)
    .Include(a => a.DoctorSpeciality)
    .FirstOrDefault(a => a.Id == model.Id);

            if (centerTutor.User.Email != model.PersonEmail)
            {
                centerTutor.User.Email = model.PersonEmail;
                var resul1 = await _userManager.UpdateAsync(centerTutor.User);

                if (!resul1.Succeeded)
                {
                    string msg = "";

                    foreach (var item in resul1.Errors)
                    {
                        msg += item.Description + " ";
                    }

                    ViewBag.ErrorMessage = _localizer["MakeSureThatEmailisUnique"];

                    return View(model);
                }

                centerTutor.User.EmailConfirmed = true;
            }

            if (centerTutor.User.UserName != model.PersonUserName)
            {
                centerTutor.User.UserName = model.PersonUserName;
                var resul2 = await _userManager.UpdateAsync(centerTutor.User);

                if (!resul2.Succeeded)
                {
                    string msg = "";
                    foreach (var item in resul2.Errors)
                    {
                        msg += item.Description + " ";
                    }
                    ViewBag.ErrorMessage = _localizer["MakeSureThatUserisUnique"];
                    return View(model);
                }
                centerTutor.User.EmailConfirmed = true;
            }

            if (centerTutor.User.PhoneNumber != model.PersonPhone)
            {
                centerTutor.User.PhoneNumber = model.PersonPhone;
                var resul3 = await _userManager.UpdateAsync(centerTutor.User);
                if (!resul3.Succeeded)
                {
                    string msg = "";
                    foreach (var item in resul3.Errors)
                    {
                        msg += item.Description + " ";
                    }

                    ViewBag.ErrorMessage = "يرجى التأكد أن الهاتف";

                    return View(model);
                }
                centerTutor.User.PhoneNumberConfirmed = true;
            }


            if (model.ImageFile != null)
            {
                string uniqueFileName = FileHandler.UpdateProfileImage(model.ImageFile, model.ImageName);
                centerTutor.ImageName = uniqueFileName;
            }

            centerTutor.FirstName = model.FirstName;
            centerTutor.MiddleName = model.MiddleName;
            centerTutor.LastName = model.LastName;
            centerTutor.NationalCardId = model.NationalCardId;
            centerTutor.JobCardNumber = model.JobCardNumber;
            centerTutor.PassportNumber = model.PassportNumber;
            centerTutor.Nationality = model.Nationality;

            centerTutor.PersonEmail = model.PersonEmail;
            centerTutor.PersonPhone = model.PersonPhone;
            centerTutor.PersonUserName = model.PersonUserName;
            centerTutor.PersonUserName = model.PersonUserName;
            centerTutor.DoctorSpeciality=model.DoctorSpeciality;


            //centerTutor.DoctorSpeciality = new List<DocSpecBreakRelation>();

            //foreach (var item in model.DoctorSpeciality)
            //{
            //    centerTutor.DoctorSpeciality.Add(new DocSpecBreakRelation
            //    {
            //        DoctorId=model.Id,
            //        DoctorSpecialityId=item.Id,
            //    });
            //}




            cmsContext.DoctorTranslation.Attach(model.DoctorTranslation[0]);
            cmsContext.Entry(model.DoctorTranslation[0]).State = EntityState.Modified;

            cmsContext.DoctorTranslation.Attach(model.DoctorTranslation[1]);
            cmsContext.Entry(model.DoctorTranslation[1]).State = EntityState.Modified;

            cmsContext.SaveChanges();


            cmsContext.Doctor.Attach(centerTutor);
            cmsContext.Entry(centerTutor).State = EntityState.Modified;

            cmsContext.SaveChanges();




            return RedirectToAction("IndexDoctors");

        }




        public async Task<IActionResult> Destroy_Doctor([DataSourceRequest] DataSourceRequest request, Doctor item)
        {
            {
                //foreach (var item in ContactInfos)
                {
                    item.DeleteFromDb(); 
                    //item.SoftDelte();
                }
                return Json(_localizer["Success"]);
            }
        }


        #region Certificate

        public async Task<IActionResult> Certificate_Doctor(Guid id)
        {

            ViewBag.PreviousActionDispalyName = _localizer["Doctor Certificates"];

            ViewBag.PreviousActionDispalyName = _localizer["Doctors"];

            ViewBag.PreviousAction = "IndexDoctors";


            ViewBag.Id=id ;

            return View("Admin/_Certificate_Doctor");
        }

        public async Task<IActionResult> Certificate_Read([DataSourceRequest] DataSourceRequest request, Guid id)
        {
            return Json(medicalCenterService.GetMyDoctorCertificate(request, id));
        }

        public async Task<IActionResult> Create_Certificate([DataSourceRequest] DataSourceRequest request, Certificate model ,string docId)
        {
            //string uniqueFileName = FileHandler.SaveUploadedFile(task.ImageFile);
            string uniqueFileName = FileHandler.SaveUploadedFileFrom64(model.Image64);

            cmsContext.Certificate.Add(new Certificate
            {
                DoctorId=Guid.Parse(docId),
                ImageName = uniqueFileName,
                CertificateTranslation = model.CertificateTranslation,
            });

            cmsContext.SaveChanges();

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        public async Task<IActionResult> Destroy_Certificate([DataSourceRequest] DataSourceRequest request, Certificate task)
        {
            task.DeleteFromDb() ;
            return Json(_localizer["Success"]);
        }

        public async Task<IActionResult> Update_Certificate([DataSourceRequest] DataSourceRequest request, Certificate task)
        {
            if (!string.IsNullOrEmpty(task.Image64))
            {
                FileHandler.DeleteImageFile(task.ImageName);
                string uniqueFileName = FileHandler.SaveUploadedFileFrom64(task.Image64);
                task.ImageName = uniqueFileName;
            }


            cmsContext.Certificate.Attach(task);

            cmsContext.CertificateTranslation.Attach(task.CertificateTranslation[0]);
            cmsContext.CertificateTranslation.Attach(task.CertificateTranslation[1]);

            cmsContext.Entry(task.CertificateTranslation[0]).State = EntityState.Modified;
            cmsContext.Entry(task.CertificateTranslation[1]).State = EntityState.Modified;
            cmsContext.Entry(task).State = EntityState.Modified;

            cmsContext.SaveChanges();
            return Json(_localizer["Success"]);
        }


        #endregion


    }

}

