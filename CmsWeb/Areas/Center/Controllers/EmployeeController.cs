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
using ServicesLibrary.PersonServices;
using System.Reflection;

namespace CmsWeb.Areas.Center.Controllers
{
    [Area("CenterAdmin")]
    public class EmployeeController : Controller
    {
        private readonly IStringLocalizer<CmsResources.Messages> _localizer;
        private readonly ILogger<EmployeeController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;

        private readonly IEmailSender _emailSender;
        private readonly IMedicalCenterService medicalCenterService;
        private readonly IPersonService personService ;

        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext cmsContext;

        public EmployeeController(
            IPersonService personService_,
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
            ILogger<EmployeeController> logger)
        {
            personService = personService_;
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

       

        public async Task<IActionResult> IndexEmployee()
        {

            ViewBag.DispalyName = _localizer["Center Employees"];
            ViewBag.PreviousActionDispalyName = _localizer["Employees"];


            ViewBag.PreviousAction = "IndexEmployee";

            return View("CenterAdmin/_Employee");
        }

        public ActionResult Read_Employee([DataSourceRequest] DataSourceRequest request, string preferredCulture = "en-US")
        {
            return Json(medicalCenterService.GetMyCenterEmployees(request, preferredCulture));
        }

        public async Task<IActionResult> Create_Employee()
        {
            ViewBag.DispalyName = _localizer["New Employee"];
            ViewBag.PreviousActionDispalyName = _localizer["Employees"];
            ViewBag.PreviousAction = "IndexEmployee";

            ViewBag.Password = PasswordUtil.GenerateRandomPassword();
            ViewBag.ErrorMessage = "";

            return View("CenterAdmin/_Employee_Create");
        }


        [HttpPost]
        public async Task<IActionResult> Create_Employee(Employee model)
        {


            ViewBag.DispalyName = _localizer["New Employee"];
            ViewBag.PreviousActionDispalyName = _localizer["Employees"];
            ViewBag.PreviousAction = "IndexEmployee";

            Guid guid = (Guid)_userService.GetMyCenterIdWeb();

            Guid docId=Guid.NewGuid();

            

            string res = personService.AddNewEmployee(model);


            if (res.IsNullOrEmpty())
            {
                return RedirectToAction("IndexEmployee");
            }
            else
            {
                ViewBag.ErrorMessage = res;

                return View("CenterAdmin/_Employee_Create", model);
            }

        }


        public async Task<IActionResult> Edit_Employee(Guid id)
        {

            ViewBag.DispalyName = _localizer["Edit Employee"];
            ViewBag.PreviousActionDispalyName = _localizer["Employees"];
            ViewBag.PreviousAction = "IndexEmployee";

            return View("CenterAdmin/_Employee_Edit", personService.GetEmployeeById(id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit_Employee(Employee model)
        {

            ViewBag.DispalyName = _localizer["Edit Employee"];
            ViewBag.PreviousActionDispalyName = _localizer["Employees"];
            ViewBag.PreviousAction = "IndexEmployee";

            Guid guid = (Guid)_userService.GetMyCenterIdWeb();

            if (model.ImageFile != null)
            {
                model.ImageName = FileHandler.UpdateImageFile(model.ImageName, model.ImageFile);
            }



            Employee centerTutor = cmsContext.Employee
    .Include(a => a.User)
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

            if (centerTutor.EmployeeRole != model.EmployeeRole)
            {
                await _userManager.RemoveFromRoleAsync(centerTutor.User, "reception");
                await _userManager.RemoveFromRoleAsync(centerTutor.User, "ordersmanagement");

                string empRole = "";

                if (model.EmployeeRole == 0)
                {
                    empRole = "reception";
                }
                else
                {
                    empRole = "ordersmanagement";
                }

                try
                {
                    await _roleManager.CreateAsync(new IdentityRole(empRole));
                }
                catch
                {
                }
                await _userManager.AddToRoleAsync(centerTutor.User, empRole);
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
            centerTutor.EmployeeRole = model.EmployeeRole;



            cmsContext.Employee.Attach(centerTutor);
            cmsContext.Entry(centerTutor).State = EntityState.Modified;

            cmsContext.SaveChanges();

            return RedirectToAction("IndexEmployee");

        }




        public async Task<IActionResult> Destroy_Employee([DataSourceRequest] DataSourceRequest request, Employee item)
        {
            {
                //foreach (var item in ContactInfos)
                {
                    item.SoftDeleteFromDb(); 
                    //item.SoftDelte();
                }
                return Json("Success");
            }
        }





    }

}

