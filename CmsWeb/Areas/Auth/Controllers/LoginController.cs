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
using System.Web;
using ServicesLibrary.MedicalCenterServices;
using CmsDataAccess.Utils.FilesUtils;
using Microsoft.AspNetCore.SignalR;
using CmsWeb.Hubs;
using Microsoft.IdentityModel.Tokens;
using ServicesLibrary;
using NoificationManager.MobileModels;
using Google;

namespace CmsWeb.Areas.Auth.Controllers
{
    [Area("Auth")]
    public class LoginController : Controller
    {
        private readonly INotificationService notiService;


        private readonly IStringLocalizer<CmsResources.Messages> _localizer;
        private readonly ILogger<LoginController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;

        private readonly IEmailSender _emailSender;
        private readonly IHubContext<MyHub> _hubContext;


        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly ISeedDb seedDb;
        private readonly IMedicalCenterService medicalCenterService;
        private readonly ApplicationDbContext cmsContext;

        public LoginController(
            INotificationService notiService_,

            ISeedDb seedDb_,
            IHubContext<MyHub> hubContext,

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
            ILogger<LoginController> logger)
        {

            notiService = notiService_;


            seedDb = seedDb_;
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
            _hubContext= hubContext;
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<JsonResult> IsEmailUnique(string PersonEmail, string? PreviousEmail="")
        {

            if (PreviousEmail.IsNullOrEmpty())
            {
                var user = await _userManager.FindByEmailAsync(PersonEmail);
                bool isUnique = (user == null);
                return Json(isUnique);
            }
            else
            {
                if (PreviousEmail==PersonEmail)
                {
                    return Json(true);
                }
                else
                {
                    var user = await _userManager.FindByEmailAsync(PersonEmail);
                    bool isUnique = (user == null);
                    return Json(isUnique);
                }
            }


        }

        [HttpGet]
        public async Task<JsonResult> IsUserNameUnique(string PersonUserName, string? PreviousUsername="")
        {
            if (PreviousUsername.IsNullOrEmpty())
            {
                var user = await _userManager.FindByNameAsync(PersonUserName);
                bool isUnique = (user == null);
                return Json(isUnique);
            }
            else
            {
                if (PersonUserName == PreviousUsername)
                {
                    return Json(true);
                }
                else
                {
                    var user = await _userManager.FindByNameAsync(PersonUserName);
                    bool isUnique = (user == null);
                    return Json(isUnique);
                }
            }


        }

        public async Task<IActionResult> verify(string uname,string code)
        {

            if (cmsContext.AccountVerificationToken.FirstOrDefault(a=>a.username==uname && a.code==code)!=null)
            {
                PetOwner petOwner = cmsContext.PetOwner.Include(a => a.User).FirstOrDefault(a => a.User.UserName == uname);
                petOwner.AccountVerfied = true;
                cmsContext.PetOwner.Attach(petOwner);
                cmsContext.Entry(petOwner).Property(a=>a.AccountVerfied).IsModified=true;
                cmsContext.SaveChanges();
                return View("Close");


            }
            return Json("Error");

        }
        public async Task<IActionResult> Index()
        {

            //seedDb.SeedDbTables();

            //cmsContext.UserRoles.RemoveRange(cmsContext.UserRoles);
            //cmsContext.SaveChanges();

            //cmsContext.Roles.RemoveRange(cmsContext.Roles);
            //cmsContext.SaveChanges();

            //cmsContext.Person.RemoveRange(cmsContext.Person);
            //cmsContext.SaveChanges();

            //cmsContext.Users.RemoveRange(cmsContext.Users);
            //cmsContext.SaveChanges();



            //return Json(cmsContext.CenterAdmin.ToList());

            //List<CenterSupervisor> c = cmsContext.CenterSupervisor.Include(a => a.User).ToList();
            //List<CenterTutor> t = cmsContext.CenterTutor.Include(a => a.WorkingHours).Include(a => a.User).ToList();
            //List<Student> s = cmsContext.Student.Include(a => a.User).ToList();
            //List<SysAdmin> sa = cmsContext.SysAdmin.Include(a => a.User).ToList();

            //cmsContext.CenterTutor.RemoveRange(t);
            //cmsContext.CenterSupervisor.RemoveRange(c);
            //cmsContext.Student.RemoveRange(s);
            //cmsContext.SysAdmin.RemoveRange(sa);
            //cmsContext.SaveChanges();

            //var user1 = await _userManager.FindByNameAsync("admin");
            //_userManager.DeleteAsync(user1);

            //var role = await _roleManager.FindByNameAsync("Admin");
            //_roleManager.DeleteAsync(role);

            ////Insert Admin into database



            //// First add the user login info
            //try
            //{
            //    var user = new IdentityUser
            //    {
            //        Email = "admin@admin.com",
            //        PhoneNumber = "0000",
            //        UserName = "admin",
            //        PhoneNumberConfirmed = true,
            //        EmailConfirmed = true,
            //    };

            //    await _roleManager.CreateAsync(new IdentityRole("Admin"));
            //    await _userManager.CreateAsync(user, "P@ssw0rd");
            //    await _userManager.AddToRoleAsync(user, "Admin");

            //    cmsContext.SysAdmin.Add(
            //    new SysAdmin
            //    {
            //        FirstName = "Admin",
            //        LastName = "Admin",
            //        MiddleName = "Admin",
            //        User = user,
            //    }
            //    );

            //    cmsContext.SaveChanges();
            //}
            //catch
            //{

            //}

            ViewBag.ErrorMessage = "";

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(string username, string password)
        {

            //return RedirectToAction("Index", "Home", new { area = "Admin" });


            //notiService.SendNotification(new NotificationModel { Body= "Body",Title= "Title",
            //    Token= "ctVXZXMDSyqf-eiwEhZDk5:APA91bHZbtO4jGwmatXyw64CvNk1zmGGUSpUpQaqgSUmU_OrBTvWkALN8Wdebrl_enwspecshF-_JRToKcvgdXPrB-6sSdoRbZNVDDQunlmpI_jmQ1qc4UsTihbDGPK4qpsmE23mVMoY"
            //});

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = _localizer["EnterUserNameAndPassword"];
                return View();
            }

            var user = await _userManager.FindByEmailAsync(username);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(username);
            }

            if (user != null)
            {
                
                Person prsn = cmsContext.Person.Include(a => a.User).FirstOrDefault(a => a.User.Id == user.Id);

                if (prsn.IsDeleted)
                {
                    ViewBag.ErrorMessage = _localizer["WrongLoginInfo"];
                    return View();
                }

                var result = await _signInManager.PasswordSignInAsync(user, password, false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    foreach (var item in roles)
                    {
                        string tem = item.ToString().ToLower();
                        switch (tem)
                        {
                            case "admin":

                                //https://weblog.west-wind.com/posts/2022/Mar/29/Combining-Bearer-Token-and-Cookie-Auth-in-ASPNET

                                Person person = cmsContext.Person.Include(a => a.User).Where(a=>!a.IsDeleted)
                                    .FirstOrDefault(a => a.User.Id == user.Id);

                                List<Claim> claims = new List<Claim>()
                                {
                                    new Claim(ClaimTypes.NameIdentifier, user.UserName),
                                    new Claim(ClaimTypes.SerialNumber, user.Id.ToString()),
                                    new Claim("FirstName", person.FirstName),
                                    new Claim(ClaimTypes.Name, person.FullName ),
                                    new Claim(ClaimTypes.Email, user.Email),
                                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                                    new Claim(ClaimTypes.Role, "Admin"),
                                    new Claim("ProfileImage", person.ImageFullPath),
                                };

                                // also add cookie auth for Swagger Access
                                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);

                                identity.AddClaims(claims);


                                var principal = new ClaimsPrincipal(identity);
                                await HttpContext.SignInAsync(
                                    CookieAuthenticationDefaults.AuthenticationScheme,
                                    principal,
                                    new AuthenticationProperties
                                    {
                                        IsPersistent = true,
                                        AllowRefresh = true,
                                        ExpiresUtc = DateTime.UtcNow.AddDays(1)
                                    });


                                return RedirectToAction("Index", "Home", new { area = "Admin" });

                            case "centeradmin":


                                //https://weblog.west-wind.com/posts/2022/Mar/29/Combining-Bearer-Token-and-Cookie-Auth-in-ASPNET

                                Person person1 = cmsContext.Person.Include(a => a.User)
                                    .FirstOrDefault(a => a.User.Id == user.Id);

                                CenterAdmin centerAdmin = cmsContext.CenterAdmin.Find(person1.Id);

                                MedicalCenter medicalCenter1 = cmsContext.MedicalCenter.Find(centerAdmin.MedicalCenterId);
                                if (medicalCenter1.Deleted)
                                {
                                    ViewBag.ErrorMessage = _localizer["CenterIsDeleted"];
                                    return View();
                                }


                                List<Claim> claims1 = new List<Claim>()
                                {
                                    new Claim(ClaimTypes.NameIdentifier, user.UserName),
                                    new Claim(ClaimTypes.SerialNumber, user.Id.ToString()),
                                    new Claim("FirstName", person1.FirstName),
                                    new Claim(ClaimTypes.Name, person1.FullName ),
                                    new Claim(ClaimTypes.Email, user.Email),
                                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                                    new Claim(ClaimTypes.Role, "Supervisor"),
                                    new Claim("ProfileImage", person1.ImageFullPath),
                                    new Claim("CenterId", centerAdmin.MedicalCenterId.ToString()),

                                    

                                };

                                // also add cookie auth for Swagger Access
                                var identity1 = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);

                                identity1.AddClaims(claims1);


                                var principal1 = new ClaimsPrincipal(identity1);
                                await HttpContext.SignInAsync(
                                    CookieAuthenticationDefaults.AuthenticationScheme,
                                    principal1,
                                    new AuthenticationProperties
                                    {
                                        IsPersistent = true,
                                        AllowRefresh = true,
                                        ExpiresUtc = DateTime.UtcNow.AddDays(1)
                                    });


                                return RedirectToAction("Index", "Home", new { area = "CenterAdmin" });

                            case "reception":


                                //https://weblog.west-wind.com/posts/2022/Mar/29/Combining-Bearer-Token-and-Cookie-Auth-in-ASPNET

                                Person person11 = cmsContext.Person.Include(a => a.User)
                                    .FirstOrDefault(a => a.User.Id == user.Id);

                                Employee centerAdmin1 = cmsContext.Employee.Find(person11.Id);

                                MedicalCenter medicalCenter2 = cmsContext.MedicalCenter.Find(centerAdmin1.MedicalCenterId);
                                if (medicalCenter2.Deleted)
                                {
                                    ViewBag.ErrorMessage = _localizer["CenterIsDeleted"];
                                    return View();
                                }


                                List<Claim> claims11 = new List<Claim>()
                                {
                                    new Claim(ClaimTypes.NameIdentifier, user.UserName),
                                    new Claim(ClaimTypes.SerialNumber, user.Id.ToString()),
                                    new Claim("FirstName", person11.FirstName),
                                    new Claim(ClaimTypes.Name, person11.FullName ),
                                    new Claim(ClaimTypes.Email, user.Email),
                                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                                    new Claim(ClaimTypes.Role, "employee"),
                                    new Claim("ProfileImage", person11.ImageFullPath),
                                    new Claim("CenterId", centerAdmin1.MedicalCenterId.ToString()),



                                };

                                // also add cookie auth for Swagger Access
                                var identity11 = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);

                                identity11.AddClaims(claims11);


                                var principal11 = new ClaimsPrincipal(identity11);
                                await HttpContext.SignInAsync(
                                    CookieAuthenticationDefaults.AuthenticationScheme,
                                    principal11,
                                    new AuthenticationProperties
                                    {
                                        IsPersistent = true,
                                        AllowRefresh = true,
                                        ExpiresUtc = DateTime.UtcNow.AddDays(1)
                                    });


                                return RedirectToAction("Timeline", "Appointment", new { area = "CenterEmployeeReception" });

                            case "ordersmanagement":


                                //https://weblog.west-wind.com/posts/2022/Mar/29/Combining-Bearer-Token-and-Cookie-Auth-in-ASPNET

                                Person person1111 = cmsContext.Person.Include(a => a.User)
                                    .FirstOrDefault(a => a.User.Id == user.Id);

                                Employee centerAdmin111 = cmsContext.Employee.Find(person1111.Id);

                                MedicalCenter medicalCenter211 = cmsContext.MedicalCenter.Find(centerAdmin111.MedicalCenterId);
                                if (medicalCenter211.Deleted)
                                {
                                    ViewBag.ErrorMessage = _localizer["CenterIsDeleted"];
                                    return View();
                                }


                                List<Claim> claims1111 = new List<Claim>()
                                {
                                    new Claim(ClaimTypes.NameIdentifier, user.UserName),
                                    new Claim(ClaimTypes.SerialNumber, user.Id.ToString()),
                                    new Claim("FirstName", person1111.FirstName),
                                    new Claim(ClaimTypes.Name, person1111.FullName ),
                                    new Claim(ClaimTypes.Email, user.Email),
                                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                                    new Claim(ClaimTypes.Role, "employee"),
                                    new Claim("ProfileImage", person1111.ImageFullPath),
                                    new Claim("CenterId", centerAdmin111.MedicalCenterId.ToString()),



                                };

                                // also add cookie auth for Swagger Access
                                var identity1111 = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);

                                identity1111.AddClaims(claims1111);


                                var principal1111 = new ClaimsPrincipal(identity1111);
                                await HttpContext.SignInAsync(
                                    CookieAuthenticationDefaults.AuthenticationScheme,
                                    principal1111,
                                    new AuthenticationProperties
                                    {
                                        IsPersistent = true,
                                        AllowRefresh = true,
                                        ExpiresUtc = DateTime.UtcNow.AddDays(1)
                                    });


                                return RedirectToAction("Index", "Orders", new { area = "CenterEmployeeOrders" });

                            case "doctor":


                                //https://weblog.west-wind.com/posts/2022/Mar/29/Combining-Bearer-Token-and-Cookie-Auth-in-ASPNET

                                Person person111 = cmsContext.Person.Include(a => a.User)
                                    .FirstOrDefault(a => a.User.Id == user.Id);

                                Doctor centerAdmin11 = cmsContext.Doctor.Find(person111.Id);


                                MedicalCenter medicalCenter3 = cmsContext.MedicalCenter.Find(centerAdmin11.MedicalCenterId);
                                if (medicalCenter3.Deleted)
                                {
                                    ViewBag.ErrorMessage = _localizer["CenterIsDeleted"];
                                    return View();
                                }


                                List<Claim> claims111 = new List<Claim>()
                                {
                                    new Claim(ClaimTypes.NameIdentifier, user.UserName),
                                    new Claim(ClaimTypes.SerialNumber, user.Id.ToString()),
                                    new Claim("FirstName", person111.FirstName),
                                    new Claim(ClaimTypes.Name, person111.FullName ),
                                    new Claim(ClaimTypes.Email, user.Email),
                                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                                    new Claim(ClaimTypes.Role, "doctor"),
                                    new Claim("ProfileImage", person111.ImageFullPath),
                                    new Claim("CenterId", centerAdmin11.MedicalCenterId.ToString()),



                                };

                                // also add cookie auth for Swagger Access
                                var identity111 = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);

                                identity111.AddClaims(claims111);


                                var principal111 = new ClaimsPrincipal(identity111);
                                await HttpContext.SignInAsync(
                                    CookieAuthenticationDefaults.AuthenticationScheme,
                                    principal111,
                                    new AuthenticationProperties
                                    {
                                        IsPersistent = true,
                                        AllowRefresh = true,
                                        ExpiresUtc = DateTime.UtcNow.AddDays(1)
                                    });


                                return RedirectToAction("Index", "Home", new { area = "Doctor" });

                            default:
                                ViewBag.ErrorMessage = _localizer["WrongLoginInfo"];
                                return View();
                        
                        }
                    }

                    ViewBag.ErrorMessage = _localizer["WrongLoginInfo"];
                    return View();


                }
                else
                {
                    ViewBag.ErrorMessage = _localizer["WrongLoginInfo"];
                    return View();

                }
            }
            else
            {
                ViewBag.ErrorMessage = _localizer["WrongLoginInfo"];
                return View();
            }

        }

        public IActionResult ForgotPassword(string? msg)
        {

            ViewBag.ErrorMessage = string.IsNullOrEmpty(msg) ? "" : msg;

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> renewPassword(string username, string? msg)
        {
            ViewBag.ErrorMessage = "";
            ViewBag.UserName = username;

            var user = await _userManager.FindByEmailAsync(username);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(username);
            }

            if (user == null)
            {
                return RedirectToAction("ForgotPassword", new { msg = _localizer["EnsureEmail"] });
            }


            string token = new PasswordUtil(_userManager).CreateRandomTokenN(5);



            try
            {


                MySystemConfiguration mySystemConfiguration = cmsContext.MySystemConfiguration.FirstOrDefault();


                string subject = _localizer["Reset passsword"];
                string body = $@"
            <center>  <img src='{mySystemConfiguration.ApiUrl}public/images/footerLogo.png'>  </center>
            <h2>{_localizer["Please use the following token to reset password:"]}<br/> {token}</h2></ul>
            <center>  <img src='{mySystemConfiguration.ApiUrl}public/images/mailFooter.png'>  </center>
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

            return View();


        }

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
                    ViewBag.ErrorMessage = _localizer["TokenError"];
                    return View("renewPassword");
                }

                if (Password != repPassword)
                {
                    ViewBag.ErrorMessage = _localizer["Passwords fields must match"];
                    return View("renewPassword");
                }

                List<UserResetToken> tokens = cmsContext.UserResetToken
                    .Where(a => a.UserName == user.UserName
                    ).ToList();

                cmsContext.UserResetToken.RemoveRange(tokens);
                cmsContext.SaveChanges();

                string token = await _userManager.GeneratePasswordResetTokenAsync(user);

                await _userManager.ResetPasswordAsync(user, token, repPassword);

                // if token is not correct redirect to renewPassword

                ViewBag.ErrorMessage = "";
                return RedirectToAction("Index");
            }
            catch
            {
                return View("ForgotPassword");
            }
        }

        public async Task<IActionResult> logOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> CreateNewAccount()
        {
            TermsAndConditions model = cmsContext.TermsAndConditions.Include(a => a.TermsAndConditionsTranslation).FirstOrDefault();

            ViewBag.Terms1 = HttpUtility.HtmlDecode(model.TermsAndConditionsTranslation[0].TermsAndConditionsText);
            ViewBag.Terms2 = HttpUtility.HtmlDecode(model.TermsAndConditionsTranslation[1].TermsAndConditionsText);
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewAccount(
            string? Firstname,
            string? Middlename,
            string? Lastname,
            string? Nationality,
            string? PassportNumber,
            string? NationalCardId,
            string? SubscriberPhone,
            string? SubscriberEmail,
            string? LicenseNumber,
            DateTime? LicenseExpireDate,
            string? CenterName,
            string? CenterAddress,
            IFormFile? PassportImageFile,
            IFormFile? LicenseImageFile

            )
        {
            TermsAndConditions model = cmsContext.TermsAndConditions.Include(a => a.TermsAndConditionsTranslation).FirstOrDefault();
            ViewBag.Terms1 = HttpUtility.HtmlDecode(model.TermsAndConditionsTranslation[0].TermsAndConditionsText);
            ViewBag.Terms2 = HttpUtility.HtmlDecode(model.TermsAndConditionsTranslation[1].TermsAndConditionsText);


            string PassportImageFile_=FileHandler.SaveUploadedFile(PassportImageFile);
            string LicenseImageFile_ = FileHandler.SaveUploadedFile(LicenseImageFile);

            SubscriptionApplication app = new SubscriptionApplication
            {
                FirstName = Firstname,
                MiddleName = Middlename,
                LastName = Lastname,
                Nationality = Nationality,
                PassportNumber = PassportNumber,
                NationalCardId = NationalCardId,
                SubscriberPhone = SubscriberPhone,
                SubscriberEmail = SubscriberEmail,
                LicenseNumber = LicenseNumber,
                LicenseExpireDate = LicenseExpireDate,
                CenterName = CenterName,
                CenterAddress = CenterAddress,
                PassportImageFile = PassportImageFile,
                LicenseImageFile = LicenseImageFile,
                CreateDate = medicalCenterService.ConvertToLocalTime(DateTime.Now),
                ResponseFromAdmin = "Waiting for approval",
            };

            string baseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

            medicalCenterService.AddNewSubscriptionApplication(baseUrl, app);

            return View("VerifyMailPage");
        }

        public async Task<IActionResult> VerifyMailPage()
        {
            return View();
        }

        public async Task<IActionResult> VerifyNewSubscriptionApplication(string token)
        {
            string baseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

            medicalCenterService.VerifyNewSubscriptionApplication(baseUrl,token);

            await _hubContext.Clients.All.SendAsync("RefreshNewApplicationCount_", "ddd");

            return View();
        }


    }
}
