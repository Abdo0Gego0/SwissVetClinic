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

namespace CmsWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ClinicController : Controller
    {
        private readonly IStringLocalizer<CmsResources.Messages> _localizer;
        private readonly ILogger<ClinicController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;

        private readonly IEmailSender _emailSender;
        private readonly IMedicalCenterService medicalCenterService;

        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext cmsContext;

        public ClinicController(
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
            ILogger<ClinicController> logger)
        {
            medicalCenterService= medicalCenterService_;
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
            ViewBag.PreviousActionDispalyName = _localizer["Clinics"];
            ViewBag.PreviousAction = "IndexSpeicality";

            return View("Admin/_ClinicSpeicality");
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request, string preferredCulture = "en-US")
        {
            var result = cmsContext.ClinicSpecialty
                .Include(a=>a.ClinicSpecialtyTranslation)
                .ToList().ToDataSourceResult(request);
            return Json(result);
        }

        public async Task<IActionResult> Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ClinicSpecialty> ContactInfos)
        {

            //if (ModelState.IsValid)
            {
                foreach (var item in ContactInfos)
                {
                    cmsContext.ClinicSpecialty.Add(
                        new ClinicSpecialty
                        {
                            ClinicSpecialtyTranslation= new List<ClinicSpecialtyTranslation>
                            {
                                new ClinicSpecialtyTranslation
                                {
                                    Name=item.ClinicSpecialtyTranslation[0].Name,
                                    LangCode="en-us"
                                },
                                 new ClinicSpecialtyTranslation
                                {
                                    Name=item.ClinicSpecialtyTranslation[1].Name,
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
        public async Task<IActionResult> Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ClinicSpecialty> clinicSpecialties)
        {
            {
                foreach (var updatedItem in clinicSpecialties)
                {
                    cmsContext.ClinicSpecialtyTranslation.Attach(updatedItem.ClinicSpecialtyTranslation[0]);
                    cmsContext.Entry(updatedItem.ClinicSpecialtyTranslation[0]).State=EntityState.Modified;



                    cmsContext.ClinicSpecialtyTranslation.Attach(updatedItem.ClinicSpecialtyTranslation[1]);
                    cmsContext.Entry(updatedItem.ClinicSpecialtyTranslation[1]).State = EntityState.Modified;

                    cmsContext.SaveChanges();
                }
                return Json(_localizer["Success"]);
            }
        }

        public async Task<IActionResult> Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ClinicSpecialty> ContactInfos)
        {
            {
                foreach (var item in ContactInfos)
                {
                    item.DeleteFromDb();
                }
                return Json(_localizer["Success"]);
            }
        }


        public async Task<IActionResult> IndexClinic()
        {
            //List<BaseClinic> list = cmsContext.BaseClinic.ToList();
            //foreach (var item in list)
            //{
            //    item.DeleteFromDb();
            //}


            ViewBag.DispalyName = _localizer["Center Clinics"];
            ViewBag.PreviousActionDispalyName = _localizer["Clinics"];
            ViewBag.PreviousAction = "IndexClinic";


            return View("Admin/_Clinic");
        }


        public ActionResult Read_Clinic([DataSourceRequest] DataSourceRequest request, string preferredCulture = "en-US")
        {
            var result = cmsContext.BaseClinic
                .Where(a =>  !a.IsDraft)
                .Include(a => a.BaseClinicTranslation)
                .ToList().ToDataSourceResult(request);
            return Json(result);
        }

        public async Task<IActionResult> Create_Clinic()
        {

            ViewBag.DispalyName = _localizer["New Clinic"];
            ViewBag.PreviousActionDispalyName = _localizer["Clinics"];
            ViewBag.PreviousAction = "IndexClinic";

            return View("Admin/_Clinic_Create");
        }


        [HttpPost]
        public async Task<IActionResult> Create_Clinic(BaseClinic baseClinic)
        {
            ViewBag.DispalyName = _localizer["New Clinic"];
            ViewBag.PreviousActionDispalyName = _localizer["Clinics"];
            ViewBag.PreviousAction = "IndexClinic";


            baseClinic.ImageName=FileHandler.SaveUploadedFile(baseClinic.ImageFile);
            baseClinic.IsDraft = false;
   

            baseClinic.BaseClinicTranslation = new List<BaseClinicTranslation>
            {
                new BaseClinicTranslation { LangCode="en-US",Name=baseClinic.BaseClinicTranslation[0].Name,Description=baseClinic.BaseClinicTranslation[0].Description},
                new BaseClinicTranslation { LangCode="ar",Name=baseClinic.BaseClinicTranslation[1].Name,Description=baseClinic.BaseClinicTranslation[1].Description}
            };

            cmsContext.BaseClinic.Add(baseClinic);
            cmsContext.SaveChanges(true);

            
            return RedirectToAction("IndexClinic");
        }





        public async Task<IActionResult> Edit_Clinic(Guid id)
        {
            
            ViewBag.DispalyName = _localizer["Edit Clinic"];
            ViewBag.PreviousActionDispalyName = _localizer["Clinics"];
            ViewBag.PreviousAction = "IndexClinic";

            
            BaseClinic baseClinic=cmsContext.BaseClinic
                .Include(a=>a.BaseClinicTranslation)
                .Include(a=>a.OpeningHours)
                .FirstOrDefault(a=>a.Id==id);

            return View("Admin/_Clinic_Edit", baseClinic);
        }


        [HttpPost]
        public async Task<IActionResult> Edit_Clinic(BaseClinic baseClinic)
        {
            ViewBag.DispalyName = _localizer["Edit Clinic"];
            ViewBag.PreviousActionDispalyName = _localizer["Clinics"];
            ViewBag.PreviousAction = "IndexClinic";


            if (baseClinic.ImageFile!=null)
            {
                baseClinic.ImageName = FileHandler.UpdateImageFile(baseClinic.ImageName, baseClinic.ImageFile);
            }


            cmsContext.BaseClinicTranslation.Attach(baseClinic.BaseClinicTranslation[0]);
            cmsContext.Entry(baseClinic.BaseClinicTranslation[0]).State = EntityState.Modified;



            cmsContext.BaseClinicTranslation.Attach(baseClinic.BaseClinicTranslation[1]);
            cmsContext.Entry(baseClinic.BaseClinicTranslation[1]).State = EntityState.Modified;


            baseClinic.IsDraft = false;
            cmsContext.BaseClinic.Attach(baseClinic);
            cmsContext.Entry(baseClinic).State=EntityState.Modified;
            cmsContext.SaveChanges(true);


            return RedirectToAction("IndexClinic");

        }




        public async Task<IActionResult> Destroy_Clinic([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ClinicSpecialty> ContactInfos)
        {
            {
                foreach (var item in ContactInfos)
                {
                    item.DeleteFromDb();
                }
                return Json(_localizer["Success"]);
            }
        }








        public ActionResult Read_OpeningHours([DataSourceRequest] DataSourceRequest request, Guid clinicId )
        {
            BaseClinic myCenter = cmsContext.BaseClinic.Include(a=>a.OpeningHours).FirstOrDefault(a=>a.Id==clinicId);
            var result = myCenter.OpeningHours.ToDataSourceResult(request);
            return Json(result);
        }


        public async Task<IActionResult> Create_OpeningHours([DataSourceRequest] DataSourceRequest request, OpeningHours ContactInfos, Guid clinicId)
        {

            BaseClinic myCenter = cmsContext.BaseClinic
                .Include(a => a.BaseClinicTranslation)
                .Include(a => a.OpeningHours)
                .FirstOrDefault(a => a.Id == clinicId);


            //if (ModelState.IsValid)
            {
                //foreach (var item in ContactInfos)
                {
                    myCenter.OpeningHours.Add(ContactInfos);
                }

                cmsContext.BaseClinic.Attach(myCenter);
                cmsContext.Entry(myCenter).State = EntityState.Modified;

                cmsContext.SaveChanges();
                return Json(_localizer["Success"]);
            }



        }


        public async Task<IActionResult> Create_OpeningHoursXX([DataSourceRequest] DataSourceRequest request, OpeningHours ContactInfos, Guid clinicId)
        {

            return Json(_localizer["Success"]);



        }

        [AcceptVerbs("Post")]
        public async Task<IActionResult> Update_OpeningHours([DataSourceRequest] DataSourceRequest request, OpeningHours item)
        {
            {
                //foreach (var item in ContactInfos)
                {

                    cmsContext.OpeningHours.Attach(item);
                    cmsContext.Entry(item).State = EntityState.Modified;
                    cmsContext.SaveChanges();
                }
                return Json(_localizer["Success"]);
            }



        }

        public async Task<IActionResult> Destroy_OpeningHours([DataSourceRequest] DataSourceRequest request, OpeningHours item)
        {
            {
                //foreach (var item in ContactInfos)
                {
                    cmsContext.OpeningHours.Remove(item);
                    cmsContext.SaveChanges();
                }
                return Json(_localizer["Success"]);
            }

        }



    }

}

