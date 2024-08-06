    using System.Linq.Expressions;
    using System.Security.Claims;

    using CmsDataAccess;
    using CmsDataAccess.DbModels;
    using Microsoft.EntityFrameworkCore;

    using Microsoft.AspNetCore.Http;
    using ServicesLibrary.PasswordsAndClaims;
    using Microsoft.AspNetCore.Identity;
    using System;
    using static System.Runtime.InteropServices.JavaScript.JSType;
    using CmsDataAccess.TimeZoneUtils;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using ServicesLibrary.EmailServices.EmailServices;
    using CmsDataAccess.Enum;
    using CmsDataAccess.Utils.FilesUtils;
    using ServicesLibrary.PersonServices;
    using Microsoft.IdentityModel.Tokens;
    using Azure.Core;
    using Microsoft.AspNetCore.Mvc;
    using Stripe;
    using CmsDataAccess.Models;
    using ServicesLibrary.UserServices;
    using System.Numerics;
    using Microsoft.AspNetCore.Http.HttpResults;
    using System.Diagnostics;
    using Kendo.Mvc.UI;
    using Kendo.Mvc.Extensions;
    using static Kendo.Mvc.UI.UIPrimitives;

    namespace ServicesLibrary.MedicalCenterServices
    {
        public interface IMedicalCenterService 
        {
            //public Guid? GetMyCenterId();

            Task<MedicalCenter?> GetMyCenterAsync();
            public List<BaseClinic> GetBaseAllClinics();
            public List<PetOwner> GetAllPatients();

            public DataSourceResult GetMyCenterClinics([DataSourceRequest] DataSourceRequest request, string preferredCulture = "en-US");
            public List<BaseClinic> GetMyCenterClinics();
            public List<PetOwner> GetMyCenterPatient();

            public DataSourceResult GetMyCenterDoctors([DataSourceRequest] DataSourceRequest request, string preferredCulture = "en-US");
            public DataSourceResult GetMyDoctorCertificate([DataSourceRequest] DataSourceRequest request, Guid id);

            public DataSourceResult GetMyCenterEmployees([DataSourceRequest] DataSourceRequest request, string preferredCulture = "en-US");

            public DataSourceResult GetMyCenterShifts([DataSourceRequest] DataSourceRequest request, string preferredCulture = "en-US");

            //public MedicalCenter? GetMyCenter();
            public BookingPolicy? GetMyBookingPolicy(); 
            public NotificationPolicy? GetMyNotificationPolicy(); 
            public string GetMyCenterTimeZone();
            public string GetMyCenterTimeZoneDispaly();
            public string GetMyCenterTimeZoneId();
            public DateTime ConvertToLocalTime(DateTime? date1);
            public DateTime ConvertToLocalTime(Guid CenterId,DateTime? date1);
            public DateTime ConvertToSystemTime(DateTime? date1);
            public void ExpireCenterSubscription(Guid id);
            public void ToggleBlockCenter(Guid id);
            public bool IsCenterBlocked();
            public bool IsCenterSubscriptionExpired();
            public void ToggleCenterLock();
            public bool IsCenterLocked();
            public string AddNewMedicalCenter(MedicalCenter model);
            public string UpdateMedicalCenter(MedicalCenter model);
            public string AddNewSubscriptionApplication(string baseUrl,SubscriptionApplication model);
            public string VerifyNewSubscriptionApplication(string baseUrl,string token);
            public string AcceptNewSubscriptionApplication(Guid id, int accept, string msg = "");
            public string PaySubscription(CardInfo cardInfo, Guid planId);
            public string PaySubscription(string cardToken, Guid planId);
            public string UpdateSubscription(CardInfo cardInfo, Guid planId);
            public string CancelSubscription();
            public string UpdateCenterSetUpSteps(Guid id, int step);
            public string AddNewCenterAdminWithTransaction(CenterAdmin model);
        }

        public class MedicalCenterService : IMedicalCenterService
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IUserService _userService;
            private readonly ApplicationDbContext cmsDbContext;
            private readonly IEmailSender emailSender;
            private readonly UserManager<IdentityUser> _userManager;
            private readonly ApplicationDbContext cmsContext;
            private RoleManager<IdentityRole> _roleManager;



            public MedicalCenterService(
                RoleManager<IdentityRole> roleManager,

                IUserService userService,
                IEmailSender emailSender_,
                UserManager<IdentityUser> userManager,
                IHttpContextAccessor httpContextAccessor, ApplicationDbContext _cmsDbContext)
            {
                _userManager = userManager; 
                _httpContextAccessor = httpContextAccessor;
                cmsDbContext = _cmsDbContext;
                emailSender = emailSender_;
                _userService = userService;
                cmsContext = _cmsDbContext;
                _roleManager = roleManager;

            }

            public DataSourceResult GetMyCenterShifts([DataSourceRequest] DataSourceRequest request, string preferredCulture = "en-US")
            {
                Guid guid = (Guid)_userService.GetMyCenterIdWeb();
                var result = cmsContext.ShiftTable
                    .Where(a => a.MedicalCenterId == guid)
                    .ToList().ToDataSourceResult(request);
                return result;
            }

            public List<BaseClinic> GetBaseAllClinics()
            {
                return cmsContext.BaseClinic.Include(a => a.BaseClinicTranslation).ToList();
            }

            public List<PetOwner> GetAllPatients()
            {
                return cmsContext.PetOwner.ToList();
            }

            public DataSourceResult GetMyDoctorCertificate([DataSourceRequest] DataSourceRequest request, Guid id)
            {

                return cmsContext.Certificate.Include(a=>a.CertificateTranslation).Where(a=>a.DoctorId==id).ToDataSourceResult(request);
            }

            


            public DataSourceResult GetMyCenterDoctors([DataSourceRequest] DataSourceRequest request, string preferredCulture = "en-US")
            {
                Guid guid = (Guid)_userService.GetMyCenterIdWeb();
                var result = cmsContext.Doctor
                    .Where(a => a.MedicalCenterId == guid && !a.IsDeleted)
                    .Include(a => a.DoctorTranslation)
                    .Include(a => a.User)
                    .ToList().ToDataSourceResult(request);
                return result;
            }   
            
            
            public DataSourceResult GetMyCenterEmployees([DataSourceRequest] DataSourceRequest request, string preferredCulture = "en-US")
            {
                Guid guid = (Guid)_userService.GetMyCenterIdWeb();
                var result = cmsContext.Employee
                    .Where(a => a.MedicalCenterId == guid && !a.IsDeleted) 
                    .Include(a => a.User)
                    .ToList().ToDataSourceResult(request);
                return result;
            }

            public DataSourceResult GetMyCenterClinics([DataSourceRequest] DataSourceRequest request, string preferredCulture = "en-US")
            {
                Guid guid = (Guid)_userService.GetMyCenterIdWeb();
                var result = cmsContext.BaseClinic
                    .Where(a => a.MedicalCenterId == guid && !a.IsDraft)
                    .Include(a => a.BaseClinicTranslation)
                    .ToList().ToDataSourceResult(request);
                return result;
            }

            public List<BaseClinic> GetMyCenterClinics()
            {
                Guid guid = (Guid)_userService.GetMyCenterIdWeb();
                var result = cmsContext.BaseClinic
                    .Where(a => a.MedicalCenterId == guid && !a.IsDraft)
                    .Include(a => a.BaseClinicTranslation)
                    .ToList();
                result[0].IsExpand = true;
                return result;
            }

            public List<PetOwner> GetMyCenterPatient()
            {
                Guid guid = (Guid)_userService.GetMyCenterIdWeb();
                return cmsContext.PetOwner
                    .Where(a => a.MedicalCenterId == guid)
                    .ToList();
            }

            public bool CanAddNewCenter()
            {
                Guid guid = (Guid)_userService.GetMyId();

                CenterAdmin centerAdmin = cmsContext.CenterAdmin.Include(a => a.User).FirstOrDefault(a => a.User.Id == guid.ToString());

                if (centerAdmin.MedicalCenterId != null)
                {
                    return false;
                }

                return true;
            }

            public MedicalCenter AddNewCenter(DateTime SubscriptionExpireDate)
            {
                Guid guid = (Guid)_userService.GetMyId();
                CenterAdmin centerAdmin = cmsContext.CenterAdmin.Include(a => a.User).FirstOrDefault(a => a.User.Id == guid.ToString());
                MedicalCenter medical = new MedicalCenter
                {
                    MedicalCenterTranslation = new List<MedicalCenterTranslation>
                    {
                        new MedicalCenterTranslation { Name = "Name", Description = "Description", LangCode = "en-us" },
                        new MedicalCenterTranslation { Name = "الاسم", Description = "الوصف", LangCode = "ar" },
                    },
                    OpeningHours = new List<OpeningHours>
                    {
                        new OpeningHours { DayOfWeek = DayOfWeek.Monday, OpeningTime = new TimeSpan(9, 0, 0), ClosingTime = new TimeSpan(17, 0, 0) },
                        new OpeningHours { DayOfWeek = DayOfWeek.Tuesday, OpeningTime = new TimeSpan(9, 0, 0), ClosingTime = new TimeSpan(17, 0, 0) },
                        new OpeningHours { DayOfWeek = DayOfWeek.Wednesday, OpeningTime = new TimeSpan(9, 0, 0), ClosingTime = new TimeSpan(17, 0, 0) },
                        new OpeningHours { DayOfWeek = DayOfWeek.Thursday, OpeningTime = new TimeSpan(9, 0, 0), ClosingTime = new TimeSpan(17, 0, 0) },
                        new OpeningHours { DayOfWeek = DayOfWeek.Friday, OpeningTime = new TimeSpan(9, 0, 0), ClosingTime = new TimeSpan(17, 0, 0) },
                        new OpeningHours { DayOfWeek = DayOfWeek.Saturday, OpeningTime = new TimeSpan(9, 0, 0), ClosingTime = new TimeSpan(17, 0, 0) },
                        new OpeningHours { DayOfWeek = DayOfWeek.Sunday, OpeningTime = new TimeSpan(9, 0, 0), ClosingTime = new TimeSpan(17, 0, 0) },
                    },
                    BlockedByAdmin = false,
                    ContactInfo = new List<ContactInfo>
                    {
                        new ContactInfo { ContactType = "E-mail", ContactValue = "E-mail address" },
                        new ContactInfo { ContactType = "Phone", ContactValue = "Phone Number" },
                        new ContactInfo { ContactType = "Mobile", ContactValue = "Mobile Number" },
                        new ContactInfo { ContactType = "Instagram", ContactValue = "Instagram Link" },
                    },
                    Address = new CmsDataAccess.DbModels.Address
                    {
                        AddressTranslation = new List<AddressTranslation>
                        {
                            new AddressTranslation { FullAddress = "Detailed address", Building = "Building number",
                                City = "City name", Country = "Country name", Street = "Street name", LangCode = "en-us"
                            },
                            new AddressTranslation { FullAddress = "العنوان المفصل", Building = "رقم البناء",
                                City = "اسم المدينة", Country = "اسم الدولة", Street = "اسم الشارع", LangCode = "ar"
                            },
                        }
                    },
                    CreateDate = ConvertToSystemTime(DateTime.Now),
                    SubscriptionExpired = false,
                    SubscriptionExpireDate = SubscriptionExpireDate,
                    PaidAccountIsActive=true,
                    UseEmailVerfication=false,
                    ShowInMobileApplication=false,
                    
                };

                cmsContext.MedicalCenter.Add(medical);
                cmsContext.SaveChanges();

                return medical;
            }

        public async Task<MedicalCenter?> GetMyCenterAsync()
        {
            Guid guid = (Guid)_userService.GetMyId();

            // Retrieve CenterAdmin for the user
            CenterAdmin? centerAdmin = await cmsContext.CenterAdmin
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.User.Id == guid.ToString());

            if (centerAdmin == null)
            {
                return null; // No CenterAdmin found
            }

            // Retrieve SubscriptionApplication if needed
            SubscriptionApplication? subscriptionApplication = await cmsContext.SubscriptionApplication
                .FindAsync(centerAdmin.SubscriptionPlanId);

            // If MedicalCenterId is present, retrieve the MedicalCenter details
            if (centerAdmin.MedicalCenterId != null)
            {
                return await cmsContext.MedicalCenter
                    .Include(a => a.MedicalCenterTranslation)
                    .Include(a => a.OpeningHours)
                    .Include(a => a.ContactInfo)
                    .Include(a => a.Address)
                        .ThenInclude(a => a.AddressTranslation)
                    .FirstOrDefaultAsync(a => a.Id == centerAdmin.MedicalCenterId);
            }

            return null; // No MedicalCenter found
        }



        public BookingPolicy? GetMyBookingPolicy()
            {
                Guid guid = (Guid)_userService.GetMyCenterIdWeb();

                BookingPolicy bookingPolicy = cmsContext.BookingPolicy.FirstOrDefault(a=>a.MedicalCenterId== guid);

                if (bookingPolicy==null)
                {
                    BookingPolicy temp = new BookingPolicy
                    {
                        AllowedBookingNumberPerday = 1,
                        MaximunAllowedMissedAppointments = 1,
                        MedicalCenterId = guid,
                    };

                    cmsContext.BookingPolicy.Add(temp);
                    cmsContext.SaveChanges();

                    return temp;
                }

                return bookingPolicy;
            }

            public NotificationPolicy? GetMyNotificationPolicy()
            {
                Guid guid = (Guid)_userService.GetMyCenterIdWeb();

                NotificationPolicy bookingPolicy = cmsContext.NotificationPolicy.FirstOrDefault(a => a.MedicalCenterId == guid);

                if (bookingPolicy == null)
                {
                    NotificationPolicy temp = new NotificationPolicy
                    {
                        HoursBeforeAppointment = 1,
                        MedicalCenterId = guid,

                    };

                    cmsContext.NotificationPolicy.Add(temp);
                    cmsContext.SaveChanges();

                    return temp;
                }

                return bookingPolicy;
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


                    CenterAdmin temp = new CenterAdmin
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
                        MedicalCenterId = GetMyCenterIdWeb(),
                        ImageName = ImageFileName,
                        PassportFileName = PassportFileName,
                        FamilyBookFileName = FamilyBookFileName,
                        LaborCardFileName = LaborCardFileName,
                        CreateDate = ConvertToLocalTime(DateTime.Now),
                    };
                    cmsContext.CenterAdmin.Add(temp);

                    try
                    {
                        await _roleManager.CreateAsync(new IdentityRole("centeradmin"));
                    }
                    catch
                    {
                    }

                    await _userManager.AddToRoleAsync(user, "centeradmin");

                    cmsContext.SaveChanges();
                    return temp.Id.ToString();
                }

                catch (Exception ex)
                {
                    try
                    {
                        var user1 = await _userManager.FindByNameAsync(model.PersonUserName);
                        _userManager.DeleteAsync(user1);
                    }
                    catch { }
                    return "ERROR"+ex.ToString();
                }
            }

            public string AddNewCenterAdminWithoutTransaction(CenterAdmin model)
            {
                var insertTask = AddNewCenterAdminWithoutTransactionAsync(model);
                insertTask.Wait();

                string res = insertTask.Result;
                return res;

            }


            public async Task<string> AddNewCenterAdminWithTransactionAsync(CenterAdmin model)
            {

                using(var trans=cmsContext.Database.BeginTransaction())
                {
                    try
                    {

                        model.User = new IdentityUser
                        {
                            Email=model.PersonEmail,
                            UserName=model.PersonUserName,
                            PhoneNumber=model.PersonPhone,
                            EmailConfirmed=true,
                        };

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


                        CenterAdmin temp = new CenterAdmin
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
                            MedicalCenterId = model.MedicalCenterId,
                            ImageName = ImageFileName,
                            PassportFileName = PassportFileName,
                            FamilyBookFileName = FamilyBookFileName,
                            LaborCardFileName = LaborCardFileName,
                            CreateDate = ConvertToSystemTime(DateTime.Now),
                        };
                        cmsContext.CenterAdmin.Add(temp);

                        try
                        {
                            await _roleManager.CreateAsync(new IdentityRole("centeradmin"));
                        }
                        catch
                        {
                        }

                        await _userManager.AddToRoleAsync(user, "centeradmin");

                        cmsContext.SaveChanges();

                        trans.Commit();
                        return temp.Id.ToString();

                    }
                    catch(Exception ex)
                    {
                        trans.Rollback();
                        try
                        {
                            var user1 = await _userManager.FindByNameAsync(model.PersonUserName);
                            _userManager.DeleteAsync(user1);
                        }
                        catch { }
                        return "ERROR" + ex.ToString();
                    }
                }

            
            }

            public string AddNewCenterAdminWithTransaction(CenterAdmin model)
            {
                var insertTask = AddNewCenterAdminWithTransactionAsync(model);
                insertTask.Wait();
                string res = insertTask.Result;
                return res;

            }


            //public Guid? GetMyCenterId()
            //{
            //    if (_httpContextAccessor != null)
            //    {
            //        try
            //        {

            //            return Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.SerialNumber));


            //        }
            //        catch
            //        {
            //            return null;
            //        }
            //    }
            //    return null;
            //}

            public Guid? GetMyCenterIdWeb()
            {
                if (_httpContextAccessor != null)
                {
                    try
                    {
                        return Guid.Parse(_httpContextAccessor.HttpContext.Request.Headers["CenterId"].ToString());
                    }
                    catch
                    {
                        return null;
                    }
                }
                return null;
            }

            public string GetMyCenterTimeZone()
            {
                Guid? id = _userService.GetMyCenterIdWeb();

                if (id != null)
                {
                    try
                    {
                        Guid guid = Guid.Parse(cmsDbContext.MedicalCenter.FirstOrDefault(a => a.Id == id).TimeZone);
                        return cmsContext.SystemTimeZone.Find(guid).StandardName;
                    }
                    catch
                    {
                        return "Arabian Standard Time";
                    }

                }

                return "Arabian Standard Time";
            }

            public string GetMyCenterTimeZone(Guid? id)
            {

                if (id != null)
                {
                    try
                    {
                        Guid guid = Guid.Parse(cmsDbContext.MedicalCenter.FirstOrDefault(a => a.Id == id).TimeZone);
                        return cmsContext.SystemTimeZone.Find(guid).StandardName;
                    }
                    catch
                    {
                        return "Arabian Standard Time";
                    }

                }

                return "Arabian Standard Time";
            }

            public string GetMyCenterTimeZoneDispaly()
            {
            

                Guid? id = _userService.GetMyCenterIdWeb();

                if (id != null)
                {
                    try
                    {
                        Guid guid = Guid.Parse(cmsDbContext.MedicalCenter.FirstOrDefault(a => a.Id == id).TimeZone);
                        return cmsContext.SystemTimeZone.Find(guid).DisplayName;
                    }
                    catch
                    {
                        return "UTC+04:00";
                    }

                }

                return "UTC+04:00";

            }

            public string GetMyCenterTimeZoneId()
            {

                return "Asia/Dubai";

                Guid? id = _userService.GetMyCenterIdWeb();

                if (id != null)
                {
                    try
                    {
                        Guid guid = Guid.Parse(cmsDbContext.MedicalCenter.FirstOrDefault(a => a.Id == id).TimeZone);
                        return cmsContext.SystemTimeZone.Find(guid).IANAID;
                    }
                    catch
                    {
                        return "Asia/Dubai";
                    }

                }

                return "Asia/Dubai";
            }
            public DateTime ConvertToLocalTime(DateTime? date1)
            {
                string TimeZone = GetMyCenterTimeZone();
                return TimeZoneConverter.getLocalTime(date1, TimeZone);
            }
            public DateTime ConvertToLocalTime(Guid CenterId, DateTime? date1)
            {
                string TimeZone = GetMyCenterTimeZone(CenterId);
                return TimeZoneConverter.getLocalTime(date1, TimeZone);
            }

            public DateTime ConvertToSystemTime(DateTime? date1)
            {
                string TimeZone = cmsContext.MySystemConfiguration.FirstOrDefault().SystemTimeZone;
                //TimeZone = cmsContext.SystemTimeZone.Find(Guid.Parse(TimeZone)).StandardName;

                return TimeZoneConverter.getLocalTime(date1, TimeZone);
            }
            public void ExpireCenterSubscription(Guid id)
            {
                MedicalCenter med = cmsDbContext.MedicalCenter.Find(id);
                med.SubscriptionExpired = true;
                cmsDbContext.MedicalCenter.Attach(med);
                cmsDbContext.Entry(med).Property(x => x.SubscriptionExpired).IsModified = true;
                cmsDbContext.SaveChanges();
            }
            public void ToggleBlockCenter(Guid id)
            {
                MedicalCenter med = cmsDbContext.MedicalCenter.Find(id);
                med.BlockedByAdmin = !med.BlockedByAdmin;
                cmsDbContext.MedicalCenter.Attach(med);
                cmsDbContext.Entry(med).Property(x => x.BlockedByAdmin).IsModified = true;
                cmsDbContext.SaveChanges();
            }
            public bool IsCenterBlocked()
            {
                Guid id = (Guid)_userService.GetMyCenterIdWeb();
                MedicalCenter med = cmsDbContext.MedicalCenter.Find(id);
                if (med != null)
                {
                    return med.BlockedByAdmin;
                }
                return false;
            }
            public bool IsCenterSubscriptionExpired()
            {
                Guid id = (Guid)_userService.GetMyCenterIdWeb();
                MedicalCenter med = cmsDbContext.MedicalCenter.Find(id);
                if (med != null)
                {
                    if (med.SubscriptionExpireDate!=null)
                    {
                        if (med.SubscriptionExpireDate>=ConvertToLocalTime(DateTime.Now) && !med.SubscriptionExpired)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                return false;
            }
            public void ToggleCenterLock()
            {
                Guid id=(Guid)_userService.GetMyCenterIdWeb();
                MedicalCenter med=cmsDbContext.MedicalCenter.Find(id);
                if (med != null)
                {
                    med.LockMyCenter = !med.LockMyCenter;
                    cmsDbContext.MedicalCenter.Attach(med);
                    cmsDbContext.Entry(med).Property(x => x.LockMyCenter).IsModified = true;
                    cmsDbContext.SaveChanges();
                }
            }
            public bool IsCenterLocked()
            {
                Guid id = (Guid)_userService.GetMyCenterIdWeb();
                MedicalCenter med = cmsDbContext.MedicalCenter.Find(id);
                if (med!=null)
                {
                    return med.LockMyCenter;
                }
                return false;
            }
            public string AddNewMedicalCenter(MedicalCenter model)
            {
                using (var transaction=cmsDbContext.Database.BeginTransaction())
                {
                    try
                    {
                        model.ImageName = FileHandler.SaveUploadedFile(model.ImageFile);
                        model.CreateDate = ConvertToSystemTime(DateTime.Now);
                        model.BlockedByAdmin = false;
                        model.SubscriptionExpired = false;
                        model.PaidAccountIsActive = false;
                        model.LockMyCenter = false;
                        
                        cmsDbContext.MedicalCenter.Add(model);
                        cmsDbContext.SaveChanges();

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
            public string UpdateMedicalCenter(MedicalCenter model)
            {
                using (var transaction = cmsDbContext.Database.BeginTransaction())
                {
                    try
                    {

                        

                        transaction.Commit();
                        return "";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return ex.ToString();
                    }
                }

                return "";
            }
            public string AddNewSubscriptionApplication(string baseUrl, SubscriptionApplication model)
            {
                string res = "";

                using (var transaction =cmsDbContext.Database.BeginTransaction())
                {
                    try
                    {
                        string verficationToken =new PasswordUtil()
                            .CreateRandomTokenN();


                        emailSender.SendEmailAsync(model.SubscriberEmail,
                            "Application verification", EmailMessages.NewApplicationMessage(baseUrl, verficationToken));

                        string LicenseImageFile = FileHandler.SaveUploadedFile(model.LicenseImageFile);
                        string PassportImageFile = FileHandler.SaveUploadedFile(model.PassportImageFile);

                        model.LicenseImageName = LicenseImageFile;
                        model.PassportImageName = PassportImageFile;

                        model.VerificationToken = verficationToken;
                        model.EmailVerfied = false;
                        model.VerificationExpireDate = ConvertToLocalTime(DateTime.Now).AddHours(1);
                        
                        cmsDbContext.SubscriptionApplication.Add(model);
                        cmsDbContext.SaveChanges();

                        transaction.Commit();
                        return res;
                    }
                    catch (Exception ex) 
                    {
                        transaction.Rollback();
                        return ex.ToString();
                    }
                }

            }
            public string VerifyNewSubscriptionApplication(string baseUrl,string token)
            {
                string res = "";

                using (var transaction = cmsDbContext.Database.BeginTransaction())
                {
                    try
                    {

                        DateTime dateTime = ConvertToLocalTime(DateTime.Now);

                        SubscriptionApplication subscriptionApplication = cmsDbContext.SubscriptionApplication.FirstOrDefault(a => a.VerificationToken == token && a.VerificationExpireDate >= dateTime);

                        if (subscriptionApplication == null)
                        {
                            return "Either Token is not correct or expired, please try again";
                        }

                        string Password = PasswordUtil.GenerateRandomPassword();

                        CenterAdmin centerAdmin = new CenterAdmin
                        {

                            AccountVerfied = true,
                            AdminEmail = subscriptionApplication.SubscriberEmail,
                            AdminPhone = subscriptionApplication.SubscriberPhone,
                            PersonEmail = subscriptionApplication.SubscriberEmail,
                            PersonPhone = subscriptionApplication.SubscriberPhone,
                            AdminUserName = subscriptionApplication.SubscriberEmail,
                            BlockedByAdmin = false,
                            CenterStatus = 0,
                            FirstName= subscriptionApplication.FirstName,
                            MiddleName= subscriptionApplication.MiddleName,
                            LastName= subscriptionApplication.LastName,
                            CreateDate = ConvertToLocalTime(DateTime.Now),
                            User = new IdentityUser()
                            {
                                Email = subscriptionApplication.SubscriberEmail,
                                PhoneNumber = subscriptionApplication.SubscriberPhone,
                                UserName = subscriptionApplication.SubscriberEmail,
                                PhoneNumberConfirmed = true,
                                EmailConfirmed = true,
                            },
                            Password = Password,
                        };

                        res = AddNewCenterAdminWithoutTransaction(centerAdmin);

                        if (res.StartsWith("ERROR"))
                        {
                            transaction.Rollback();
                            return res.ToString();
                        }

                        subscriptionApplication.CenterAdminId =Guid.Parse(res);

                        subscriptionApplication.EmailVerfied = true;
                        subscriptionApplication.VerificationToken = string.Empty;

                        cmsDbContext.SubscriptionApplication.Attach(subscriptionApplication);
                        cmsDbContext.Entry(subscriptionApplication).State = EntityState.Modified;   
                        cmsDbContext.SaveChanges();

                        transaction.Commit();


                        emailSender.SendEmailAsync(subscriptionApplication.SubscriberEmail,
                        "Myth Account Created", EmailMessages.NewAccountMessage(baseUrl, subscriptionApplication.SubscriberEmail,Password));




                        return res;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return ex.ToString();
                    }
                }


            }
            public string AcceptNewSubscriptionApplication(Guid id,int accept,string msg="")
            {
                string res = "";

                using (var transaction = cmsDbContext.Database.BeginTransaction())
                {
                    try
                    {
                        SubscriptionApplication subscriptionApplication = cmsDbContext.SubscriptionApplication.Find(id);

                        if (subscriptionApplication == null)
                        {
                            return "Application was deleted";
                        }

                        subscriptionApplication.Accepted = accept;
                        subscriptionApplication.IsSeen = true;
                        subscriptionApplication.ResponseFromAdmin = msg;
                        
                        if (accept>0)
                        {
                            // create user account

                            //string Password = PasswordUtil.GenerateRandomPassword();

                            //CenterAdmin centerAdmin= new CenterAdmin
                            //{

                            //    AccountVerfied = true,
                            //    AdminEmail=subscriptionApplication.SubscriberEmail,
                            //    AdminPhone=subscriptionApplication.SubscriberPhone,
                            //    PersonEmail=subscriptionApplication.SubscriberEmail,
                            //    PersonPhone =subscriptionApplication.SubscriberPhone,
                            //    AdminUserName= subscriptionApplication.SubscriberEmail,
                            //    BlockedByAdmin=false,
                            //    CenterStatus=0,
                            //    CreateDate= ConvertToLocalTime(DateTime.Now),
                            //    User=new IdentityUser()
                            //    {
                            //        Email = subscriptionApplication.SubscriberEmail,
                            //        PhoneNumber = subscriptionApplication.SubscriberPhone,
                            //        UserName = subscriptionApplication.SubscriberEmail,
                            //        PhoneNumberConfirmed = true,
                            //        EmailConfirmed = true,
                            //    },
                            //    Password= Password,
                            //};
                            //res= AddNewCenterAdminWithoutTransaction(centerAdmin);

                            //subscriptionApplication.CenterAdminId = centerAdmin.Id;

                            subscriptionApplication.ResponseFromAdmin = "Accepted";


                            emailSender.SendEmailAsync(subscriptionApplication.SubscriberEmail, EmailMessages.AcceptSubject, EmailMessages.AcceptMessage(msg));
                        }
                        else
                        {
                            subscriptionApplication.ResponseFromAdmin = msg;
                            emailSender.SendEmailAsync(subscriptionApplication.SubscriberEmail, EmailMessages.RejectSubject, EmailMessages.RejectMessage(msg));
                        }

                        cmsDbContext.SubscriptionApplication.Attach(subscriptionApplication);
                        cmsDbContext.Entry(subscriptionApplication).State = EntityState.Modified;
                        cmsDbContext.SaveChanges();

                        if (!res.IsNullOrEmpty())
                        {
                            transaction.Rollback();
                            return res;
                        }

                        transaction.Commit();

                        return res;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        if (res.IsNullOrEmpty())
                        {
                            return ex.ToString();
                        }
                        return res;
                    }
                }
            }
            public string PaySubscription(CardInfo cardInfo,Guid planId)
            {


                Guid UserId = (Guid)_userService.GetMyId();
                CenterAdmin customerDto = cmsDbContext.CenterAdmin.Include(a=>a.User).Where(a => a.User.Id == UserId.ToString()).FirstOrDefault();
                SubscriptionPlan subscriptionPlan = cmsDbContext.SubscriptionPlan.FirstOrDefault(a=>a.Id==planId);
                StripeConfiguration.ApiKey = cmsDbContext.MySystemConfiguration.FirstOrDefault().StripePrivateKey;

                DateTime expireDate = subscriptionPlan.PriceRecuencyInterval == "month" ? DateTime.Now.AddMonths(1) : DateTime.Now.AddMonths(12);
                expireDate = ConvertToLocalTime(expireDate);

                var options = new CustomerCreateOptions();
                var customer1 = new Customer();
                Subscription subscription;

                var tokenCreateOptionsoptions = new TokenCreateOptions();
                var tokenId = "";

                try
                {
                    tokenCreateOptionsoptions = new TokenCreateOptions
                    {
                        //Type = "card",//cardInfo.paymentMethodType, // "card",
                        Card = new TokenCardOptions
                        {
                            Number = cardInfo.CardNumber, // "4242424242424242",
                            ExpMonth = cardInfo.CardExpMonth.ToString(), // 8,
                            ExpYear = cardInfo.CardExpYear.ToString(), // 2023,
                            Cvc = cardInfo.CardExpCvc, //   "314",

                        },
                    };
                    var toeknService = new TokenService();
                    var tokenResponse = toeknService.Create(tokenCreateOptionsoptions);
                    tokenId = tokenResponse.Id;
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }

                var paymentMethodCreateOptionsoptions = new PaymentMethodCreateOptions();
                var pMethodId = "";
                try
                {
                    paymentMethodCreateOptionsoptions = new PaymentMethodCreateOptions
                    {
                        Type = "card",
                        Card = new PaymentMethodCardOptions
                        {
                        Token= tokenId,
                        },
                    };

                    var PMService = new PaymentMethodService();
                    var pmMethod= PMService.Create(paymentMethodCreateOptionsoptions);
                    pMethodId=pmMethod.Id;

                }
                catch (Exception)
                {

                    throw;
                }

                try
                {
                    options = new CustomerCreateOptions
                    {
                        Name = customerDto.FirstName + " " + customerDto.LastName, // "customer1",
                        Email = customerDto.User.Email, // "customer1@gmail.com",
                        Description = customerDto.Id.ToString(), // "My First Test Customer", 
                        PaymentMethod = pMethodId,
                        InvoiceSettings = new CustomerInvoiceSettingsOptions
                        {
                            DefaultPaymentMethod = pMethodId,
                        },
                    };
                    var service = new CustomerService();
                    customer1 = service.Create(options);
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }

                try
                {
                    var options1 = new SubscriptionCreateOptions
                    {
                        Customer = customer1.Id, //"cus_My7F0cgwvVTWmj", // customer should add card and payment method
                        Items = new List<SubscriptionItemOptions>
                            {
                                new SubscriptionItemOptions
                                {
                                Price = subscriptionPlan.PriceId, // "price_1MEB2kItIzR7Vx2u4X4Ui2cY", //recurring price
                                },
                            },

                        //TrialPeriodDays= subscriptionPlan.FreeDays,

                        //CancelAtPeriodEnd = true,
                        CancelAt = expireDate,
                        Description = "Subscription fees for new center",
                        Metadata = new Dictionary<string, string>
                        {
                            { "FullName", customerDto.FirstName + " " + customerDto.LastName},
                            { "UserName", customerDto.User.UserName},
                            { "Email", customerDto.User.Email},
                            { "Type","0001# Subscription fees for Opening new center"},
                            { "CustomOperationCode","0001"},
                            { "Date",ConvertToSystemTime(DateTime.Now).ToString()},
                        }
                    };
                    var service1 = new SubscriptionService();
                    subscription = service1.Create(options1);
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }

                using (var transaction=cmsDbContext.Database.BeginTransaction())
                {
                    try
                    {
                        Guid centerID=AddNewCenter(expireDate).Id;

                        customerDto.StipeId = customer1.Id;
                        customerDto.StripeSubcriptionId = subscription.Id;
                        customerDto.SubscriptionPlanId = subscriptionPlan.Id;
                        customerDto.PaidAt = DateTime.Now;
                        customerDto.SubscriptionExpired = false;
                        customerDto.SubscriptionExpireDate = expireDate;
                        customerDto.SetUpFeesIsPayed = true;
                        customerDto.StripePaymentMethodId = pMethodId;
                        customerDto.PaidAccountIsActive = true;
                        customerDto.MedicalCenterId= centerID;

                        cmsDbContext.CenterAdmin.Attach(customerDto);

                        cmsDbContext.Entry(customerDto).Property(x => x.StipeId).IsModified = true;
                        cmsDbContext.Entry(customerDto).Property(x => x.StripeSubcriptionId).IsModified = true;
                        cmsDbContext.Entry(customerDto).Property(x => x.SubscriptionPlanId).IsModified = true;
                        cmsDbContext.Entry(customerDto).Property(x => x.PaidAt).IsModified = true;
                        cmsDbContext.Entry(customerDto).Property(x => x.SubscriptionExpired).IsModified = true;
                        cmsDbContext.Entry(customerDto).Property(x => x.SubscriptionExpireDate).IsModified = true;
                        cmsDbContext.Entry(customerDto).Property(x => x.SetUpFeesIsPayed).IsModified = true;
                        cmsDbContext.Entry(customerDto).Property(x => x.StripePaymentMethodId).IsModified = true;
                        cmsDbContext.Entry(customerDto).Property(x => x.PaidAccountIsActive).IsModified = true;
                        cmsDbContext.Entry(customerDto).Property(x => x.MedicalCenterId).IsModified = true;

                        cmsDbContext.SaveChanges();


                        CenterSetUpSteps centerSetUpSteps = new CenterSetUpSteps
                        {
                            CenterAdminId = customerDto.Id,
                            StepNumer = 0
                        };
                        cmsDbContext.CenterSetUpSteps.Add(centerSetUpSteps);
                        cmsDbContext.SaveChanges();

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

            public string PaySubscription(string cardToken, Guid planId)
            {


                Guid UserId = (Guid)_userService.GetMyId();
                CenterAdmin customerDto = cmsDbContext.CenterAdmin.Include(a => a.User).Where(a => a.User.Id == UserId.ToString()).FirstOrDefault();
                SubscriptionPlan subscriptionPlan = cmsDbContext.SubscriptionPlan.FirstOrDefault(a => a.Id == planId);
                StripeConfiguration.ApiKey = cmsDbContext.MySystemConfiguration.FirstOrDefault().StripePrivateKey;

                DateTime expireDate = subscriptionPlan.PriceRecuencyInterval == "month" ? DateTime.Now.AddMonths(1) : DateTime.Now.AddMonths(12);
                expireDate = ConvertToSystemTime(expireDate);

                var options = new CustomerCreateOptions();
                var customer1 = new Customer();
                Subscription subscription;



                var paymentMethodCreateOptionsoptions = new PaymentMethodCreateOptions();
                var pMethodId = "";
                try
                {
                    paymentMethodCreateOptionsoptions = new PaymentMethodCreateOptions
                    {
                        Type = "card",
                        Card = new PaymentMethodCardOptions
                        {
                            Token = cardToken,
                        },
                    };

                    var PMService = new PaymentMethodService();
                    var pmMethod = PMService.Create(paymentMethodCreateOptionsoptions);
                    pMethodId = pmMethod.Id;

                }
                catch (Exception)
                {

                    throw;
                }

                try
                {
                    options = new CustomerCreateOptions
                    {
                        Name = customerDto.FirstName + " " + customerDto.LastName, // "customer1",
                        Email = customerDto.User.Email, // "customer1@gmail.com",
                        Description = customerDto.Id.ToString(), // "My First Test Customer", 
                        PaymentMethod = pMethodId,
                        InvoiceSettings = new CustomerInvoiceSettingsOptions
                        {
                            DefaultPaymentMethod = pMethodId,
                        },
                    };
                    var service = new CustomerService();
                    customer1 = service.Create(options);
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }

                try
                {
                    var options1 = new SubscriptionCreateOptions
                    {
                        Customer = customer1.Id, //"cus_My7F0cgwvVTWmj", // customer should add card and payment method
                        Items = new List<SubscriptionItemOptions>
                            {
                                new SubscriptionItemOptions
                                {
                                Price = subscriptionPlan.PriceId, // "price_1MEB2kItIzR7Vx2u4X4Ui2cY", //recurring price
                                },
                            },

                        //TrialPeriodDays= subscriptionPlan.FreeDays,

                        //CancelAtPeriodEnd = true,
                        CancelAt = expireDate,
                        Description = "Subscription fees for new center",
                        Metadata = new Dictionary<string, string>
                        {
                            { "FullName", customerDto.FirstName + " " + customerDto.LastName},
                            { "UserName", customerDto.User.UserName},
                            { "Email", customerDto.User.Email},
                            { "Type","0001# Subscription fees for Opening new center"},
                            { "CustomOperationCode","0001"},
                            { "Date",ConvertToSystemTime(DateTime.Now).ToString()},
                        }
                    };
                    var service1 = new SubscriptionService();
                    subscription = service1.Create(options1);
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }

                using (var transaction = cmsDbContext.Database.BeginTransaction())
                {
                    try
                    {

                        Guid centerID = AddNewCenter(expireDate).Id;


                        customerDto.StipeId = customer1.Id;
                        customerDto.StripeSubcriptionId = subscription.Id;
                        customerDto.SubscriptionPlanId = subscriptionPlan.Id;
                        customerDto.PaidAt = DateTime.Now;
                        customerDto.SubscriptionExpired = false;
                        customerDto.SubscriptionExpireDate = expireDate;
                        customerDto.SetUpFeesIsPayed = true;
                        customerDto.StripePaymentMethodId = pMethodId;
                        customerDto.PaidAccountIsActive = true;
                        customerDto.MedicalCenterId = centerID;

                        cmsDbContext.CenterAdmin.Attach(customerDto);

                        cmsDbContext.Entry(customerDto).Property(x => x.StipeId).IsModified = true;
                        cmsDbContext.Entry(customerDto).Property(x => x.StripeSubcriptionId).IsModified = true;
                        cmsDbContext.Entry(customerDto).Property(x => x.SubscriptionPlanId).IsModified = true;
                        cmsDbContext.Entry(customerDto).Property(x => x.PaidAt).IsModified = true;
                        cmsDbContext.Entry(customerDto).Property(x => x.SubscriptionExpired).IsModified = true;
                        cmsDbContext.Entry(customerDto).Property(x => x.SubscriptionExpireDate).IsModified = true;
                        cmsDbContext.Entry(customerDto).Property(x => x.SetUpFeesIsPayed).IsModified = true;
                        cmsDbContext.Entry(customerDto).Property(x => x.StripePaymentMethodId).IsModified = true;
                        cmsDbContext.Entry(customerDto).Property(x => x.PaidAccountIsActive).IsModified = true;
                        cmsDbContext.Entry(customerDto).Property(x => x.MedicalCenterId).IsModified = true;

                        cmsDbContext.SaveChanges();

                        
                        CenterSetUpSteps centerSetUpSteps = new CenterSetUpSteps
                        {
                            CenterAdminId = customerDto.Id,
                            StepNumer = 0
                        };
                        cmsDbContext.CenterSetUpSteps.Add(centerSetUpSteps);
                        cmsDbContext.SaveChanges();

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

            public string UpdateSubscription(CardInfo cardInfo, Guid planId)
            {
                Guid UserId = (Guid)_userService.GetMyId();
                CenterAdmin customerDto = cmsDbContext.CenterAdmin.Include(a => a.User).FirstOrDefault(a => a.Id == UserId);
                StripeConfiguration.ApiKey = cmsDbContext.MySystemConfiguration.FirstOrDefault().StripePrivateKey;

                Subscription subscription;
                SubscriptionPlan subscriptionPlan = cmsDbContext.SubscriptionPlan.Find(planId);

                DateTime expireDate = subscriptionPlan.PriceRecuencyInterval == "month" ? DateTime.Now.AddMonths(1) : DateTime.Now.AddMonths(12);
                expireDate = ConvertToLocalTime(expireDate);

                var paymentMethodCreateOptionsoptions = new PaymentMethodCreateOptions();
                var pMethodId = "";

                if (customerDto.StipeId == "")
                {
                    return "To use this function you have to pay for the first subscription";
                }

                //Cancel old subscription
                try
                {
                    var service11 = new SubscriptionService();
                    Subscription subscription11 = service11.Cancel(customerDto.StripeSubcriptionId);
                }
                catch { }


                // update customer payemnet method
                try
                {
                    var serviceda = new PaymentMethodService();
                    serviceda.Detach(customerDto.StripePaymentMethodId);

                    paymentMethodCreateOptionsoptions = new PaymentMethodCreateOptions
                    {
                        Type = cardInfo.paymentMethodType, // "card",
                        Card = new PaymentMethodCardOptions
                        {
                            Number = cardInfo.CardNumber, // "4242424242424242",
                            ExpMonth = cardInfo.CardExpMonth, // 8,
                            ExpYear = cardInfo.CardExpYear, // 2023,
                            Cvc = cardInfo.CardExpCvc, //   "314",
                        },
                    };
                    var PMService = new PaymentMethodService();
                    var pMethod = PMService.Create(paymentMethodCreateOptionsoptions);
                    pMethodId = pMethod.Id;

                    var options = new PaymentMethodAttachOptions
                    {
                        Customer = customerDto.StipeId,
                    };
                    var service = new PaymentMethodService();
                    service.Attach(pMethodId, options);


                    var cuo = new CustomerUpdateOptions
                    {
                        InvoiceSettings = new CustomerInvoiceSettingsOptions
                        {
                            DefaultPaymentMethod = pMethodId,
                        },
                        Description = "Update subscription for medical center admin",
                        Metadata = new Dictionary<string, string>
                        {
                            { "FullName", customerDto.FirstName + " " + customerDto.LastName},
                            { "UserName", customerDto.User.UserName},
                            { "Email", customerDto.User.Email},
                            { "Type","0002# Subscription update fees"},
                            { "CustomOperationCode","0002"},
                            { "Date",ConvertToLocalTime(DateTime.Now).ToString()},
                        }
                    };

                    var cs = new CustomerService();
                    cs.Update(customerDto.StipeId, cuo);


                }
                catch (Exception ec)
                {
                    return ec.ToString();
                }

                try
                {

                    var options1 = new SubscriptionCreateOptions
                    {
                        Customer = customerDto.StipeId, //"cus_My7F0cgwvVTWmj", // customer should add card and payment method
                        Items = new List<SubscriptionItemOptions>
                            {
                                new SubscriptionItemOptions
                                {
                                Price =subscriptionPlan.PriceId, // "price_1MEB2kItIzR7Vx2u4X4Ui2cY", //recurring price
                                },
                            },

                        //TrialEnd= ghafDbContext.SubscriptionPlan.Find(request.PlanId).PriceRecuencyInterval=="month"?DateTime.Now.AddMonths(1): DateTime.Now.AddMonths(12),

                        CancelAt = expireDate,


                    };

                    var service1 = new SubscriptionService();
                    subscription = service1.Create(options1);
                }

                catch
                {
                    return "Failed to create subsription, make sure that you have enoough balance";
                }


                customerDto.StripeSubcriptionId = subscription.Id;
                customerDto.SubscriptionPlanId = subscriptionPlan.Id;
                customerDto.PaidAt = DateTime.Now;
                customerDto.SubscriptionExpired = false;
                customerDto.SubscriptionExpireDate = expireDate;
                customerDto.SetUpFeesIsPayed = true;
                customerDto.StripePaymentMethodId = pMethodId;
                customerDto.PaidAccountIsActive = true;

                cmsDbContext.CenterAdmin.Attach(customerDto);

                cmsDbContext.Entry(customerDto).Property(x => x.StipeId).IsModified = true;
                cmsDbContext.Entry(customerDto).Property(x => x.StripeSubcriptionId).IsModified = true;
                cmsDbContext.Entry(customerDto).Property(x => x.SubscriptionPlanId).IsModified = true;
                cmsDbContext.Entry(customerDto).Property(x => x.PaidAt).IsModified = true;
                cmsDbContext.Entry(customerDto).Property(x => x.SubscriptionExpired).IsModified = true;
                cmsDbContext.Entry(customerDto).Property(x => x.SubscriptionExpireDate).IsModified = true;
                cmsDbContext.Entry(customerDto).Property(x => x.SetUpFeesIsPayed).IsModified = true;
                cmsDbContext.Entry(customerDto).Property(x => x.StripePaymentMethodId).IsModified = true;
                cmsDbContext.Entry(customerDto).Property(x => x.PaidAccountIsActive).IsModified = true;

                cmsDbContext.SaveChanges();


                return "";
            }
            public string CancelSubscription()
            {
                Guid UserId = (Guid)_userService.GetMyId();
                CenterAdmin customerDto = cmsDbContext.CenterAdmin.Include(a => a.User).Where(a => a.Id == UserId).FirstOrDefault();
                SubscriptionPlan subscriptionPlan = cmsDbContext.SubscriptionPlan.FirstOrDefault(a => a.Id == customerDto.SubscriptionPlanId);
                StripeConfiguration.ApiKey = cmsDbContext.MySystemConfiguration.FirstOrDefault().StripePrivateKey;

                //Cancel subscription
                var service = new SubscriptionService();
                try
                {

                    Subscription subscription = service.Cancel(customerDto.StripeSubcriptionId);
                }
                catch
                {
                    return  "You failed to cancel your subscription";

                }

                //delete customer
                var service1 = new CustomerService();

                try
                {
                    service1.Delete(customerDto.StipeId);
                }
                catch
                {
                    return  "You failed to delete customer";
                }

                try
                {
                    customerDto.StipeId = "";
                    customerDto.StripeSubcriptionId = null;
                    customerDto.SubscriptionPlanId = null;
                    customerDto.PaidAt = null;
                    customerDto.PaidAccountIsActive = false;
                    customerDto.SetUpFeesIsPayed = false;

                    cmsDbContext.CenterAdmin.Attach(customerDto);

                    cmsDbContext.Entry(customerDto).Property(x => x.StipeId).IsModified = true;
                    cmsDbContext.Entry(customerDto).Property(x => x.StripeSubcriptionId).IsModified = true;
                    cmsDbContext.Entry(customerDto).Property(x => x.SubscriptionPlanId).IsModified = true;
                    cmsDbContext.Entry(customerDto).Property(x => x.PaidAt).IsModified = true;
                    cmsDbContext.Entry(customerDto).Property(x => x.PaidAccountIsActive).IsModified = true;
                    cmsDbContext.Entry(customerDto).Property(x => x.SetUpFeesIsPayed).IsModified = true;

                    cmsDbContext.SaveChanges();

                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }


                return "";
            }
            public string UpdateCenterSetUpSteps(Guid id, int step)
            {
                using (var transaction=cmsDbContext.Database.BeginTransaction())
                {
                    try
                    {
                        CenterSetUpSteps centerSetUpSteps = cmsDbContext.CenterSetUpSteps.Find(id);
                        centerSetUpSteps.StepNumer=step;

                        cmsDbContext.CenterSetUpSteps.Attach(centerSetUpSteps);
                        cmsDbContext.SaveChanges();

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
