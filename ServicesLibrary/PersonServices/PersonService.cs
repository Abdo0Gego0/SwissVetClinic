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

namespace ServicesLibrary.PersonServices
{

    public interface IPersonService
    {
        public string AddNewCenterAdmin(CenterAdmin model);

        public bool DeletePetOwner(Guid id);
        public string AddNewCenterAdminWithoutTransaction(CenterAdmin model);

        public string AddNewDoctor(Doctor model);
        public Doctor GetDoctorById(Guid id);

        public string AddNewEmployee(Employee model);
        public Employee GetEmployeeById(Guid id);

        public string AddNewPatientFromDashBoard(PetOwner model);
        public PetOwner GetPatienById(Guid id);
        public Pet GetPetById(Guid id);


    }

    public class PersonService : IPersonService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMedicalCenterService medicalCenterService;
        private readonly ApplicationDbContext cmsContext;
        private readonly UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;

        public PersonService(
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
        }

        #region Employee
        public async Task<string> AddNewEmployeeAsync(Employee model)
        {
            string res = "";
            using (var transaction = cmsContext.Database.BeginTransaction())
            {
                try
                {
                    var user = new IdentityUser
                    {
                        Email = model.PersonEmail,
                        PhoneNumber = model.PersonPhone,
                        UserName = model.PersonUserName,
                        PhoneNumberConfirmed = true,
                        EmailConfirmed = true,
                    };

                    var resul = await _userManager.CreateAsync(user, model.Password);

                    if (!resul.Succeeded)
                    {
                        string msg = "";
                        foreach (var item in resul.Errors)
                        {
                            msg += item.Description + " ";
                        }
                        return "Please make sure that the employee username is not used earlier, and password has capital letter, numbers and special character";
                    }

                    string ImageFileName = FileHandler.SaveUploadedFile(model.ImageFile);
                    string PassportFileName = FileHandler.SaveUploadedFile(model.PassportFile);
                    string FamilyBookFileName = FileHandler.SaveUploadedFile(model.FamillyBookFile);
                    string LaborCardFileName = FileHandler.SaveUploadedFile(model.LaborCardFile);




                    cmsContext.Employee.Add(
                        new Employee
                        {
                            FirstName = model.FirstName,
                            MiddleName = model.MiddleName,
                            LastName = model.LastName,
                            MotherName = model.MotherName,
                            Description = model.Description,
                            Gendre = model.Gendre,
                            Address = model.Address,
                            NationalCardId = model.NationalCardId,
                            JobCardNumber = model.JobCardNumber,
                            PassportNumber = model.PassportNumber,
                            Nationality = model.Nationality,
                            User = user,
                            PersonEmail = model.PersonEmail,
                            PersonPhone = model.PersonPhone,
                            PersonUserName = model.PersonUserName,
                            MedicalCenterId = _userService.GetMyCenterIdWeb(),
                            ImageName = ImageFileName,
                            PassportFileName = PassportFileName,
                            FamilyBookFileName = FamilyBookFileName,
                            LaborCardFileName = LaborCardFileName,
                            CreateDate = medicalCenterService.ConvertToLocalTime(DateTime.Now),
                            EmployeeRole = model.EmployeeRole,


                        }
                        );


                    string empRole = "";

                    if (model.EmployeeRole == 0)
                    {
                        empRole = "reception";
                    }
                    else
                    {
                        empRole = "ordersmanagement";

                    }
                    try
                    {
                        await _roleManager.CreateAsync(new IdentityRole(empRole));
                    }
                    catch
                    {
                    }


                    await _userManager.AddToRoleAsync(user, empRole);

                    cmsContext.SaveChanges();
                    transaction.Commit();
                    return "";
                }

                catch (Exception ex)
                {
                    transaction.Rollback();
                    try
                    {
                        var user1 = await _userManager.FindByNameAsync(model.PersonUserName);
                        _userManager.DeleteAsync(user1);
                    }
                    catch { }
                    return ex.ToString();
                }
            }
        }
        public string AddNewEmployee(Employee model)
        {
            var insertTask = AddNewEmployeeAsync(model);
            insertTask.Wait();

            string res = insertTask.Result;
            return res;

        }

        public async Task<string> UpdateEmployeeAsync(Employee model)
        {
            string res = "";
            return res;
        }
        public string UpdateEmployee(Employee model)
        {
            var res = UpdateEmployeeAsync(model);
            return res.ToString();
        }

        public async Task<string> DeleteEmployeeAsync(Employee model)
        {
            string res = "";
            return res;
        }
        public string DeleteEmployee(Employee model)
        {
            var res = DeleteEmployeeAsync(model);
            return res.ToString();
        }


        public Employee GetEmployeeById(Guid id)
        {
            Employee temp = cmsContext.Employee
                .Include(a => a.User)
                .FirstOrDefault(a => a.Id == id);

            temp.PersonEmail = temp.User.Email;
            temp.PersonPhone = temp.User.PhoneNumber;
            temp.PersonUserName = temp.User.UserName;

            return temp;
        }


        #endregion


        #region Doctor
        public async Task<string> AddNewDoctorAsync(Doctor model)
        {
            string res = "";
            using (var transaction = cmsContext.Database.BeginTransaction())
            {
                try
                {
                    var user = new IdentityUser
                    {
                        Email = model.PersonEmail,
                        PhoneNumber = model.PersonPhone,
                        UserName = model.PersonUserName,
                        PhoneNumberConfirmed = true,
                        EmailConfirmed = true,
                    };

                    var resul = await _userManager.CreateAsync(user, model.Password);

                    if (!resul.Succeeded)
                    {
                        string msg = "";
                        foreach (var item in resul.Errors)
                        {
                            msg += item.Description + " ";
                        }
                        return "Please make sure that the employee username is not used earlier, and password has capital letter, numbers and special character";
                    }

                    string ImageFileName = FileHandler.SaveUploadedFile(model.ImageFile);
                    string PassportFileName = FileHandler.SaveUploadedFile(model.PassportFile);
                    string FamilyBookFileName = FileHandler.SaveUploadedFile(model.FamillyBookFile);
                    string LaborCardFileName = FileHandler.SaveUploadedFile(model.LaborCardFile);

                    if (model.Certificate != null)
                    {
                        foreach (var item in model.Certificate)
                        {
                            item.SaveCertificateImage();
                        }
                    }


                    foreach (var item in model.DoctorSpeciality)
                    {
                        cmsContext.Entry(item).State = EntityState.Unchanged;
                    }

                    cmsContext.Doctor.Add(
                        new Doctor
                        {
                            FirstName = model.FirstName,
                            MiddleName = model.MiddleName,
                            LastName = model.LastName,
                            MotherName = model.MotherName,
                            Description = model.Description,
                            Gendre = model.Gendre,
                            Address = model.Address,
                            NationalCardId = model.NationalCardId,
                            JobCardNumber = model.JobCardNumber,
                            PassportNumber = model.PassportNumber,
                            Nationality = model.Nationality,
                            User = user,
                            PersonEmail = model.PersonEmail,
                            PersonPhone = model.PersonPhone,
                            PersonUserName = model.PersonUserName,
                            MedicalCenterId = _userService.GetMyCenterIdWeb(),
                            ImageName = ImageFileName,
                            PassportFileName = PassportFileName,
                            FamilyBookFileName = FamilyBookFileName,
                            LaborCardFileName = LaborCardFileName,
                            CreateDate = medicalCenterService.ConvertToLocalTime(DateTime.Now),

                            DoctorTranslation = new List<DoctorTranslation>
                            {
                                new DoctorTranslation {LangCode="en-us",Name=model.DoctorTranslation[0].Name,Description=model.DoctorTranslation[0].Description },
                                new DoctorTranslation {LangCode="ar",Name=model.DoctorTranslation[1].Name,Description=model.DoctorTranslation[0].Description },
                            },

                            DoctorSpeciality = model.DoctorSpeciality,
                            ClinicId = model.ClinicId,
                            Certificate = model.Certificate

                        }
                        );

                    try
                    {
                        await _roleManager.CreateAsync(new IdentityRole("doctor"));
                    }
                    catch
                    {
                    }

                    await _userManager.AddToRoleAsync(user, "doctor");



                    cmsContext.SaveChanges();
                    transaction.Commit();
                    return "";
                }

                catch (Exception ex)
                {
                    transaction.Rollback();
                    try
                    {
                        var user1 = await _userManager.FindByNameAsync(model.PersonUserName);
                        _userManager.DeleteAsync(user1);
                    }
                    catch { }
                    return ex.ToString();
                }
            }
        }
        public string AddNewDoctor(Doctor model)
        {
            var insertTask = AddNewDoctorAsync(model);
            insertTask.Wait();

            string res = insertTask.Result;
            return res;

        }







        public async Task<string> UpdateDoctorAsync(Doctor model)
        {
            string res = "";
            return res;
        }
        public string UpdateDoctor(Doctor model)
        {
            var res = UpdateDoctorAsync(model);
            return res.ToString();
        }

        public async Task<string> DeleteDoctorAsync(Doctor model)
        {
            string res = "";
            return res;
        }
        public string DeleteDoctor(Doctor model)
        {
            var res = DeleteDoctorAsync(model);
            return res.ToString();
        }

        public Doctor GetDoctorById(Guid id)
        {
            Doctor temp = cmsContext.Doctor
                .Include(a => a.Certificate)
                .Include(a => a.User)
                .Include(a => a.DoctorSpeciality).ThenInclude(b => b.DoctorSpecialityTranslation)
                .Include(a => a.DoctorTranslation)
                .FirstOrDefault(a => a.Id == id);

            temp.PersonEmail = temp.User.Email;
            temp.PersonPhone = temp.User.PhoneNumber;
            temp.PersonUserName = temp.User.UserName;

            return temp;
        }

        #endregion


        #region Patient

        public PetOwner GetPatienById(Guid id)
        {
            PetOwner temp = cmsContext.PetOwner
    .Include(a => a.User)
    .FirstOrDefault(a => a.Id == id);

            temp.PersonEmail = temp.User.Email;
            temp.PersonPhone = temp.User.PhoneNumber;
            temp.PersonUserName = temp.User.UserName;

            return temp;

        }
        public Pet GetPetById(Guid id)
        {
            Pet temp = cmsContext.Pet
    .Include(a => a.PatientAllergy)
    .Include(a => a.PatientFamilyHistory)
    .Include(a => a.PatientMedicalHistory)
    .Include(a => a.PatientMedicineHistory)
    .Include(a => a.PatientSurgicalHistory)
    .FirstOrDefault(a => a.Id == id);



            return temp;

        }


        public async Task<bool> DeletePetOwnerAsync(Guid Id)
        {
            try
            {
                PetOwner temp = cmsContext.PetOwner
                    .Include(a => a.User)
                    .FirstOrDefault(a => a.Id == Id); ;

                if (cmsContext.Appointment.Any(a => a.PetOwnerId == Id) || cmsContext.COrder.Any(a => a.PetOwnerId == Id))
                {

                    try
                    {

                        //FileHandler.DeleteImageFile(temp.ImageName);

                        temp.IsDeleted = true;
                        cmsContext.PetOwner.Attach(temp);
                        cmsContext.Entry(temp).State = EntityState.Modified;
                        cmsContext.SaveChanges();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }

                }



                FileHandler.DeleteImageFile(temp.ImageName);
                cmsContext.PetOwner.Remove(temp);
                cmsContext.SaveChanges();

                await _userManager.DeleteAsync(temp.User);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool DeletePetOwner(Guid Id)
        {
            var res = DeletePetOwnerAsync(Id);
            return (bool)res.Result;
        }



        public async Task<string> UpdatePatientAsync(PetOwner model)
        {
            string res = "";
            return res;
        }
        public string UpdatePatient(PetOwner model)
        {
            var res = UpdatePatientAsync(model);
            return res.ToString();
        }

        public async Task<string> DeletePatientAsync(PetOwner model)
        {
            string res = "";
            return res;
        }
        public string DeletePatient(PetOwner model)
        {
            var res = DeletePatientAsync(model);
            return res.ToString();
        }






        public async Task<string> AddNewPatientFromDashBoardAsync(PetOwner model)
        {
            string res = "";
            using (var transaction = cmsContext.Database.BeginTransaction())
            {
                try
                {
                    var user = new IdentityUser
                    {
                        Email = model.PersonEmail,
                        PhoneNumber = model.PersonPhone,
                        UserName = model.PersonUserName,
                        PhoneNumberConfirmed = true,
                        EmailConfirmed = true,
                    };

                    var resul = await _userManager.CreateAsync(user, model.Password);

                    if (!resul.Succeeded)
                    {
                        string msg = "";
                        foreach (var item in resul.Errors)
                        {
                            msg += item.Description + " ";
                        }
                        return "Please make sure that the employee username is not used earlier, and password has capital letter, numbers and special character";
                    }

                    string ImageFileName = FileHandler.SaveUploadedFile(model.ImageFile);
                    string PassportFileName = FileHandler.SaveUploadedFile(model.PassportFile);
                    string FamilyBookFileName = FileHandler.SaveUploadedFile(model.FamillyBookFile);
                    string LaborCardFileName = FileHandler.SaveUploadedFile(model.LaborCardFile);





                    cmsContext.PetOwner.Add(
                        new PetOwner
                        {
                            FirstName = model.FirstName,
                            MiddleName = model.MiddleName,
                            LastName = model.LastName,
                            MotherName = model.MotherName,
                            Description = model.Description,
                            Gendre = model.Gendre,
                            Address = model.Address,
                            NationalCardId = model.NationalCardId,
                            JobCardNumber = model.JobCardNumber,
                            PassportNumber = model.PassportNumber,
                            Nationality = model.Nationality,
                            User = user,
                            PersonEmail = model.PersonEmail,
                            PersonPhone = model.PersonPhone,
                            PersonUserName = model.PersonUserName,
                            MedicalCenterId = _userService.GetMyCenterIdWeb(),
                            ImageName = ImageFileName,
                            PassportFileName = PassportFileName,
                            FamilyBookFileName = FamilyBookFileName,
                            LaborCardFileName = LaborCardFileName,
                            CreateDate = medicalCenterService.ConvertToLocalTime(DateTime.Now),

                            InsuranceCompany = model.InsuranceCompany,
                            InsuranceId = model.InsuranceId,
                            BloodType = model.BloodType,
                            GeneralNumber = cmsContext.PetOwner.Where(a => a.MedicalCenterId == cmsContext.MedicalCenter.FirstOrDefault().Id).Count() + 1,

                        }
                        );

                    try
                    {
                        await _roleManager.CreateAsync(new IdentityRole("patient"));
                    }
                    catch
                    {

                    }

                    await _userManager.AddToRoleAsync(user, "patient");

                    cmsContext.SaveChanges();
                    transaction.Commit();
                    return "";
                }

                catch (Exception ex)
                {
                    transaction.Rollback();
                    try
                    {
                        var user1 = await _userManager.FindByNameAsync(model.PersonUserName);
                        _userManager.DeleteAsync(user1);
                    }
                    catch { }
                    return ex.ToString();
                }
            }
        }

        public string AddNewPatientFromDashBoard(PetOwner model)
        {
            var insertTask = AddNewPatientFromDashBoardAsync(model);
            insertTask.Wait();

            string res = insertTask.Result;
            return res;
        }


        #endregion


        #region Center Admin
        public async Task<string> AddNewCenterAdminAsync(CenterAdmin model)
        {
            string res = "";
            using (var transaction = cmsContext.Database.BeginTransaction())
            {
                try
                {
                    var user = model.User;

                    var resul = await _userManager.CreateAsync(user, model.Password);

                    if (!resul.Succeeded)
                    {
                        string msg = "";
                        foreach (var item in resul.Errors)
                        {
                            msg += item.Description + " ";
                        }
                        return "Please make sure that the employee username is not used earlier, and password has capital letter, numbers and special character";
                    }

                    string ImageFileName = FileHandler.SaveUploadedFile(model.ImageFile);
                    string PassportFileName = FileHandler.SaveUploadedFile(model.PassportFile);
                    string FamilyBookFileName = FileHandler.SaveUploadedFile(model.FamillyBookFile);
                    string LaborCardFileName = FileHandler.SaveUploadedFile(model.LaborCardFile);


                    cmsContext.CenterAdmin.Add(
                        new CenterAdmin
                        {
                            FirstName = model.FirstName,
                            MiddleName = model.MiddleName,
                            LastName = model.LastName,
                            MotherName = model.MotherName,
                            Description = model.Description,
                            Gendre = model.Gendre,
                            Address = model.Address,
                            NationalCardId = model.NationalCardId,
                            JobCardNumber = model.JobCardNumber,
                            PassportNumber = model.PassportNumber,
                            Nationality = model.Nationality,
                            User = user,
                            PersonEmail = model.PersonEmail,
                            PersonPhone = model.PersonPhone,
                            PersonUserName = model.PersonUserName,
                            MedicalCenterId = _userService.GetMyCenterIdWeb(),
                            ImageName = ImageFileName,
                            PassportFileName = PassportFileName,
                            FamilyBookFileName = FamilyBookFileName,
                            LaborCardFileName = LaborCardFileName,
                            CreateDate = medicalCenterService.ConvertToLocalTime(DateTime.Now),
                        }
                        );

                    try
                    {
                        await _roleManager.CreateAsync(new IdentityRole("centeradmin"));
                    }
                    catch
                    {
                    }

                    await _userManager.AddToRoleAsync(user, "centeradmin");

                    cmsContext.SaveChanges();
                    transaction.Commit();
                    return "";
                }

                catch (Exception ex)
                {
                    transaction.Rollback();
                    try
                    {
                        var user1 = await _userManager.FindByNameAsync(model.PersonUserName);
                        _userManager.DeleteAsync(user1);
                    }
                    catch { }
                    return ex.ToString();
                }
            }
        }

        public string AddNewCenterAdmin(CenterAdmin model)
        {
            var res = AddNewCenterAdminAsync(model);
            return res.ToString();
        }










        public async Task<string> AddNewCenterAdminWithoutTransactionAsync(CenterAdmin model)
        {
            string res = "";
            try
            {
                var user = model.User;

                var resul = await _userManager.CreateAsync(user, model.Password);

                if (!resul.Succeeded)
                {
                    string msg = "";
                    foreach (var item in resul.Errors)
                    {
                        msg += item.Description + " ";
                    }
                    return "Please make sure that the employee username is not used earlier, and password has capital letter, numbers and special character";
                }

                string ImageFileName = FileHandler.SaveUploadedFile(model.ImageFile);
                string PassportFileName = FileHandler.SaveUploadedFile(model.PassportFile);
                string FamilyBookFileName = FileHandler.SaveUploadedFile(model.FamillyBookFile);
                string LaborCardFileName = FileHandler.SaveUploadedFile(model.LaborCardFile);


                cmsContext.CenterAdmin.Add(
                    new CenterAdmin
                    {
                        FirstName = model.FirstName,
                        MiddleName = model.MiddleName,
                        LastName = model.LastName,
                        MotherName = model.MotherName,
                        Description = model.Description,
                        Gendre = model.Gendre,
                        Address = model.Address,
                        NationalCardId = model.NationalCardId,
                        JobCardNumber = model.JobCardNumber,
                        PassportNumber = model.PassportNumber,
                        Nationality = model.Nationality,
                        User = user,
                        PersonEmail = model.PersonEmail,
                        PersonPhone = model.PersonPhone,
                        PersonUserName = model.PersonUserName,
                        MedicalCenterId = _userService.GetMyCenterIdWeb(),
                        ImageName = ImageFileName,
                        PassportFileName = PassportFileName,
                        FamilyBookFileName = FamilyBookFileName,
                        LaborCardFileName = LaborCardFileName,
                        CreateDate = medicalCenterService.ConvertToLocalTime(DateTime.Now),
                    }
                    );

                try
                {
                    await _roleManager.CreateAsync(new IdentityRole("centeradmin"));
                }
                catch
                {
                }

                await _userManager.AddToRoleAsync(user, "centeradmin");

                cmsContext.SaveChanges();
                return "";
            }

            catch (Exception ex)
            {
                try
                {
                    var user1 = await _userManager.FindByNameAsync(model.PersonUserName);
                    _userManager.DeleteAsync(user1);
                }
                catch { }
                return ex.ToString();
            }
        }

        public string AddNewCenterAdminWithoutTransaction(CenterAdmin model)
        {
            var res = AddNewCenterAdminWithoutTransactionAsync(model);
            return res.ToString();
        }

        public async Task<string> UpdateCenterAdminAsync(Employee model)
        {
            string res = "";
            return res;
        }
        public string UpdateCenterAdmin(Employee model)
        {
            var res = UpdateCenterAdminAsync(model);
            return res.ToString();
        }

        public async Task<string> DeleteCenterAdminAsync(Employee model)
        {
            string res = "";
            return res;
        }
        public string DeleteCenterAdmin(Employee model)
        {
            var res = DeleteCenterAdminAsync(model);
            return res.ToString();
        }

        public Employee GetCenterAdminById(Guid id)
        {
            return null;
        }

        #endregion




    }

}
