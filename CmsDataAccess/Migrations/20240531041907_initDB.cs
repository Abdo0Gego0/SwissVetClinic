using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CmsDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class initDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AboutUs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AboutUs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Appointment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionDuration = table.Column<int>(type: "int", nullable: false),
                    MedicalCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaseClinicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notified = table.Column<bool>(type: "bit", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AppointmentType = table.Column<int>(type: "int", nullable: false),
                    AppointmentStatus = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsFinished = table.Column<bool>(type: "bit", nullable: false),
                    IsStarted = table.Column<bool>(type: "bit", nullable: false),
                    IsFirstVisit = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAllDay = table.Column<bool>(type: "bit", nullable: false),
                    IsReadonly = table.Column<bool>(type: "bit", nullable: false),
                    RecurrenceRule = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecurrenceException = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecurrenceID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timezone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndTimezone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTimezone = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceTable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdentityUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicalCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShiftTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnteringTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LeavingTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShiftEnteringTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShiftLeavingTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DayOff = table.Column<bool>(type: "bit", nullable: false),
                    ReasonForAbsence = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BaseClinic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MedicalCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscriptionExpired = table.Column<bool>(type: "bit", nullable: false),
                    IsShownOnMobile = table.Column<bool>(type: "bit", nullable: false),
                    IsDraft = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ClinicSpecialtyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisitCost = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseClinic", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookingPolicy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllowedBookingNumberPerday = table.Column<int>(type: "int", nullable: true),
                    MaximunAllowedMissedAppointments = table.Column<int>(type: "int", nullable: true),
                    MedicalCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingPolicy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CenterSetUpSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CenterAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StepNumer = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CenterSetUpSteps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClinicRating",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdentityUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaseClinicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RatingOwner = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicRating", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClinicSpecialty",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicalCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicSpecialty", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocSpecBreakRelation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorSpecialityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocSpecBreakRelation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DoctorRating",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RatingOwner = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorRating", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DoctorSpeciality",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicalCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorSpeciality", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeShift",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdentityUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShiftTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeShift", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Family",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FamilyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Family", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FAQ",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQ", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MedicalCenterRating",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdentityUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicalCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RatingOwner = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalCenterRating", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MySystemConfiguration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApiUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SystemTimeZone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UseEmailVerfication = table.Column<bool>(type: "bit", nullable: false),
                    UseCashPayementForSubscriber = table.Column<bool>(type: "bit", nullable: false),
                    StripePrivateKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StripePublicKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailHost = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailPort = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailPassword = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MySystemConfiguration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationPolicy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicalCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoursBeforeAppointment = table.Column<int>(type: "int", nullable: true),
                    SessionVideos = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationPolicy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PatientArchivedNotification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotiType = table.Column<int>(type: "int", nullable: true),
                    PetOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsAuto = table.Column<bool>(type: "bit", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientArchivedNotification", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PatientVisit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MedicalCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaseClinicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsFirstVisit = table.Column<bool>(type: "bit", nullable: false),
                    IsFinished = table.Column<bool>(type: "bit", nullable: false),
                    VisistDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<double>(type: "float", nullable: false),
                    BloodPressure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Glucose = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HearBeat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Height = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weight = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Age = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Diagnosis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisitVideo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientVisit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonNotification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MedicalCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaseClinicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonNotification", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BloodType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MedicalCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShiftTable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicalCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShiftName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionApplication",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassportNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NationalCardId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubscriberPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubscriberEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VerificationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VerificationExpireDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailVerfied = table.Column<bool>(type: "bit", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicenseExpireDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LicenseImageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PassportImageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Accepted = table.Column<int>(type: "int", nullable: false),
                    IsSeen = table.Column<bool>(type: "bit", nullable: false),
                    ResponseFromAdmin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CenterName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CenterAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CenterAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionApplication", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriceCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PriceRecuencyInterval = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PriceAmount = table.Column<long>(type: "bigint", nullable: false),
                    PriceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SetUpCost = table.Column<long>(type: "bigint", nullable: false),
                    StripeId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FreeDays = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemTimeZone",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StandardName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IANAID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemTimeZone", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TermsAndConditions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermsAndConditions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TherapyGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicalCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TherapyGoals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TherapyPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicalCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientVideoEveryNumberOfSessions = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TherapyPlan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserResetToken",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserResetToken", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ValidQR",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicalCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Secretkey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QRType = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidQR", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AboutUsTranslation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LangCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AboutUsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AboutUsText = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AboutUsTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AboutUsTranslation_AboutUs_AboutUsId",
                        column: x => x.AboutUsId,
                        principalTable: "AboutUs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AddressTranslation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LangCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Building = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AddressTranslation_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicalCenter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BlockedByAdmin = table.Column<bool>(type: "bit", nullable: false),
                    SubscriptionExpired = table.Column<bool>(type: "bit", nullable: false),
                    PaidAccountIsActive = table.Column<bool>(type: "bit", nullable: false),
                    SubscriptionExpireDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LockMyCenter = table.Column<bool>(type: "bit", nullable: false),
                    ShowInMobileApplication = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    TimeZone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeyWords = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsTwentyFourHours = table.Column<bool>(type: "bit", nullable: false),
                    AddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UseEmailVerfication = table.Column<bool>(type: "bit", nullable: false),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalCenter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalCenter_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MotherName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gendre = table.Column<int>(type: "int", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassportNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NationalCardId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobCardNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BlockedByAdmin = table.Column<bool>(type: "bit", nullable: false),
                    AccountVerfied = table.Column<bool>(type: "bit", nullable: false),
                    DateTimeCreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VerificationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VerificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PreferredLanguage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fcm_token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CenterStatus = table.Column<int>(type: "int", nullable: false),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    PassportFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FamilyBookFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LaborCardFileName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Person_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Person_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BaseClinicTranslation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LangCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BaseClinicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseClinicTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaseClinicTranslation_BaseClinic_BaseClinicId",
                        column: x => x.BaseClinicId,
                        principalTable: "BaseClinic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClinicSpecialtyTranslation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LangCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClinicSpecialtyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicSpecialtyTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClinicSpecialtyTranslation_ClinicSpecialty_ClinicSpecialtyId",
                        column: x => x.ClinicSpecialtyId,
                        principalTable: "ClinicSpecialty",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DoctorSpecialityTranslation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LangCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DoctorSpecialityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorSpecialityTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorSpecialityTranslation_DoctorSpeciality_DoctorSpecialityId",
                        column: x => x.DoctorSpecialityId,
                        principalTable: "DoctorSpeciality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FAQTranslation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FAQId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicalCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LangCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FAQTranslation_FAQ_FAQId",
                        column: x => x.FAQId,
                        principalTable: "FAQ",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitMeasurement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BaseClinicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientVisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitMeasurement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitMeasurement_PatientVisit_PatientVisitId",
                        column: x => x.PatientVisitId,
                        principalTable: "PatientVisit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitTreatment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BaseClinicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientVisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitTreatment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitTreatment_PatientVisit_PatientVisitId",
                        column: x => x.PatientVisitId,
                        principalTable: "PatientVisit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientAllergy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientAllergy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientAllergy_Pet_PetId",
                        column: x => x.PetId,
                        principalTable: "Pet",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PatientDiagnosis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiagnosisDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientDiagnosis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientDiagnosis_Pet_PetId",
                        column: x => x.PetId,
                        principalTable: "Pet",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PatientFamilyHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientFamilyHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientFamilyHistory_Pet_PetId",
                        column: x => x.PetId,
                        principalTable: "Pet",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PatientMedicalHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientMedicalHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientMedicalHistory_Pet_PetId",
                        column: x => x.PetId,
                        principalTable: "Pet",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PatientMedicineHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientMedicineHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientMedicineHistory_Pet_PetId",
                        column: x => x.PetId,
                        principalTable: "Pet",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PatientSurgicalHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientSurgicalHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientSurgicalHistory_Pet_PetId",
                        column: x => x.PetId,
                        principalTable: "Pet",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PatientTherapyGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TherapyGoal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDone = table.Column<int>(type: "int", nullable: false),
                    PetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientTherapyGoals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientTherapyGoals_Pet_PetId",
                        column: x => x.PetId,
                        principalTable: "Pet",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPlanTranslation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LangCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubscriptionPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlanTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionPlanTranslation_SubscriptionPlan_SubscriptionPlanId",
                        column: x => x.SubscriptionPlanId,
                        principalTable: "SubscriptionPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TermsAndConditionsTranslation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LangCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TermsAndConditionsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TermsAndConditionsText = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermsAndConditionsTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TermsAndConditionsTranslation_TermsAndConditions_TermsAndConditionsId",
                        column: x => x.TermsAndConditionsId,
                        principalTable: "TermsAndConditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TherapyGoalsTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LangCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TherapyGoalsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TherapyGoalsTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TherapyGoalsTranslations_TherapyGoals_TherapyGoalsId",
                        column: x => x.TherapyGoalsId,
                        principalTable: "TherapyGoals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TherapyGoalsTherapyPlan",
                columns: table => new
                {
                    TherapyGoalsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TherapyPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TherapyGoalsTherapyPlan", x => new { x.TherapyGoalsId, x.TherapyPlanId });
                    table.ForeignKey(
                        name: "FK_TherapyGoalsTherapyPlan_TherapyGoals_TherapyGoalsId",
                        column: x => x.TherapyGoalsId,
                        principalTable: "TherapyGoals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TherapyGoalsTherapyPlan_TherapyPlan_TherapyPlanId",
                        column: x => x.TherapyPlanId,
                        principalTable: "TherapyPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TherapyPlanTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LangCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TherapyPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TherapyPlanTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TherapyPlanTranslations_TherapyPlan_TherapyPlanId",
                        column: x => x.TherapyPlanId,
                        principalTable: "TherapyPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MedicalCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactInfo_MedicalCenter_MedicalCenterId",
                        column: x => x.MedicalCenterId,
                        principalTable: "MedicalCenter",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MedicalCenterTranslation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LangCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MedicalCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalCenterTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalCenterTranslation_MedicalCenter_MedicalCenterId",
                        column: x => x.MedicalCenterId,
                        principalTable: "MedicalCenter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpeningHours",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    OpeningTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ClosingTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ClinicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsTwentyFourHours = table.Column<bool>(type: "bit", nullable: false),
                    BaseClinicId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MedicalCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpeningHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpeningHours_BaseClinic_BaseClinicId",
                        column: x => x.BaseClinicId,
                        principalTable: "BaseClinic",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OpeningHours_MedicalCenter_MedicalCenterId",
                        column: x => x.MedicalCenterId,
                        principalTable: "MedicalCenter",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CenterAdmin",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StipeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StripeSubcriptionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StripePaymentMethodId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SetUpFeesIsPayed = table.Column<bool>(type: "bit", nullable: false),
                    SubscriptionExpired = table.Column<bool>(type: "bit", nullable: false),
                    PaidAccountIsActive = table.Column<bool>(type: "bit", nullable: false),
                    SubscriptionExpireDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubscriptionPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    SAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MedicalCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CenterAdmin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CenterAdmin_Person_Id",
                        column: x => x.Id,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Doctor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClinicId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MedicalCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Doctor_Person_Id",
                        column: x => x.Id,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClinicId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MedicalCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employee_Person_Id",
                        column: x => x.Id,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PetOwner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GeneralNumber = table.Column<int>(type: "int", nullable: false),
                    MedicalRecordNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InsuranceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InsuranceCompany = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BloodType = table.Column<int>(type: "int", nullable: true),
                    MedicalCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetOwner", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PetOwner_Person_Id",
                        column: x => x.Id,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SysAdmin",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    SAddress = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysAdmin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SysAdmin_Person_Id",
                        column: x => x.Id,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Certificate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificate_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctor",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DoctorDoctorSpeciality",
                columns: table => new
                {
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorSpecialityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorDoctorSpeciality", x => new { x.DoctorId, x.DoctorSpecialityId });
                    table.ForeignKey(
                        name: "FK_DoctorDoctorSpeciality_DoctorSpeciality_DoctorSpecialityId",
                        column: x => x.DoctorSpecialityId,
                        principalTable: "DoctorSpeciality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DoctorDoctorSpeciality_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DoctorTranslation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LangCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorTranslation_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CertificateTranslation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LangCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateTranslation_Certificate_CertificateId",
                        column: x => x.CertificateId,
                        principalTable: "Certificate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AboutUsTranslation_AboutUsId",
                table: "AboutUsTranslation",
                column: "AboutUsId");

            migrationBuilder.CreateIndex(
                name: "IX_AddressTranslation_AddressId",
                table: "AddressTranslation",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BaseClinicTranslation_BaseClinicId",
                table: "BaseClinicTranslation",
                column: "BaseClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_DoctorId",
                table: "Certificate",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateTranslation_CertificateId",
                table: "CertificateTranslation",
                column: "CertificateId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicSpecialtyTranslation_ClinicSpecialtyId",
                table: "ClinicSpecialtyTranslation",
                column: "ClinicSpecialtyId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactInfo_MedicalCenterId",
                table: "ContactInfo",
                column: "MedicalCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorDoctorSpeciality_DoctorSpecialityId",
                table: "DoctorDoctorSpeciality",
                column: "DoctorSpecialityId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorSpecialityTranslation_DoctorSpecialityId",
                table: "DoctorSpecialityTranslation",
                column: "DoctorSpecialityId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorTranslation_DoctorId",
                table: "DoctorTranslation",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_FAQTranslation_FAQId",
                table: "FAQTranslation",
                column: "FAQId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalCenter_AddressId",
                table: "MedicalCenter",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalCenterTranslation_MedicalCenterId",
                table: "MedicalCenterTranslation",
                column: "MedicalCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_OpeningHours_BaseClinicId",
                table: "OpeningHours",
                column: "BaseClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_OpeningHours_MedicalCenterId",
                table: "OpeningHours",
                column: "MedicalCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAllergy_PetId",
                table: "PatientAllergy",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDiagnosis_PetId",
                table: "PatientDiagnosis",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientFamilyHistory_PetId",
                table: "PatientFamilyHistory",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMedicalHistory_PetId",
                table: "PatientMedicalHistory",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMedicineHistory_PetId",
                table: "PatientMedicineHistory",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientSurgicalHistory_PetId",
                table: "PatientSurgicalHistory",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientTherapyGoals_PetId",
                table: "PatientTherapyGoals",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_Person_AddressId",
                table: "Person",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Person_UserId",
                table: "Person",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlanTranslation_SubscriptionPlanId",
                table: "SubscriptionPlanTranslation",
                column: "SubscriptionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_TermsAndConditionsTranslation_TermsAndConditionsId",
                table: "TermsAndConditionsTranslation",
                column: "TermsAndConditionsId");

            migrationBuilder.CreateIndex(
                name: "IX_TherapyGoalsTherapyPlan_TherapyPlanId",
                table: "TherapyGoalsTherapyPlan",
                column: "TherapyPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_TherapyGoalsTranslations_TherapyGoalsId",
                table: "TherapyGoalsTranslations",
                column: "TherapyGoalsId");

            migrationBuilder.CreateIndex(
                name: "IX_TherapyPlanTranslations_TherapyPlanId",
                table: "TherapyPlanTranslations",
                column: "TherapyPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitMeasurement_PatientVisitId",
                table: "VisitMeasurement",
                column: "PatientVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitTreatment_PatientVisitId",
                table: "VisitTreatment",
                column: "PatientVisitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AboutUsTranslation");

            migrationBuilder.DropTable(
                name: "AddressTranslation");

            migrationBuilder.DropTable(
                name: "Appointment");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AttendanceTable");

            migrationBuilder.DropTable(
                name: "BaseClinicTranslation");

            migrationBuilder.DropTable(
                name: "BookingPolicy");

            migrationBuilder.DropTable(
                name: "CenterAdmin");

            migrationBuilder.DropTable(
                name: "CenterSetUpSteps");

            migrationBuilder.DropTable(
                name: "CertificateTranslation");

            migrationBuilder.DropTable(
                name: "ClinicRating");

            migrationBuilder.DropTable(
                name: "ClinicSpecialtyTranslation");

            migrationBuilder.DropTable(
                name: "ContactInfo");

            migrationBuilder.DropTable(
                name: "DocSpecBreakRelation");

            migrationBuilder.DropTable(
                name: "DoctorDoctorSpeciality");

            migrationBuilder.DropTable(
                name: "DoctorRating");

            migrationBuilder.DropTable(
                name: "DoctorSpecialityTranslation");

            migrationBuilder.DropTable(
                name: "DoctorTranslation");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "EmployeeShift");

            migrationBuilder.DropTable(
                name: "Family");

            migrationBuilder.DropTable(
                name: "FAQTranslation");

            migrationBuilder.DropTable(
                name: "MedicalCenterRating");

            migrationBuilder.DropTable(
                name: "MedicalCenterTranslation");

            migrationBuilder.DropTable(
                name: "MySystemConfiguration");

            migrationBuilder.DropTable(
                name: "NotificationPolicy");

            migrationBuilder.DropTable(
                name: "OpeningHours");

            migrationBuilder.DropTable(
                name: "PatientAllergy");

            migrationBuilder.DropTable(
                name: "PatientArchivedNotification");

            migrationBuilder.DropTable(
                name: "PatientDiagnosis");

            migrationBuilder.DropTable(
                name: "PatientFamilyHistory");

            migrationBuilder.DropTable(
                name: "PatientMedicalHistory");

            migrationBuilder.DropTable(
                name: "PatientMedicineHistory");

            migrationBuilder.DropTable(
                name: "PatientSurgicalHistory");

            migrationBuilder.DropTable(
                name: "PatientTherapyGoals");

            migrationBuilder.DropTable(
                name: "PersonNotification");

            migrationBuilder.DropTable(
                name: "PetOwner");

            migrationBuilder.DropTable(
                name: "ShiftTable");

            migrationBuilder.DropTable(
                name: "SubscriptionApplication");

            migrationBuilder.DropTable(
                name: "SubscriptionPlanTranslation");

            migrationBuilder.DropTable(
                name: "SysAdmin");

            migrationBuilder.DropTable(
                name: "SystemTimeZone");

            migrationBuilder.DropTable(
                name: "TermsAndConditionsTranslation");

            migrationBuilder.DropTable(
                name: "TherapyGoalsTherapyPlan");

            migrationBuilder.DropTable(
                name: "TherapyGoalsTranslations");

            migrationBuilder.DropTable(
                name: "TherapyPlanTranslations");

            migrationBuilder.DropTable(
                name: "UserResetToken");

            migrationBuilder.DropTable(
                name: "ValidQR");

            migrationBuilder.DropTable(
                name: "VisitMeasurement");

            migrationBuilder.DropTable(
                name: "VisitTreatment");

            migrationBuilder.DropTable(
                name: "AboutUs");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Certificate");

            migrationBuilder.DropTable(
                name: "ClinicSpecialty");

            migrationBuilder.DropTable(
                name: "DoctorSpeciality");

            migrationBuilder.DropTable(
                name: "FAQ");

            migrationBuilder.DropTable(
                name: "BaseClinic");

            migrationBuilder.DropTable(
                name: "MedicalCenter");

            migrationBuilder.DropTable(
                name: "Pet");

            migrationBuilder.DropTable(
                name: "SubscriptionPlan");

            migrationBuilder.DropTable(
                name: "TermsAndConditions");

            migrationBuilder.DropTable(
                name: "TherapyGoals");

            migrationBuilder.DropTable(
                name: "TherapyPlan");

            migrationBuilder.DropTable(
                name: "PatientVisit");

            migrationBuilder.DropTable(
                name: "Doctor");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
