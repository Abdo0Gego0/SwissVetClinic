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

namespace CmsWeb.Areas.Center.Controllers
{
    [Area("CenterAdmin")]
    public class HomeController : Controller
    {
        private readonly IStringLocalizer<CmsResources.Messages> _localizer;
        private readonly ILogger<HomeController> _logger;
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

        public HomeController(
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
            ILogger<HomeController> logger)
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

        public async Task<IActionResult> Index()
        {

            ViewBag.DispalyName = _localizer["Home Page"];
            ViewBag.PreviousActionDispalyName = _localizer["Home"];

            ViewBag.PreviousAction = "Index";

            return View("CenterAdmin/_CenterAdminIndex");

            Guid guid = (Guid)_userService.GetMyId();

            CenterAdmin centerAdmin = cmsContext.CenterAdmin
                .Include(a => a.User).FirstOrDefault(a => a.User.Id == guid.ToString());

            if (!centerAdmin.PaidAccountIsActive)
            {
                ViewBag.Id = guid;
                ViewBag.Id1 = centerAdmin.Id;
                ViewBag.Id2 = centerAdmin.User.Id;
                SubscriptionApplication model=cmsContext.SubscriptionApplication
                    .FirstOrDefault(a=>a.CenterAdminId== centerAdmin.Id);

                return View("CenterAdmin/_CenterAdminApplication",model);
            }

            CenterSetUpSteps centerSetUpSteps = cmsContext.CenterSetUpSteps.FirstOrDefault(a=>a.CenterAdminId== centerAdmin.Id);

            if (centerSetUpSteps.StepNumer==0)
            {
                return RedirectToAction("Index","Center",new { area= "CenterAdmin" });
            }

            return View("CenterAdmin/_CenterAdminIndex");
        }


        public async Task<IActionResult> SubmissionAction(SubscriptionApplication model)
        {
            SubscriptionApplication oldModel=cmsContext.SubscriptionApplication.Find(model.Id);

            if (oldModel.Accepted==0 || oldModel.Accepted == -1) // Waiting // Must be edited
            {

                oldModel.SubscriberEmail = model.SubscriberEmail;
                oldModel.SubscriberPhone= model.SubscriberPhone;
                oldModel.PassportNumber = model.PassportNumber;
                oldModel.LicenseNumber = model.LicenseNumber;
                oldModel.FirstName= model.FirstName;
                oldModel.LastName= model.LastName;
                oldModel.MiddleName= model.MiddleName;
                oldModel.NationalCardId= model.NationalCardId;
                oldModel.ResponseFromAdmin = oldModel.ResponseFromAdmin;
                oldModel.VerificationExpireDate = oldModel.VerificationExpireDate;
                oldModel.Nationality= model.Nationality;
                oldModel.CenterAddress=model.CenterAddress;
                oldModel.CenterAdminId=oldModel.CenterAdminId;
                oldModel.EmailVerfied=oldModel.EmailVerfied;
                oldModel.IsSeen = false;
                oldModel.CreateDate=medicalCenterService.ConvertToSystemTime(DateTime.Now);

                if (model.PassportImageFile != null)
                {
                    oldModel.PassportImageName = FileHandler.UpdateProfileImage(model.PassportImageFile, oldModel.PassportImageName);
                }
                if (model.LicenseImageName != null)
                {
                    oldModel.LicenseImageName = FileHandler.UpdateProfileImage(model.LicenseImageFile, oldModel.LicenseImageName);
                }

                cmsContext.SubscriptionApplication.Attach(oldModel);
                cmsContext.Entry(oldModel).State=EntityState.Modified;
                cmsContext.SaveChanges();

            }

            if (oldModel.Accepted == -2) // Rejected
            {

            }

            if (oldModel.Accepted == 1) // Approved
            {

            }

            SubscriptionApplication oldModel1 = cmsContext.SubscriptionApplication.Find(model.Id);

            return View("CenterAdmin/_CenterAdminApplication",oldModel1);


        }

        public async Task<IActionResult> ChoosePlan()
        {
            List<SubscriptionPlan> res=cmsContext.SubscriptionPlan.Include(a=>a.SubscriptionPlanTranslation.Where(a => a.LangCode == "en-us"))
                .OrderBy(a=>a.PriceRecuencyInterval)
                .ToList();
            return View("CenterAdmin/_ChoosePlan",res);
        }

        public async Task<IActionResult> CheckOut(Guid Id,string? errorMessage="")
        {

            SubscriptionPlan res = cmsContext.SubscriptionPlan.Include(a => a.SubscriptionPlanTranslation.Where(a => a.LangCode == "en-us"))
    .OrderBy(a => a.PriceRecuencyInterval)
    .FirstOrDefault(a => a.Id == Id);


            ViewBag.ErrorMessage = errorMessage;

            return View("CenterAdmin/_CheckOut", 
                new {
                    Id=Id,
                    Name= res.SubscriptionPlanTranslation[0].Name ,
                    Reccurency = res.PriceRecuencyInterval.ToUpper() ,
                    Amount = res.PriceAmount });
        }


        [HttpPost]
        public IActionResult ProcessPayment()
        {

            var stripeToken = Request.Form["cardToken"];
            var planId = Request.Form["planId"];

            string res=medicalCenterService.PaySubscription(stripeToken, Guid.Parse(planId));

            if (!res.IsNullOrEmpty())
            {
                Guid yourId = Guid.Parse(planId);
                string errorMessage = res; // Replace with your actual error message
                return RedirectToAction("CheckOut", new { Id = yourId, errorMessage });
            }

            return RedirectToAction("Index");
        }


        public IActionResult MyAccount()
        {


            ViewBag.DispalyName = _localizer["Account"];
            ViewBag.PreviousActionDispalyName = _localizer["Home"];

            ViewBag.PreviousAction = "Index";

            Guid Id = (Guid)_userService.GetMyId();

            CenterAdmin centerTutor = cmsContext.CenterAdmin
                 .Include(a => a.Address)
                .Include(a => a.User)
                .FirstOrDefault(a => a.User.Id == Id.ToString());

            centerTutor.AdminEmail = centerTutor.User.Email;
            centerTutor.AdminUserName = centerTutor.User.UserName;
            centerTutor.AdminPhone = centerTutor.User.PhoneNumber;

            return View("CenterAdmin/_MyAccount", centerTutor);

        }

        [HttpPost]
        public async Task<IActionResult> UpdateAccountInfo(string AdminEmail,
            string AdminPhone,
            string AdminUserName)
        {

            ViewBag.DispalyName = _localizer["Account"];
            ViewBag.PreviousActionDispalyName = _localizer["Home"];
            ViewBag.PreviousAction = "Index";

            Guid Id = (Guid)_userService.GetMyId();

            CenterAdmin centerSupervisor = cmsContext.CenterAdmin
                 .Include(a => a.Address).ThenInclude(a => a.AddressTranslation)
                .Include(a => a.User)
                .FirstOrDefault(a => a.User.Id == Id.ToString());

            cmsContext.CenterAdmin.Attach(centerSupervisor);

            if (centerSupervisor.User.Email != AdminEmail)
            {
                centerSupervisor.User.Email = AdminEmail;
                var resul1 = await _userManager.UpdateAsync(centerSupervisor.User);

                if (!resul1.Succeeded)
                {
                    string msg = "";

                    foreach (var item in resul1.Errors)
                    {
                        msg += item.Description + " ";
                    }

                    ViewBag.ErrorMessage = _localizer["MakeSureThatEmailisUnique"];

                    return RedirectToAction("MyAccount");
                }

                centerSupervisor.User.EmailConfirmed = true;
                cmsContext.Entry(centerSupervisor).State = EntityState.Modified;
            }

            if (centerSupervisor.User.UserName != AdminUserName)
            {

                centerSupervisor.User.UserName = AdminUserName;
                var resul2 = await _userManager.UpdateAsync(centerSupervisor.User);


                if (!resul2.Succeeded)
                {
                    string msg = "";

                    foreach (var item in resul2.Errors)
                    {
                        msg += item.Description + " ";
                    }

                    ViewBag.ErrorMessage = _localizer["MakeSureThatUserisUnique"];


                    return RedirectToAction("MyAccount");

                }

                centerSupervisor.User.EmailConfirmed = true;
                cmsContext.Entry(centerSupervisor).State = EntityState.Modified;


            }

            if (centerSupervisor.User.PhoneNumber != AdminPhone)
            {
                centerSupervisor.User.PhoneNumber = AdminPhone;

                var resul3 = await _userManager.UpdateAsync(centerSupervisor.User);

                if (!resul3.Succeeded)
                {
                    string msg = "";

                    foreach (var item in resul3.Errors)
                    {
                        msg += item.Description + " ";
                    }

                    ViewBag.ErrorMessage = _localizer["MakeSureThatUserisUnique"];

                    return RedirectToAction("MyAccount");
                }
                centerSupervisor.User.PhoneNumberConfirmed = true;
                cmsContext.Entry(centerSupervisor).State = EntityState.Modified;
            }

            return RedirectToAction("MyAccount");
        }


        [HttpPost]
        public async Task<ActionResult> updateProfilePicture(IFormFile ImageFile)
        {

            ViewBag.DispalyName = _localizer["Account"];
            ViewBag.PreviousActionDispalyName = _localizer["Home"];
            ViewBag.PreviousAction = "Index";


            Guid Id = (Guid)_userService.GetMyId();

            CenterAdmin centerSupervisor = cmsContext.CenterAdmin
                 .Include(a => a.Address).ThenInclude(a => a.AddressTranslation)
                .Include(a => a.User)
                .FirstOrDefault(a => a.User.Id == Id.ToString());

            if (ImageFile != null)
            {
                string uniqueFileName = FileHandler.UpdateProfileImage(ImageFile, centerSupervisor.ImageName);
                centerSupervisor.ImageName = uniqueFileName;
            }

            cmsContext.CenterAdmin.Attach(centerSupervisor);
            cmsContext.Entry(centerSupervisor).State = EntityState.Modified;
            cmsContext.SaveChanges();

            return RedirectToAction("MyAccount");

        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
        {
            ViewBag.DispalyName = _localizer["Account"];
            ViewBag.PreviousActionDispalyName = _localizer["Home"];
            ViewBag.PreviousAction = "Index";
            string userName = _userService.GetUserName();

            ViewBag.ErrorMessage = "";

            IdentityUser identityUser = await _userManager.FindByNameAsync(userName);

            var token = await _userManager.GeneratePasswordResetTokenAsync(identityUser);

            var result = await _userManager.ChangePasswordAsync(identityUser, oldPassword, newPassword);


            if (!result.Succeeded)
            {
                string msg = "";

                foreach (var item in result.Errors)
                {
                    msg += item.Description + " ";
                }

                ViewBag.ErrorMessage = msg;

                return View();

            }

            return RedirectToAction("MyAccount");
        }
    }

}

