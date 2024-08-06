// See https://aka.ms/new-console-template for more information
using CmsDataAccess.DbModels;
using CmsDataAccess.ModelsDto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("Hello, World!");

ApplicationDbContext context = new ApplicationDbContext();

//context.Database.EnsureDeleted();


List<Doctor> doctors = context.Doctor.Include(a => a.User)
    .Include(a=>a.DoctorSpeciality)
    .Include(a=>a.DoctorTranslation)
    .Include(a=>a.Certificate).ThenInclude(a=>a.CertificateTranslation).ToList();
List<Certificate> doctors1 = context.Certificate.Include(a => a.CertificateTranslation).ToList();

int x1 = 0;
x1++;

#region Insert Medical center
//List<MedicalCenter> medicalCenters = context.MedicalCenter.ToList();

//foreach (var item in medicalCenters)
//{
//    item.DeleteFromDb();
//}

//MedicalCenter medicalCenter = new MedicalCenter
//{
//    Address = new Address
//    {
//        AddressTranslation= new List<AddressTranslation>
//        {
//            new AddressTranslation
//            {
//                Building="Building",
//                City="City",
//                Country="Country",
//                Street="Street",
//                FullAddress="FullAddress",
//                LangCode="en-US",
//            }
//        }
//    },
//    ContactInfo= new List<ContactInfo> { new ContactInfo
//    {
//        ContactType="ContactType",
//        ContactValue="ContactValue"
//    }
//    },
//    KeyWords= "KeyWords",
//    LogoImageName= "LogoImageName.png",
//    MedicalCenterTranslation= new List<MedicalCenterTranslation> { new MedicalCenterTranslation
//    {
//        LangCode="ar",
//        Description="Description",
//        Name="Name"
//    },
//    new MedicalCenterTranslation
//    {
//        LangCode="en-us",
//        Description="Description",
//        Name="Name"
//    }

//    },
//    OpeningHours= new List<OpeningHours> {  new OpeningHours
//    {
//        DayOfWeek= DayOfWeek.Sunday,
//        OpeningTime= new TimeSpan(09,00,00),
//        ClosingTime= new TimeSpan(04,00,00)
//    } ,

//     new OpeningHours
//    {
//        DayOfWeek= DayOfWeek.Monday,
//        OpeningTime= new TimeSpan(10,00,00),
//        ClosingTime= new TimeSpan(05,00,00)
//    }

//    },
//    TimeZone= ""
//};

//medicalCenter.InsertIntoDb();
//Console.WriteLine("Inserted");

#endregion



MedicalCenter Medic = context.MedicalCenter
    .Include(a=>a.Address).ThenInclude(a=>a.AddressTranslation)
    .Include(a=>a.ContactInfo)
    .Include(a=>a.MedicalCenterTranslation)
    .Include(a=>a.OpeningHours)
    .FirstOrDefault().GetModelByLnag("AR");


Console.WriteLine(Medic.ToString());


#region Insert Clinic Speciality

//ClinicSpecialty clinicSpecialty = new ClinicSpecialty
//{
//    ClinicSpecialtyTranslation =new List<ClinicSpecialtyTranslation> { 
//        new ClinicSpecialtyTranslation
//        {
//            LangCode="en-us",
//            Name="NameEN"
//        },
//        new ClinicSpecialtyTranslation
//        {

//            LangCode="ar",
//            Name="NameAR"
//        }
//        }
//};

//clinicSpecialty.InsertIntoDb();



//Console.WriteLine(clinicSpecialty.ToString());

#endregion


#region Insert Clinic

//BaseClinic baseClinic = new BaseClinic
//{
//    ClinicSpecialtyId = context.ClinicSpecialty.FirstOrDefault().Id,
//    MedicalCenterId = Medic.Id,
//    OpeningHours = new List<OpeningHours> {
//    new OpeningHours
//    {
//        DayOfWeek = DayOfWeek.Sunday,
//        OpeningTime=new TimeSpan(09,00,00),
//        ClosingTime=new TimeSpan(05,00,00)
//    },

//    new OpeningHours
//    {
//        DayOfWeek = DayOfWeek.Monday,
//        OpeningTime=new TimeSpan(09,00,00),
//        ClosingTime=new TimeSpan(05,00,00)
//    }
//    },
//    RoomNumber = "RoomNumber1",
//    BaseClinicTranslation = new List<BaseClinicTranslation> {
//        new BaseClinicTranslation
//        {
//            LangCode="en-us",
//            Description="DescriptionEN",
//            Name="NameEN"
//        },
//        new BaseClinicTranslation
//        {

//            LangCode="ar",
//            Description="DescriptionAR",
//            Name="NameAR"
//        },
//        }
//};
//baseClinic.InsertIntoDb();
#endregion



BaseClinic baseClinic1 = context.BaseClinic.FirstOrDefault();
//BaseClinic baseClinico=baseClinic1.GetFromDb();
//baseClinico._LangCode= "en-us";
//Console.WriteLine(baseClinico.ToString());

#region Insert Docotr Speciality

//DoctorSpeciality doctorSpeciality1 = new DoctorSpeciality
//{
//    DoctorSpecialityTranslation =new List<DoctorSpecialityTranslation> {
//        new DoctorSpecialityTranslation
//        {
//            LangCode = "en-us",
//            Description="Description1EN"
//        },
//        new DoctorSpecialityTranslation
//        {
//            LangCode="ar",
//            Description="Description1AR"
//        }
//    }
//};

//DoctorSpeciality doctorSpeciality2 = new DoctorSpeciality
//{
//    DoctorSpecialityTranslation = new List<DoctorSpecialityTranslation> {
//        new DoctorSpecialityTranslation
//        {
//            LangCode = "en-us",
//            Description="Description2EN"
//        },
//        new DoctorSpecialityTranslation
//        {
//            LangCode="ar",
//            Description="Description2AR"
//        }
//    }
//};

//doctorSpeciality1.InsertIntoDb();
//doctorSpeciality2.InsertIntoDb();

#endregion

int x = 0;
x++;




