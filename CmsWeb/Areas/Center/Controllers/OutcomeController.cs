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
using System;


namespace CmsWeb.Areas.Center.Controllers
{
    [Area("CenterAdmin")]

    public class OutcomeController : Controller
    {

        private readonly IStringLocalizer<CmsResources.Messages> _localizer;
        private readonly ILogger<OutcomeController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;

        private readonly IEmailSender _emailSender;
        private readonly IMedicalCenterService medicalCenterService;
        private readonly IPersonService personService;

        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext cmsContext;

        public OutcomeController(
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
            ILogger<OutcomeController> logger)
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



        public IActionResult Index()
        {

            ViewBag.DispalyName = _localizer["Outcome"];
            ViewBag.PreviousActionDispalyName = _localizer["Outcome"];
            ViewBag.PreviousAction = "Index";

            return View("CenterAdmin/_Outcome");
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request, Guid? CourseId, string preferredCulture = "en-US")
        {
            var result = cmsContext.Outcome
                .ToDataSourceResult(request);
            return Json(result);
        }

        public async Task<IActionResult> Create([DataSourceRequest] DataSourceRequest request, Outcome task)
        {
            //string uniqueFileName = FileHandler.SaveUploadedFile(task.ImageFile);

            cmsContext.Outcome.Add(new Outcome
            {
                Amount=task.Amount,
                EmployeeId= (Guid)_userService.GetMyId(),
                CreateDate=medicalCenterService.ConvertToLocalTime(DateTime.Now),
                Description=task.Description,
                Title=task.Title,
            }); 

            cmsContext.SaveChanges();

            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }

        public async Task<IActionResult> Destroy([DataSourceRequest] DataSourceRequest request, Outcome task)
        {
            cmsContext.Outcome.Remove(task);
            cmsContext.SaveChanges();
            return Json("Success");
        }

        public async Task<IActionResult> Update([DataSourceRequest] DataSourceRequest request, Outcome task)
        {

            cmsContext.Outcome.Attach(task);
            cmsContext.Entry(task).State = EntityState.Modified;
            cmsContext.SaveChanges();

            return Json("Success");
        }






    }
}
