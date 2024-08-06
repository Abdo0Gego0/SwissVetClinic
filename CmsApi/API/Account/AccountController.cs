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
using ServicesLibrary.MedicalCenterServices;
using ServicesLibrary.PasswordsAndClaims;
using ServicesLibrary.UserServices;
using System.Linq;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CmsApi.API.Account
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IMedicalCenterService medicalCenterService;

        private readonly IStringLocalizer<Messages> _localizer;

        private RoleManager<IdentityRole> _roleManager;

        private readonly ILogger<AccountController> _logger;

		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IUserStore<IdentityUser> _userStore;
		private readonly IUserEmailStore<IdentityUser> _emailStore;

		private readonly IEmailSender _emailSender;


		private readonly IConfiguration _config;
		private readonly IUserService _userService;
		private readonly ApplicationDbContext cmsContext;

		public AccountController(
            IMedicalCenterService medicalCenterService_,

            RoleManager<IdentityRole> roleManager,
                        IStringLocalizer<Messages> localizer,
			IConfiguration config,
			IUserService userService,
			ApplicationDbContext _cMDbContext,

			UserManager<IdentityUser> userManager,
			IUserStore<IdentityUser> userStore,
			SignInManager<IdentityUser> signInManager,
			IEmailSender emailSender,

			ILogger<AccountController> logger)
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
            _roleManager= roleManager;


        }

		/// <summary>
		/// This API is used to test if the site is UP
		/// </summary>
		/// <returns></returns>
		[HttpGet("ok")]
		public ActionResult<object> ok()
		{
            return new ObjectResult(new { status = StatusCodes.Status200OK, data ="Test language message: "+ _localizer["UnauthorizedMessage"].Value, message = "CMS is up" });
		}


        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string returnUrl = Url.Content("~/");

            var user = new IdentityUser
            {
                Email = request.Email,
                PhoneNumber = request.Telephone,
                UserName = request.UserName,
                PhoneNumberConfirmed = true,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                try
                {
                    await _roleManager.CreateAsync(new IdentityRole("patient"));
                }
                catch
                {
                    // Handle the error
                }

                await _userManager.AddToRoleAsync(user, "patient");

                int gn = 0;

                try
                {
                    gn = cmsContext.PetOwner.Where(a => a.MedicalCenterId == (Guid)_userService.GetMyCenterIdWeb()).Count() + 1;
                }
                catch { }

                PetOwner patient = new PetOwner
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    MiddleName = request.MiddleName,
                    MedicalCenterId = cmsContext.MedicalCenter.FirstOrDefault().Id,
                    User = user,
                    AccountVerfied = !cmsContext.MySystemConfiguration.FirstOrDefault().UseEmailVerfication,
                    GeneralNumber = gn,
                    CreateDate = medicalCenterService.ConvertToLocalTime(DateTime.Now),
                };

                cmsContext.PetOwner.Add(patient);
                cmsContext.SaveChanges();

                if (!patient.AccountVerfied)
                {
                    string token = new PasswordUtil().CreateRandomTokenN();

                    cmsContext.AccountVerificationToken.Add(new AccountVerificationToken { username = user.UserName, code = token });
                    cmsContext.SaveChanges();

                    await _emailSender.SendEmailAsync(request.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{cmsContext.MySystemConfiguration.FirstOrDefault().ApiUrl + "Auth/LogIn/verify?uname=" + user.UserName + "&code=" + token}'>clicking here</a>.");

                    return new ObjectResult(new { status = StatusCodes.Status200OK, data = request, message = "Registered Successfully, Check your e-mail to verify your account" });
                }

                return new ObjectResult(new { status = StatusCodes.Status200OK, data = request, message = "Registered Successfully" });
            }
            else
            {
                string msg = string.Join(" ", result.Errors.Select(e => e.Description));
                return new ObjectResult(new { status = StatusCodes.Status400BadRequest, data = request, message = msg });
            }
        }




        /// <summary>
        /// This API is used to login
        /// </summary>
        /// <returns>Success: Token, Failure: error message</returns>

        [HttpPost("login")]
		public async Task<ActionResult<string>> Login(LogInModel request)
		{
			var user = await _userManager.FindByEmailAsync(request.UserName);
			if (user == null)
			{
				user = await _userManager.FindByNameAsync(request.UserName);
			}

			if (user != null)
			{
				var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, lockoutOnFailure: false);
				if (result.Succeeded)
				{
					Person patient = await cmsContext.Person.Include(a=>a.User).FirstOrDefaultAsync(p => p.User == user); // Retrieve the associated Patient object

                    if (patient==null)
                    {
                        return new ObjectResult(new { status = StatusCodes.Status401Unauthorized, data = "", message = "This account was deleted" });
                    }

                    if (patient.IsDeleted)
					{
						return new ObjectResult(new { status = StatusCodes.Status401Unauthorized, data = "", message = "This account was deleted" });
					}


                    if (patient.Status<0)
                    {
                        return new ObjectResult(new { status = StatusCodes.Status401Unauthorized, data = "", message = "Your account is suspendend contact the SWISS clinic support" });
                    }

                    if (!patient.AccountVerfied)
                    {
                        return new ObjectResult(new { status = StatusCodes.Status401Unauthorized, data = "", message = "Your account is not verified, please check your e-mail" });
                    }


                    if (request.fcm_token!= null)
					{
						patient.fcm_token= request.fcm_token;
						cmsContext.Attach(patient);
						cmsContext.SaveChanges();
					}

                    if (request.LangCode != null)
                    {
                        patient.PreferredLanguage = request.LangCode;
                        cmsContext.Attach(patient);
                        cmsContext.SaveChanges();
                    }

                    return new ObjectResult(new { status = StatusCodes.Status200OK,
                        Token= new PasswordUtil(_userManager).CreateTokenForUser(patient),
                        UserInfo= _userService.GetMyInfo(patient.Id),
                        message = "Token Generated and Data retrieved" });
				}
				else
				{
					return new ObjectResult(new { status = StatusCodes.Status401Unauthorized, data = "", message = "Login info are not correct" });

				}
			}
			else
			{
				return new ObjectResult(new { status = StatusCodes.Status401Unauthorized, data = "", message = "Login info are not correct" });

			}


		}


        /// <summary>
        /// This API is used to get user info
		/// Auth: bearer token
        /// </summary>
        /// <returns>User information such as user name fullname etc..</returns>

        [HttpGet("get-my-info"), Authorize]
		public ActionResult<object> GetMyInfo()
		{
			var userName = _userService.GetMyInfo();
			return new ObjectResult(new { status = StatusCodes.Status200OK, data = userName, message = "" });

		}





        //[HttpGet("get-my-details"), Authorize]
        //public ActionResult<object> GetMyDetails()
        //{
        //          Guid id = (Guid)_userService.GetMyId();
        //	Person person=cmsContext.Person.Include(a=>a.User).FirstOrDefault(a=>a.Id==id);
        //	return new ObjectResult(new { status = StatusCodes.Status200OK, data = _userService.GetMyDetails(), message = "" });
        //}

        /// <summary>
        /// This API is used to change password
        /// Auth: bearer token
        /// </summary>
        /// <returns>Message</returns>
        [HttpPost("change-my-password"), Authorize]
		public async Task<ActionResult> ChangeMyPassword(string OldPassword, string NewPassword)
		{
			Guid id=(Guid)_userService.GetMyId();
			IdentityUser identityUser = cmsContext.Person.Include(a => a.User).FirstOrDefault(a=>a.Id==id).User;

			var user = await _userManager.FindByIdAsync(identityUser.Id);
			if (user == null)
			{
				return new ObjectResult(new { status = StatusCodes.Status200OK, data ="", message = "User Not Found" });
			}

			var changePasswordResult = await _userManager.ChangePasswordAsync(user, OldPassword, NewPassword);
			if (!changePasswordResult.Succeeded)

			{
				string msg = "";
				foreach (var error in changePasswordResult.Errors)
				{
					msg += error.Description;
				}
				return new ObjectResult(new { status = StatusCodes.Status200OK, data ="", message = msg });

			}

			return new ObjectResult(new { status = StatusCodes.Status200OK, data ="", message = "Ok" });

		}

        /// <summary>
        /// This API is used to update user profile
        /// Auth: bearer token
        /// </summary>
        /// <returns>Message</returns>

        [HttpPost("update-my-profile"), Authorize]
		public async Task<ActionResult> UpdateMyProfile(UserViewModel request)
		{

			// name + image

			Guid id = (Guid)_userService.GetMyId();

			Person person = cmsContext.Person.Include(a=>a.User).FirstOrDefault(a => a.Id == id);

			person.FirstName= request.FirstName;
			person.MiddleName = request.MiddleName;
			person.LastName=request.LastName;
			

			cmsContext.Person.Attach(person);

			cmsContext.Entry(person).Property(x => x.FirstName).IsModified = true;
			cmsContext.Entry(person).Property(x => x.MiddleName).IsModified = true;
			cmsContext.Entry(person).Property(x => x.LastName).IsModified = true;

			if (request.BirthDate!=null)
			{
                person.BirthDate =new DateTime (request.BirthDate.Value.Year, request.BirthDate.Value.Month, request.BirthDate.Value.Day);
                cmsContext.Entry(person).Property(x => x.BirthDate).IsModified = true;

            }



            cmsContext.SaveChanges();

            person.User.PhoneNumber = request.PhoneNumber;

			await _userManager.UpdateAsync(person.User);

			return new ObjectResult(new { status = StatusCodes.Status200OK, data = "", message = "Ok" });

		}

        /// <summary>
        /// This API is used to update user image
        /// Auth: bearer token
        /// </summary>
        /// <returns>Message</returns>

        [HttpPost("update-my-profile-image"), Authorize]
		public async Task<IActionResult> UpdateMyProfileImage([FromForm] IFormFile file)
		{
			Guid id = (Guid)_userService.GetMyId();
			Person person = cmsContext.Person.FirstOrDefault(a => a.Id == id);

			// Check if a file is present in the request
			if (file != null && file.Length > 0)
			{
				//string uploadfolder = _config.GetSection("Images:PathToImages").Value.ToString();
				//string uniqueFileName = Guid.NewGuid().ToString().Replace("-", "") + DateTime.Now.Ticks.ToString() + Path.GetExtension(file.FileName);

                string uniqueFileName =FileHandler.UpdateProfileImage(file,person.ImageName);

				//using (var stream = new FileStream(uploadfolder+ uniqueFileName, FileMode.Create))
				//{
				//	await file.CopyToAsync(stream);
				//}

				//if(System.IO.File.Exists(person.ImageFullPath))
				//{ 
				//	System.IO.File.Delete(person.ImageFullPath);
				//}

				person.ImageName = uniqueFileName;
				cmsContext.Person.Attach(person);
				cmsContext.Entry(person).Property(x => x.ImageName).IsModified = true;
				cmsContext.SaveChanges();

				return new ObjectResult(new { status = StatusCodes.Status200OK, data = person.ImageFullPath, message = "Ok" });

			}

			return new ObjectResult(new { status = StatusCodes.Status200OK, data = "", message = "No file was uploaded." });
		}

        /// <summary>
        /// This API is used to when user forgets his password.
        /// </summary>
        /// <param name="username">username</param>
        /// <returns>In response the system will send you a token to your e-mail to reset password</returns>

        [HttpPost("forgot-password")]
        public async Task<ActionResult> renewPassword(string username)
        {

            var user = await _userManager.FindByEmailAsync(username);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(username);
            }

            if (user == null)
            {
                return new ObjectResult(new { status = StatusCodes.Status200OK, data = "", message = "تأكد أن الإيميل أو اسم المستخدم صحيح." });
            }


            string token = Convert.ToHexString(RandomNumberGenerator.GetBytes(5));



            try
            {


                string subject = "Reset passsword";
                string body = $@"
            <center>  <img src='{_config.GetValue<string>("ApiUrl")}public/images/footerLogo.png'>  </center>

    <h2>Please use the following token to reset password:<br/> {token}</h2></ul>
            <center>  <img src='{_config.GetValue<string>("ApiUrl")}public/images/mailFooter.png'>  </center>

    ";
                var d = _emailSender.SendEmailAsync(user.Email, subject, body);
            }
            catch
            {

            }



            cmsContext.UserResetToken.Add(new UserResetToken
            {
                UserName = username,
                Token = token
            });

            cmsContext.SaveChanges();

            return new ObjectResult(new { status = StatusCodes.Status200OK, data = "", message = "تم إرسال الرمز السري إلى إيميلك الخاص، يرجى استخدامه من أجل إعادة ضبط كلمة المرور" });



        }

     
        /// <summary>
        /// This API is used to save the new password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="usrToken"></param>
        /// <param name="Password"></param>
        /// <param name="repPassword"></param>
        /// <returns></returns>
        [HttpPost("set-new-password")]
        public async Task<IActionResult> saveNewPassword(string username, string usrToken, string Password, string repPassword)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(username);
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(username);
                }

                // check token 

                bool isTokenValid = (cmsContext.UserResetToken
                    .Where(a => a.UserName == user.UserName
                    && a.Token == usrToken
                    && DateTime.Now <= a.EndDate
                    ).Any());


                if (!isTokenValid)
                {
                    return new ObjectResult(new
                    {
                        status = StatusCodes.Status200OK,
                        data = "",
                        message = "إما الرمز السري غير صحيح أو أن وقته قد انتهى"
                    });

                }

                if (Password != repPassword)
                {
                    return new ObjectResult(new
                    {
                        status = StatusCodes.Status200OK,
                        data = "",
                        message = "كلمة السر وكلمة السر المكررة غير متطابقتين"
                    });
                }

                List<UserResetToken> tokens = cmsContext.UserResetToken
                    .Where(a => a.UserName == user.UserName
                    ).ToList();

                cmsContext.UserResetToken.RemoveRange(tokens);
                cmsContext.SaveChanges();

                string token = await _userManager.GeneratePasswordResetTokenAsync(user);

                await _userManager.ResetPasswordAsync(user, token, repPassword);

                // if token is not correct redirect to renewPassword

                return new ObjectResult(new
                {
                    status = StatusCodes.Status200OK,
                    data = "",
                    message = "تمت عملية إعادة تعيين كلمة السر"
                });
            }
            catch
            {
                return new ObjectResult(new
                {
                    status = StatusCodes.Status200OK,
                    data = "",
                    message = "حدث خطأ ما يرجى المحاولة مرة أخرى"
                });
            }
        }




    }
}