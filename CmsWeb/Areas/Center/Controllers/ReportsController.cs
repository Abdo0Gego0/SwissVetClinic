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
using CmsDataAccess.ChartModels;



namespace CmsWeb.Areas.Center.Controllers
{
    [Area("CenterAdmin")]
    public class ReportsController : Controller
    {

        private readonly IHubContext<MyHub> _hubContext;

        private readonly IStringLocalizer<CmsResources.Messages> _localizer;
        private readonly ILogger<ReportsController> _logger;
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

        public ReportsController(
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
            ILogger<ReportsController> logger)
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

        public async Task<IActionResult> Index()
        {
            ViewBag.DispalyName = _localizer["Reports"];
            ViewBag.PreviousActionDispalyName = _localizer["Reports"];
            ViewBag.PreviousAction = "Index";

            return View("CenterAdmin/_IndexReports");
        }
        public ActionResult CustomersGrowth_Read([DataSourceRequest] DataSourceRequest request)
        {

            Guid guid = (Guid)_userService.GetMyCenterIdWeb();



            var currentYear = DateTime.Now.Year;

            // Query for previous years
            var previousYearsCounts = cmsContext.PetOwner
                .Where(c => c.CreateDate.HasValue && c.CreateDate.Value.Year < currentYear)
                .GroupBy(c => c.CreateDate.Value.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Count = g.Count()
                })
                .ToList();

            // Query for current 
            var currentYearCounts = cmsContext.PetOwner
                .Where(c => c.CreateDate.HasValue && c.CreateDate.Value.Year == currentYear)
                .GroupBy(c => c.CreateDate.Value.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Count = g.Count()
                })
                .ToList();

            // Initialize result list with all months
            var result = Enumerable.Range(1, 12).Select(m => new CustomerCount
            {
                Month = new DateTime(1, m, 1).ToString("MMMM"),
                Old = 0,
                New = 0
            }).ToList();

            // Fill in old values (previous years)
            foreach (var item in previousYearsCounts)
            {
                var monthName = new DateTime(1, item.Month, 1).ToString("MMMM");
                var customerCount = result.FirstOrDefault(r => r.Month == monthName);
                if (customerCount != null)
                {
                    customerCount.Old = item.Count;
                }
            }

            // Fill in new values (current year)
            foreach (var item in currentYearCounts)
            {
                var monthName = new DateTime(1, item.Month, 1).ToString("MMMM");
                var customerCount = result.FirstOrDefault(r => r.Month == monthName);
                if (customerCount != null)
                {
                    customerCount.New = item.Count;
                }
            }
            return Json(result);

        }

        public ActionResult Read_StockData([DataSourceRequest] DataSourceRequest request)
        {

            Guid guid = (Guid)_userService.GetMyCenterIdWeb();


            List<StockDataPoint> points = new List<StockDataPoint>();

            COrder cOrder = cmsContext.COrder.OrderBy(a => a.OrderNumber).FirstOrDefault();

            if (cOrder.CreatedDate.Year==DateTime.Now.Year)
            {
                for (int i = 0; i < 5; i++)
                {
                    points.Add(
                        new StockDataPoint
                        {
                            Value = 0,
                            Date = DateTime.Now.AddYears(-i)
                        }

                        );
                }
            }


            points.AddRange(cmsContext.COrder.Include(a=>a.COrderItems).ToList().Select(a => new StockDataPoint { Value = a.TotalCost, Date = a.CreatedDate }));

            return Json(points);

        }
             
        public ActionResult CustomersOrders_Read([DataSourceRequest] DataSourceRequest request)
        {

            Guid guid = (Guid)_userService.GetMyCenterIdWeb();



            // Query for previous years
            var ordersCount = cmsContext.COrder
                
                .GroupBy(c => c.PetOwnerId)
                .ToList()
                .Select(g => new
                {
                    Owner = g.Key,
                    TotalOrderAmount = g.Sum(o => o.TotalCost)
                })
                            .OrderByDescending(c => c.TotalOrderAmount)
            .Take(5)
            .ToList();

            var result = new List<TopCustomers>();

            // Fill in new values (current year)
            foreach (var item in ordersCount)
            {
                result.Add(new TopCustomers { 
                
                    Name=cmsContext.PetOwner.Find(item.Owner).FullName,
                    OrdersCount=cmsContext.COrder.Where(a=>a.PetOwnerId==item.Owner).Count(),
                    OrdersValue=item.TotalOrderAmount,
                });
            }

            return Json(result.ToDataSourceResult(request));

        }

        public ActionResult TopProducts_Read([DataSourceRequest] DataSourceRequest request)
        {

            Guid guid = (Guid)_userService.GetMyCenterIdWeb();



            // Query for previous years
            var ordersCount = cmsContext.COrderItems

                .GroupBy(c => c.SubProductId)
                .Select(g => new
                {
                    SubId = g.Key,
                    TotalOrderAmount = g.Sum(o => o.ItemQuantity*o.ItemPrice),
                    OrdersCount=g.Count()* g.Sum(o => o.ItemQuantity),
                    
                })
                            .OrderByDescending(c => c.TotalOrderAmount)
            .Take(5)
            .ToList();

            var result = new List<TopProducts>();

            // Fill in new values (current year)
            foreach (var item in ordersCount)
            {

                CmsDataAccess.DbModels.Product ProductName = cmsContext.Product.Include(a => a.ProductTranslation).FirstOrDefault(a=>a.Id==cmsContext.SubProduct.Find(item.SubId).ProductId);
              
                result.Add(new TopProducts
                {

                    Name = ProductName.ProductTranslation[0].Name+ " " + ProductName.ProductTranslation[1].Name,
                    OrdersCount = item.OrdersCount,
                    OrdersValue=item.TotalOrderAmount,
                    SubProductImage = cmsContext.SubProduct.Include(a=>a.SubProductImage).FirstOrDefault(a=>a.Id==item.SubId).SubProductImage,

                });
            }

            return Json(result.ToDataSourceResult(request));
        }  
        
        
        public ActionResult Visits_Read([DataSourceRequest] DataSourceRequest request)
        {

            Guid guid = (Guid)_userService.GetMyCenterIdWeb();


            var result=new  List<VisitCount>();

            result.Add(
                new VisitCount
                {
                    category="Unique Visist",
                    Color= "rgba(255, 199, 0, 1)",
                    value=cmsContext.Appointment.Where(a=>a.IsFirstVisit).Count(),

                }
                );

            result.Add(
    new VisitCount
    {
        category = "Repeated Visist",
        Color = "rgba(31, 210, 134, 1)",
        value = cmsContext.Appointment.Where(a => !a.IsFirstVisit).Count(),

    }
    );

            return Json(result);
        }




    }

   


}

