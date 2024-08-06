﻿using Kendo.Mvc.Resources;
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
using Microsoft.AspNetCore.Localization;
using Newtonsoft.Json.Linq;

namespace CmsWeb.Areas.CcenterDoctor.Controllers
{
    [Area("Doctor")]
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


        public async Task<IActionResult> ChangeClinic(Guid ClinicId)
        {
            Guid userId = (Guid)_userService.GetMyId();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var user = await _userManager.FindByIdAsync(userId.ToString());

            CmsDataAccess.DbModels.Person person111 = cmsContext.Person.Include(a => a.User)
                                                .FirstOrDefault(a => a.User.Id == userId.ToString());

            Doctor centerAdmin11 = cmsContext.Doctor.Find(person111.Id);

            List<Claim> claims111 = new List<Claim>()
                                {
                                    new Claim(ClaimTypes.NameIdentifier, user.UserName),
                                    new Claim(ClaimTypes.SerialNumber, user.Id.ToString()),
                                    new Claim("FirstName", person111.FirstName),
                                    new Claim(ClaimTypes.Name, person111.FullName ),
                                    new Claim(ClaimTypes.Email, user.Email),
                                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                                    new Claim(ClaimTypes.Role, "doctor"),
                                    new Claim("ProfileImage", person111.ImageFullPath),
                                    new Claim("CenterId", centerAdmin11.MedicalCenterId.ToString()),
                                    new Claim("ClinicId", ClinicId.ToString()),
                                };

            // also add cookie auth for Swagger Access
            var identity111 = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);

            identity111.AddClaims(claims111);


            var principal111 = new ClaimsPrincipal(identity111);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal111,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(1)
                });


            //return Redirect("Index");


            string returnUrl = Request.Headers["Referer"].ToString() ?? "/";
            return Redirect(returnUrl);
        }









        public async Task<IActionResult> Index()
        {


            ViewBag.DispalyName = _localizer["Current Appointments"];
            ViewBag.PreviousActionDispalyName = _localizer["Appointments"];

            ViewBag.PreviousAction = "Index";

            return View("CcenterDoctor/_DoctorAdminIndex");
        }



        public IActionResult MyAccount()
        {

            ViewBag.DispalyName = _localizer["Account"];
            ViewBag.PreviousActionDispalyName = _localizer["Home"];

            ViewBag.PreviousAction = "Index";

            Guid Id = (Guid)_userService.GetMyId();

            Doctor centerTutor = cmsContext.Doctor
                 .Include(a => a.Address)
                .Include(a => a.User)
                .FirstOrDefault(a => a.User.Id == Id.ToString());

            centerTutor.PersonEmail = centerTutor.User.Email;
            centerTutor.PersonUserName = centerTutor.User.UserName;
            centerTutor.PersonPhone = centerTutor.User.PhoneNumber;

            return View("CcenterDoctor/_MyAccount", centerTutor);

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

            Doctor centerSupervisor = cmsContext.Doctor
                 .Include(a => a.Address).ThenInclude(a => a.AddressTranslation)
                .Include(a => a.User)
                .FirstOrDefault(a => a.User.Id == Id.ToString());

            cmsContext.Doctor.Attach(centerSupervisor);

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

            Doctor centerSupervisor = cmsContext.Doctor
                .Include(a => a.User)
                .FirstOrDefault(a => a.User.Id == Id.ToString());

            if (ImageFile != null)
            {
                string uniqueFileName = FileHandler.UpdateProfileImage(ImageFile, centerSupervisor.ImageName);
                centerSupervisor.ImageName = uniqueFileName;
            }

            cmsContext.Doctor.Attach(centerSupervisor);
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

