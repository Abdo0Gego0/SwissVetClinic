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
    public class BillsController : Controller
    {

        private readonly IHubContext<MyHub> _hubContext;

        private readonly IStringLocalizer<CmsResources.Messages> _localizer;
        private readonly ILogger<BillsController> _logger;
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

        public BillsController(
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
            ILogger<BillsController> logger)
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
            return RedirectToAction("IndexPatient", "Patient", new {area= "CenterAdmin" });
        }

        public async Task<IActionResult> Index(Guid id)
        {

            ViewBag.DispalyName = _localizer["Bills List"];
            ViewBag.PreviousActionDispalyName = _localizer["Bills"];

            ViewBag.PreviousAction = "Index";



            return View("CenterAdmin/_IndexBills");
        }


        public async Task<IActionResult> Read([DataSourceRequest] DataSourceRequest request)
        {
            var result = cmsContext.VisistBill
                
                .OrderBy(a=>a.CreateDate)
                .OrderBy(a=>a.Status)
                
            .ToList().ToDataSourceResult(request);
            return Json(result);
        }      
        
        public async Task<IActionResult> Read_VisitMedicine([DataSourceRequest] DataSourceRequest request,Guid PatientVisitId)
        {
            var result = cmsContext.VisitMedicine
                .Where(a=>a.PatientVisitId== PatientVisitId)
                .ToList().ToDataSourceResult(request);
            return Json(result);
        }
        

        public async Task<IActionResult> Edit_Bill(Guid id)
        {
            return View("CenterAdmin/_Edit_Bill", cmsContext.VisistBill.Find(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit_Bill(VisistBill model)
        {

            if (model.Status == 1)
            {
                return View("Close");
            }

            cmsContext.VisistBill.Attach(model);
            model.PaymenrDate = medicalCenterService.ConvertToLocalTime(DateTime.Now);
            model.Status =1;

            

            cmsContext.Entry(model).Property(a => a.PaymentType).IsModified = true;
            cmsContext.Entry(model).Property(a => a.PaymenrDate).IsModified = true;
            cmsContext.Entry(model).Property(a => a.Status).IsModified = true;
            cmsContext.Entry(model).Property(a => a.Discount).IsModified = true;
            cmsContext.SaveChanges();
            await _hubContext.Clients.All.SendAsync("RefreshBillStatus_", _userService.GetMyCenterIdWeb().ToString());

            return View("Close");
        }



    }

}

