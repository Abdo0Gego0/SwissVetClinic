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
using ServicesLibrary.AppointmentSRVC;
using System.ComponentModel.Design;
using ServicesLibrary.Services.AppointmentSRVC;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.SignalR;
using CmsWeb.Hubs;
using System.Web;



namespace CmsWeb.Areas.Center.Controllers
{
    [Area("CenterAdmin")]
    public class CenterCategoriesController : Controller
    {

        private readonly IHubContext<MyHub> _hubContext;

        private readonly IStringLocalizer<CmsResources.Messages> _localizer;
        private readonly ILogger<CenterCategoriesController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;

        private readonly IEmailSender _emailSender;
        private readonly IMedicalCenterService medicalCenterService;

        private readonly SchedulerMeetingService meetingService;

        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext cmsContext;

        public CenterCategoriesController(
            IHubContext<MyHub> hubContext,

            IMedicalCenterService medicalCenterService_,
            IStringLocalizer<CmsResources.Messages> localizer,
            IConfiguration config,
            IUserService userService,
            ApplicationDbContext _cMDbContext,
            SchedulerMeetingService meetingService_,

            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,

            RoleManager<IdentityRole> roleManager,
            ILogger<CenterCategoriesController> logger)
        {
            _hubContext = hubContext;
            meetingService = meetingService_;
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

        public async Task<IActionResult> MyHome()
        {
            return RedirectToAction("IndexPatient", "Patient", new { area = "CenterAdmin" });
        }

        public async Task<IActionResult> Index(Guid id)
        {
            ViewBag.DispalyName = _localizer["Categories List"];
            ViewBag.PreviousActionDispalyName = _localizer["Categories"];
            ViewBag.PreviousAction = "Index";

            return View("Ecommerce/_IndexCategories");
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request, string preferredCulture = "en-US")
        {
            Guid guid = (Guid)_userService.GetMyCenterIdWeb();
            var result = cmsContext.ProductCategories
                .Where(a => a.MedicalCenterId == guid)
                .Include(a => a.ProductCategoriesTranslation)
                .ToList().ToDataSourceResult(request);
            return Json(result);
        }

        public async Task<IActionResult> Create([DataSourceRequest] DataSourceRequest request, ProductCategories model, string docId)
        {
            // Save the image file from base64
            string uniqueFileName = FileHandler.SaveUploadedFileFrom64(model.Image64);

            // Create a new ProductCategories instance and set the MedicalCenterId
            var newCategory = new ProductCategories(cmsContext)
            {
                ImageName = uniqueFileName,
                ProductCategoriesTranslation = model.ProductCategoriesTranslation,
                MedicalCenterId = Guid.Parse("e177f712-6956-49fb-10af-08dc89f49c1c") // Set MedicalCenterId
            };

            // Add the new category to the context
            cmsContext.ProductCategories.Add(newCategory);

            // Save changes to the database
            await cmsContext.SaveChangesAsync();

            // Return the result as JSON
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }


        public async Task<IActionResult> Destroy([DataSourceRequest] DataSourceRequest request, ProductCategories task)
        {
            task.DeleteFromDb();
            return Json(_localizer["Success"]);
        }

        public async Task<IActionResult> Update([DataSourceRequest] DataSourceRequest request, ProductCategories task)
        {
            if (!string.IsNullOrEmpty(task.Image64))
            {
                FileHandler.DeleteImageFile(task.ImageName);
                string uniqueFileName = FileHandler.SaveUploadedFileFrom64(task.Image64);
                task.ImageName = uniqueFileName;
            }


            cmsContext.ProductCategories.Attach(task);

            cmsContext.ProductCategoriesTranslation.Attach(task.ProductCategoriesTranslation[0]);
            cmsContext.ProductCategoriesTranslation.Attach(task.ProductCategoriesTranslation[1]);

            cmsContext.Entry(task.ProductCategoriesTranslation[0]).State = EntityState.Modified;
            cmsContext.Entry(task.ProductCategoriesTranslation[1]).State = EntityState.Modified;
            cmsContext.Entry(task).State = EntityState.Modified;

            cmsContext.SaveChanges();

            return Json(_localizer["Success"]);
        }




    }

}

