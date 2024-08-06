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
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.ComponentModel.DataAnnotations;
using ServicesLibrary.StripeServices;
using static Azure.Core.HttpHeader;

namespace CmsWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubscriptionPlanController : Controller
    {
        private readonly IStringLocalizer<CmsResources.Messages> _localizer;
        private readonly ILogger<SubscriptionPlanController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;

        private readonly IEmailSender _emailSender;


        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext cmsContext;
        private readonly IStripeService stripeService;

        public SubscriptionPlanController(
            IStringLocalizer<CmsResources.Messages> localizer,
            IConfiguration config,
            IUserService userService,
            IStripeService stripeService_,
            ApplicationDbContext _cMDbContext,

            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,

            RoleManager<IdentityRole> roleManager,
            ILogger<SubscriptionPlanController> logger)
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
            stripeService = stripeService_;
        }


        public async Task<IActionResult> Index()
        {
            return View("Admin/_SubscriptionPlan");
        }


        public ActionResult Read([DataSourceRequest] DataSourceRequest request, string preferredCulture = "en-US")
        {
            var result = cmsContext.SubscriptionPlan
                .Include(a => a.SubscriptionPlanTranslation)
                .Where(a=> !a.IsDeleted)
                .ToList().ToDataSourceResult(request);

            return Json(result);
        }



        public IActionResult CreatePlan()
        {

            return View("Admin/_CreatePlan");

        }


        [HttpPost]
        public IActionResult CreatePlan(

string NameAr,
string NameEn,
string DescriptionAr,
string DescriptionEn,
        string PriceRecuencyInterval,
    long PriceAmount,
    long SetUpCost = 0,
    int? FreeDays = 0
            )
        {

            List<SubscriptionPlanTranslation> subscriptionPlanTranslations = new List<SubscriptionPlanTranslation>()
            {
                new SubscriptionPlanTranslation
                {
                    Id = Guid.NewGuid(),
                    SubscriptionPlanId = Guid.NewGuid(),
                    Description = DescriptionAr,
                    Name = NameAr,
                    LangCode = "ar"
                },
                new SubscriptionPlanTranslation
                {
                    Id = Guid.NewGuid(),
                    SubscriptionPlanId = Guid.NewGuid(),
                    Description = DescriptionEn,
                    Name = NameEn,
                    LangCode = "en-us"

                }
            };

            stripeService.AddNewSubscriptionPlan(
                subscriptionPlanTranslations,
                         PriceRecuencyInterval,
     PriceAmount,
     SetUpCost,
      FreeDays
                );

            return RedirectToAction("Index");
        }








        public IActionResult EditPlan(Guid id)
        {
            SubscriptionPlan model =cmsContext.SubscriptionPlan
                .Include(a=>a.SubscriptionPlanTranslation)
                .FirstOrDefault(a=>a.Id == id);


            return View("Admin/_EditPlan", model.ToDto());
        }


        [HttpPost]
        public IActionResult EditPlan(

            Guid Id,

string NameAr,
string NameEn,
string DescriptionAr,
string DescriptionEn,
        string PriceRecuencyInterval,
    long PriceAmount,
    long SetUpCost = 0,
    int? FreeDays = 0
            )
        {

            List<SubscriptionPlanTranslation> subscriptionPlanTranslations = new List<SubscriptionPlanTranslation>()
            {
                new SubscriptionPlanTranslation
                {
                    SubscriptionPlanId =Id,
                    Description = DescriptionAr,
                    Name = NameAr,
                    LangCode = "ar"
                },
                new SubscriptionPlanTranslation
                {
                    SubscriptionPlanId = Id,
                    Description = DescriptionEn,
                    Name = NameEn,
                    LangCode = "en-us"

                }
            };

            stripeService.UpdateSubscriptionPlan(
                Id,
                subscriptionPlanTranslations,
                         PriceRecuencyInterval,
     PriceAmount,
     SetUpCost,
      FreeDays
                );

            return RedirectToAction("Index");
        }


        [HttpPost]

        public async Task<IActionResult> Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] SubscriptionPlan model,string Id)
        {

            stripeService.DeleteSubscriptionPlan(Guid.Parse(Id));


            return Json("");

        }




    }
}
