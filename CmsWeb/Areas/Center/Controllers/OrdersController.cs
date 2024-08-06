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
    public class OrdersController : Controller
    {

        private readonly IHubContext<MyHub> _hubContext;

        private readonly IStringLocalizer<CmsResources.Messages> _localizer;
        private readonly ILogger<OrdersController> _logger;
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

        public OrdersController(
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
            ILogger<OrdersController> logger)
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

       
        public async Task<IActionResult> Index()
        {
            ViewBag.DispalyName = _localizer["Orders List"];
            ViewBag.PreviousActionDispalyName = _localizer["Orders"];
            ViewBag.PreviousAction = "Index";

            return View("Ecommerce/_IndexOrders");
        }


        public ActionResult OrderItems_Read([DataSourceRequest] DataSourceRequest request, Guid id)
        {


            Guid guid = (Guid)_userService.GetMyCenterIdWeb();

            var coderList = cmsContext.COrderItems.Where(a => a.COrderId == id).ToDataSourceResult(request);

            return Json(coderList);
        }


        public ActionResult Order_Read([DataSourceRequest] DataSourceRequest request, Guid? ClinicId, string? view, string? date)
        {


            Guid guid = (Guid)_userService.GetMyCenterIdWeb();

            var coderList = cmsContext.COrder.Include(a=>a.COrderItems).Where(a=>a.MedicalCenterId==guid).ToDataSourceResult(request);

            return Json(coderList);
        }


        [HttpPost]
        public async Task<IActionResult> Order_Update([DataSourceRequest] DataSourceRequest request, COrder task)
        {

            //{
            //    meetingService.Update(task, ModelState);
            //}

            cmsContext.COrder.Attach(task);
            cmsContext.Entry(task).Property(a => a.Status).IsModified = true;
            cmsContext.Entry(task).Property(a => a.DeliveryCost).IsModified = true;
            cmsContext.SaveChanges();


            await _hubContext.Clients.All.SendAsync("RefreshNewOrderCount_", _userService.GetMyCenterIdWeb().ToString());

            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }

    }

}

