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
using ServicesLibrary;
using System.Web;
using CmsDataAccess.ModelsDto;

namespace CmsWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SettingController : Controller
    {
        private readonly IStringLocalizer<CmsResources.Messages> _localizer;
        private readonly ILogger<SettingController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;

        private readonly ISeedDb seedDb;

        private readonly IEmailSender _emailSender;


        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext cmsContext;

        public SettingController(
            IStringLocalizer<CmsResources.Messages> localizer,
            IConfiguration config,
            IUserService userService,
            ISeedDb seedDb_,
            ApplicationDbContext _cMDbContext,

            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,

            RoleManager<IdentityRole> roleManager,
            ILogger<SettingController> logger)
        {
            seedDb = seedDb_;
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

            ViewBag.DispalyName = _localizer["System Settings"];
            ViewBag.PreviousActionDispalyName = _localizer["Settings"];
            ViewBag.PreviousAction = "Index";

            return View("Admin/_Settings");
        }

        public async Task<IActionResult> AboutUs()
        {

            ViewBag.DispalyName = _localizer["About the System"];
            ViewBag.PreviousActionDispalyName = _localizer["About Us"];
            ViewBag.PreviousAction = "AboutUs";


            //seedDb.SeedDbTables();
            AboutUs model1 =cmsContext.AboutUs.Include(a=>a.AboutUsTranslation).FirstOrDefault();
            AboutUsDto model = model1.ToDto();
            return View("Admin/_AdminAboutUs", model);
        }

        [HttpPost]
        public async Task<IActionResult> AboutUs(AboutUsDto model1)
        {
            ViewBag.DispalyName = _localizer["About the System"];
            ViewBag.PreviousActionDispalyName = _localizer["About Us"];
            ViewBag.PreviousAction = "AboutUs";

            //seedDb.SeedDbTables();
            AboutUs model=model1.ToModel();
            model.DeleteFromDb();
            model.InsertIntoDb();
            return View("Admin/_AdminAboutUs", model.ToDto());
        }

        public async Task<IActionResult> TermsAndConditions()
        {

            ViewBag.DispalyName = _localizer["System Terms and Conditions"];
            ViewBag.PreviousActionDispalyName = _localizer["Terms and Conditions"];
            ViewBag.PreviousAction = "TermsAndConditions";


            //seedDb.SeedDbTables();
            TermsAndConditions model1 = cmsContext.TermsAndConditions.Include(a => a.TermsAndConditionsTranslation).FirstOrDefault();
            TermsAndConditionsDto model = model1.ToDto();
            return View("Admin/_TermsAndConditions", model);
        }

        [HttpPost]
        public async Task<IActionResult> TermsAndConditions(TermsAndConditionsDto model1)
        {
            ViewBag.DispalyName = _localizer["System Terms and Conditions"];
            ViewBag.PreviousActionDispalyName = _localizer["Terms and Conditions"];
            ViewBag.PreviousAction = "TermsAndConditions";

            //seedDb.SeedDbTables();
            TermsAndConditions model = model1.ToModel();
            model.DeleteFromDb();
            model.InsertIntoDb();
            return View("Admin/_TermsAndConditions", model.ToDto());
        }


        public async Task<IActionResult> SystemConfig()
        {
            ViewBag.DispalyName = _localizer["System Settings"];
            ViewBag.PreviousActionDispalyName = _localizer["Settings"];
            ViewBag.PreviousAction = "SystemConfig";

            //seedDb.SeedDbTables();
            MySystemConfiguration model = cmsContext.MySystemConfiguration.FirstOrDefault();
            return View("Admin/_SystemConfig", model);
        }


        [HttpPost]
        public async Task<IActionResult> SystemConfig(MySystemConfiguration model)
        {
            ViewBag.DispalyName = _localizer["System Settings"];
            ViewBag.PreviousActionDispalyName = _localizer["Settings"];
            ViewBag.PreviousAction = "SystemConfig";

            //seedDb.SeedDbTables();
            model.DeleteFromDb();
            model.InsertIntoDb();
            return View("Admin/_SystemConfig", model);
        }



    }
}
