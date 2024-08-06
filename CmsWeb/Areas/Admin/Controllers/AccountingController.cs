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
using ServicesLibrary.MedicalCenterServices;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using CmsDataAccess.Utils.FilesUtils;
using Mono.TextTemplating;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CmsWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountingController : Controller
    {
        private readonly IStringLocalizer<CmsResources.Messages> _localizer;
        private readonly ILogger<AccountingController> _logger;
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

        public AccountingController(
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
            ILogger<AccountingController> logger)
        {
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
            ViewBag.DispalyName = _localizer["Accounting"];
            ViewBag.PreviousActionDispalyName = _localizer["Centers"];
            ViewBag.PreviousAction = "Index";


            return View("Admin/_AccountingIndex");
        }


        public ActionResult Read_Patient([DataSourceRequest] DataSourceRequest request,
            DateTime? StartDate,
            DateTime? EndDate,
            Guid? MedicalCenterId,
            int? Paid,

            string preferredCulture = "en-US"

            )
        {

            if (MedicalCenterId == null )
            {
                var result1 = cmsContext.PetOwner
                .Where(a => !a.IsDeleted )
                .ToList().ToDataSourceResult(request);
                return Json(result1);
            }

            IQueryable<PetOwner> patients = cmsContext.PetOwner
               .Where(a => !a.IsDeleted && a.MedicalCenterId== (Guid)MedicalCenterId)
               .AsQueryable();

            //if (Paid!=null)
            //{

            //    //new { Id = 0, Name = _localizer["All"] },
            //    //new { Id = 1, Name = _localizer["Paid"] },
            //    //new { Id = 2, Name = _localizer["Not Paid"] },

            //    if (Paid==1)
            //    {
            //        patients = patients.Where(a => a.IsPaid==1).AsQueryable();
            //    }
            //    if (Paid == 2)
            //    {
            //        patients = patients.Where(a => a.IsPaid==0).AsQueryable();
            //    }

            //}

            if (StartDate!=null)
            {
                patients = patients.Where(a =>a.CreateDate >= StartDate);
            }
            if (EndDate!=null)
            {
                patients = patients.Where(a =>a.CreateDate <= EndDate);

            }

            var result = patients
                .ToList().ToDataSourceResult(request);
            return Json(result);
        }


        [HttpPost]
        public ActionResult MarkAsPaid(List<Guid> patientsId)
        {

            //foreach (var item in patientsId)
            //{
            //    PetOwner patient = cmsContext.PetOwner.Find(item);
            //    patient.IsPaid = 1;
            //    cmsContext.Entry(patient).Property(a=>a.IsPaid).IsModified=true;
            //    cmsContext.SaveChanges();
            //}

            return Json(_localizer["Success"]);
        }

    }
}
