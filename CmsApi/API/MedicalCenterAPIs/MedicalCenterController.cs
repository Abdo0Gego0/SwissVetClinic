
using CmsDataAccess;
using CmsDataAccess.DbModels;
using CmsDataAccess.Models;
using CmsResources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using ServicesLibrary.UserServices;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CmsApi.API.MedicalCenterAPIs
{
    [ApiController]
    [Route("[controller]")]
    public class MedicalCenterController : ControllerBase
    {
		private readonly IStringLocalizer<Messages> _localizer;


		private readonly ILogger<MedicalCenterController> _logger;

		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IUserStore<IdentityUser> _userStore;
		private readonly IUserEmailStore<IdentityUser> _emailStore;

		private readonly IEmailSender _emailSender;


		private readonly IConfiguration _config;
		private readonly IUserService _userService;
		private readonly ApplicationDbContext cmsContext;

		public MedicalCenterController(
			IStringLocalizer<Messages> localizer,
			IConfiguration config,
			IUserService userService,
			ApplicationDbContext _cMDbContext,

			UserManager<IdentityUser> userManager,
			IUserStore<IdentityUser> userStore,
			SignInManager<IdentityUser> signInManager,
			IEmailSender emailSender,


			ILogger<MedicalCenterController> logger)
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

		}


        /// <summary>
        /// Get General Info of the center
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-center-info")]
        public ActionResult<object> GetCenterInfo()
        {

			string lang=_userService.GetMyLanguage();

			MedicalCenter res = cmsContext.MedicalCenter
				.Include(a => a.Address).ThenInclude(a=>a.AddressTranslation.Where(a=>a.LangCode==lang))
				.Include(a=>a.MedicalCenterTranslation.Where(a=>a.LangCode== lang))
				.Include(a=>a.OpeningHours)
                .FirstOrDefault();
            

            return new ObjectResult(new { status = StatusCodes.Status200OK,
                data = new { Name = res.MedicalCenterTranslation[0].Name, Address = res.Address.AddressTranslation[0],Image=res.ImageFullPath },
                message = "" });

        }

        /// <summary>
        /// Get terms and conditions
        /// </summary>
        /// <returns></returns>
        [HttpGet("terms-and-conditions")]
        public async Task<IActionResult> TermsAndCinditions()
        {
            string lang = _userService.GetMyLanguage();

            TermsAndConditions res = cmsContext.TermsAndConditions.Include(a => a.TermsAndConditionsTranslation.Where(a => a.LangCode == lang)).FirstOrDefault();

            return new ObjectResult(new
            {
                status = StatusCodes.Status200OK,
                data =
                res.TermsAndConditionsTranslation[0].TermsAndConditionsText
                ,
                message = ""
            });
        }


        /// <summary>
        /// Get About us
        /// </summary>
        /// <returns></returns>

        [HttpGet("about-us")]
        public async Task<IActionResult> AboutUs()
        {
            string lang = _userService.GetMyLanguage();

            AboutUs res = cmsContext.AboutUs.Include(a => a.AboutUsTranslation.Where(a => a.LangCode == lang)).FirstOrDefault();

            return new ObjectResult(new
            {
                status = StatusCodes.Status200OK,
                data =
                res.AboutUsTranslation[0].AboutUsText
                ,
                message = ""
            });
        }



        /// <summary>
        /// Get Banners
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-center-banners")]
        public ActionResult<object> GetCenterBanners()
        {

            string lang = _userService.GetMyLanguage();

            var res = cmsContext.Banner.Where(a=>a.StartDate>=DateTime.Now && a.EndDate<=DateTime.Now).ToList()
                .Select(a=> new { Text=a.BannerText, Image=a.ImageFullPath, Start=a.StartDate, End=a.EndDate});

            return new ObjectResult(new { status = StatusCodes.Status200OK, data = res, message = "" });

        }

        /// <summary>
        /// Get clinics of the center
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-center-clinics")]
        public ActionResult<object> GetCenterClinics()
        {

            string lang = _userService.GetMyLanguage().ToLower();

            var res = cmsContext.BaseClinic.Include(a => a.BaseClinicTranslation.Where(a=>a.LangCode==lang))
                .Where(a => !a.IsDeleted)

                .ToList()
                .Select(a => new 
                {
                    Id=a.Id,
                    Name = a.BaseClinicTranslation[0].Name,
                    Description = a.BaseClinicTranslation[0].Description,
                    Speicality=a.ClinicSpecialty(lang),
                    Image=a.ImageFullPath
                }
                );

            return new ObjectResult(new { status = StatusCodes.Status200OK, data = res, message = "" });

        }


        /// <summary>
        /// Get doctors of the center
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-center-doctors")]
        public ActionResult<object> GetCenterDoctors()
        {

            string lang = _userService.GetMyLanguage().ToLower();

            var res = cmsContext.Doctor

                .Include(a => a.DoctorTranslation.Where(a => a.LangCode.ToLower() == lang))
                .Include(a => a.Certificate).ThenInclude(a=>a.CertificateTranslation.Where(a=>a.LangCode.ToLower() == lang))
                .Include(a => a.DoctorSpeciality).ThenInclude(a=>a.DoctorSpecialityTranslation.Where(a=>a.LangCode.ToLower()==lang))
                .Where(a=>!a.IsDeleted)
                .ToList()

                .Select(a => new {
                    Id = a.Id,
                    Image=a.ImageFullPath,
                    Name = a.DoctorTranslation[0].Name, 
                    Description = a.DoctorTranslation[0].Description,
                    Certificate= (a.Certificate.Count()==0)?null:a.Certificate.Select(a=> new { Name = a.CertificateTranslation[0].Name,Image=a.ImageFullPath }),
                    Speicality = (a.DoctorSpeciality.Count() == 0) ? null : a.DoctorSpeciality.Select(a=> new { Name = a.DoctorSpecialityTranslation[0].Description })
                });

            return new ObjectResult(new { status = StatusCodes.Status200OK, data = res, message = "" });

        }



        /// <summary>
        /// Get Center services
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-center-services")]
        public ActionResult<object> GetCenterServices()
        {
            string lang = _userService.GetMyLanguage().ToLower();

            var res = cmsContext.CenterServices
                .Include(a => a.CenterServicesTranslation.Where(a => a.LangCode.ToLower() == lang))
                .Where(a => !a.IsDeleted)
                .ToList()
                .Select(a => new {
                    Id = a.Id,
                    Name = a.CenterServicesTranslation.FirstOrDefault()?.Name ?? "No translation available",
                    Cost = a.Price,
                    Image = a.ImageFullPath
                });

            return new ObjectResult(new { status = StatusCodes.Status200OK, data = res, message = "" });
        }


    }
}