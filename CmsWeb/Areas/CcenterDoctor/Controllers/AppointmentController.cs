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
using Ical.Net.DataTypes;
using Ical.Net;
using System.Threading.Tasks;
using Ical.Net.CalendarComponents;
using System.Web;
using NoificationManager.MobileModels;
using System.Reflection;

namespace CmsWeb.Areas.CcenterDoctor.Controllers
{
    [Area("Doctor")]
    public class AppointmentController : Controller
    {


        private readonly INotificationService notiService;

        private readonly IHubContext<MyHub> _hubContext;

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
            INotificationService notiService_,

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
            ILogger<AppointmentController> logger)
        {

            notiService = notiService_;

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



        public async Task<IActionResult> GetCenterMedicineList()
        {
            // Fetch the data from your service or database
            var centerMedicineList = cmsContext.CenterMedicineList.Where(a => !a.IsDeleted).ToList().Select(c => new { CenterMedicineListId = c.Id, Text = c.Name });
            return Json(centerMedicineList);
        }

        public async Task<IActionResult> GetCenterMedicineUnits(Guid centerMedicineListId)
        {
            // Fetch the data based on the selected CenterMedicineListId
            var centerMedicineUnits = cmsContext.CenterMedicineUnit
                                            .Where(u => u.CenterMedicineListId == centerMedicineListId)
                                            .Select(u => new { CenterMedicineUnitId = u.Id, Text = u.SmallestUnit });
            return Json(centerMedicineUnits);
        }




        #region Appointment

        public async Task<IActionResult> GetPatient_Fitering(string text)
        {
            //return Json(cmsContext.BaseClinic.Include(a => a.BaseClinicTranslation)
            //    .Select(a => new { Id = a.Id, Name = a.BaseClinicTranslation[0].Name + " " + a.BaseClinicTranslation[1].Name }));




            if (!string.IsNullOrEmpty(text))
            {


                List<PetOwner> res1 = cmsContext.PetOwner.Where(a => a.MedicalCenterId == (Guid)_userService.GetMyCenterIdWeb()).ToList();

                List<PetOwner> res2 = res1.Where(a => a.FullName.ToLower().Contains(text)).ToList();

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


        public JsonResult Cascading_GetPets(Guid? ownerId, Guid? petId)
        {




            using (var northwind = cmsContext)
            {
                var products = northwind.Pet.AsQueryable();


                if (ownerId != null)
                {
                    products = products.Where(p => p.PetOwnerId == ownerId);
                }

                return Json(products.Select(p => new { Id = p.Id, Name = p.PetName }).ToList());
            }
        }

        public async Task<IActionResult> Timeline()
        {
            ViewBag.DispalyName = _localizer["Appointment Calendar"];
            ViewBag.PreviousActionDispalyName = _localizer["Timeline"];
            ViewBag.PreviousAction = "Timeline";

            return View("CcenterDoctor/_CenterCalendar");
        }
       
        public ActionResult Timeline_Read([DataSourceRequest] DataSourceRequest request, int? TypeFilter, Guid? ClinicId, Guid? PetOwnerId, string? view, string? date)
        {

            //List<Appointment> appointments = cmsContext.Appointment.ToList();
            //cmsContext.Appointment.RemoveRange(appointments);
            //cmsContext.SaveChanges();

            Guid guid = (Guid)_userService.GetMyCenterIdWeb();
            return Json(meetingService.GetAll(TypeFilter, guid, ClinicId, view, date, PetOwnerId).ToDataSourceResult(request));
        }

        public async Task<IActionResult> Timeline_Create([DataSourceRequest] DataSourceRequest request, AppointmentViewModel meeting)
        {
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
            //if (ModelState.IsValid)
            {
                meetingService.Delete(meeting, ModelState);
            }

            return Json(new[] { meeting }.ToDataSourceResult(request, ModelState));
        }

        public async Task<IActionResult> Timeline_Update([DataSourceRequest] DataSourceRequest request, AppointmentViewModel task)
        {
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

            Guid guid = (Guid)_userService.GetMyCenterIdWeb();


            return Json(meetingService.GetAllPatient(guid).ToDataSourceResult(request));
        }
       
        public async Task<IActionResult> ReadServices([DataSourceRequest] DataSourceRequest request, bool? asSelectList)
        {

            Guid guid = (Guid)_userService.GetMyCenterIdWeb();


            {
                return Json(cmsContext.CenterServices
                    .Include(a => a.CenterServicesTranslation)
                    .Where(a => !a.IsDeleted).Select(a => new { Id = a.Id, Name = a.CenterServicesTranslation[0].Name + " " + a.CenterServicesTranslation[1].Name }).ToList());
            }

            if (asSelectList != null)
            {
                return Json(cmsContext.CenterServices
                    .Include(a => a.CenterServicesTranslation)
                    .Where(a => !a.IsDeleted).Select(a => new { Id = a.Id, Name = a.CenterServicesTranslation[0].Name + " " + a.CenterServicesTranslation[1].Name }).ToList());
            }

            return Json(cmsContext.CenterServices.Where(a => !a.IsDeleted).ToList().ToDataSourceResult(request));
        }

        public async Task<IActionResult> ReadClinic([DataSourceRequest] DataSourceRequest request, bool? asSelectList)
        {
            Guid guid = (Guid)_userService.GetMyCenterIdWeb();


            if (asSelectList != null)
            {
                return Json(meetingService.GetAllClinic(guid).Select(a => new { Id = a.Value, Name = a.Text }).ToList());

            }



            return Json(meetingService.GetAllClinic(guid).ToDataSourceResult(request));
        }


        #endregion


        #region Visits

        public async Task<IActionResult> Home()
        {
            return RedirectToAction("Index", "Home",new {area="Doctor"});
        }

        public async Task<IActionResult> StartVisit(string appId)
        {
            ViewBag.DispalyName = _localizer["Examination"];
            ViewBag.PreviousActionDispalyName = _localizer["Home"];

            ViewBag.PreviousAction = "Home";

            if (cmsContext.PatientVisit.Any(a=>a.AppointmentId==Guid.Parse(appId) && a.IsFinished))
            {
                RedirectToAction("Index",new {Controller="Home", area = "Doctor" });
            }
            else
            {
                PatientVisit patientVisit1= cmsContext.PatientVisit
                    .Include(a=>a.VisitMeasurement)
                    .Include(a=>a.VisitTreatment)
                    .FirstOrDefault(a => a.AppointmentId == Guid.Parse(appId));



                if (patientVisit1!=null)
                {
                    patientVisit1.Diagnosis = patientVisit1.Diagnosis.IsNullOrEmpty() ? "" : HttpUtility.HtmlDecode(patientVisit1.Diagnosis);

                    patientVisit1.VisistDate = medicalCenterService.ConvertToLocalTime(DateTime.Now);

                    return View("CcenterDoctor/_StartVisit", patientVisit1);
                }
            }


            Appointment appointment = cmsContext.Appointment.Find(Guid.Parse(appId));

            


            //notiService.SendNotification(new NotificationModel { Body= "Body",Title= "Title",
            //    Token= "ctVXZXMDSyqf-eiwEhZDk5:APA91bHZbtO4jGwmatXyw64CvNk1zmGGUSpUpQaqgSUmU_OrBTvWkALN8Wdebrl_enwspecshF-_JRToKcvgdXPrB-6sSdoRbZNVDDQunlmpI_jmQ1qc4UsTihbDGPK4qpsmE23mVMoY"
            //});


            PatientVisit patientVisit = new PatientVisit
            {
                AppointmentId=appointment.Id,
                CenterServicesId=appointment.CenterServicesId,
                BaseClinicId=appointment.BaseClinicId,
                MedicalCenterId=appointment.MedicalCenterId,
                PetOwnerId = appointment.PetOwnerId,
                PetId=appointment.PetId,
                DoctorId = (Guid)_userService.GetMyDoctorId(),
                IsFirstVisit = appointment.IsFirstVisit,
                VisistDate= medicalCenterService.ConvertToLocalTime(DateTime.Now),
                VisitMeasurement=new List<VisitMeasurement> (),
            };

            cmsContext.PatientVisit.Add(patientVisit);
            cmsContext.SaveChanges();




            //todo: send notification to mobile user

            string Title = $"إشعار بدء جلسة علاج";
            string Body = $"لقد بدأت الجلسة في العيادة {appointment.ClinicName()}";
            string fcm_token = cmsContext.Person.Find(appointment.PetOwnerId).fcm_token;
            //notiService.SendNotification(new NotificationModel { Body = Body, Title = Title, Token = fcm_token });

            //PersonNotification personNotification = new PersonNotification
            //{
            //    BaseClinicId = appointment.BaseClinicId,
            //    Body = Body,
            //    CreateDate = medicalCenterService.ConvertToLocalTime(DateTime.Now),
            //    MedicalCenterId = appointment.MedicalCenterId,
            //    Title = Title,
            //    PersonId = appointment.PetOwnerId
            //};

            //cmsContext.PersonNotification.Add(personNotification);
            //cmsContext.SaveChanges();






            return View("CcenterDoctor/_StartVisit", patientVisit);

        }

        [HttpPost]
        public async Task<IActionResult> StartVisit(PatientVisit visit, List<VisitMedicineViewModel> VisitMedicineViewModel)
        {

            visit.IsFinished = true;
            visit.Duration= (int)( medicalCenterService.ConvertToLocalTime(DateTime.Now)-visit.VisistDate).TotalMinutes;
            visit.Diagnosis = visit.Diagnosis;
            visit.BloodPressure = visit.BloodPressure;
            visit.Glucose = visit.Glucose;
            visit.Height = visit.Height;
            visit.Weight = visit.Weight;
            visit.Notes = visit.Notes;
            visit.DoctorId=(Guid)_userService.GetMyDoctorId();
            visit.Cost = visit.Cost;

            visit.Vaccinated = visit.Vaccinated;
            visit.BodyCondition = visit.BodyCondition;
            visit.Behavior = visit.Behavior;
            visit.Prognosis = visit.Prognosis;
            visit.Temperature = visit.Temperature;
            visit.Weight = visit.Weight;

            visit.Abdomen = visit.Abdomen;
            visit.SKIN = visit.SKIN;
            visit.Ears = visit.Ears;
            visit.HearBeat = visit.HearBeat;
            visit.MuscSkel = visit.MuscSkel;

            visit.VisitMedicine = new List<VisitMedicine>();



            double medicinceCost = 0;
            foreach (var item in VisitMedicineViewModel)
            {
                VisitMedicine temp = new VisitMedicine { 
                BaseClinicId=visit.BaseClinicId,
                DoctorId=visit.DoctorId,
                PatientVisitId=visit.Id,
                Duration=item.Duration,
                MedicineName=cmsContext.CenterMedicineList.Find(item.CenterMedicineListId).Name,
                MedicineUnit=cmsContext.CenterMedicineUnit.Find(item.CenterMedicineUnitId).SmallestUnit,
                PetOwnerId=visit.PetOwnerId,
                Cost= cmsContext.CenterMedicineUnit.Find(item.CenterMedicineUnitId).PricePerUnit*item.Duration

                };
                cmsContext.VisitMedicine.Add(temp);
                cmsContext.SaveChanges();
                medicinceCost += temp.Cost;
            }

            
            cmsContext.PatientVisit.Attach(visit);
            cmsContext.Entry(visit).Property(a=>a.IsFinished).IsModified=true;
            cmsContext.Entry(visit).Property(a=>a.Duration).IsModified=true;
            cmsContext.Entry(visit).Property(a=>a.Diagnosis).IsModified=true;
            cmsContext.Entry(visit).Property(a=>a.BloodPressure).IsModified=true;
            cmsContext.Entry(visit).Property(a=>a.Glucose).IsModified=true;
            cmsContext.Entry(visit).Property(a=>a.Height).IsModified=true;
            cmsContext.Entry(visit).Property(a=>a.Weight).IsModified=true;
            cmsContext.Entry(visit).Property(a=>a.Notes).IsModified=true;
            cmsContext.Entry(visit).Property(a=>a.DoctorId).IsModified=true;
            cmsContext.Entry(visit).Property(a=>a.Cost).IsModified=true;


            cmsContext.Entry(visit).Property(a => a.Vaccinated).IsModified = true;
            cmsContext.Entry(visit).Property(a => a.BodyCondition).IsModified = true;
            cmsContext.Entry(visit).Property(a => a.Behavior).IsModified = true;
            cmsContext.Entry(visit).Property(a => a.Prognosis).IsModified = true;
            cmsContext.Entry(visit).Property(a => a.Abdomen).IsModified = true;
            cmsContext.Entry(visit).Property(a => a.SKIN).IsModified = true;
            cmsContext.Entry(visit).Property(a => a.Ears).IsModified = true;
            cmsContext.Entry(visit).Property(a => a.HearBeat).IsModified = true;
            cmsContext.Entry(visit).Property(a => a.MuscSkel).IsModified = true;


            cmsContext.SaveChanges();

            Appointment appointment = cmsContext.Appointment.Find(visit.AppointmentId);
            cmsContext.Appointment.Attach(appointment);
            appointment.IsFinished=true;
            appointment.IsStarted=false;
            appointment.Cost=visit.Cost;
            cmsContext.Entry(appointment).Property(a => a.IsFinished).IsModified = true;
            cmsContext.Entry(appointment).Property(a => a.IsStarted).IsModified = true;
            cmsContext.Entry(appointment).Property(a => a.Cost).IsModified = true;
            cmsContext.SaveChanges();

            await _hubContext.Clients.All.SendAsync("RefreshClinicStatus_", _userService.GetMyCenterIdWeb().ToString());




            VisistBill visistBill = new VisistBill
            {
                Number=cmsContext.VisistBill.Count()+1,
                BaseClinicId = visit.BaseClinicId,
                CenterServicesId = appointment.CenterServicesId,
                DoctorId = visit.DoctorId,
                MedicnieCost = medicinceCost,
                PatientVisitId = visit.Id,
                PetOwnerId = visit.PetOwnerId,
                ServiceCost = cmsContext.CenterServices.Find(appointment.CenterServicesId).Price,
                CreateDate=medicalCenterService.ConvertToLocalTime(DateTime.Now)
            };

            cmsContext.VisistBill.Add(visistBill);
            cmsContext.SaveChanges();

            await _hubContext.Clients.All.SendAsync("RefreshBillStatus_", _userService.GetMyCenterIdWeb().ToString());



            string Title = $"إشعار نهاية جلسة علاج";
            string Body = $"لقد انتهت الجلسة في العيادة {appointment.ClinicName()}";
            string fcm_token = cmsContext.Person.Find(appointment.PetOwnerId).fcm_token;
            //notiService.SendNotification(new NotificationModel { Body = Body, Title = Title, Token = fcm_token });

            //PersonNotification personNotification = new PersonNotification
            //{
            //    BaseClinicId = appointment.BaseClinicId,
            //    Body = Body,
            //    CreateDate = medicalCenterService.ConvertToLocalTime(DateTime.Now),
            //    MedicalCenterId = appointment.MedicalCenterId,
            //    Title = Title,
            //    PersonId = appointment.PetOwnerId
            //};

            //cmsContext.PersonNotification.Add(personNotification);
            //cmsContext.SaveChanges();




            //return Content("<script language='javascript' type='text/javascript'>    function closeMe() {window.close();} closeMe(); </script>");
            return View("Close");

        }
        
        public async Task<IActionResult> Open_Visit(Guid id)
        {

            ViewBag.DispalyName = _localizer["Old Examination"];
            ViewBag.PreviousActionDispalyName = _localizer["Home"];

            ViewBag.PreviousAction = "Home";


            PatientVisit patientVisit = cmsContext.PatientVisit
                .Include(a=>a.VisitTreatment)
                .Include(a=>a.VisitMeasurement)
                .FirstOrDefault(a=>a.Id==id);

            patientVisit.Diagnosis = HttpUtility.HtmlDecode(patientVisit.Diagnosis);

            return View("CcenterDoctor/_OldVisit", patientVisit);
        }



        public async Task<IActionResult> VisitMeasurement_Read([DataSourceRequest] DataSourceRequest request, Guid VisitId)
        {
            var result = cmsContext.VisitMeasurement
            .Where(a => a.PatientVisitId == VisitId)
            .ToList().ToDataSourceResult(request);
            return Json(result);
        }
        
        public async Task<IActionResult> Create_VisitMeasurement([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<VisitMeasurement> ContactInfos,
            Guid PetOwnerId,
            Guid VisitId,
            Guid BaseClinicId
            
            )
        {

            //if (ModelState.IsValid)
            {
                foreach (var item in ContactInfos)
                {
                    cmsContext.VisitMeasurement.Add(
                        new VisitMeasurement
                        {
                            PetOwnerId = PetOwnerId,
                            Value=item.Value,
                            Type=item.Type,
                            PatientVisitId=VisitId,
                            BaseClinicId=BaseClinicId
                        }
                        );
                }
                cmsContext.SaveChanges();
                return Json(_localizer["Success"]);
            }
        }
        
        
        
        
        
        [AcceptVerbs("Post")]
        public async Task<IActionResult> Update_VisitMeasurement([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<VisitMeasurement> clinicSpecialties)
        {
            {
                foreach (var updatedItem in clinicSpecialties)
                {
                    cmsContext.VisitMeasurement.Attach(updatedItem);
                    cmsContext.Entry(updatedItem).State = EntityState.Modified;

                    cmsContext.SaveChanges();
                }
                return Json(_localizer["Success"]);
            }
        }
        public async Task<IActionResult> Destroy_VisitMeasurement([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<VisitMeasurement> ContactInfos)
        {
            {
                foreach (var item in ContactInfos)
                {
                    cmsContext.VisitMeasurement.Remove(item);
                    cmsContext.SaveChanges();
                }
                return Json(_localizer["Success"]);
            }
        }


        public async Task<IActionResult> VisitTreatment_Read([DataSourceRequest] DataSourceRequest request, Guid VisitId)
        {
            var result = cmsContext.VisitTreatment
            .Where(a => a.PatientVisitId == VisitId)
            .ToList().ToDataSourceResult(request);
            return Json(result);
        }
        public async Task<IActionResult> Create_VisitTreatment([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<VisitTreatment> ContactInfos,
            Guid PetOwnerId,
            Guid VisitId,
            Guid BaseClinicId

            )
        {

            //if (ModelState.IsValid)
            {
                foreach (var item in ContactInfos)
                {
                    cmsContext.VisitTreatment.Add(
                        new VisitTreatment
                        {
                            PetOwnerId = PetOwnerId,
                            Name = item.Name,
                            Note = item.Note,
                            PatientVisitId = VisitId,
                            BaseClinicId = BaseClinicId
                        }
                        );
                }
                cmsContext.SaveChanges();
                return Json(_localizer["Success"]);
            }
        }
        [AcceptVerbs("Post")]
        public async Task<IActionResult> Update_VisitTreatment([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<VisitTreatment> clinicSpecialties)
        {
            {
                foreach (var updatedItem in clinicSpecialties)
                {
                    cmsContext.VisitTreatment.Attach(updatedItem);
                    cmsContext.Entry(updatedItem).State = EntityState.Modified;

                    cmsContext.SaveChanges();
                }
                return Json(_localizer["Success"]);
            }
        }
        public async Task<IActionResult> Destroy_VisitTreatment([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<VisitTreatment> ContactInfos)
        {
            {
                foreach (var item in ContactInfos)
                {
                    cmsContext.VisitTreatment.Remove(item);
                    cmsContext.SaveChanges();
                }
                return Json(_localizer["Success"]);
            }
        }



        #endregion


    }


    public class MultiSelected
    {

        public string Text { get; set; }
        public string Value { get; set; }

        public bool Selected { get; set; }

        public string? RecurrenceID { get; set; }= Guid.NewGuid().ToString().Replace("-", DateTime.Now.Second.ToString()) ;

    }



}

