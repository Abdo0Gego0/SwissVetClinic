using CmsDataAccess.DbModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using ServicesLibrary.MedicalCenterServices;
using ServicesLibrary.UserServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ServicesLibrary.SelectListServices
{

    public interface ISelectListService
    {
        public SelectList PaymentStatusSL();
        public SelectList PaymentTypeSL();

        public List<object> AppointmentTypeList();
        public SelectList AppointmentTypeSL();
        public SelectList TreatmentDurationSL();
        public SelectList CenterMedicineSL();
        public SelectList CenterMedicineUnitSL();
        public SelectList WeightSL();

        public SelectList TemperatureSL();
        public SelectList SexSL();

        public SelectList OrderStatusSL(int? minValue=0);
        public SelectList SubProductSL();
        public SelectList ProductStatusSL();

        public SelectList ServiceTypeSL();
        public SelectList TpGoalStausSL();
        public SelectList EmployeeRoleSL();
        public SelectList EmployeeSL();

        public List<object> AppointmentStatusList();
        public SelectList AppointmentStatusSL();
        public SelectList BloodTypeSL();
        public SelectList PaidSL();
        public SelectList PaidAsBoolSL();
        public string BloodTypeSL(int bloodTyep);
        public SelectList CentersSL();
        public SelectList DoctorSepcialtySL();
        public SelectList DoctorSL();
        public SelectList ClinicSepcialtySL();
        public SelectList DaysSL();
        public SelectList TimeZonesSL();
        public SelectList PriceRecuencyIntervalSL();
        public SelectList ApplicationStatusSL();

        public SelectList ClinicsSL();
        public SelectList SystemClinicsSL();
        public SelectList PatientsSL();


        public SelectList CategoriesSL();
    }

    public class SelectListService: ISelectListService
    {
        private readonly IStringLocalizer<CmsResources.Messages> _localizer;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMedicalCenterService medicalCenterService;
        private readonly IUserService _userService;

        private readonly ApplicationDbContext cmsContext;
        private readonly UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public SelectListService(
            IStringLocalizer<CmsResources.Messages> localizer,

            IUserService userService,

            RoleManager<IdentityRole> roleManager,
            IMedicalCenterService medicalCenterService_,
            UserManager<IdentityUser> userManager,
            IHttpContextAccessor httpContextAccessor, ApplicationDbContext _cmsDbContext)
        {
            medicalCenterService = medicalCenterService_;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            cmsContext = _cmsDbContext;
            _roleManager = roleManager;
            _userService = userService;

            _localizer = localizer;

        }


        public SelectList AppointmentTypeSL()
        {
            return new SelectList(new List<object>
            {
                new { Id=0,Name=_localizer["From Dashboard"]},
                new { Id=1,Name=_localizer["From Mobile"]}
            }
, "Id", "Name");

        }

        public SelectList WeightSL()
        {
            return new SelectList(Enumerable.Range(1,250).Select(a=> new {Id=a, Name=$"{a} Kg"})
, "Id", "Name");

        }

        public SelectList TreatmentDurationSL()
        {
            return new SelectList(Enumerable.Range(1,52).Select(a=> new {Id=a, Name=$"{a} Week"})
, "Id", "Name");

        }

        public SelectList CenterMedicineSL()
        {
            return new SelectList(cmsContext.CenterMedicineList.Where(a=>!a.IsDeleted).ToList().Select(a=> new { Id=a.Id,Name=a.Name})
, "Id", "Name");

        }


        public SelectList CenterMedicineUnitSL()
        {
            return new SelectList(cmsContext.CenterMedicineUnit.Where(a => !a.IsDeleted).ToList().Select(a => new { Id = a.Id, Name = a.SmallestDose })
, "Id", "Name");

        }

        public SelectList TemperatureSL()
        {
            return new SelectList(Enumerable.Range(10, 75).Select(a => new { Id = a, Name = $"{a} C" })
, "Id", "Name");

        }
        public SelectList SexSL()
        {
            return new SelectList(new List<object>
            {
                new { Id=0,Name=_localizer["Male"]},
                new { Id=1,Name=_localizer["Female"]},
                new { Id=2,Name=_localizer["F-Spayed"]},
                new { Id=3,Name=_localizer["M– Neutered"]},
                new { Id=4,Name=_localizer["M/Cryptorchid"]},
            }
, "Id", "Name");

        }


        public SelectList OrderStatusSL(int? minValue = 0)
        {

            switch (minValue)
            {
                case 0:
                    return new SelectList(new List<object>
            {
                new { Id=0,Name="Intialized"},
                new { Id=1,Name="Waiting"},
                new { Id=2,Name="In Progress"},
                new { Id=3,Name="On the Way"},
                new { Id=4,Name="Done"},
            }
, "Id", "Name");
                    break;

                case 1:
                    return new SelectList(new List<object>
            {
                new { Id=1,Name="Waiting"},
                new { Id=2,Name="In Progress"},
                new { Id=3,Name="On the Way"},
                new { Id=4,Name="Done"},
            }
, "Id", "Name");
                    break;

                case 2:
                    return new SelectList(new List<object>
            {
                new { Id=2,Name="In Progress"},
                new { Id=3,Name="On the Way"},
                new { Id=4,Name="Done"},
            }
, "Id", "Name");
                    break;

                case 3:
                    return new SelectList(new List<object>
            {
                new { Id=3,Name="On the Way"},
                new { Id=4,Name="Done"},
            }
, "Id", "Name");
                    break;

                case 4:
                    return new SelectList(new List<object>
            {
                new { Id=4,Name="Done"},
            }
, "Id", "Name");
                    break;

                default:
                    break;
            }

            return new SelectList(new List<object>
            {
                new { Id=0,Name="Intialized"},
                new { Id=1,Name="Paid"},
                new { Id=2,Name="In Progress"},
                new { Id=3,Name="On the Way"},
                new { Id=4,Name="Done"},
            }
, "Id", "Name");

        }


        public SelectList SubProductSL()
        {
            Guid guid = (Guid)_userService.GetMyCenterIdWeb();

            return new SelectList( cmsContext.SubProduct.Select(a=> new { Id=a.Id,Name=a.ParentProductName})
, "Id", "Name");

        }

        public SelectList ProductStatusSL()
        {

            return new SelectList(new List<object>
            {
                new { Id=0,Name="Not Active"},
                new { Id=1,Name="Active"},


            }
, "Id", "Name");

        }

        public SelectList ServiceTypeSL()
        {
            Guid guid = (Guid)_userService.GetMyCenterIdWeb();

            return new SelectList(cmsContext.CenterServices.Where(a => a.MedicalCenterId == guid).Include(a => a.CenterServicesTranslation)
                .Select(a => new { Id = a.Id, Name =  a.Price.ToString() + " AED"+" - "+ a.CenterServicesTranslation[0].Name + " " + a.CenterServicesTranslation[1].Name  })
            , "Id", "Name");

        }

        public SelectList TpGoalStausSL()

        {
            return new SelectList(new List<object>
            {
                new { Id=0,Name="No"},
                new { Id=1,Name="Yes"}
            }
, "Id", "Name");

        }

        public SelectList EmployeeRoleSL()

        {
            return new SelectList(new List<object>
            {
                new { Id=0,Name=_localizer["Reception"]},
                new { Id=1,Name=_localizer["OrdersManagement"]}
            }
, "Id", "Name");

        }

        public SelectList EmployeeSL()

        {
            return new SelectList(cmsContext.Employee.Include(a=>a.User).Where(a=>a.MedicalCenterId==(Guid)_userService.GetMyCenterIdWeb())
                .Select(a=> new { Id=a.User.Id, Name=a.FullName})
, "Id", "Name");

        }
        public List<object> AppointmentTypeList()
        {
            return new  List<object>
            {
                new { Id=0,Name="Diagnostic visit"},
                new { Id=1,Name="Periodic visit"}
            };

        }


        public SelectList AppointmentStatusSL()
        {
            return new SelectList(new List<object>
            {
                new { Id=0,Name="Done"},
                new { Id=1,Name="Missed"},
                new { Id=2,Name="Processing"},
                new { Id=3,Name=" Upcoming"},
            }
, "Id", "Name");

        }
        public List<object> AppointmentStatusList()
        {
            return new List<object>
            {
                new { Id=0,Name="Done"},
                new { Id=1,Name="Missed"},
                new { Id=2,Name="Processing"},
                new { Id=3,Name=" Upcoming"},
            };

        }

        public SelectList ClinicsSL()
        {
            try
            {


                Guid guid = (Guid)_userService.GetMyCenterIdWeb();

                return new SelectList(cmsContext.BaseClinic.Where(a=>a.MedicalCenterId==guid).Include(a => a.BaseClinicTranslation)
                    .Select(a => new { Id = a.Id, Name =  a.BaseClinicTranslation[0].Name + " " + a.BaseClinicTranslation[1].Name })
                , "Id", "Name");
            }
            catch
            {

                return new SelectList(cmsContext.BaseClinic.Include(a => a.BaseClinicTranslation)
                    .Select(a => new { Id = a.Id, Name = a.BaseClinicTranslation[0].Name + " " + a.BaseClinicTranslation[1].Name })
                , "Id", "Name");
            }
        }


        public SelectList CategoriesSL()

        {
            try
            {


                Guid guid = (Guid)_userService.GetMyCenterIdWeb();

                return new SelectList(cmsContext.ProductCategories.Where(a=>a.MedicalCenterId==guid).Include(a => a.ProductCategoriesTranslation)
                    .Select(a => new { Id = a.Id, Name =  a.ProductCategoriesTranslation[0].Name + " " + a.ProductCategoriesTranslation[1].Name })
                , "Id", "Name");
            }
            catch
            {

                return new SelectList(cmsContext.ProductCategories.Include(a => a.ProductCategoriesTranslation)
                    .Select(a => new { Id = a.Id, Name = a.ProductCategoriesTranslation[0].Name + " " + a.ProductCategoriesTranslation[1].Name })
                , "Id", "Name");
            }
        }    
        public SelectList SystemClinicsSL()
        {
            return new SelectList(cmsContext.BaseClinic.Include(a => a.BaseClinicTranslation)
     .Select(a => new { Id = a.Id, Name = a.BaseClinicTranslation[0].Name + " " + a.BaseClinicTranslation[1].Name })
 , "Id", "Name");
        }

        public SelectList PatientsSL()
        {
            Guid guid = (Guid)_userService.GetMyCenterIdWeb();

            return new SelectList(cmsContext.PetOwner
                .Select(a => new { Id = a.Id, Name = a.FirstName + " " + a.LastName })
            , "Id", "Name");
        }

        public SelectList BloodTypeSL()
        {
            return new SelectList(new List<object>
            {
                new { Id=0,Name="A+"},
                new { Id=1,Name="A-"},
                new { Id=2,Name="B+"},
                new { Id=3,Name="B-"},
                new { Id=4,Name="AB+"},
                new { Id=5,Name="AB-"},
                new { Id=6,Name="O+"},
                new { Id=7,Name="O-"},
            }
, "Id", "Name");
        }
        public SelectList PaidSL()
        {
            return new SelectList(new List<object>
            {
                new { Id=0,Name=_localizer["All"]},
                new { Id=1,Name=_localizer["Paid"]},
                new { Id=2,Name=_localizer["Not Paid"]},

            }
, "Id", "Name");
        }

        public SelectList PaymentStatusSL()
        {
            return new SelectList(new List<object>
            {
                new { Id=0,Name=_localizer["Not Paid"]},
                new { Id=1,Name=_localizer["Paid"]},
            }
, "Id", "Name");
        }


        public SelectList PaymentTypeSL()
        {
            return new SelectList(new List<object>
            {
                new { Id=0,Name=_localizer["Cash"]},
                new { Id=1,Name=_localizer["Online"]},
            }
, "Id", "Name");
        }



        public SelectList PaidAsBoolSL()

        {
            return new SelectList(new List<object>
            {
                new { Id=1,Name=_localizer["Paid"]},
                new { Id=0,Name=_localizer["Not Paid"]},

            }
, "Id", "Name");
        }


        public string BloodTypeSL(int bloodTyep)
        {
            List<string> list = new List<string> {
            "A+",
            "A-",
            "B+",
            "B-",
            "AB+",
            "AB-",
            "O+",
            "O-",
            };


            return list[bloodTyep];
        }

        public SelectList CentersSL()
        {
            return new SelectList(cmsContext.MedicalCenter.Include(a=>a.MedicalCenterTranslation)
                .Select(a=> new { Id=a.Id, Name=a.MedicalCenterTranslation[0].Name+ " "+ a.MedicalCenterTranslation[1].Name })
            , "Id", "Name");
        }

        public SelectList DaysSL()
        {
            return new SelectList(new List<object>
            { 
                new { Id=1,Name="Monday"},
                new { Id=2,Name="Tuesday"},
                new { Id=3,Name="Wednesday"},
                new { Id=4,Name="Thursday"},
                new { Id=5,Name="Friday"},
                new { Id=6,Name="Saturday"},
                new { Id=0,Name="Sunday"},
            }
            , "Id", "Name");
        }


        public SelectList DoctorSepcialtySL()
        {
            try
            {
                Guid guid = (Guid)_userService.GetMyCenterIdWeb();

                return new SelectList(cmsContext.DoctorSpeciality
                    .Where(a => a.MedicalCenterId == guid)
                    .Include(a => a.DoctorSpecialityTranslation)
                    .Select(a => new { Id = a.Id, Name = a.DoctorSpecialityTranslation[0].Description + " " + a.DoctorSpecialityTranslation[1].Description }), "Id", "Name"
                    );

            }
            catch
            {
                return new SelectList(cmsContext.DoctorSpeciality
    .Include(a => a.DoctorSpecialityTranslation)
    .Select(a => new { Id = a.Id, Name = a.DoctorSpecialityTranslation[0].Description + " " + a.DoctorSpecialityTranslation[1].Description }), "Id", "Name"
    );
            }
        }   
        
        public SelectList DoctorSL()
        {
            try
            {
                Guid guid = (Guid)_userService.GetMyCenterIdWeb();

                return new SelectList(cmsContext.Doctor
                    .Where(a => a.MedicalCenterId == guid)
                    .Include(a => a.DoctorTranslation)
                    .Select(a => new { Id = a.Id, Name = a.DoctorTranslation[0].Name + " " + a.DoctorTranslation[1].Name }), "Id", "Name"
                    );
            }
            catch
            {

                return new SelectList(cmsContext.Doctor
                    .Include(a => a.DoctorTranslation)
                    .Select(a => new { Id = a.Id, Name = a.DoctorTranslation[0].Name + " " + a.DoctorTranslation[1].Name }), "Id", "Name"
                    );
            }
        }

        public SelectList ClinicSepcialtySL()
        {
            try
            {

                Guid guid = (Guid)_userService.GetMyCenterIdWeb();

                return new SelectList(cmsContext.ClinicSpecialty
                    .Where(a => a.MedicalCenterId == guid)
                    .Include(a => a.ClinicSpecialtyTranslation)
                    .Select(a => new { Id = a.Id, Name = a.ClinicSpecialtyTranslation[0].Name + " " + a.ClinicSpecialtyTranslation[1].Name }), "Id", "Name"
                    );
            }
            catch
            {

                return new SelectList(cmsContext.ClinicSpecialty
                    .Include(a => a.ClinicSpecialtyTranslation)
                    .Select(a => new { Id = a.Id, Name = a.ClinicSpecialtyTranslation[0].Name + " " + a.ClinicSpecialtyTranslation[1].Name }), "Id", "Name"
                    );
            }
        }


        public SelectList TimeZonesSL()
        {
            return new SelectList(cmsContext.SystemTimeZone
                .Select(a => new { Id = a.Id, Name = a.StandardName + " " + a.DisplayName }), "Id", "Name"
                );
        }

        public SelectList PriceRecuencyIntervalSL()
        {
            return new SelectList(new List<object> { new {Id="month" , Name = "Monthly" },new { Id = "year", Name = "Yearly" } }, "Id", "Name");
        }


        public SelectList ApplicationStatusSL()
        {
            return new SelectList(new List<object> 

            {
                new { Id = "-2", Name = "Rejected" },
                new { Id = "-1", Name = "Resubmit" },
                new { Id = "0", Name = "Waiting" } ,
                new { Id = "1", Name = "Approved" } ,
            }, "Id", "Name");
        }



    }
}
