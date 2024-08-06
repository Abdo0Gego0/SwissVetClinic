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

namespace CmsWeb.Areas.Center.Controllers
{
    [Area("CenterAdmin")]
    public class AttendanceController : Controller
    {
        private readonly IStringLocalizer<CmsResources.Messages> _localizer;
        private readonly ILogger<AttendanceController> _logger;
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

        public AttendanceController(
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
            ILogger<AttendanceController> logger)
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



        #region ShiftTable

        public async Task<IActionResult> IndexShiftTable(int fromDashboard = 0)
        {

            ViewBag.FromDashboard = fromDashboard;
            return View("CenterAdmin/_ShiftList");
        }


        public ActionResult Read_ShiftTable([DataSourceRequest] DataSourceRequest request, string preferredCulture = "en-US")
        {

            return Json(medicalCenterService.GetMyCenterShifts(request, preferredCulture));
        }

        public async Task<IActionResult> Create_ShiftTable([DataSourceRequest] DataSourceRequest request, ShiftTable model)
        {

            Guid guid = (Guid)_userService.GetMyCenterIdWeb();  
            model.MedicalCenterId = guid;
            model.InsertIntoDb();

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        public async Task<IActionResult> Destroy_ShiftTable([DataSourceRequest] DataSourceRequest request, ShiftTable task)
        {
            task.DeleteFromDb();
            return Json("Success");
        }

        public async Task<IActionResult> Update_ShiftTable([DataSourceRequest] DataSourceRequest request, ShiftTable task)
        {
            cmsContext.ShiftTable.Attach(task);

            cmsContext.Entry(task).State = EntityState.Modified;

            cmsContext.SaveChanges();

            return Json("Success");
        }


        #endregion

    }

}

