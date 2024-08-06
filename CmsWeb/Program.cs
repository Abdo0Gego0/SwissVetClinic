using CmsDataAccess;
using CmsDataAccess.DbModels;
using ServicesLibrary.RepeatedJobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Quartz;
using System.Configuration;
using System.Data;
using System.Reflection;
using System.Text;
using ServicesLibrary.EmailServices.EmailServices;
using ServicesLibrary.UserServices;
using Kendo.Mvc.Resources;
using ServicesLibrary;
using ServicesLibrary.NavigationBar;
using ServicesLibrary.SelectListServices;
using ServicesLibrary.MedicalCenterServices;
using ServicesLibrary.PersonServices;
using ServicesLibrary.MySystemConfigurationServices;
using ServicesLibrary.StripeServices;
using CmsWeb.Hubs;
using ServicesLibrary.AppointmentSRVC;
using Kendo.Mvc.UI;

using ServicesLibrary.BlazorAppointments;
using CmsWeb.MyMiddleWare;
using NoificationManager.MobileModels;
using ServicesLibrary.CenterProductServices;

using DinkToPdf.Contracts;
using DinkToPdf;
using ServicesLibrary.PrintServices;



if (!Directory.Exists("PatientFiles"))
{
    Directory.CreateDirectory("PatientFiles");
}

if (!Directory.Exists("Uploads"))
{
    Directory.CreateDirectory("Uploads");
}

if (!Directory.Exists("Uploads/Files"))
{
    Directory.CreateDirectory("Uploads/Files");
}

if (!Directory.Exists("Uploads/Images"))
{
    Directory.CreateDirectory("Uploads/Images");
}







var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR().AddJsonProtocol(x => x.PayloadSerializerOptions.PropertyNamingPolicy = null);


builder.Services.AddScoped<ISeedDb, SeedDb>();
builder.Services.AddScoped<IPersonService, ServicesLibrary.PersonServices.PersonService>();
builder.Services.AddScoped<ICenterProductService, CenterProductService>();
builder.Services.AddScoped<SchedulerMeetingService, SchedulerMeetingService>();
builder.Services.AddScoped<IMedicalCenterService, MedicalCenterService>();
builder.Services.AddScoped<ISelectListService, SelectListService>();
builder.Services.AddScoped<IMySystemConfigurationService, MySystemConfigurationService>();
builder.Services.AddScoped<IStripeService, StripeService>();
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
builder.Services.AddScoped<PdfConversionService>();

builder.Services.Configure<FcmNotificationSetting>(builder.Configuration.GetSection("FcmNotificationSetting"));
builder.Services.AddScoped<INotificationService, NotificationService>();


builder.Services.AddScoped<INavService, NavService>();

builder.Services.AddScoped<AppointmentDataService>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddServerSideBlazor(o => o.DetailedErrors = true);

builder.Services.AddRazorPages()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
        {
            var assemblyName = new AssemblyName(typeof(Messages).GetTypeInfo().Assembly.FullName);
            return factory.Create("Messages", assemblyName.Name);
        };
    });


// Add services to the container.
builder.Services.AddControllersWithViews()
                // Maintain property names during serialization. See:
                // https://github.com/aspnet/Announcements/issues/194
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver());
// Add Kendo UI services to the services container"
builder.Services.AddKendo();

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
}

)
//.AddDefaultUI()
.AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>(TokenOptions.DefaultProvider)
.AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddHttpContextAccessor();


builder.Services.AddAuthentication(options =>
{
    // custom scheme defined in .AddPolicyScheme() below


    options.DefaultAuthenticateScheme = "JWT_OR_COOKIE";
    options.DefaultChallengeScheme = "JWT_OR_COOKIE";
    options.DefaultScheme = "JWT_OR_COOKIE";

})
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = new PathString("/Auth/Login/Index");
        options.ExpireTimeSpan = TimeSpan.FromDays(15);
        options.AccessDeniedPath = new PathString("/Auth/Login/Index");
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("jwt:Key").Value)),
            ValidIssuer = builder.Configuration.GetSection("jwt:Issuer").Value,
            ValidAudience = builder.Configuration.GetSection("jwt:Audience").Value,

            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
        };
    })
    // this is the key piece!
    .AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
    {
        // runs on each request
        options.ForwardDefaultSelector = context =>
        {
            return "Cookies";


            // filter by auth type
            string authorization = context.Request.Headers[HeaderNames.Authorization];
            if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                return "Bearer";
            // otherwise always check for cookie auth
            return "Cookies";
        };
    });


builder.Services.AddDbContext<ApplicationDbContext>
    (options =>
    {
        options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings:DefaultConnection").Value);
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    });





builder.Services.AddQuartz(configure =>
{
    var jobKey = new JobKey(nameof(ProcessAppointmentsJob));

    configure
        .AddJob<ProcessAppointmentsJob>(jobKey)
        .AddTrigger(
            trigger => trigger.ForJob(jobKey).WithSimpleSchedule(
                schedule => schedule.WithIntervalInMinutes(5).RepeatForever()));

    configure.UseMicrosoftDependencyInjectionJobFactory();
});

builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

//app.UseStaticFiles(new StaticFileOptions()
//{
//    FileProvider = new PhysicalFileProvider(
//         Path.Combine(Directory.GetCurrentDirectory(), @"DesignFiles\\KendoUi")),
//    RequestPath = new PathString("/KendoUi"),
//    ServeUnknownFileTypes = true,
//});


app.MapBlazorHub();


app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
         Path.Combine(Directory.GetCurrentDirectory(), builder.Configuration.GetSection("Images:PathToImages").Value.ToString())),
    RequestPath = new PathString("/pImages"),
    ServeUnknownFileTypes = true,
});

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
         Path.Combine(Directory.GetCurrentDirectory(), builder.Configuration.GetSection("Files:PathToFiles").Value.ToString())),
    RequestPath = new PathString("/pFiles"),
    ServeUnknownFileTypes = true,
});

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
         Path.Combine(Directory.GetCurrentDirectory(), "PatientFiles")),
    RequestPath = new PathString("/visit"),
    ServeUnknownFileTypes = true,
});


var supportedCultures = new string[] { "en-US", "ar" };

var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);


app.UseRequestLocalization(localizationOptions);


app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<MyHub>("/chatHub");


//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

ControllerEndpointRouteBuilderExtensions.MapControllerRoute(app, "areas", "{area:exists}/{controller=Login}/{action=Index}/{id?}", (object)new
{
    area = "Auth",
    controller = "Login",
    action = "Index"
});

app.MapRazorPages();


//app.UseMiddleware<UpdateUserClaims>();

app.Run();