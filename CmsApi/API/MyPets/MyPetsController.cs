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
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CmsApi.API.MyPets
{
    //[ApiController]
    [Route("[controller]")]
    public class MyPetsController : ControllerBase
    {
		private readonly IStringLocalizer<Messages> _localizer;

        private RoleManager<IdentityRole> _roleManager;

        private readonly ILogger<MyPetsController> _logger;

		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IUserStore<IdentityUser> _userStore;
		private readonly IUserEmailStore<IdentityUser> _emailStore;

		private readonly IEmailSender _emailSender;

		private readonly IConfiguration _config;
		private readonly IUserService _userService;
		private readonly ApplicationDbContext cmsContext;

		public MyPetsController(
            RoleManager<IdentityRole> roleManager,
                        IStringLocalizer<Messages> localizer,
			IConfiguration config,
			IUserService userService,
			ApplicationDbContext _cMDbContext,

			UserManager<IdentityUser> userManager,
			IUserStore<IdentityUser> userStore,
			SignInManager<IdentityUser> signInManager,
			IEmailSender emailSender,

			ILogger<MyPetsController> logger)
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
            _roleManager= roleManager;


        }

        #region Animal

        [HttpPost("add-new-animal"),Authorize]
        public async Task<ActionResult> AddNewAnimal( string PetName,  string PetType,IFormFile? image,DateTime BirthDate, string? LifeStyle = "", string? Breed="",  string? BloodType="")
        {
            Pet model = new Pet
			{
				PetName = PetName,
				PetOwnerId = (Guid)_userService.GetMyId(),
				BloodType = BloodType,
				PetType= PetType,
                LifeStyle=LifeStyle,
                Breed=Breed,
                BirthDate=BirthDate,
                ImageName=FileHandler.SaveUploadedFile(image)
			};

            //if (request.PatientAllergy!=null || request.PatientAllergy.Count!=0)
            //{
            //	model.PatientAllergy = new List<PatientAllergy>();
            //             foreach (var item in request.PatientAllergy)
            //	{
            //		model.PatientAllergy.Add(new PatientAllergy { value=item.value});
            //             }
            //}

            //         if (request.PatientMedicalHistory != null || request.PatientMedicalHistory.Count != 0)
            //         {
            //             model.PatientMedicalHistory = new List<PatientMedicalHistory>();
            //             foreach (var item in request.PatientMedicalHistory)
            //             {
            //                 model.PatientMedicalHistory.Add(new PatientMedicalHistory { value = item.value });
            //             }
            //         }

            //         if (request.PatientMedicineHistory != null || request.PatientMedicineHistory.Count != 0)
            //         {
            //             model.PatientMedicineHistory = new List<PatientMedicineHistory>();
            //             foreach (var item in request.PatientMedicineHistory)
            //             {
            //                 model.PatientMedicineHistory.Add(new PatientMedicineHistory { value = item.value });
            //             }
            //         }

            //         if (request.PatientSurgicalHistory != null || request.PatientSurgicalHistory.Count != 0)
            //         {
            //             model.PatientSurgicalHistory = new List<PatientSurgicalHistory>();
            //             foreach (var item in request.PatientSurgicalHistory)
            //             {
            //                 model.PatientSurgicalHistory.Add(new PatientSurgicalHistory { value = item.value });
            //             }
            //         }

            try
            {
                model.InsertIntoDb();

                return new ObjectResult(new { status = StatusCodes.Status200OK, data = "", message = "Added Successfully" });
            }
            catch(Exception ex) 
            {
                return new ObjectResult(new { status = StatusCodes.Status400BadRequest, data = ex.ToString(), message = "Error" });

            }

        }

        [HttpPost("delete-animal"), Authorize]
        public async Task<ActionResult> DeleteAnimal(Guid PetId)
        {
            Pet pet = cmsContext.Pet.Find(PetId);

            if (cmsContext.Appointment.Any(a=>a.PetId==PetId))
            {
                pet.SoftFromDb();
            }
            else
            {
                pet.DeleteFromDb();
            }


            return new ObjectResult(new { status = StatusCodes.Status200OK, data = "", message = "Deleted Successfully" });

        }

        [HttpGet("get-my-animals"), Authorize]
        public async Task<ActionResult> GetMyAnimals()
        {
            List<Pet> res = cmsContext.Pet.Where(a=>a.PetOwnerId==(Guid)_userService.GetMyId()).ToList();
            return new ObjectResult(new { status = StatusCodes.Status200OK, data = res, message = "User Pets" });
        }

        [HttpGet("get-animal-by-id"), Authorize]
        public async Task<ActionResult> GetAnimalById(Guid PetId)
        {
            Pet res = cmsContext.Pet.Find(PetId).GetFromDb();
            return new ObjectResult(new { status = StatusCodes.Status200OK, data = res, message = "Pets" });
        }

        [HttpPost("update-animal-info"), Authorize]
        public async Task<ActionResult> UpdateAnimalInfo(Guid PetId, string PetName, string PetType, IFormFile? image, DateTime BirthDate, string? LifeStyle = "", string? Breed = "", string? BloodType = "")
        {
            Pet res = cmsContext.Pet.Find(PetId);

            res.PetName = PetName;
            res.PetType= PetType;
            res.BloodType=BloodType;
            res.LifeStyle= LifeStyle;
            res.BirthDate = BirthDate;
            res.Breed=Breed;

            cmsContext.Pet.Attach(res);
            cmsContext.Entry(res).Property(a => a.PetName).IsModified = true;
            cmsContext.Entry(res).Property(a => a.PetType).IsModified = true;
            cmsContext.Entry(res).Property(a => a.BloodType).IsModified = true;
            cmsContext.Entry(res).Property(a => a.LifeStyle).IsModified = true;
            cmsContext.Entry(res).Property(a => a.Breed).IsModified = true;
            cmsContext.Entry(res).Property(a => a.BirthDate).IsModified = true;

            if (image!=null)
            {
                res.ImageName=FileHandler.UpdateImageFile(res.ImageName, image);
                cmsContext.Entry(res).Property(a => a.ImageName).IsModified = true;
            }

            cmsContext.SaveChanges();

            return new ObjectResult(new { status = StatusCodes.Status200OK, data = res, message = "Updated Successfully" });
        }

        #endregion


        #region Allergy

        [HttpPost("add-animal-allergy"), Authorize]
        public async Task<ActionResult> AddAnimalAllergy(Guid PetId,string value)
        {
            PatientAllergy patientAllergy = new PatientAllergy {

                value = value,
                PetId = PetId,
                PetOwnerId = (Guid)_userService.GetMyId()
            };
            cmsContext.PatientAllergy.Add(patientAllergy);
            cmsContext.SaveChanges ();
            return new ObjectResult(new { status = StatusCodes.Status200OK, data = "", message = "Added Successfully" });
        }


        [HttpPost("update-animal-allergy"), Authorize]
        public async Task<ActionResult> UpdateAnimalAllergy(Guid id, string value)
        {
            PatientAllergy patientAllergy = cmsContext.PatientAllergy.Find(id);
            patientAllergy.value= value;
            cmsContext.PatientAllergy.Attach(patientAllergy);
            cmsContext.SaveChanges();
            return new ObjectResult(new { status = StatusCodes.Status200OK, data = "", message = "Updated Successfully" });
        }


        [HttpPost("delete-animal-allergy"), Authorize]
        public async Task<ActionResult> DeleteAnimalAllergy(Guid id)
        {
            PatientAllergy patientAllergy = cmsContext.PatientAllergy.Find(id);
            cmsContext.PatientAllergy.Remove(patientAllergy);
            cmsContext.SaveChanges();
            return new ObjectResult(new { status = StatusCodes.Status200OK, data = "", message = "Deleted Successfully" });
        }



        [HttpGet("get-animal-allergy"), Authorize]
        public async Task<ActionResult> GetAnimalAllergy(Guid PetId)
        {
            return new ObjectResult(new { status = StatusCodes.Status200OK, data = cmsContext.PatientAllergy.Where(a=>a.PetId==PetId).ToList(), message = "Allergy List" });
        }

        #endregion


        #region PatientMedicalHistory

        [HttpPost("add-animal-medicalhistory"), Authorize]
        public async Task<ActionResult> AddAnimalMedicalHistory(Guid PetId, string value)
        {
            PatientMedicalHistory patientAllergy = new PatientMedicalHistory
            {

                value = value,
                PetId = PetId,
                PetOwnerId = (Guid)_userService.GetMyId()
            };
            cmsContext.PatientMedicalHistory.Add(patientAllergy);
            cmsContext.SaveChanges();
            return new ObjectResult(new { status = StatusCodes.Status200OK, data = "", message = "Added Successfully" });
        }


        [HttpPost("update-animal-medicalhistory"), Authorize]
        public async Task<ActionResult> UpdateAnimalMedicalHistory(Guid id, string value)
        {
            PatientMedicalHistory patientAllergy = cmsContext.PatientMedicalHistory.Find(id);
            patientAllergy.value = value;
            cmsContext.PatientMedicalHistory.Attach(patientAllergy);
            cmsContext.SaveChanges();
            return new ObjectResult(new { status = StatusCodes.Status200OK, data = "", message = "Updated Successfully" });
        }


        [HttpPost("delete-animal-medicalhistory"), Authorize]
        public async Task<ActionResult> DeleteAnimalMedicalHistory(Guid id)
        {
            PatientMedicalHistory patientAllergy = cmsContext.PatientMedicalHistory.Find(id);
            cmsContext.PatientMedicalHistory.Remove(patientAllergy);
            cmsContext.SaveChanges();
            return new ObjectResult(new { status = StatusCodes.Status200OK, data = "", message = "Deleted Successfully" });
        }

        [HttpGet("get-animal-medicalhistory"), Authorize]
        public async Task<ActionResult> GetAnimalMedicalHistory(Guid PetId)
        {
            return new ObjectResult(new { status = StatusCodes.Status200OK, data = cmsContext.PatientMedicalHistory.Where(a => a.PetId == PetId).ToList(), message = "Allergy List" });
        }

        #endregion


        #region PatientMedicineHistory

        [HttpPost("add-animal-medicinehistory"), Authorize]
        public async Task<ActionResult> AddAnimalMedicineHistory(Guid PetId, string value)
        {
            PatientMedicineHistory patientAllergy = new PatientMedicineHistory
            {

                value = value,
                PetId = PetId,
                PetOwnerId = (Guid)_userService.GetMyId()
            };
            cmsContext.PatientMedicineHistory.Add(patientAllergy);
            cmsContext.SaveChanges();
            return new ObjectResult(new { status = StatusCodes.Status200OK, data = "", message = "Added Successfully" });
        }


        [HttpPost("update-animal-medicinehistory"), Authorize]
        public async Task<ActionResult> UpdateAnimalMedicineHistory(Guid id, string value)
        {
            PatientMedicineHistory patientAllergy = cmsContext.PatientMedicineHistory.Find(id);
            patientAllergy.value = value;
            cmsContext.PatientMedicineHistory.Attach(patientAllergy);
            cmsContext.SaveChanges();
            return new ObjectResult(new { status = StatusCodes.Status200OK, data = "", message = "Updated Successfully" });
        }


        [HttpPost("delete-animal-medicinehistory"), Authorize]
        public async Task<ActionResult> DeleteAnimalMedicineHistory(Guid id)
        {
            PatientMedicineHistory patientAllergy = cmsContext.PatientMedicineHistory.Find(id);
            cmsContext.PatientMedicineHistory.Remove(patientAllergy);
            cmsContext.SaveChanges();
            return new ObjectResult(new { status = StatusCodes.Status200OK, data = "", message = "Deleted Successfully" });
        }


        [HttpGet("get-animal-medicinehistory"), Authorize]
        public async Task<ActionResult> GetAnimalMedicineHistory(Guid PetId)
        {
            return new ObjectResult(new { status = StatusCodes.Status200OK, data = cmsContext.PatientMedicineHistory.Where(a => a.PetId == PetId).ToList(), message = "Allergy List" });
        }

        #endregion


        #region PatientSurgicalHistory

        [HttpPost("add-animal-surgicalhistory"), Authorize]
        public async Task<ActionResult> AddAnimalSurgicalHistory(Guid PetId, string value)
        {
            PatientSurgicalHistory patientAllergy = new PatientSurgicalHistory
            {

                value = value,
                PetId = PetId,
                PetOwnerId = (Guid)_userService.GetMyId()
            };
            cmsContext.PatientSurgicalHistory.Add(patientAllergy);
            cmsContext.SaveChanges();
            return new ObjectResult(new { status = StatusCodes.Status200OK, data = "", message = "Added Successfully" });
        }


        [HttpPost("update-animal-surgicalhistory"), Authorize]
        public async Task<ActionResult> UpdateAnimalSurgicalHistory(Guid id, string value)
        {
            PatientSurgicalHistory patientAllergy = cmsContext.PatientSurgicalHistory.Find(id);
            patientAllergy.value = value;
            cmsContext.PatientSurgicalHistory.Attach(patientAllergy);
            cmsContext.SaveChanges();
            return new ObjectResult(new { status = StatusCodes.Status200OK, data = "", message = "Updated Successfully" });
        }


        [HttpPost("delete-animal-surgicalhistory"), Authorize]
        public async Task<ActionResult> DeleteAnimalSurgicalHistory(Guid id)
        {
            PatientSurgicalHistory patientAllergy = cmsContext.PatientSurgicalHistory.Find(id);
            cmsContext.PatientSurgicalHistory.Remove(patientAllergy);
            cmsContext.SaveChanges();
            return new ObjectResult(new { status = StatusCodes.Status200OK, data = "", message = "Deleted Successfully" });
        }


        [HttpGet("get-animal-surgicalhistory"), Authorize]
        public async Task<ActionResult> GetAnimalSurgicalHistory(Guid PetId)
        {
            return new ObjectResult(new { status = StatusCodes.Status200OK, data = cmsContext.PatientSurgicalHistory.Where(a => a.PetId == PetId).ToList(), message = "Allergy List" });
        }

        #endregion


    }
}