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
using ServicesLibrary.MedicalCenterServices;

namespace CmsWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NewSubscriptionController : Controller
    {
        private readonly IStringLocalizer<CmsResources.Messages> _localizer;
        private readonly ILogger<NewSubscriptionController> _logger;
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
        private readonly IMedicalCenterService medicalCenterService;


        public NewSubscriptionController(
        IMedicalCenterService medicalCenterService_,

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
            ILogger<NewSubscriptionController> logger)
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
            medicalCenterService = medicalCenterService_;
        }

        public async Task<IActionResult> Index()
        {
            return View("Admin/_NewSubscriptionList");
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request, string preferredCulture = "en-US")
        {
            var result = cmsContext.SubscriptionApplication
                .Where(a=>a.EmailVerfied)
                .OrderBy(a=>a.CreateDate)
                .ToList().ToDataSourceResult(request);

            return Json(result);
        }

        public IActionResult Edit(Guid id)
        {
            SubscriptionApplication model =cmsContext.SubscriptionApplication
                .FirstOrDefault(a=>a.Id == id);

            return View("Admin/_EditSubscriptionRequest",model);
        }


        [HttpPost]
        public IActionResult Accept(Guid Id)
        {

            medicalCenterService.AcceptNewSubscriptionApplication(Id,1);



            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Reject(Guid Id, string rejectReason,bool finalReject)
        {
            if (finalReject)
            {
                medicalCenterService.AcceptNewSubscriptionApplication(Id, -2, rejectReason);

            }
            else
            {
                medicalCenterService.AcceptNewSubscriptionApplication(Id, -1, rejectReason);

            }

            return RedirectToAction("Index");
        }





    }
}
