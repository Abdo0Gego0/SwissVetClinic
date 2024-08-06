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
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace CmsWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContactInfoController : Controller
    {
        private readonly IStringLocalizer<CmsResources.Messages> _localizer;
        private readonly ILogger<ContactInfoController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;

        private readonly IEmailSender _emailSender;


        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext cmsContext;

        public ContactInfoController(
            IStringLocalizer<CmsResources.Messages> localizer,
            IConfiguration config,
            IUserService userService,
            ApplicationDbContext _cMDbContext,

            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,

            RoleManager<IdentityRole> roleManager,
            ILogger<ContactInfoController> logger)
        {
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

        public async Task<IActionResult> Index()
        {

            ViewBag.DispalyName = _localizer["Contact List"];
            ViewBag.PreviousActionDispalyName = _localizer["Contant Info"];
            ViewBag.PreviousAction = "Index";

            return View("Admin/_ContactInfo");
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request, string preferredCulture = "en-US")
        {

            var result = cmsContext.ContactInfo
                .Where(contactInfo => !cmsContext.MedicalCenter
                    .Any(medicalCenter => medicalCenter.ContactInfo.Any(ci => ci.Id == contactInfo.Id)))
                .ToDataSourceResult(request);

            return Json(result);

        }



        public async Task<IActionResult> Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ContactInfo> ContactInfos)
        {
            //if (ModelState.IsValid)
            {
                foreach (var item in ContactInfos)
                {
                    cmsContext.ContactInfo.Add(item);
                }
                cmsContext.SaveChanges();
                return Json(_localizer["Success"]);
            }



        }

        [AcceptVerbs("Post")]

        public async Task<IActionResult> Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ContactInfo> ContactInfos)
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

        public async Task<IActionResult> Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ContactInfo> ContactInfos)
        {
            {
                foreach (var item in ContactInfos)
                {
                    item.DeleteFromDb();
                }
                return Json(_localizer["Success"]);
            }


        }




    }
}
