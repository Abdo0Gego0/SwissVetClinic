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
using System;

namespace CmsWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PatientController : Controller
    {
        private readonly IStringLocalizer<CmsResources.Messages> _localizer;
        private readonly ILogger<PatientController> _logger;
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

        public PatientController(
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
            ILogger<PatientController> logger)
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



        public async Task<IActionResult> IndexPatient()
        {

            ViewBag.DispalyName = _localizer["Center Patients"];
            ViewBag.PreviousActionDispalyName = _localizer["Patients"];
            ViewBag.PreviousAction = "IndexPatient";

            return View("Admin/_IndexPatient");
        }

        public ActionResult Read_Patient([DataSourceRequest] DataSourceRequest request, string preferredCulture = "en-US")
        {
            var result = cmsContext.PetOwner
                .Where(a => !a.IsDeleted)
                .ToList().ToDataSourceResult(request);
            return Json(result);
        }

        public async Task<IActionResult> Create_Patient()
        {

            ViewBag.DispalyName = _localizer["New Patient"];
            ViewBag.PreviousActionDispalyName = _localizer["Patients"];
            ViewBag.PreviousAction = "IndexPatient";

            ViewBag.Password = PasswordUtil.GenerateRandomPassword();
            ViewBag.ErrorMessage = "";
            return View("Admin/_Create_Patient");
        }


        [HttpPost]
        public async Task<IActionResult> Create_Patient(PetOwner model)
        {

            ViewBag.DispalyName = _localizer["New Patient"];
            ViewBag.PreviousActionDispalyName = _localizer["Patients"];

            ViewBag.PreviousAction = "IndexPatient";

            string res = personService.AddNewPatientFromDashBoard(model);

            if (res.IsNullOrEmpty())
            {
                PetOwner doc = cmsContext.PetOwner.Include(a => a.User).FirstOrDefault(a => a.User.UserName == model.PersonUserName);
                return RedirectToAction("Create_Patient_Records", new { id = doc.Id });
            }
            else
            {
                ViewBag.ErrorMessage = res;
                return View("Admin/_Create_Patient", model);
            }

        }


        public async Task<IActionResult> Edit_Patient(Guid id)
        {
            ViewBag.DispalyName = _localizer["Edit Patient"];
            ViewBag.PreviousActionDispalyName = _localizer["Patients"];
            ViewBag.PreviousAction = "IndexPatient";



            return View("Admin/_Edit_Patient", personService.GetPatienById(id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit_Patient(PetOwner model)
        {
            ViewBag.DispalyName = _localizer["Edit Patient"];
            ViewBag.PreviousActionDispalyName = _localizer["Patients"];
            ViewBag.PreviousAction = "IndexPatient";


            if (model.ImageFile != null)
            {
                model.ImageName = FileHandler.UpdateImageFile(model.ImageName, model.ImageFile);
            }



            PetOwner centerTutor = cmsContext.PetOwner
    .Include(a => a.User)
    .FirstOrDefault(a => a.Id == model.Id);

            if (centerTutor.User.Email != model.PersonEmail)
            {
                centerTutor.User.Email = model.PersonEmail;
                var resul1 = await _userManager.UpdateAsync(centerTutor.User);

                if (!resul1.Succeeded)
                {
                    string msg = "";

                    foreach (var item in resul1.Errors)
                    {
                        msg += item.Description + " ";
                    }

                    ViewBag.ErrorMessage = _localizer["MakeSureThatEmailisUnique"];

                    return View(model);
                }

                centerTutor.User.EmailConfirmed = true;
            }

            if (centerTutor.User.UserName != model.PersonUserName)
            {
                centerTutor.User.UserName = model.PersonUserName;
                var resul2 = await _userManager.UpdateAsync(centerTutor.User);

                if (!resul2.Succeeded)
                {
                    string msg = "";
                    foreach (var item in resul2.Errors)
                    {
                        msg += item.Description + " ";
                    }
                    ViewBag.ErrorMessage = _localizer["MakeSureThatUserisUnique"];
                    return View(model);
                }
                centerTutor.User.EmailConfirmed = true;
            }

            if (centerTutor.User.PhoneNumber != model.PersonPhone)
            {
                centerTutor.User.PhoneNumber = model.PersonPhone;
                var resul3 = await _userManager.UpdateAsync(centerTutor.User);
                if (!resul3.Succeeded)
                {
                    string msg = "";
                    foreach (var item in resul3.Errors)
                    {
                        msg += item.Description + " ";
                    }

                    ViewBag.ErrorMessage = "يرجى التأكد أن الهاتف";

                    return View(model);
                }
                centerTutor.User.PhoneNumberConfirmed = true;
            }


            if (model.ImageFile != null)
            {
                string uniqueFileName = FileHandler.UpdateProfileImage(model.ImageFile, model.ImageName);
                centerTutor.ImageName = uniqueFileName;
            }

            centerTutor.FirstName = model.FirstName;
            centerTutor.MiddleName = model.MiddleName;
            centerTutor.LastName = model.LastName;
            centerTutor.MotherName = model.MotherName;
            centerTutor.NationalCardId = model.NationalCardId;
            centerTutor.JobCardNumber = model.JobCardNumber;
            centerTutor.PassportNumber = model.PassportNumber;
            centerTutor.Nationality = model.Nationality;

            centerTutor.PersonEmail = model.PersonEmail;
            centerTutor.PersonPhone = model.PersonPhone;
            centerTutor.PersonUserName = model.PersonUserName;


            centerTutor.BloodType= model.BloodType;
            centerTutor.InsuranceId = model.InsuranceId;
            centerTutor.InsuranceCompany = model.InsuranceCompany;



            cmsContext.SaveChanges();


            cmsContext.PetOwner.Attach(centerTutor);
            cmsContext.Entry(centerTutor).State = EntityState.Modified;

            cmsContext.SaveChanges();




            return RedirectToAction("IndexPatient");

        }

        public async Task<IActionResult> Destroy_Patient([DataSourceRequest] DataSourceRequest request, Doctor item)
        {
            {
                //foreach (var item in ContactInfos)
                {
                    item.DeleteFromDb(); 
                    //item.SoftDelte();
                }
                return Json("Success");
            }
        }


        #region Medical records

        public async Task<IActionResult> Create_Patient_Records(Guid id)
        {
            ViewBag.DispalyName = _localizer["Patient Medical Records"];
            ViewBag.PreviousActionDispalyName = _localizer["Patients"];
            ViewBag.PreviousAction = "IndexPatient";

            ViewBag.Id=id ;

            return View("Admin/_Create_Patient_Records");
        }

        public async Task<IActionResult> PatientDiagnosis_Read([DataSourceRequest] DataSourceRequest request, Guid id)
        {
            var result = cmsContext.PatientDiagnosis
            .Where(a => a.PetOwnerId == id)
            .ToList().ToDataSourceResult(request);
            return Json(result);
        }


        public async Task<IActionResult> Create_PatientDiagnosis([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PatientDiagnosis> ContactInfos,Guid id)
        {

            //if (ModelState.IsValid)
            {
                foreach (var item in ContactInfos)
                {
                    cmsContext.PatientDiagnosis.Add(
                        new PatientDiagnosis
                        {
                            PetOwnerId= id,
                            value=item.value,
                            DiagnosisDate= medicalCenterService.ConvertToLocalTime(item.DiagnosisDate),
                        }
                        );
                }
                cmsContext.SaveChanges();
                return Json(_localizer["Success"]);

            }
        }


        [AcceptVerbs("Post")]
        public async Task<IActionResult> Update_PatientDiagnosis([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PatientDiagnosis> clinicSpecialties)
        {
            {
                foreach (var updatedItem in clinicSpecialties)
                {
                    cmsContext.PatientDiagnosis.Attach(updatedItem);
                    cmsContext.Entry(updatedItem).State = EntityState.Modified;

                    cmsContext.SaveChanges();
                }
                return Json(_localizer["Success"]);
            }
        }

        public async Task<IActionResult> Destroy_PatientDiagnosis([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PatientDiagnosis> ContactInfos)
        {
            {
                foreach (var item in ContactInfos)
                {
                    cmsContext.PatientDiagnosis.Remove(item);
                    cmsContext.SaveChanges();
                }
                return Json(_localizer["Success"]);

            }
        }



        public async Task<IActionResult> PatientAllergy_Read([DataSourceRequest] DataSourceRequest request, Guid id)
        {
            var result = cmsContext.PatientAllergy
            .Where(a => a.PetOwnerId == id)
            .ToList().ToDataSourceResult(request);
            return Json(result);
        }
        public async Task<IActionResult> Create_PatientAllergy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PatientAllergy> ContactInfos, Guid id)
        {

            //if (ModelState.IsValid)
            {
                foreach (var item in ContactInfos)
                {
                    cmsContext.PatientAllergy.Add(
                        new PatientAllergy
                        {
                            PetOwnerId = id,
                            value = item.value,
                        }
                        );
                }
                cmsContext.SaveChanges();
                return Json(_localizer["Success"]);

            }
        }
        [AcceptVerbs("Post")]
        public async Task<IActionResult> Update_PatientAllergy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PatientAllergy> clinicSpecialties)
        {
            {
                foreach (var updatedItem in clinicSpecialties)
                {
                    cmsContext.PatientAllergy.Attach(updatedItem);
                    cmsContext.Entry(updatedItem).State = EntityState.Modified;

                    cmsContext.SaveChanges();
                }
                return Json(_localizer["Success"]);

            }
        }
        public async Task<IActionResult> Destroy_PatientAllergy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PatientAllergy> ContactInfos)
        {
            {
                foreach (var item in ContactInfos)
                {
                    cmsContext.PatientAllergy.Remove(item);
                    cmsContext.SaveChanges();
                }
                return Json(_localizer["Success"]);

            }
        }



        public async Task<IActionResult> PatientFamilyHistory_Read([DataSourceRequest] DataSourceRequest request, Guid id)
        {
            var result = cmsContext.PatientFamilyHistory
            .Where(a => a.PetOwnerId == id)
            .ToList().ToDataSourceResult(request);
            return Json(result);
        }
        public async Task<IActionResult> Create_PatientFamilyHistory([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PatientFamilyHistory> ContactInfos, Guid id)
        {

            //if (ModelState.IsValid)
            {
                foreach (var item in ContactInfos)
                {
                    cmsContext.PatientFamilyHistory.Add(
                        new PatientFamilyHistory
                        {
                            PetOwnerId = id,
                            value = item.value,
                        }
                        );
                }
                cmsContext.SaveChanges();
                return Json(_localizer["Success"]);

            }
        }
        [AcceptVerbs("Post")]
        public async Task<IActionResult> Update_PatientFamilyHistory([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PatientFamilyHistory> clinicSpecialties)
        {
            {
                foreach (var updatedItem in clinicSpecialties)
                {
                    cmsContext.PatientFamilyHistory.Attach(updatedItem);
                    cmsContext.Entry(updatedItem).State = EntityState.Modified;

                    cmsContext.SaveChanges();
                }
                return Json(_localizer["Success"]);

            }
        }
        public async Task<IActionResult> Destroy_PatientFamilyHistory([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PatientFamilyHistory> ContactInfos)
        {
            {
                foreach (var item in ContactInfos)
                {
                    cmsContext.PatientFamilyHistory.Remove(item);
                    cmsContext.SaveChanges();
                }
                return Json(_localizer["Success"]);

            }
        }


        public async Task<IActionResult> PatientMedicalHistory_Read([DataSourceRequest] DataSourceRequest request, Guid id)
        {
            var result = cmsContext.PatientMedicalHistory
            .Where(a => a.PetOwnerId == id)
            .ToList().ToDataSourceResult(request);
            return Json(result);
        }
        public async Task<IActionResult> Create_PatientMedicalHistory([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PatientMedicalHistory> ContactInfos, Guid id)
        {

            //if (ModelState.IsValid)
            {
                foreach (var item in ContactInfos)
                {
                    cmsContext.PatientMedicalHistory.Add(
                        new PatientMedicalHistory
                        {
                            PetOwnerId = id,
                            value = item.value,
                        }
                        );
                }
                cmsContext.SaveChanges();
                return Json(_localizer["Success"]);

            }
        }
        [AcceptVerbs("Post")]
        public async Task<IActionResult> Update_PatientMedicalHistory([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PatientMedicalHistory> clinicSpecialties)
        {
            {
                foreach (var updatedItem in clinicSpecialties)
                {
                    cmsContext.PatientMedicalHistory.Attach(updatedItem);
                    cmsContext.Entry(updatedItem).State = EntityState.Modified;

                    cmsContext.SaveChanges();
                }
                return Json(_localizer["Success"]);

            }
        }
        public async Task<IActionResult> Destroy_PatientMedicalHistory([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PatientMedicalHistory> ContactInfos)
        {
            {
                foreach (var item in ContactInfos)
                {
                    cmsContext.PatientMedicalHistory.Remove(item);
                    cmsContext.SaveChanges();
                }
                return Json(_localizer["Success"]);

            }
        }







        public async Task<IActionResult> PatientMedicineHistory_Read([DataSourceRequest] DataSourceRequest request, Guid id)
        {
            var result = cmsContext.PatientMedicineHistory
            .Where(a => a.PetOwnerId == id)
            .ToList().ToDataSourceResult(request);
            return Json(result);
        }
        public async Task<IActionResult> Create_PatientMedicineHistory([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PatientMedicineHistory> ContactInfos, Guid id)
        {

            //if (ModelState.IsValid)
            {
                foreach (var item in ContactInfos)
                {
                    cmsContext.PatientMedicineHistory.Add(
                        new PatientMedicineHistory
                        {
                            PetOwnerId = id,
                            value = item.value,
                        }
                        );
                }
                cmsContext.SaveChanges();
                return Json(_localizer["Success"]);

            }
        }
        [AcceptVerbs("Post")]
        public async Task<IActionResult> Update_PatientMedicineHistory([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PatientMedicineHistory> clinicSpecialties)
        {
            {
                foreach (var updatedItem in clinicSpecialties)
                {
                    cmsContext.PatientMedicineHistory.Attach(updatedItem);
                    cmsContext.Entry(updatedItem).State = EntityState.Modified;

                    cmsContext.SaveChanges();
                }
                return Json(_localizer["Success"]);

            }
        }
        public async Task<IActionResult> Destroy_PatientMedicineHistory([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PatientMedicineHistory> ContactInfos)
        {
            {
                foreach (var item in ContactInfos)
                {
                    cmsContext.PatientMedicineHistory.Remove(item);
                    cmsContext.SaveChanges();
                }
                return Json(_localizer["Success"]);

            }
        }



        public async Task<IActionResult> PatientSurgicalHistory_Read([DataSourceRequest] DataSourceRequest request, Guid id)
        {
            var result = cmsContext.PatientSurgicalHistory
            .Where(a => a.PetOwnerId == id)
            .ToList().ToDataSourceResult(request);
            return Json(result);
        }
        public async Task<IActionResult> Create_PatientSurgicalHistory([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PatientSurgicalHistory> ContactInfos, Guid id)
        {

            //if (ModelState.IsValid)
            {
                foreach (var item in ContactInfos)
                {
                    cmsContext.PatientSurgicalHistory.Add(
                        new PatientSurgicalHistory
                        {
                            PetOwnerId = id,
                            value = item.value,
                        }
                        );
                }
                cmsContext.SaveChanges();
                return Json(_localizer["Success"]);

            }
        }
        [AcceptVerbs("Post")]
        public async Task<IActionResult> Update_PatientSurgicalHistory([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PatientSurgicalHistory> clinicSpecialties)
        {
            {
                foreach (var updatedItem in clinicSpecialties)
                {
                    cmsContext.PatientSurgicalHistory.Attach(updatedItem);
                    cmsContext.Entry(updatedItem).State = EntityState.Modified;

                    cmsContext.SaveChanges();
                }
                return Json(_localizer["Success"]);

            }
        }
        public async Task<IActionResult> Destroy_PatientSurgicalHistory([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PatientSurgicalHistory> ContactInfos)
        {
            {
                foreach (var item in ContactInfos)
                {
                    cmsContext.PatientSurgicalHistory.Remove(item);
                    cmsContext.SaveChanges();
                }
                return Json(_localizer["Success"]);

            }
        }


        #endregion


    }

}

