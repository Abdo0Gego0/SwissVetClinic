using CmsDataAccess;
using CmsDataAccess.DbModels;
using CmsDataAccess.MobileViewModels.AuthModels;
using CmsDataAccess.Models;
using CmsDataAccess.Utils.FilesUtils;
using CmsResources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using ServicesLibrary.PasswordsAndClaims;
using ServicesLibrary.UserServices;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CmsApi.API.MedicalVisits
{
    [ApiController]
    [Route("[controller]")]
    public class MedicalVisitController : ControllerBase
    {
		private readonly string APIURL;


		private readonly IStringLocalizer<Messages> _localizer;


		private readonly ILogger<MedicalVisitController> _logger;

		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IUserStore<IdentityUser> _userStore;
		private readonly IUserEmailStore<IdentityUser> _emailStore;

		private readonly IEmailSender _emailSender;


		private readonly IConfiguration _config;
		private readonly IUserService _userService;
		private readonly ApplicationDbContext cmsContext;

		public MedicalVisitController(
			IStringLocalizer<Messages> localizer,
			IConfiguration config,
			IUserService userService,
			ApplicationDbContext _cMDbContext,

			UserManager<IdentityUser> userManager,
			IUserStore<IdentityUser> userStore,
			SignInManager<IdentityUser> signInManager,
			IEmailSender emailSender,

			ILogger<MedicalVisitController> logger)
        {
			_localizer = localizer;

			_logger = logger;
            _config = config;
            _userService = userService;
            cmsContext = _cMDbContext;

			_userManager = userManager;
			_userStore = userStore;
			_signInManager = signInManager;

			_emailSender = emailSender;

            APIURL = cmsContext.MySystemConfiguration.FirstOrDefault().ApiUrl+ "/visit";


        }

        /// <summary>
        /// This API  is used to get all patient visits
        /// </summary>
        /// <param name="StartDate"> start time to search</param>
        /// <param name="EndDate"> end time to search</param>
        /// <param name="ClinicId">clinic id</param>
        /// <param name="PageSize">for pagination</param>
        /// <param name="PageIndex">for pagination</param>
        /// <returns></returns>
        [HttpGet("get-my-visits"), Authorize]
        public async Task<ActionResult> GetMyVisits(DateTime? StartDate,
           DateTime? EndDate,
           Guid? ClinicId,
           Guid? PetId,
           int PageSize = 50, int PageIndex = 1
           )
        {

            Guid userId = (Guid)_userService.GetMyId();
            List<PatientVisit> appointments = new List<PatientVisit>();

            if (PetId==null)
            {
                 appointments = cmsContext.PatientVisit.Where(a => a.PetOwnerId == userId)
    .Skip((PageIndex - 1) * PageSize)
    .Take(PageSize)
    .ToList();
            }
            else
            {
                 appointments = cmsContext.PatientVisit.Where(a => a.PetOwnerId == userId && a.PetId==(Guid)PetId)
.Skip((PageIndex - 1) * PageSize)
.Take(PageSize)
.ToList();
            }




            if (StartDate != null)
            {

                appointments = appointments.Where(a => a.VisistDate >= StartDate).ToList();
            }

            if (EndDate != null)
            {

                appointments = appointments.Where(a => a.VisistDate <= EndDate).ToList();
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
                    Date =item.VisistDate,
                    Doctor=item.DoctorName(lang),
                    ClinicName =item.ClinicName(lang),
                    Diagnosis =HttpUtility.HtmlDecode(item.Diagnosis),
                    Video= APIURL+item.VisitVideo
                });

            }

            return new ObjectResult(new { status = StatusCodes.Status200OK, data = res, message = "Visits list." });

        }


        /// <summary>
        /// This API is used to get the details of the a single visit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get-visit-by-id"), Authorize]
        public async Task<ActionResult> GetVisitById(Guid id)
        {

            string lang = _userService.GetMyLanguage();

            PatientVisit item =cmsContext.PatientVisit
                .Include(a=>a.VisitMeasurement)
                .Include(a=>a.VisitTreatment)
                .FirstOrDefault(a=>a.Id==id);

            item.Diagnosis = HttpUtility.HtmlDecode(item.Diagnosis);

            item.VisitVideo = APIURL + item.VisitVideo;


            var res = new
            {
                Id = item.Id,
                Date = item.VisistDate,
                DoctorId=item.DoctorId,
                Doctor = item.DoctorName(lang),
                ClinicId=item.BaseClinicId,
                ClinicName = item.ClinicName(lang),
                Diagnosis = HttpUtility.HtmlDecode(item.Diagnosis),
                Video = APIURL + item.VisitVideo,
                PetId = item.PetId,
                PetName =item.Pet.PetName,
                CenterServicesId = item.CenterServicesId,
                Service = item.ServiceName(),
                Measurement=item.VisitMeasurement.Select(a=> new { Type=a.Type, Value=a.Value}),
                Treatment = item.VisitTreatment.Select(a=> new { Name = a.Name, Note = a.Note}),

            };

            return new ObjectResult(new { status = StatusCodes.Status200OK, data = res, message = "Visits details." });

        }


        /// <summary>
        /// This api is used to get only diagnosis of the patient
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="ClinicId"></param>
        /// <param name="PetId"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        [HttpGet("get-pet-diagnosis"), Authorize]
        public async Task<ActionResult> GetMyDiagnosis(DateTime? StartDate,
           DateTime? EndDate,
           Guid? ClinicId,
           Guid? PetId,
           int PageSize = 50, int PageIndex = 1
          
           )
        {

            Guid userId = (Guid)_userService.GetMyId();


            List<PatientVisit> appointments = cmsContext.PatientVisit.Where(a => a.PetOwnerId == userId).ToList();

            if (PetId==null)
            {

                appointments = cmsContext.PatientVisit.Where(a => a.PetId==(Guid)PetId).ToList();
            }


            if (StartDate != null)
            {

                appointments = appointments.Where(a => a.VisistDate >= StartDate).ToList();
            }

            if (EndDate != null)
            {

                appointments = appointments.Where(a => a.VisistDate <= EndDate).ToList();
            }
            if (ClinicId != null)
            {

                appointments = appointments.Where(a => a.BaseClinicId == ClinicId).ToList();
            }


             appointments = appointments.Skip((PageIndex - 1)*PageSize).Take(PageSize).ToList();


            List<object> res = new List<object>();

            string lang = _userService.GetMyLanguage();


            foreach (var item in appointments)
            {
                res.Add(new
                {
                    VisitId=item.Id,
                    Date = item.VisistDate,
                    Doctor = item.DoctorName(),
                    PetId=item.PetId,
                    PetName=item.Pet.PetName,
                    ClinicId=item.BaseClinicId,
                    ClinicName = item.ClinicName(),
                    Diagnosis = HttpUtility.HtmlDecode(item.Diagnosis),
                });;

            }

            return new ObjectResult(new { status = StatusCodes.Status200OK, data = res, message = "Diagnosis list." });

        }


        /// <summary>
        /// This API is used to get the treament that is described in a visit
        /// </summary>
        /// <param name="id">id of the pet</param>
        /// <returns></returns>

        [HttpGet("get-pet-treament-in-visit"), Authorize]
        public async Task<ActionResult> GetMyTreamentInVisit(Guid id)
        {
            PatientVisit v = cmsContext.PatientVisit.Include(a => a.VisitTreatment)
                .FirstOrDefault(a => a.Id == id);
            return new ObjectResult(new { status = StatusCodes.Status200OK, data = v.VisitTreatment.Select(a=> new { Name=a.Name,Note=a.Note}), message = "Treament list." });
        }


        /// <summary>
        /// This API is used to get the therapy plan of the pet
        /// </summary>
        /// <param name="PetId"></param>
        /// <returns></returns>
        [HttpGet("get-pet-therapy-plan"), Authorize]
        public async Task<ActionResult> GetPetTherapyPlan(Guid PetId)
        {
            return new ObjectResult(new { status = StatusCodes.Status200OK, data = cmsContext.PatientTherapyGoals.Where(a => a.PetId == PetId), message = "Therapy plan" });

        }



        ///// <summary>
        ///// This API is used to get only videos of the patient
        ///// </summary>
        ///// <param name="StartDate"></param>
        ///// <param name="EndDate"></param>
        ///// <param name="ClinicId"></param>
        ///// <param name="PageSize"></param>
        ///// <param name="PageIndex"></param>
        ///// <returns></returns>

        //[HttpGet("get-my-videos"), Authorize]
        //public async Task<ActionResult> GetMyVideos(DateTime? StartDate,
        //   DateTime? EndDate,
        //   Guid? ClinicId,
        //   int PageSize = 50, int PageIndex = 1
        //   )
        //{

        //    Guid userId = (Guid)_userService.GetMyId();


        //    List<PatientVisit> appointments = cmsContext.PatientVisit.Where(a => a.PetOwnerId == userId   && a.VisitVideo!= null)
        //        .Skip((PageIndex - 1) * PageSize)
        //        .Take(PageSize)
        //        .ToList();


        //    if (StartDate != null)
        //    {

        //        appointments = appointments.Where(a => a.VisistDate >= StartDate).ToList();
        //    }

        //    if (EndDate != null)
        //    {

        //        appointments = appointments.Where(a => a.VisistDate <= EndDate).ToList();
        //    }
        //    if (ClinicId != null)
        //    {

        //        appointments = appointments.Where(a => a.BaseClinicId == ClinicId).ToList();
        //    }


        //    List<object> res = new List<object>();

        //    string lang = _userService.GetMyLanguage();


        //    foreach (var item in appointments)
        //    {
        //        res.Add(new
        //        {
        //            VisitId=item.Id,
        //            Date = item.VisistDate,
        //            Doctor = item.DoctorName(),
        //            ClinicName = item.ClinicName(),
        //            Video = APIURL+item.VisitVideo,
        //        });

        //    }

        //    return new ObjectResult(new { status = StatusCodes.Status200OK, data = res, message = "Videos list." });

        //}




        /*

        /// <summary>
        /// This API is used to get the medical history of the patient
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-my-medical-history"), Authorize]
        public async Task<ActionResult> GetMyMedicalHistory()
        {
            Guid userId = (Guid)_userService.GetMyId();

            return new ObjectResult(new { status = StatusCodes.Status200OK, data = cmsContext.PatientMedicalHistory.Where(a => a.PetOwnerId == userId), 
                message = "Medical history."
            });
        }


        /// <summary>
        /// This API is used to get the medicine history of the patient
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-my-medicine-history"), Authorize]
        public async Task<ActionResult> GetMyMedicineHistory()
        {
            Guid userId = (Guid)_userService.GetMyId();

            return new ObjectResult(new { status = StatusCodes.Status200OK, data = cmsContext.PatientMedicineHistory.Where(a => a.PetOwnerId == userId),
                message = "Medicine history."
            });
        }


        /// <summary>
        /// This API is used to get the Patient Allergy
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-my-allergy"), Authorize]
        public async Task<ActionResult> GetMyAllergy()
        {
            Guid userId = (Guid)_userService.GetMyId();

            return new ObjectResult(new { status = StatusCodes.Status200OK, data = cmsContext.PatientAllergy.Where(a => a.PetOwnerId == userId),
                message = "Allergy"
            });
        }


        /// <summary>
        /// This API is used to get the Patient Family History
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-my-family-history"), Authorize]
        public async Task<ActionResult> GetMyFamilyHistory()
        {
            Guid userId = (Guid)_userService.GetMyId();

            return new ObjectResult(new { status = StatusCodes.Status200OK, data = cmsContext.PatientFamilyHistory.Where(a => a.PetOwnerId == userId),
                message = "Family history."

            });
        }


        /// <summary>
        /// This API is used to get the Patient Surgical History
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-my-surgical-history"), Authorize]
        public async Task<ActionResult> GetMySurgicalHistory()
        {
            Guid userId = (Guid)_userService.GetMyId();

            return new ObjectResult(new { status = StatusCodes.Status200OK, data = cmsContext.PatientSurgicalHistory.Where(a => a.PetOwnerId == userId),
                message = "Surgical history."

            });
        }
        */


    }
}