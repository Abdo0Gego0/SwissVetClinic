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

namespace CmsWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AppointmentController : Controller
    {
        private readonly IStringLocalizer<CmsResources.Messages> _localizer;
        private readonly ILogger<AppointmentController> _logger;
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

        public AppointmentController(
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
            ILogger<AppointmentController> logger)
        {
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

        public async Task<IActionResult> GetPatient_Fitering(string text)
        {
            //return Json(cmsContext.BaseClinic.Include(a => a.BaseClinicTranslation)
            //    .Select(a => new { Id = a.Id, Name = a.BaseClinicTranslation[0].Name + " " + a.BaseClinicTranslation[1].Name }));




            if (!string.IsNullOrEmpty(text))
            {


                List<PetOwner> res1 = cmsContext.PetOwner.Where(a => a.MedicalCenterId== (Guid)_userService.GetMyCenterIdWeb()).ToList();

                List<PetOwner> res2 = res1.Where(a=>a.FullName.ToLower().Contains(text)).ToList();

                var res = res2
                .Select(
                    a => new
                    {
                        Id = a.Id,
                        Name = a.FullName
                    }
                    ).ToList();

                return Json(res.ToList());
            }
            else
            {
                var res3 = cmsContext.PetOwner.Where(a => a.MedicalCenterId == (Guid)_userService.GetMyCenterIdWeb()).Select(
    a => new
    {
        Id = a.Id,
        Name = a.FullName
    }
    ).ToList();
                return Json(res3.ToList());
            }





        }
        
        public async Task<IActionResult> GetClinics_Fitering(string text)
        {
            //return Json(cmsContext.BaseClinic.Include(a => a.BaseClinicTranslation)
            //    .Select(a => new { Id = a.Id, Name = a.BaseClinicTranslation[0].Name + " " + a.BaseClinicTranslation[1].Name }));




            if (!string.IsNullOrEmpty(text))
            {


                List<BaseClinicTranslation> res1 = cmsContext.BaseClinicTranslation.Where(a => a.Name.ToLower().Contains(text)).ToList();

                List<BaseClinic> res2 = cmsContext.BaseClinic
    .Include(a => a.BaseClinicTranslation)
    .Where(a => a.MedicalCenterId == (Guid)_userService.GetMyCenterIdWeb() && res1.Select(a => a.BaseClinicId).Contains(a.Id))

.ToList();

                var res = res2
                .Select(
                    a => new
                    {
                        Id = a.Id,
                        Name = a.BaseClinicTranslation[0].Name + " " + a.BaseClinicTranslation[1].Name
                    }
                    ).ToList();


                return Json(res.ToList());
            }
            else
            {
                List<BaseClinic> res3 = cmsContext.BaseClinic
    .Include(a => a.BaseClinicTranslation)
    .Where(a => a.MedicalCenterId == (Guid)_userService.GetMyCenterIdWeb())
.ToList();

                var res4 = res3
.Select(
    a => new
    {
        Id = a.Id,
        Name = a.BaseClinicTranslation[0].Name + " " + a.BaseClinicTranslation[1].Name
    }
    ).ToList();
                return Json(res4.ToList());
            }





        }

        public async Task<IActionResult> Timeline()
        {
            ViewBag.DispalyName = _localizer["Appointment Calendar"];
            ViewBag.PreviousActionDispalyName = _localizer["Timeline"];
            ViewBag.PreviousAction = "Timeline";

            return View("Admin/_CenterCalendar");
        }
        public ActionResult Timeline_Read([DataSourceRequest] DataSourceRequest request, Guid? ClinicId, string? view, string? date)
        {

            //List<Appointment> appointments = cmsContext.Appointment.ToList();
            //cmsContext.Appointment.RemoveRange(appointments);
            //cmsContext.SaveChanges();

            return Json(meetingService.GetAll( ClinicId, view, date, null).ToDataSourceResult(request));
        }

        public async Task<IActionResult> Timeline_Create([DataSourceRequest] DataSourceRequest request, AppointmentViewModel meeting)
        {

            return Json(_localizer["Cannnot edit calendar"]);

            Guid guid = (Guid)_userService.GetMyCenterIdWeb();

            meeting.MedicalCenterId = guid;
            //meeting.Description = meeting.Description_;
            //meeting.Title = meeting.Title_;
            //meeting.Start = medicalCenterService.ConvertToLocalTime(meeting.Start.ToLocalTime());
            //meeting.End = medicalCenterService.ConvertToLocalTime(meeting.End.ToLocalTime());



            //meeting.RecurrenceException = "No";
            //meeting.RecurrenceRule = "No";

            //meeting.Start = medicalCenterService.ConvertToLocalTime(meeting.Start);
            //meeting.End = medicalCenterService.ConvertToLocalTime(meeting.End);
            //meeting.Title_= meeting.Title;
            //meeting.Description_ = meeting.Description;
            //meeting.RecurrenceException = "";
            {
                meetingService.Insert(meeting, ModelState);
            }

            return Json(new[] { meeting }.ToDataSourceResult(request, ModelState));
        }


        public async Task<IActionResult> Timeline_Destroy([DataSourceRequest] DataSourceRequest request, AppointmentViewModel meeting)
        {
            return Json(_localizer["Cannnot edit calendar"]);


            //if (ModelState.IsValid)
            {
                meetingService.Delete(meeting, ModelState);
            }

            return Json(new[] { meeting }.ToDataSourceResult(request, ModelState));
        }


        public async Task<IActionResult> Timeline_Update([DataSourceRequest] DataSourceRequest request, AppointmentViewModel task)
        {
            return Json(_localizer["Cannnot edit calendar"]);



            //example custom validation:
            //if (task.Start.Hour < 8 || task.Start.Hour > 22)
            //{
            //    ModelState.AddModelError("start", "Start date must be in working hours (8h - 22h)");
            //}

            //if (ModelState.IsValid)
            {
                meetingService.Update(task, ModelState);
            }

            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }


        public async Task<IActionResult> ReadPatient([DataSourceRequest] DataSourceRequest request)
        {

            return Json(meetingService.GetAllPatient().ToDataSourceResult(request));
        }

        public async Task<IActionResult> ReadClinic([DataSourceRequest] DataSourceRequest request)
        {
            return Json(meetingService.GetAllClinic().ToDataSourceResult(request));
        }


    }

}

