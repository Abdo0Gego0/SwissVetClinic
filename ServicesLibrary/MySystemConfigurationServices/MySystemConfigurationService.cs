using CmsDataAccess.DbModels;
using CmsDataAccess.Utils.FilesUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLibrary.MySystemConfigurationServices
{
    public interface IMySystemConfigurationService
    {
        public string UpdateMySystemConfiguration(MySystemConfiguration model);
        public string UpdateAboutUs(AboutUs model);
    }
    public class MySystemConfigurationService :IMySystemConfigurationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext cmsContext;
        private readonly UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public MySystemConfigurationService(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            IHttpContextAccessor httpContextAccessor, ApplicationDbContext _cmsDbContext)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            cmsContext = _cmsDbContext;
            _roleManager = roleManager;
        }


        public string UpdateMySystemConfiguration(MySystemConfiguration model)
        {
            using (var transaction=cmsContext.Database.BeginTransaction())
            {
                try
                {
                    MySystemConfiguration mySystemConfiguration = cmsContext.MySystemConfiguration.Find(model.Id);
                    string ImageName = FileHandler.UpdateImageFile(mySystemConfiguration.ImageName, model.ImageFile);
                    mySystemConfiguration.ImageName = ImageName;
                    cmsContext.MySystemConfiguration.Attach(mySystemConfiguration);
                    cmsContext.Entry(mySystemConfiguration).State=Microsoft.EntityFrameworkCore.EntityState.Modified;
                    cmsContext.SaveChanges ();
                    transaction.Commit();
                    return "";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return ex.ToString();
                }
            }

        }

        public string UpdateAboutUs(AboutUs model)
        {
            using (var transaction = cmsContext.Database.BeginTransaction())
            {
                try
                {
                    cmsContext.AboutUs.Attach(model);
                    cmsContext.SaveChanges();
                    transaction.Commit();
                    return "";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return ex.ToString();
                }
            }

        }
    }
}
