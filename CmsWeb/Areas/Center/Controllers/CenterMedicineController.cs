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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Stripe.Climate;

namespace CmsWeb.Areas.Center.Controllers
{
    [Area("CenterAdmin")]
    public class CenterMedicineController : Controller
    {
        private readonly IStringLocalizer<CmsResources.Messages> _localizer;
        private readonly ILogger<CenterMedicineController> _logger;
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

        public CenterMedicineController(
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
            ILogger<CenterMedicineController> logger)
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


   






        #region Services
        public async Task<IActionResult> Index()
        {

            ViewBag.DispalyName = _localizer["Medicines List"];
            ViewBag.PreviousActionDispalyName = _localizer["Medicines"] ;
            ViewBag.PreviousAction = "Index";

            return View("CenterAdmin/_CenterMedicines");
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request, string preferredCulture = "en-US")
        {
            Guid guid = (Guid)_userService.GetMyCenterIdWeb();
            var result = cmsContext.CenterMedicineList
                .Where(a => a.MedicalCenterId == guid && !a.IsDeleted)
                .ToList().ToDataSourceResult(request);
            return Json(result);
        }

      

        [HttpPost]
        public async Task<IActionResult> Create_Medicine(
            string Name,
  
            List<CenterMedicineUnit> CenterMedicineUnit)
        {

            ViewBag.DispalyName = _localizer["Medicines List"];
            ViewBag.PreviousActionDispalyName = _localizer["Medicines"];
            ViewBag.PreviousAction = "Index";

            CenterMedicineList model = new CenterMedicineList();
            Guid guid = (Guid)_userService.GetMyCenterIdWeb();

            model.Id = Guid.Empty;
            model.Name = Name;
            model.MedicalCenterId=guid;

            model.CenterMedicineUnit = new List<CenterMedicineUnit>();
            model.CenterMedicineUnit = CenterMedicineUnit;

            cmsContext.CenterMedicineList.Add(model);
            cmsContext.SaveChanges();

            return RedirectToAction("Index");

        }




        [HttpPost]
        public async Task<IActionResult> Edit_Medicine(
          Guid Id__,
      string? Name_

          )
        {

            ViewBag.DispalyName = _localizer["Medicines List"];
            ViewBag.PreviousActionDispalyName = _localizer["Medicines"];
            ViewBag.PreviousAction = "Index";

            CenterMedicineList model = cmsContext.CenterMedicineList.Find(Id__);

            model.Name = Name_;

            cmsContext.CenterMedicineList.Attach(model);
            cmsContext.Entry(model).Property(a => a.Name).IsModified = true;
            cmsContext.SaveChanges();

            return RedirectToAction("Index");


        }


        public async Task<IActionResult> Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<CenterMedicineList> task)
        {
            foreach (var item in task)
            {
                item.DeleteFromDb();
            }
            return Json(_localizer["Success"]);
        }




        public ActionResult Read_CenterMedicineUnit([DataSourceRequest] DataSourceRequest request, Guid? id, string preferredCulture = "en-US")
        {
            var result = cmsContext.CenterMedicineUnit
                .Where(a => a.CenterMedicineListId == (Guid)id && !a.IsDeleted)
                .ToList().ToDataSourceResult(request);
            return Json(result);
        }

        public async Task<IActionResult> Create_CenterMedicineUnit([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<CenterMedicineUnit> ContactInfos, Guid id)
        {
            Guid guid = (Guid)_userService.GetMyCenterIdWeb();
            foreach (var item in ContactInfos)
            {
                cmsContext.CenterMedicineUnit.Add(
                    new CenterMedicineUnit
                    {
                       
                        CenterMedicineListId = id,
                        PricePerDose = item.PricePerDose,
                        SmallestDose = item.SmallestDose,
                    }
                    );
            }
            cmsContext.SaveChanges();
            return Json(_localizer["Success"]);
        }

        [AcceptVerbs("Post")]
        public async Task<IActionResult> Update_CenterMedicineUnit([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<CenterMedicineUnit> clinicSpecialties)
        {
            {
                foreach (var updatedItem in clinicSpecialties)
                {
                    cmsContext.CenterMedicineUnit.Attach(updatedItem);
                    cmsContext.Entry(updatedItem).State = EntityState.Modified;
                    cmsContext.SaveChanges();
                }
                return Json(_localizer["Success"]);
            }
        }

        public async Task<IActionResult> Destroy_CenterMedicineUnit([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<CenterMedicineUnit> ContactInfos)
        {
            {
                foreach (var item in ContactInfos)
                {
                    item.DeleteFromDb();
                }
                return Json(_localizer["Success"]);
            }
        }





        #endregion





    }

}

