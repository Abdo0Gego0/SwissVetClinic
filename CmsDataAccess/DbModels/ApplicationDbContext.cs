using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.DbModels
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string constring1 = "Data Source=SQL8006.site4now.net,1433;Initial Catalog=db_aa9b63_veterinary;User Id=db_aa9b63_veterinary_admin;Password=95dd3ghFF_GG_;TrustServerCertificate=True;MultipleActiveResultSets=true";
            string constring2 = "Data Source=DESKTOP-NT0OFC8\\MSSQLSERVER01;Initial Catalog=VETERINARY;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

            string current = constring1;

            optionsBuilder.UseSqlServer(current);

            optionsBuilder.ConfigureWarnings(w => w.Ignore(SqlServerEventId.SavepointsDisabledBecauseOfMARS));

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(current);
                optionsBuilder.ConfigureWarnings(w => w.Ignore(SqlServerEventId.SavepointsDisabledBecauseOfMARS));
            }
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PetOwner>().ToTable("PetOwner");
            modelBuilder.Entity<Doctor>().ToTable("Doctor");
            modelBuilder.Entity<Employee>().ToTable("Employee");
            modelBuilder.Entity<CenterAdmin>().ToTable("CenterAdmin");
            modelBuilder.Entity<SysAdmin>().ToTable("SysAdmin");

            modelBuilder.Entity<TherapyPlan>()
                .HasMany(e => e.TherapyGoals)
                .WithMany(e => e.TherapyPlan)
                .UsingEntity<TherapyGoalsTherapyPlan>();

            modelBuilder.Entity<Doctor>()
                .HasMany(d => d.DoctorSpeciality)
                .WithMany(ds => ds.Doctor)
                .UsingEntity<Dictionary<string, object>>(
                    "DoctorDoctorSpeciality",
                    j => j.HasOne<DoctorSpeciality>().WithMany().HasForeignKey("DoctorSpecialityId"),
                    j => j.HasOne<Doctor>().WithMany().HasForeignKey("DoctorId")
                );

            // Receipt and ReceiptProduct Configuration
            modelBuilder.Entity<Receipt>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<ReceiptProduct>()
                .HasKey(rp => rp.ReceiptProductID);

            modelBuilder.Entity<Bill>()
                .HasMany(b => b.BillProducts)
                .WithOne(bp => bp.Bill)
                .HasForeignKey(bp => bp.BillID);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.BillProducts)
                .WithOne(bp => bp.Product)
                .HasForeignKey(bp => bp.ProductId);

            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Product>()
            .HasMany(p => p.ProductTranslation)
            .WithOne()
            .HasForeignKey(pt => pt.ProductId);

        }

        public DbSet<OpeningHours> OpeningHours { get; set; }
        public DbSet<ContactInfo> ContactInfo { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<AddressTranslation> AddressTranslation { get; set; }
        public DbSet<MySystemConfiguration> MySystemConfiguration { get; set; }
        public DbSet<MedicalCenter> MedicalCenter { get; set; }
        public DbSet<MedicalCenterTranslation> MedicalCenterTranslation { get; set; }
        public DbSet<BaseClinic> BaseClinic { get; set; }
        public DbSet<BaseClinicTranslation> BaseClinicTranslation { get; set; }
        public DbSet<ClinicSpecialty> ClinicSpecialty { get; set; }
        public DbSet<ClinicSpecialtyTranslation> ClinicSpecialtyTranslation { get; set; }
        public DbSet<SystemTimeZone> SystemTimeZone { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<Certificate> Certificate { get; set; }
        public DbSet<CertificateTranslation> CertificateTranslation { get; set; }
        public DbSet<DoctorRating> DoctorRating { get; set; }
        public DbSet<Doctor> Doctor { get; set; }
        public DbSet<DoctorTranslation> DoctorTranslation { get; set; }
        public DbSet<DoctorSpeciality> DoctorSpeciality { get; set; }
        public DbSet<DoctorSpecialityTranslation> DoctorSpecialityTranslation { get; set; }
        public DbSet<PetOwner> PetOwner { get; set; }
        public DbSet<BookingPolicy> BookingPolicy { get; set; }
        public DbSet<ClinicRating> ClinicRating { get; set; }
        public DbSet<MedicalCenterRating> MedicalCenterRating { get; set; }
        public DbSet<NotificationPolicy> NotificationPolicy { get; set; }
        public DbSet<ValidQR> ValidQR { get; set; }
        public DbSet<AttendanceTable> AttendanceTable { get; set; }
        public DbSet<ShiftTable> ShiftTable { get; set; }
        public DbSet<EmployeeShift> EmployeeShift { get; set; }
        public DbSet<SubscriptionApplication> SubscriptionApplication { get; set; }
        public DbSet<CenterAdmin> CenterAdmin { get; set; }
        public DbSet<TermsAndConditions> TermsAndConditions { get; set; }
        public DbSet<TermsAndConditionsTranslation> TermsAndConditionsTranslation { get; set; }
        public DbSet<AboutUs> AboutUs { get; set; }
        public DbSet<AboutUsTranslation> AboutUsTranslation { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<FAQ> FAQ { get; set; }
        public DbSet<FAQTranslation> FAQTranslation { get; set; }
        public DbSet<PatientArchivedNotification> PatientArchivedNotification { get; set; }
        public DbSet<SysAdmin> SysAdmin { get; set; }
        public DbSet<Family> Family { get; set; }
        public DbSet<CenterSetUpSteps> CenterSetUpSteps { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlan { get; set; }
        public DbSet<UserResetToken> UserResetToken { get; set; }
        public DbSet<IdentityUser> IdentityUser { get; set; }
        public DbSet<DocSpecBreakRelation> DocSpecBreakRelation { get; set; }
        public DbSet<TherapyGoals> TherapyGoals { get; set; }
        public DbSet<TherapyGoalsTranslations> TherapyGoalsTranslations { get; set; }
        public DbSet<TherapyPlan> TherapyPlan { get; set; }
        public DbSet<TherapyPlanTranslations> TherapyPlanTranslations { get; set; }
        public DbSet<TherapyGoalsTherapyPlan> TherapyGoalsTherapyPlan { get; set; }
        public DbSet<PatientDiagnosis> PatientDiagnosis { get; set; }
        public DbSet<PatientAllergy> PatientAllergy { get; set; }
        public DbSet<PatientFamilyHistory> PatientFamilyHistory { get; set; }
        public DbSet<PatientMedicalHistory> PatientMedicalHistory { get; set; }
        public DbSet<PatientMedicineHistory> PatientMedicineHistory { get; set; }
        public DbSet<PatientSurgicalHistory> PatientSurgicalHistory { get; set; }
        public DbSet<Appointment> Appointment { get; set; }
        public DbSet<PatientVisit> PatientVisit { get; set; }
        public DbSet<PatientTherapyGoals> PatientTherapyGoals { get; set; }
        public DbSet<VisitMeasurement> VisitMeasurement { get; set; }
        public DbSet<VisitTreatment> VisitTreatment { get; set; }
        public DbSet<PersonNotification> PersonNotification { get; set; }
        public DbSet<Pet> Pet { get; set; }
        public DbSet<Banner> Banner { get; set; }
        public DbSet<CenterServices> CenterServices { get; set; }
        public DbSet<CenterServicesTranslation> CenterServicesTranslation { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ProductTranslation> ProductTranslation { get; set; }
        public DbSet<ProductCategories> ProductCategories { get; set; }
        public DbSet<ProductCategoriesTranslation> ProductCategoriesTranslation { get; set; }
        public DbSet<SubProduct> SubProduct { get; set; }
        public DbSet<SubProductImage> SubProductImage { get; set; }
        public DbSet<SubproductCharacteristics> SubproductCharacteristics { get; set; }
        public DbSet<SubproductCharacteristicsTranslation> SubproductCharacteristicsTranslation { get; set; }
        public DbSet<AccountVerificationToken> AccountVerificationToken { get; set; }
        public DbSet<Basket> Basket { get; set; }
        public DbSet<Favourite> Favourite { get; set; }
        public DbSet<ProductReview> ProductReview { get; set; }
        public DbSet<COrder> COrder { get; set; }
        public DbSet<COrderItems> COrderItems { get; set; }
        public DbSet<CenterMedicineList> CenterMedicineList { get; set; }
        public DbSet<CenterMedicineUnit> CenterMedicineUnit { get; set; }
        public DbSet<VisitMedicine> VisitMedicine { get; set; }
        public DbSet<VisistBill> VisistBill { get; set; }
        public DbSet<Outcome> Outcome { get; set; }
        public DbSet<Receipt> Receipt { get; set; }
        public DbSet<ReceiptProduct> ReceiptProduct { get; set; }
        public DbSet<Bill> Bill { get; set; }
        public DbSet<BillProduct> BillProduct { get; set; }

    }
}

