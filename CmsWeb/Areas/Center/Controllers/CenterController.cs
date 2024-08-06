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

namespace CmsWeb.Areas.Center.Controllers
{
    [Area("CenterAdmin")]
    public class CenterController : Controller
    {
        private readonly IStringLocalizer<CmsResources.Messages> _localizer;
        private readonly ILogger<CenterController> _logger;
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

        public CenterController(
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
            ILogger<CenterController> logger)
        {
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

        public async Task<IActionResult> Index(int fromDashboard = 0)
        {


            ViewBag.DispalyName = _localizer["Center Details"];
            ViewBag.PreviousActionDispalyName = _localizer["Center Details"];

            ViewBag.PreviousAction = "Index?fromDashboard=1";



            ViewBag.FromDashboard = fromDashboard;
            MedicalCenter model = await medicalCenterService.GetMyCenterAsync();
            return View("CenterAdmin/_CenterDetails", model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(MedicalCenter model)
        {

            ViewBag.DispalyName = _localizer["Center Details"];
            ViewBag.PreviousActionDispalyName = _localizer["Center Details"];
            ViewBag.PreviousAction = "Index?fromDashboard=1";


            //med.SubscriptionExpireDate=DateTime.Now.AddDays(30);
            Guid guid = (Guid)_userService.GetMyId();

            CenterAdmin centerAdmin = cmsContext.CenterAdmin
                .Include(a => a.User).FirstOrDefault(a => a.User.Id == guid.ToString());
            CenterSetUpSteps centerSetUpSteps = cmsContext.CenterSetUpSteps.FirstOrDefault(a => a.CenterAdminId == centerAdmin.Id);

            //if (centerSetUpSteps.StepNumer == 0)
            //{
            //    centerSetUpSteps.StepNumer = 1;
            //    cmsContext.CenterSetUpSteps.Attach(centerSetUpSteps);
            //    cmsContext.Entry(centerSetUpSteps).State = EntityState.Modified;
            //    cmsContext.SaveChanges();
            //}

            if (model.ImageFile != null)
            {
                model.ImageName = FileHandler.UpdateProfileImage(model.ImageFile, model.ImageName);
            }


            cmsContext.MedicalCenterTranslation.Attach(model.MedicalCenterTranslation[0]);
            cmsContext.Entry(model.MedicalCenterTranslation[0]).State = EntityState.Modified;
            cmsContext.SaveChanges();

            cmsContext.MedicalCenterTranslation.Attach(model.MedicalCenterTranslation[1]);
            cmsContext.Entry(model.MedicalCenterTranslation[1]).State = EntityState.Modified;
            cmsContext.SaveChanges();

            cmsContext.Address.Attach(model.Address);
            cmsContext.Entry(model.Address).State = EntityState.Modified;
            cmsContext.SaveChanges();

            cmsContext.AddressTranslation.Attach(model.Address.AddressTranslation[0]);
            cmsContext.Entry(model.Address.AddressTranslation[0]).State = EntityState.Modified;
            cmsContext.SaveChanges();

            cmsContext.AddressTranslation.Attach(model.Address.AddressTranslation[1]);
            cmsContext.Entry(model.Address.AddressTranslation[1]).State = EntityState.Modified;
            cmsContext.SaveChanges();

            cmsContext.MedicalCenter.Attach(model);

            cmsContext.Entry(model).State = EntityState.Modified;
            cmsContext.SaveChanges();

            return RedirectToAction("Index", "Center", new { area = "CenterAdmin", fromDashboard = 1 });
        }

        public async Task<ActionResult> Read_ContactInfo([DataSourceRequest] DataSourceRequest request, string preferredCulture = "en-US")
        {
            Guid guid = (Guid)_userService.GetMyCenterIdWeb();

            MedicalCenter? myCenter = await medicalCenterService.GetMyCenterAsync();
            if (myCenter == null)
            {
                // Handle the case where myCenter is null
                return Json(new DataSourceResult { Errors = "Medical Center not found." });
            }

            var result = myCenter.ContactInfo.ToDataSourceResult(request);
            return Json(result);
        }

        public async Task<IActionResult> Create_ContactInfo([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ContactInfo> ContactInfos)
        {

            MedicalCenter myCenter = await medicalCenterService.GetMyCenterAsync();


            //if (ModelState.IsValid)
            {
                foreach (var item in ContactInfos)
                {
                    myCenter.ContactInfo.Add(item);
                }

                cmsContext.MedicalCenter.Attach(myCenter);
                cmsContext.Entry(myCenter).State = EntityState.Modified;

                cmsContext.SaveChanges();
                return Json(_localizer["Success"]);
            }



        }

        [AcceptVerbs("Post")]
        public async Task<IActionResult> Update_ContactInfo([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ContactInfo> ContactInfos)
        {
            {
                foreach (var item in ContactInfos)
                {
                    cmsContext.ContactInfo.Attach(item);
                    cmsContext.Entry(item).State = EntityState.Modified;
                    cmsContext.SaveChanges();
                }
                return Json(_localizer["Success"]);
            }



        }

        public async Task<IActionResult> Destroy_ContactInfo([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ContactInfo> ContactInfos)
        {
            {
                foreach (var item in ContactInfos)
                {
                    item.DeleteFromDb();
                }
                return Json(_localizer["Success"]);
            }


        }







        public async Task<ActionResult> Read_OpeningHours([DataSourceRequest] DataSourceRequest request, string preferredCulture = "en-US")
        {
            MedicalCenter? myCenter = await medicalCenterService.GetMyCenterAsync();
            if (myCenter == null)
            {
                // Handle the case where myCenter is null
                return Json(new DataSourceResult { Errors = "Medical Center not found." });
            }

            var result = myCenter.OpeningHours.ToDataSourceResult(request);
            return Json(result);
        }
        public async Task<IActionResult> Create_OpeningHours([DataSourceRequest] DataSourceRequest request, OpeningHours item)
        {

            MedicalCenter myCenter = await medicalCenterService.GetMyCenterAsync();


            //if (ModelState.IsValid)
            {
                //foreach (var item in ContactInfos)
                {
                    myCenter.OpeningHours.Add(item);
                }

                cmsContext.MedicalCenter.Attach(myCenter);
                cmsContext.Entry(myCenter).State = EntityState.Modified;

                cmsContext.SaveChanges();
                return Json(_localizer["Success"]);
            }



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

