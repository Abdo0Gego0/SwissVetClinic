using System.Linq.Expressions;
using System.Security.Claims;

using CmsDataAccess;
using CmsDataAccess.DbModels;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Http;
using ServicesLibrary.PasswordsAndClaims;
using Microsoft.AspNetCore.Identity;
using System;

namespace ServicesLibrary.UserServices
{

    public interface IUserService
    {
        public object GetMyInfo();
        public object GetMyInfo(Guid id);
        public string GetMyPhone();
        public Guid? GetMyCenterIdWeb();
        public Guid? GetMyCenterIdMobile();
        public Guid? GetMyId();
        public Guid? GetMyDoctorId();
        public Guid? GetMyClinicId();
        public string? GetMyClinicName();
        public string GetMyRole();
        public string GetMyLanguage();

        public string GetUserProfileImage();

        public string GetUserName();
        public string GetUserFirstName();
    }

    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext cmsContext;
        private readonly UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;



        public UserService(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            IHttpContextAccessor httpContextAccessor, ApplicationDbContext _cmsDbContext)
        {
            _userManager = userManager; 
            _httpContextAccessor = httpContextAccessor;
            cmsContext = _cmsDbContext;
            _roleManager = roleManager;
        }

        public string GetUserName()
        {
            if (_httpContextAccessor != null)
            {
                return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            return null;
        }

        public string GetUserFirstName()
        {
            if (_httpContextAccessor != null)
            {
                return _httpContextAccessor.HttpContext.User.FindFirstValue("FirstName");
            }
            return null;
        }
        public string GetUserProfileImage()
        {
            if (_httpContextAccessor != null)
            {
                return _httpContextAccessor.HttpContext.User.FindFirstValue("ProfileImage");
            }
            return "";
        }

        public object GetMyInfo()
        {
            PetOwner person = cmsContext.PetOwner
                .Include(a => a.User)
                .FirstOrDefault(a => a.Id == (Guid)GetMyId());

            if (_httpContextAccessor != null)
            {
                string role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);


                return new
                {
                    UserName = person.User.UserName,
                    FullName = person.FullName,
                    Email = person.User.Email,
                    PhoneNumber = person.User.PhoneNumber,
                    Role = role,
					BirthDate = person.BirthDate,
					MedicalCenterId = person.MedicalCenterId,
					CenterName = person.CenterName,
					GeneralNumber = person.GeneralNumber,
                    
                    Image = person.ImageFullPath,
                    MyToken = new PasswordUtil(_userManager).CreateTokenForUser(person),

                };
            }
            return null;
        }

        public string  GetMyPhone()
        {
            PetOwner person = cmsContext.PetOwner
                .Include(a => a.User)
                .FirstOrDefault(a => a.Id == (Guid)GetMyId());

            if (_httpContextAccessor != null)
            {
                return person.User.PhoneNumber;
            }
            return "";
        }


        public object GetMyInfo(Guid id)
        {
            PetOwner person = cmsContext.PetOwner
                .Include(a => a.User)
                .FirstOrDefault(a => a.Id == id);

            if (_httpContextAccessor != null)
            {
                string role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);


                return new
                {
                    UserName = person.User.UserName,
                    FullName = person.FullName,
                    Email = person.User.Email,
                    PhoneNumber = person.User.PhoneNumber,
                    Role = role,
					BirthDate = person.BirthDate,
					MedicalCenterId = person.MedicalCenterId,
					CenterName = person.CenterName,
					GeneralNumber = person.GeneralNumber,
                    
                    Image = person.ImageFullPath,

                };
            }
            return null;
        }


        public Guid? GetMyCenterIdWeb()
        {
            if (_httpContextAccessor != null)
            {
                try
                {

                    return Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue("CenterId"));

                }
                catch
                { 
                    return null;
                }
            }
            return null;
        }

        public Guid? GetMyCenterIdMobile()
        {
            if (_httpContextAccessor != null)
            {
                try
                {



                    return Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue("CenterId").ToString());
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public Guid? GetMyId()
        {
            if (_httpContextAccessor != null)
            {
                try
                {
                    return Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.SerialNumber));
                }
                catch
                { return null; }
            }
            return null;
        }

        public Guid? GetMyDoctorId()
        {
            if (_httpContextAccessor != null)
            {
                try
                {
                    string user= _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.SerialNumber);

                    return cmsContext.Doctor.Include(a=>a.User).FirstOrDefault(a=>a.User.Id== user).Id;
                }
                catch
                { return null; }
            }
            return null;
        }
        public Guid? GetMyClinicId()
        {
            if (_httpContextAccessor != null)
            {
                try
                {
                    return Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue("ClinicId"));
                }
                catch
                { return null; }
            }
            return null;
        }
        public string? GetMyClinicName()
        {
            if (_httpContextAccessor != null)
            {
                try
                {
                    Guid guid= Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue("ClinicId"));

                    string clinicName = string.Join(" ", cmsContext.BaseClinicTranslation.Where(a => a.BaseClinicId == guid).Select(a => a.Name).ToArray());

                    return clinicName;

                }
                catch
                { return ""; }
            }
            return "";
        }
        public string GetMyRole()
        {
            if (_httpContextAccessor != null)
            {
                return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            }
            return null;
        }
        public string GetMyLanguage()
        {
            if (_httpContextAccessor != null)
            {
                return _httpContextAccessor.HttpContext.Request.Headers["Accept-Language"].ToString();
            }
            return "en-US";
        }

    }




}
