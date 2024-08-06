using System.Linq.Expressions;
using System.Security.Claims;

using CmsDataAccess;
using CmsDataAccess.DbModels;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Http;
using ServicesLibrary.PasswordsAndClaims;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.ObjectModel;


namespace ServicesLibrary
{

    public interface ISeedDb
    {
        public void SeedDbTables();
    }

    public class SeedDb: ISeedDb
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext cmsContext;
        private readonly UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;



        public SeedDb(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            IHttpContextAccessor httpContextAccessor, ApplicationDbContext _cmsDbContext)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            cmsContext = _cmsDbContext;
            _roleManager = roleManager;
        }

        public async Task<bool> InsertSysAdminAsync()
        {
            // First add the user login info
            try
            {

                if (cmsContext.SysAdmin.Any())
                {
                    return true;
                }

                var user = new IdentityUser
                {
                    Email = "admin@admin.com",
                    PhoneNumber = "0000",
                    UserName = "admin",
                    PhoneNumberConfirmed = true,
                    EmailConfirmed = true,
                };

                await _roleManager.CreateAsync(new IdentityRole("Admin"));
                await _userManager.CreateAsync(user, "P@ssw0rd");
                await _userManager.AddToRoleAsync(user, "Admin");

                cmsContext.SysAdmin.Add(
                new SysAdmin
                {
                    FirstName = "Admin",
                    LastName = "Admin",
                    MiddleName = "Admin",
                    User = user,
                }
                );

                cmsContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void InsertSysAdmin()
        {
            InsertSysAdminAsync();
        }

        public void InsertTimeZones()
        {
            if (cmsContext.SystemTimeZone.Any())
            {
                return;
            }
            ReadOnlyCollection<TimeZoneInfo> tzCollection;
            tzCollection = TimeZoneInfo.GetSystemTimeZones();
            foreach (TimeZoneInfo timeZone in tzCollection)
            {
                cmsContext.SystemTimeZone.Add(new SystemTimeZone
                {
                    StandardName = timeZone.StandardName,
                    DisplayName=timeZone.DisplayName,
                    IANAID=timeZone.Id,
                });
                cmsContext.SaveChanges();
            }

        }
        
        public void InsertSystemConfiguration()
        {
            if (cmsContext.MySystemConfiguration.Any())
            {
                return;
            }

            cmsContext.MySystemConfiguration.Add(new MySystemConfiguration
            {
                ApiUrl="",
                StripePrivateKey="",
                StripePublicKey = "",
                SystemTimeZone= "Arabian Standard Time",
                UseCashPayementForSubscriber = true,
                UseEmailVerfication = true,
                
            });
            cmsContext.SaveChanges();   
        }

        public void InsertAboutUs()
        {
            if (cmsContext.AboutUs.Any())
            {
                return;
            }

            cmsContext.AboutUs.Add(new AboutUs
            {
                AboutUsTranslation= new List<AboutUsTranslation> { 
                    new AboutUsTranslation { 
                    AboutUsText="AboutUsText EN",
                    LangCode="en-us",
                    } ,
                                        new AboutUsTranslation {
                    AboutUsText="AboutUsText AR",
                    LangCode="ar",
                    }
                }

            });
            cmsContext.SaveChanges();
        }

        public void InsertTermsAndConditions()
        {
            if (cmsContext.TermsAndConditions.Any())
            {
                return;
            }

            cmsContext.TermsAndConditions.Add(new TermsAndConditions
            {
                TermsAndConditionsTranslation = new List<TermsAndConditionsTranslation> {
                    new TermsAndConditionsTranslation {
                    TermsAndConditionsText="Terms EN",
                    LangCode="en-us",
                    } ,
                                        new TermsAndConditionsTranslation {
                    TermsAndConditionsText="Terms AR",
                    LangCode="ar",
                    }
                }

            });
            cmsContext.SaveChanges();
        }

        public void SeedDbTables()
        {
            //InsertSysAdmin();
            InsertTimeZones();
            InsertSystemConfiguration();
            InsertAboutUs();
            InsertTermsAndConditions();
        }

    }

}
