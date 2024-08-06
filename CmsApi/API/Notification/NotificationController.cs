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
using ServicesLibrary.PasswordsAndClaims;
using ServicesLibrary.UserServices;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CmsApi.API.Notification
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : ControllerBase
    {
		private readonly IStringLocalizer<Messages> _localizer;


		private readonly ILogger<NotificationController> _logger;

		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IUserStore<IdentityUser> _userStore;
		private readonly IUserEmailStore<IdentityUser> _emailStore;

		private readonly IEmailSender _emailSender;


		private readonly IConfiguration _config;
		private readonly IUserService _userService;
		private readonly ApplicationDbContext cmsContext;

		public NotificationController(
			IStringLocalizer<Messages> localizer,
			IConfiguration config,
			IUserService userService,
			ApplicationDbContext _cMDbContext,

			UserManager<IdentityUser> userManager,
			IUserStore<IdentityUser> userStore,
			SignInManager<IdentityUser> signInManager,
			IEmailSender emailSender,

			ILogger<NotificationController> logger)
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
        /// This API is used to get user notification
		/// PageSize: number of rows to retieve in a single request
		/// PageIndex: used for paging
		/// Auth: bearer token
        /// </summary>
        /// <returns>list of user notifications </returns>

        [HttpGet("get-my-notifications"), Authorize]
		public ActionResult<object> GetMyNotifications(
            int PageSize=50, int PageIndex=1

            )
        {
			Guid userId =(Guid) _userService.GetMyId();

			var PersonNotification = cmsContext.PersonNotification
				.Where(a=>a.PersonId== userId)
                .Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .Select(a=> new
				{
                    CreateDate=a.CreateDate,
                    ClinicName=a.ClinicName(),
                    CenterName = a.CenterName(),
					Body=a.Body,
                    Title = a.Title,
					IsRead=a.IsRead,
				})
				.ToList();
			return new ObjectResult(new { status = StatusCodes.Status200OK, data = PersonNotification, message = "" });
		}

        /// <summary>
        /// This API is used to set the notification as read after the user open it.
        /// Auth: bearer token
        /// </summary>
        /// <returns>list of user notifications </returns>

        [HttpGet("mark-as-read"), Authorize]
        public ActionResult<object> MarkAsRead(Guid id)
        {
			PersonNotification personNotification=cmsContext.PersonNotification.Find(id);
			personNotification.IsRead = true;
            cmsContext.PersonNotification.Attach(personNotification);
			cmsContext.Entry(personNotification).Property(a => a.IsRead).IsModified = true;
			cmsContext.SaveChanges();
            return new ObjectResult(new { status = StatusCodes.Status200OK, data = "Ok", message = "" });
        }



    }
}