using CmsApi.API.Account;
using CmsResources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using CmsDataAccess;

using CmsDataAccess.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ServicesLibrary.UserServices;
using CmsDataAccess.DbModels;
using ServicesLibrary.MedicalCenterServices;
using CmsWeb.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CmsApi.API.Appointments
{
    [ApiController]
    [Route("[controller]")]
    public class AppointmentController : Controller
    {
        private readonly IHubContext<MyHub> _hubContext;

        private readonly IStringLocalizer<Messages> _localizer;


        private readonly IMedicalCenterService medicalCenterService;


        private readonly ILogger<AppointmentController> _logger;

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;

        private readonly IEmailSender _emailSender;


        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext cmsContext;

        public AppointmentController(
            IHubContext<MyHub> hubContext,

            IMedicalCenterService medicalCenterService_,
            IStringLocalizer<Messages> localizer,
            IConfiguration config,
            IUserService userService,
            ApplicationDbContext _cMDbContext,

            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,


            ILogger<AppointmentController> logger)
        {
            _hubContext = hubContext;

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

        }


        [HttpGet("get-my-appointment"),Authorize]
        public async Task<ActionResult> GetMyAppointment(DateTime? StartDate,
            DateTime? EndDate,
            Guid? ClinicId,
            int PageSize=50, int PageIndex=1
            )
        {



            Guid userId=(Guid)_userService.GetMyId();


            List<Appointment> appointments = cmsContext.Appointment.Where(a=>a.PetOwnerId==userId)
                .Skip((PageIndex-1)*PageSize)
                .Take(PageSize)
                .ToList();


            if(StartDate!=null) {

                appointments = appointments.Where(a=>a.Start>=StartDate).ToList();
            }

            if (EndDate != null)
            {

                appointments = appointments.Where(a => a.End <= EndDate).ToList();
            }
            if (ClinicId != null)
            {

                appointments = appointments.Where(a => a.BaseClinicId == ClinicId).ToList();
            }


            List<object> res = new List<object>();

            string lang = _userService.GetMyLanguage();


            foreach (var item in appointments)
            {
                res.Add(new 
                    {
                    Id= item.Id,
                    StartDate = medicalCenterService.ConvertToLocalTime(item.Start),
                    EndDate = medicalCenterService.ConvertToLocalTime(item.End),
                    ClinicName=cmsContext.BaseClinicTranslation.FirstOrDefault(a=>a.LangCode== lang && a.BaseClinicId== item.BaseClinicId).Name,
                    Service= cmsContext.CenterServicesTranslation.FirstOrDefault(a => a.LangCode == lang && a.CenterServicesId == item.CenterServicesId).Name,
                    PetName = cmsContext.Pet.Find(item.PetId).PetName,
                    UpComming=(item.Start>DateTime.Now)?true:false,
                    IsStarted=item.IsStarted,
                    IsFinished = item.IsFinished,
                });

            }

            return new ObjectResult(new { status = StatusCodes.Status200OK, data = res, message = "Appointments list." });

        }




        [HttpPost("make-appointment"), Authorize]
        public async Task<ActionResult> MakeAppointment(DateTime StartDate,
    
        Guid ClinicId,
        Guid ServiceId,
        Guid PetId,
        string Title="...",
        string Description="..."
    
        )
            {


            if (ClinicId==null || ClinicId==Guid.Empty)
            {
                return new ObjectResult(new { status = StatusCodes.Status400BadRequest, data = "", message = "ClinicId is required" });

            }

            if (ServiceId == null || ServiceId == Guid.Empty)
            {
                            return new ObjectResult(new { status = StatusCodes.Status400BadRequest, data = "", message = "ServiceId is required" });

            }

            if (PetId == null || PetId == Guid.Empty)
            {
                            return new ObjectResult(new { status = StatusCodes.Status400BadRequest, data = "", message = "PetId is required" });

            }

            Guid userId = (Guid)_userService.GetMyId();

                DateTime d =DateTime.Now;

                DateTime dayStart = new DateTime(d.Year,d.Month,d.Day,0,0,0);
                DateTime dayEnd = new DateTime(d.Year,d.Month,d.Day,23,59,59);

                int maxAppPerDay = (int)cmsContext.BookingPolicy.FirstOrDefault().AllowedBookingNumberPerday;

                if (cmsContext.Appointment.Where(a=>a.PetOwnerId== userId && (a.Start>=dayStart && a.Start<=dayEnd)).Count()>= maxAppPerDay)
                {
                    return new ObjectResult(new { status = StatusCodes.Status403Forbidden, data = "", message = $"Max Appointments per day: {maxAppPerDay}" });
                }



                Appointment model = new Appointment
                {
                    Start = StartDate,
                    End=StartDate.AddMinutes(15),
                    IsFinished=false,
                    IsStarted=false,
                    IsFirstVisit=!cmsContext.Appointment.Any(a=>a.PetId==PetId),
                    PetOwnerId=userId,
                    IsFromMobile=true,
                    PetId=PetId,
                    MedicalCenterId=cmsContext.MedicalCenter.FirstOrDefault().Id,
                    Timezone= "Asia/Dubai",
                    StartTimezone= "Asia/Dubai",
                    EndTimezone= "Asia/Dubai",
                    CenterServicesId= ServiceId,
                    BaseClinicId=ClinicId,
                    Title= Title,
                    Description= Description,

                };
           
                cmsContext.Appointment.Add(model);
                cmsContext.SaveChanges();
            await _hubContext.Clients.All.SendAsync("RefreshClinicStatus_", _userService.GetMyCenterIdWeb().ToString());


            return new ObjectResult(new { status = StatusCodes.Status200OK, data = "", message = "Added Successfully" });

            }



        [HttpPost("delete-appointment"), Authorize]
        public async Task<ActionResult> DeleteAppointment(Guid id)
        {

            Appointment model = cmsContext.Appointment.Find(id);

            if (model == null)
            {
                return new ObjectResult(new { status = StatusCodes.Status404NotFound, data = "", message = "Appointment was not found" });

            }

            if (model.IsStarted || model.IsFinished)
            {
                return new ObjectResult(new { status = StatusCodes.Status403Forbidden, data = "", message = "Appointment cannot be deleted, it's either started or finished" });
            }

            cmsContext.Appointment.Remove(model);   
            cmsContext.SaveChanges();
            await _hubContext.Clients.All.SendAsync("RefreshClinicStatus_", _userService.GetMyCenterIdWeb().ToString());

            return new ObjectResult(new { status = StatusCodes.Status200OK, data = "", message = "Deleted Successfully" });

        }

        [HttpPut("edit-appointment"), Authorize]
        public async Task<ActionResult> EditAppointment(
    Guid id,
    DateTime? StartDate,
    Guid? ClinicId,
    Guid? ServiceId,
    Guid? PetId,
    string? Title,
    string? Description
)
        {
            Guid userId = (Guid)_userService.GetMyId();

            var appointment = await cmsContext.Appointment.FindAsync(id);

            if (appointment == null)
            {
                return new ObjectResult(new { status = StatusCodes.Status404NotFound, data = "", message = "Appointment not found" });
            }

            if (appointment.PetOwnerId != userId)
            {
                return new ObjectResult(new { status = StatusCodes.Status403Forbidden, data = "", message = "You are not authorized to edit this appointment" });
            }

            if (appointment.IsStarted || appointment.IsFinished)
            {
                return new ObjectResult(new { status = StatusCodes.Status403Forbidden, data = "", message = "Appointment cannot be edited, it's either started or finished" });
            }

            if (StartDate.HasValue)
            {
                // Check if the new start date is in the past
                if (StartDate.Value < DateTime.Now)
                {
                    return new ObjectResult(new { status = StatusCodes.Status400BadRequest, data = "", message = "Cannot set appointment date in the past" });
                }

                appointment.Start = StartDate.Value;
                appointment.End = StartDate.Value.AddMinutes(15); // Assuming 15-minute appointments
            }

            if (ClinicId.HasValue && ClinicId != Guid.Empty)
            {
                if (!await cmsContext.BaseClinic.AnyAsync(c => c.Id == ClinicId))
                {
                    return new ObjectResult(new { status = StatusCodes.Status400BadRequest, data = "", message = "Invalid ClinicId" });
                }
                appointment.BaseClinicId = ClinicId.Value;
            }

            if (ServiceId.HasValue && ServiceId != Guid.Empty)
            {
                if (!await cmsContext.CenterServices.AnyAsync(s => s.Id == ServiceId))
                {
                    return new ObjectResult(new { status = StatusCodes.Status400BadRequest, data = "", message = "Invalid ServiceId" });
                }
                appointment.CenterServicesId = ServiceId.Value;
            }

            if (PetId.HasValue && PetId != Guid.Empty)
            {
                if (!await cmsContext.Pet.AnyAsync(p => p.Id == PetId && p.PetOwnerId == userId))
                {
                    return new ObjectResult(new { status = StatusCodes.Status400BadRequest, data = "", message = "Invalid PetId or pet does not belong to you" });
                }
                appointment.PetId = PetId.Value;
            }

            if (!string.IsNullOrEmpty(Title))
            {
                appointment.Title = Title;
            }

            if (!string.IsNullOrEmpty(Description))
            {
                appointment.Description = Description;
            }

            await cmsContext.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("RefreshClinicStatus_", _userService.GetMyCenterIdWeb().ToString());

            return new ObjectResult(new { status = StatusCodes.Status200OK, data = "", message = "Appointment updated successfully" });
        }






    }
}
