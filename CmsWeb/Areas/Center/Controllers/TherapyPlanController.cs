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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using ServicesLibrary.PersonServices;
using System.Reflection;

namespace CmsWeb.Areas.Center.Controllers
{
    [Area("CenterAdmin")]
    public class TherapyPlanController : Controller
    {
        private readonly IStringLocalizer<CmsResources.Messages> _localizer;
        private readonly ILogger<TherapyPlanController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;

        private readonly IEmailSender _emailSender;
        private readonly IMedicalCenterService medicalCenterService;
        private readonly IPersonService personService ;

        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext cmsContext;

        public TherapyPlanController(
            IPersonService personService_,
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
            ILogger<TherapyPlanController> logger)
        {
            personService = personService_;
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

        public async Task<IActionResult> IndexGoals()
        {
            //List<TherapyGoals> TherapyGoals = cmsContext.TherapyGoals.Include(a=>a.TherapyGoalsTranslations).ToList();

            //cmsContext.TherapyGoals.RemoveRange(TherapyGoals);
            //cmsContext.SaveChanges();

            return View("CenterAdmin/_IndexGoals");
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request, Guid planId)
        {
            Guid guid = (Guid)_userService.GetMyCenterIdWeb();

            if (planId == null)
            {
                var result = cmsContext.TherapyGoals
                    .Where(a => a.MedicalCenterId == guid && !a.IsDeleted)
                    .Include(a => a.TherapyGoalsTranslations)
                    .ToList().ToDataSourceResult(request);
                return Json(result);
            }
            else
            {
                List<Guid> guids = cmsContext.TherapyGoalsTherapyPlan.Where(a=>a.TherapyPlanId==planId && !a.IsDeleted).Select(a=>a.TherapyGoalsId).ToList();

                var result = cmsContext.TherapyGoals
                    .Where(a => a.MedicalCenterId == guid && !a.IsDeleted)
                    .Include(a => a.TherapyGoalsTranslations)
                    .Select(a => new TherapyGoals
                    {
                        Id = a.Id,
                        MedicalCenterId = a.MedicalCenterId,
                        TherapyGoalsTranslations = a.TherapyGoalsTranslations,
                        IsSelected = guids.Contains(a.Id) ? true : false
                    })
                    .OrderByDescending(a => a.IsSelected)
                    .ToList().ToDataSourceResult(request);
                return Json(result);
            }



        }

        public async Task<IActionResult> Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<TherapyGoals> ContactInfos)
        {
            Guid guid = (Guid)_userService.GetMyCenterIdWeb();

            //if (ModelState.IsValid)
            {
                foreach (var item in ContactInfos)
                {
                    cmsContext.TherapyGoals.Add(
                        new TherapyGoals
                        {
                            MedicalCenterId = guid,
                            TherapyGoalsTranslations= new List<TherapyGoalsTranslations>
                            {
                                new TherapyGoalsTranslations
                                {
                                    Description=item.TherapyGoalsTranslations[0].Description,
                                    LangCode="en-us"
                                },
                                 new TherapyGoalsTranslations
                                {
                                    Description=item.TherapyGoalsTranslations[1].Description,
                                    LangCode="ar"
                                }
                            }
                        }
                        );
                }
                cmsContext.SaveChanges();
                return Json(_localizer["Success"]);
            }



        }


        [AcceptVerbs("Post")]
        public async Task<IActionResult> Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<TherapyGoals> clinicSpecialties)
        {
            {
                foreach (var updatedItem in clinicSpecialties)
                {
                    cmsContext.TherapyGoalsTranslations.Attach(updatedItem.TherapyGoalsTranslations[0]);
                    cmsContext.Entry(updatedItem.TherapyGoalsTranslations[0]).State=EntityState.Modified;

                    cmsContext.TherapyGoalsTranslations.Attach(updatedItem.TherapyGoalsTranslations[1]);
                    cmsContext.Entry(updatedItem.TherapyGoalsTranslations[1]).State = EntityState.Modified;

                    cmsContext.SaveChanges();
                }
                return Json(_localizer["Success"]);
            }
        }

        public async Task<IActionResult> Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<TherapyGoals> ContactInfos)
        {
            {
                foreach (var item in ContactInfos)
                {
                    item.IsDeleted = true;
                    cmsContext.TherapyGoals.Attach(item);
                    cmsContext.Entry(item).State = EntityState.Modified;

                    cmsContext.SaveChanges();
                }
                return Json(_localizer["Success"]);
            }
        }



        public async Task<IActionResult> IndexPlan()
        {
            return View("CenterAdmin/_IndexPlan");
        }

        public ActionResult Read_Plan([DataSourceRequest] DataSourceRequest request, string preferredCulture = "en-US")
        {
            Guid guid = (Guid)_userService.GetMyCenterIdWeb();
            var result = cmsContext.TherapyPlan
                .Where(a => a.MedicalCenterId == guid && !a.IsDeleted)
                .Include(a => a.TherapyPlanTranslations)
                .ToList().ToDataSourceResult(request);
            return Json(result);
        }

        public async Task<IActionResult> Create_Plan()
        {

            return View("CenterAdmin/_Create_Plan");
        }


        [HttpPost]
        public async Task<IActionResult> Create_Plan(TherapyPlan model,List<TherapyGoals> SelectedGoals)
        {
            Guid guid = (Guid)_userService.GetMyCenterIdWeb();

            //var existingTherapyGoals = cmsContext.TherapyGoals
            //    .Where(a => SelectedGoals.Select(a => a.Id).ToList().Contains(a.Id))
            //    .ToList();


            model.MedicalCenterId = guid;

            //model.TherapyGoals = existingTherapyGoals;

            cmsContext.TherapyPlan.Add(model);
            cmsContext.SaveChanges(true);

            foreach (var item in SelectedGoals)
            {
                cmsContext.TherapyGoalsTherapyPlan.Add(
                    new TherapyGoalsTherapyPlan
                    {
                        TherapyGoalsId = item.Id,
                        TherapyPlanId = model.Id,
                    });

                cmsContext.SaveChanges();
            }

            return RedirectToAction("IndexPlan");
            
        }




        public async Task<IActionResult> Edit_Plan(Guid id)
        {
            Guid guid = (Guid)_userService.GetMyCenterIdWeb();

            TherapyPlan model = cmsContext.TherapyPlan
                .Include(a => a.TherapyPlanTranslations)
                .Include(a => a.TherapyGoals).ThenInclude(a => a.TherapyGoalsTranslations)
                .FirstOrDefault(a=>a.Id==id);

            return View("CenterAdmin/_Edit_Plan", model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit_Plan(TherapyPlan model, List<TherapyGoals> SelectedGoals)
        {

            cmsContext.TherapyPlanTranslations.Attach(model.TherapyPlanTranslations[0]);
            cmsContext.Entry(model.TherapyPlanTranslations[0]).State = EntityState.Modified;
            cmsContext.SaveChanges();

            cmsContext.TherapyPlanTranslations.Attach(model.TherapyPlanTranslations[1]);
            cmsContext.Entry(model.TherapyPlanTranslations[1]).State = EntityState.Modified;
            cmsContext.SaveChanges();

            cmsContext.TherapyPlan.Attach(model);
            cmsContext.Entry(model).State = EntityState.Modified;
            cmsContext.SaveChanges();

            List<TherapyGoalsTherapyPlan> therapyGoalsTherapyPlans=cmsContext.TherapyGoalsTherapyPlan
                .Where(a=>a.TherapyPlanId==model.Id).ToList();

            foreach (var item in therapyGoalsTherapyPlans)
            {
                if (!SelectedGoals.Select(a=>a.Id).Contains(item.TherapyGoalsId))
                {
                    item.IsDeleted = true;
                    cmsContext.TherapyGoalsTherapyPlan.Attach(item);
                    cmsContext.Entry(item).State = EntityState.Modified;
                    cmsContext.SaveChanges();
                }
                else
                {
                    item.IsDeleted = false;
                    cmsContext.TherapyGoalsTherapyPlan.Attach(item);
                    cmsContext.Entry(item).State = EntityState.Modified;
                    cmsContext.SaveChanges();

                }
            }

            foreach (var item in SelectedGoals)
            {
                if (!therapyGoalsTherapyPlans.Select(a => a.TherapyGoalsId).Contains(item.Id))
                {
                    cmsContext.TherapyGoalsTherapyPlan.Add(
                        new TherapyGoalsTherapyPlan
                        {
                            TherapyGoalsId = item.Id,
                            TherapyPlanId= model.Id,

                        }
                        );
                    cmsContext.SaveChanges();
                }
            }


            return RedirectToAction("IndexPlan");

        }




        public async Task<IActionResult> Destroy_Plan([DataSourceRequest] DataSourceRequest request, TherapyPlan item)
        {
            {
                //foreach (var item in ContactInfos)
                {

                    item.IsDeleted = true;
                    cmsContext.TherapyPlan.Attach(item);
                    cmsContext.Entry(item).State = EntityState.Modified;
                    cmsContext.SaveChanges();

                    //item.SoftDelte();
                }
                return Json(_localizer["Success"]);
            }
        }



    }

}

