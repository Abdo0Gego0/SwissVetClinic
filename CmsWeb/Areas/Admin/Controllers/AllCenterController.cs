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

namespace CmsWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AllCenterController : Controller
    {
        private readonly IStringLocalizer<CmsResources.Messages> _localizer;
        private readonly ILogger<AllCenterController> _logger;
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

        public AllCenterController(
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
            ILogger<AllCenterController> logger)
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
            ViewBag.DispalyName = _localizer["Centers List"];
            ViewBag.PreviousActionDispalyName = _localizer["Centers"];
            ViewBag.PreviousAction = "Index";

            //List<MedicalCenter> md = cmsContext.MedicalCenter
            //    .Include(a => a.MedicalCenterTranslation)
            //    .Include(a => a.OpeningHours)
            //    .Include(a => a.ContactInfo)
            //    .Include(b => b.Address).ThenInclude(a => a.AddressTranslation)
            //    .ToList();

            //cmsContext.MedicalCenter.RemoveRange(md);
            //cmsContext.SaveChanges();

            return View("Admin/_CentersIndex");
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request, string preferredCulture = "en-US")
        {
            var result = cmsContext.MedicalCenter
                .Include(a => a.MedicalCenterTranslation)
                .Where(a=>!a.Deleted)
                .ToList().ToDataSourceResult(request);
            return Json(result);
        }

        public IActionResult CreateCenter()
        {
            ViewBag.DispalyName = _localizer["New Center"];
            ViewBag.PreviousActionDispalyName = _localizer["Centers"];

            ViewBag.PreviousAction = "Index";
            return View("Admin/_CreateCenter");
        }

        [AcceptVerbs("Post")]
        public IActionResult CreateCenter(MedicalCenter model)
        {
            ViewBag.DispalyName = _localizer["New Center"];
            ViewBag.PreviousActionDispalyName = _localizer["Centers"];
            ViewBag.PreviousAction = "Index";

            medicalCenterService.AddNewMedicalCenter(model);
            return RedirectToAction("Index");
        }


        public IActionResult EditCenter(Guid id)
        {
            ViewBag.DispalyName = _localizer["Edit Center"];
            ViewBag.PreviousActionDispalyName = _localizer["Centers"];
            ViewBag.PreviousAction = "Index";
            MedicalCenter model= cmsContext.MedicalCenter
    .Include(a => a.MedicalCenterTranslation)
    .Include(a => a.Address).ThenInclude(a => a.AddressTranslation)
    .FirstOrDefault(a => a.Id ==id);

            return View("Admin/_EditCenter",model);
        }

        [HttpPost]
        public IActionResult EditCenter(MedicalCenter model)
        {
            ViewBag.DispalyName = _localizer["Edit Center"];
            ViewBag.PreviousActionDispalyName = _localizer["Centers"];
            ViewBag.PreviousAction = "Index";

            if (model.ImageFile != null)
            {
                model.ImageName = FileHandler.UpdateProfileImage(model.ImageFile, model.ImageName);
            }

            cmsContext.MedicalCenterTranslation.Attach(model.MedicalCenterTranslation[0]);
            cmsContext.Entry(model.MedicalCenterTranslation[0]).State = EntityState.Modified;
            cmsContext.SaveChanges();

            cmsContext.MedicalCenterTranslation.Attach(model.MedicalCenterTranslation[1]);
            cmsContext.Entry(model.MedicalCenterTranslation[1]).State = EntityState.Modified;
            cmsContext.SaveChanges();

            cmsContext.Address.Attach(model.Address);
            cmsContext.Entry(model.Address).State = EntityState.Modified;
            cmsContext.SaveChanges();

            cmsContext.AddressTranslation.Attach(model.Address.AddressTranslation[0]);
            cmsContext.Entry(model.Address.AddressTranslation[0]).State = EntityState.Modified;
            cmsContext.SaveChanges();

            cmsContext.AddressTranslation.Attach(model.Address.AddressTranslation[1]);
            cmsContext.Entry(model.Address.AddressTranslation[1]).State = EntityState.Modified;
            cmsContext.SaveChanges();


            cmsContext.MedicalCenter.Attach(model);
            cmsContext.Entry(model).State = EntityState.Modified;
            cmsContext.SaveChanges(true);

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Delete_Clinic(Guid id)
        {
            MedicalCenter center = cmsContext.MedicalCenter.Find(id);

            center.SoftDelete();

            return RedirectToAction("Index");

        }


        public async Task<IActionResult> IndexAdmins()
        {


            ViewBag.DispalyName = _localizer["Admins List"];
            ViewBag.PreviousActionDispalyName = _localizer["Centers Admins"];

            ViewBag.PreviousAction = "IndexAdmins";

            //List<CenterAdmin> md = cmsContext.CenterAdmin
            //    .Include(a => a.Address).ThenInclude(a => a.AddressTranslation)
            //    .Include(a => a.User)
            //    .ToList();

            //cmsContext.CenterAdmin.RemoveRange(md);
            //cmsContext.SaveChanges();

            return View("Admin/_IndexAdmins");
        }

        public ActionResult Read_CenterAdmins([DataSourceRequest] DataSourceRequest request, string preferredCulture = "en-US")
        {
            var result = cmsContext.CenterAdmin
                .Include(a=>a.Address).ThenInclude(a=>a.AddressTranslation)
                .Include(a=>a.User)
                .Where(a=>!a.IsDeleted)
                .ToList().ToDataSourceResult(request);
            return Json(result);
        }

        public IActionResult CreateCenterAdmin()
        {

            ViewBag.DispalyName = _localizer["New Admin"];
            ViewBag.PreviousActionDispalyName = _localizer["Centers Admins"];

            ViewBag.PreviousAction = "IndexAdmins";

            return View("Admin/_CreateCenterAdmin");
        }

        [AcceptVerbs("Post")]
        public IActionResult CreateCenterAdmin(CenterAdmin model)
        {

            ViewBag.DispalyName = _localizer["New Admin"];
            ViewBag.PreviousActionDispalyName = _localizer["Centers Admins"];
            ViewBag.PreviousAction = "IndexAdmins";

            medicalCenterService.AddNewCenterAdminWithTransaction(model);
            return RedirectToAction("IndexAdmins");
        }


        public IActionResult EditCenterAdmin(Guid id)
        {



            ViewBag.DispalyName = _localizer["Edit Admin"];
            ViewBag.PreviousActionDispalyName = _localizer["Centers Admins"];

            ViewBag.PreviousAction = "IndexAdmins";

            CenterAdmin model=cmsContext.CenterAdmin
                 .Include(a => a.Address).ThenInclude(a => a.AddressTranslation)
                .Include(a => a.User)
                .FirstOrDefault(a=>a.Id== id);

            model.PersonEmail = model.User.Email;
            model.PersonPhone = model.User.PhoneNumber;
            model.PersonUserName = model.User.UserName;

            return View("Admin/_EditCenterAdmin",model);
        }

        [AcceptVerbs("Post")]
        public async Task<IActionResult> EditCenterAdmin(CenterAdmin model)
        {
            ViewBag.DispalyName = _localizer["Edit Admin"];
            ViewBag.PreviousActionDispalyName = _localizer["Centers Admins"];
            ViewBag.PreviousAction = "Index";

            if (model.ImageFile != null)
            {
                model.ImageName = FileHandler.UpdateImageFile(model.ImageName, model.ImageFile);
            }



            CenterAdmin centerTutor = cmsContext.CenterAdmin
    .Include(a => a.User)
                .Include(a => a.Address).ThenInclude(a => a.AddressTranslation)
    .FirstOrDefault(a => a.Id == model.Id);

            if (centerTutor.User.Email != model.PersonEmail)
            {
                centerTutor.User.Email = model.PersonEmail;
                var resul1 = await _userManager.UpdateAsync(centerTutor.User);

                if (!resul1.Succeeded)
                {
                    string msg = "";

                    foreach (var item in resul1.Errors)
                    {
                        msg += item.Description + " ";
                    }

                    ViewBag.ErrorMessage = _localizer["MakeSureThatEmailisUnique"];

                    return View(model);
                }

                centerTutor.User.EmailConfirmed = true;
            }

            if (centerTutor.User.UserName != model.PersonUserName)
            {
                centerTutor.User.UserName = model.PersonUserName;
                var resul2 = await _userManager.UpdateAsync(centerTutor.User);

                if (!resul2.Succeeded)
                {
                    string msg = "";
                    foreach (var item in resul2.Errors)
                    {
                        msg += item.Description + " ";
                    }
                    ViewBag.ErrorMessage = _localizer["MakeSureThatUserisUnique"];

                    return View(model);
                }
                centerTutor.User.EmailConfirmed = true;
            }

            if (centerTutor.User.PhoneNumber != model.PersonPhone)
            {
                centerTutor.User.PhoneNumber = model.PersonPhone;
                var resul3 = await _userManager.UpdateAsync(centerTutor.User);
                if (!resul3.Succeeded)
                {
                    string msg = "";
                    foreach (var item in resul3.Errors)
                    {
                        msg += item.Description + " ";
                    }

                    ViewBag.ErrorMessage = "يرجى التأكد أن الهاتف";

                    return View(model);
                }
                centerTutor.User.PhoneNumberConfirmed = true;
            }


            if (model.ImageFile != null)
            {
                string uniqueFileName = FileHandler.UpdateProfileImage(model.ImageFile, model.ImageName);
                centerTutor.ImageName = uniqueFileName;
            }

            centerTutor.FirstName = model.FirstName;
            centerTutor.MiddleName = model.MiddleName;
            centerTutor.LastName = model.LastName;
            centerTutor.NationalCardId = model.NationalCardId;
            centerTutor.JobCardNumber = model.JobCardNumber;
            centerTutor.PassportNumber = model.PassportNumber;
            centerTutor.Nationality = model.Nationality;

            centerTutor.PersonEmail = model.PersonEmail;
            centerTutor.PersonPhone = model.PersonPhone;
            centerTutor.PersonUserName = model.PersonUserName;
            centerTutor.PersonUserName = model.PersonUserName;
            centerTutor.MedicalCenterId = model.MedicalCenterId;


            cmsContext.CenterAdmin.Attach(centerTutor);
            cmsContext.Entry(centerTutor).State = EntityState.Modified;

            cmsContext.SaveChanges();




            return RedirectToAction("IndexAdmins");
        }


        [AcceptVerbs("Post")]
        public IActionResult Destroy_CenterAdmins(CenterAdmin model)
        {
            model.IsDeleted = true;
            
            cmsContext.CenterAdmin.Attach(model);
            cmsContext.Entry(model).State=EntityState.Modified;
            cmsContext.SaveChanges();

            return RedirectToAction("IndexAdmins");
        }

    }
}
