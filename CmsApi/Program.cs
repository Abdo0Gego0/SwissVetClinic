
using CmsDataAccess;
using CmsDataAccess.DbModels;
using CmsResources;
using CmsWeb.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using ServicesLibrary.EmailServices.EmailServices;
using ServicesLibrary.MedicalCenterServices;
using ServicesLibrary.StripeServices;
using ServicesLibrary.UserServices;
using Swashbuckle.AspNetCore.Filters;


//using NoificationManager.MobileNoti;
//using Swashbuckle.AspNetCore.Filters;
using System;
using System.Globalization;
using System.Reflection;
using System.Text;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR().AddJsonProtocol(x => x.PayloadSerializerOptions.PropertyNamingPolicy = null);


// Add services to the container.

// Database
builder.Services.AddDbContext<ApplicationDbContext>
    (options => options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings:DefaultConnection").Value)
    );

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<IMedicalCenterService, MedicalCenterService>();
builder.Services.AddScoped<IStripeService, StripeService>();

// Languages




builder.Services.AddControllers()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
        {
            var assemblyName = new AssemblyName(typeof(Messages).GetTypeInfo().Assembly.FullName);
            return factory.Create("Messages", assemblyName.Name);
        };
    });


builder.Services.AddHttpContextAccessor();


// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
})
    //.AddDefaultUI()

    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>(TokenOptions.DefaultProvider)
    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{

    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("jwt:Key").Value)),
        ValidIssuer = builder.Configuration.GetSection("jwt:Issuer").Value,
        ValidAudience = builder.Configuration.GetSection("jwt:Audience").Value,
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401; // Unauthorized
            context.Response.ContentType = "application/json";
            // Customize the response message
            var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<Messages>>();
            var message = localizer["UnauthorizedMessage"].Value;
            var result = JsonConvert.SerializeObject(new { message });
            return context.Response.WriteAsync(result);
        }
    };
});




builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard authorization header using the bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,

    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();


    options.SwaggerDoc("v1",
    new OpenApiInfo
    {
        Title = "SWISS CLINIC API - V1",
        Version = "v1",
        
    }
 );

    var filePath = Path.Combine(System.AppContext.BaseDirectory, "CmsApi.xml");
    options.IncludeXmlComments(filePath);


});

var app = builder.Build();

var supportedCultures = new string[] { "en-US", "ar" };


var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
    Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, builder.Configuration.GetSection("Images:PathToImages").Value.ToString())),
    RequestPath = new PathString("/pImages"),
    ServeUnknownFileTypes = true,
});


app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
    Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, builder.Configuration.GetSection("Files:PathToFiles").Value.ToString())),
    RequestPath = new PathString("/pFiles"),
    ServeUnknownFileTypes = true,
});

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
    Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, builder.Configuration.GetSection("PatientFiles:PathToFiles").Value.ToString())),
    RequestPath = new PathString("/visit"),
    ServeUnknownFileTypes = true,
});

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHub<MyHub>("/chatHub");

app.MapControllers();

app.Run();
