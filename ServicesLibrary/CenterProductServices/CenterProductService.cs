using System.Linq.Expressions;
using System.Security.Claims;

using CmsDataAccess;
using CmsDataAccess.DbModels;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Http;
using ServicesLibrary.PasswordsAndClaims;
using Microsoft.AspNetCore.Identity;
using System;
using CmsDataAccess.Utils.FilesUtils;
using ServicesLibrary.MedicalCenterServices;
using System.Web.Razor.Parser.SyntaxTree;
using ServicesLibrary.UserServices;

namespace ServicesLibrary.CenterProductServices
{

    public interface ICenterProductService
    {
        public Guid AddNewProduct(Product model);
        public Guid AddNewSubProduct(SubProduct model);


    }

    public class CenterProductService : ICenterProductService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMedicalCenterService medicalCenterService;
        private readonly ApplicationDbContext cmsContext;
        private readonly UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;

        public CenterProductService(
            IUserService userService,
            RoleManager<IdentityRole> roleManager,
            IMedicalCenterService medicalCenterService_,
            UserManager<IdentityUser> userManager,
            IHttpContextAccessor httpContextAccessor, ApplicationDbContext _cmsDbContext)
        {
            medicalCenterService= medicalCenterService_;
            _userManager = userManager; 
            _httpContextAccessor = httpContextAccessor;
            cmsContext = _cmsDbContext;
            _roleManager = roleManager;
            _userService = userService;
        }

        public Guid AddNewProduct(Product model)
        {
            try
            {
                cmsContext.Product.Add(model);
                cmsContext.SaveChanges();
                return model.Id;
            }
            catch (Exception ex)
            {
                return Guid.Empty;
            }
        }
        public Guid AddNewSubProduct(SubProduct model)
        {
            try
            {
                cmsContext.SubProduct.Add(model);
                cmsContext.SaveChanges();
                return model.Id;
            }
            catch (Exception ex)
            {
                return Guid.Empty;
            }
        }




    }

}
