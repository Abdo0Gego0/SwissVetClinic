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
using ServicesLibrary.AppointmentSRVC;
using System.ComponentModel.Design;
using ServicesLibrary.Services.AppointmentSRVC;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.SignalR;
using CmsWeb.Hubs;
using System.Web;
using ServicesLibrary.CenterProductServices;
using Stripe.Climate;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using ServicesLibrary.SelectListServices;



namespace CmsWeb.Areas.Center.Controllers
{
    [Area("CenterAdmin")]
    public class CenterProductsController : Controller
    {

        private readonly IHubContext<MyHub> _hubContext;

        private readonly IStringLocalizer<CmsResources.Messages> _localizer;
        private readonly ILogger<CenterProductsController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;

        private readonly IEmailSender _emailSender;
        private readonly IMedicalCenterService medicalCenterService;
        private readonly ICenterProductService productService;

        private readonly SchedulerMeetingService meetingService;

        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext cmsContext;

        public CenterProductsController(
            ICenterProductService productService_,
            IHubContext<MyHub> hubContext,

            IMedicalCenterService medicalCenterService_,
            IStringLocalizer<CmsResources.Messages> localizer,
            IConfiguration config,
            IUserService userService,
            ApplicationDbContext _cMDbContext,
            SchedulerMeetingService meetingService_,

            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,

            RoleManager<IdentityRole> roleManager,
            ILogger<CenterProductsController> logger)
        {
            productService= productService_;
            _hubContext = hubContext;
            meetingService = meetingService_;
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

        public JsonResult Get_SubProductImages(Guid id)
        {
            return Json(cmsContext.SubProductImage.Where(a => a.SubProductId == id).Select(a => new { Name = a.ImageName }));
        }

        public async Task<IActionResult> Index(Guid id)
        {
            ViewBag.DispalyName = _localizer["Products List"];
            ViewBag.PreviousActionDispalyName = _localizer["Products"];
            ViewBag.PreviousAction = "Index";

            return View("Ecommerce/_IndexProducts");
        }
        
        public ActionResult Read_Product([DataSourceRequest] DataSourceRequest request)
        {
            Guid guid = (Guid)_userService.GetMyCenterIdWeb();

            var result = cmsContext.Product
                .Include(a => a.ProductTranslation)
                .ToDataSourceResult(request);

            return Json(result);
        }

        public async Task<IActionResult> Create_Product()
        {
            ViewBag.DispalyName = _localizer["New Product"];
            ViewBag.PreviousActionDispalyName = _localizer["Products"];
            ViewBag.PreviousAction = "Index";
            ViewBag.ErrorMessage = "";

            return View("Ecommerce/_NewProduct");
        }

        [HttpPost]
        public async Task<IActionResult> Create_Product(CmsDataAccess.DbModels.Product model)
        {
            ViewBag.DispalyName = _localizer["New Product"];
            ViewBag.PreviousActionDispalyName = _localizer["Products"];
            ViewBag.PreviousAction = "Index";
            
            Guid res = productService.AddNewProduct(model);
            
            if (res==Guid.Empty)
            {
                ViewBag.ErrorMessage = "Db Error";
                return View("Ecommerce/_NewProduct",model);
            }

            return RedirectToAction("Create_SubProduct",new { id= res });
        }


        public async Task<IActionResult> Create_SubProduct(Guid id)
        {
            ViewBag.DispalyName = _localizer["New Product"];
            ViewBag.PreviousActionDispalyName = _localizer["Products"];
            ViewBag.PreviousAction = "Index";
            ViewBag.ErrorMessage = "";
            ViewBag.Id = id;

            return View("Ecommerce/_SubProductList");
        }


        [HttpPost]
        public async Task<IActionResult> Create_SubProduct(SubProduct model,List<IFormFile> files)
        {


            model.Id = Guid.Empty;


            ViewBag.DispalyName = _localizer["New Product"];
            ViewBag.PreviousActionDispalyName = _localizer["Products"];
            ViewBag.PreviousAction = "Index";

            ViewBag.ErrorMessage = "";
            ViewBag.Id = model.ProductId;

            model.SubProductImage = new List<SubProductImage>();

            if (model.SubproductCharacteristics==null || model.SubproductCharacteristics.Count == 0)
            {
                model.SubproductCharacteristics = new List<SubproductCharacteristics>();
                model.SubproductCharacteristics.Add(
                    new SubproductCharacteristics
                    {
                        SubproductCharacteristicsTranslation = new List<SubproductCharacteristicsTranslation> {
                        new SubproductCharacteristicsTranslation{ LangCode="en-US", Description=" "},
                        new SubproductCharacteristicsTranslation{ LangCode="ar", Description=" "},
}
                    }
                    );
            }

            foreach (var item in files)
            {
                model.SubProductImage.Add(
                    
                    new SubProductImage { ImageName=FileHandler.SaveUploadedFile(item)}
                    );
            }

            productService.AddNewSubProduct(model);

            return RedirectToAction("Create_SubProduct", new { id = model.ProductId });
        }
        
        [HttpPost]
        public async Task<IActionResult> Create_SubProductFromEdit(
            Guid ProductId,
            double? Price,
            double? Quantity,
            List<SubproductCharacteristics> subproductCharacteristics,List<IFormFile> files)
        {


            SubProduct model=new SubProduct(cmsContext);

            model.Id = Guid.Empty;
            model.Quantity = Quantity;
            model.Price = Price;
            model.ProductId=ProductId;

            ViewBag.DispalyName = _localizer["New Product"];
            ViewBag.PreviousActionDispalyName = _localizer["Products"];
            ViewBag.PreviousAction = "Index";

            ViewBag.ErrorMessage = "";
            ViewBag.Id = model.ProductId;

            model.SubProductImage = new List<SubProductImage>();

//            if (subproductCharacteristics == null || subproductCharacteristics.Count() == 0)
//            {
//                model.SubproductCharacteristics = new List<SubproductCharacteristics>();
//                model.SubproductCharacteristics.Add(
//                    new SubproductCharacteristics
//                    {
//                        SubproductCharacteristicsTranslation = new List<SubproductCharacteristicsTranslation> {
//                        new SubproductCharacteristicsTranslation{ LangCode="en-US", Description=" "},
//                        new SubproductCharacteristicsTranslation{ LangCode="ar", Description=" "},
//}
//                    }
//                    );
//            }
//            else
            {
                model.SubproductCharacteristics = new List<SubproductCharacteristics>();
                model.SubproductCharacteristics= subproductCharacteristics;
            }

            foreach (var item in files)
            {
                model.SubProductImage.Add(
                    
                    new SubProductImage { ImageName=FileHandler.SaveUploadedFile(item)}
                    );
            }

            productService.AddNewSubProduct(model);

            return RedirectToAction("Edit_Product", new { id = model.ProductId });
        }


        [HttpPost]
        public async Task<IActionResult> Edit_SubProductFromEdit(
            Guid Id__,
        Guid ProductId_,
        double? Price_,
        double? Quantity_,
        List<string>? imagesToDelete,
        List<IFormFile> files1
            )
        {

            

            ViewBag.DispalyName = _localizer["New Product"];
            ViewBag.PreviousActionDispalyName = _localizer["Products"];
            ViewBag.PreviousAction = "Index";

            SubProduct model = cmsContext.SubProduct.Find(Id__);


            foreach (var item in imagesToDelete)
            {
                cmsContext.SubProductImage.Remove(cmsContext.SubProductImage.FirstOrDefault(a => a.ImageName == item));
            }
            cmsContext.SaveChanges();




            foreach (var item in files1)
            {
                cmsContext.SubProductImage.Add(

                    new SubProductImage { ImageName = FileHandler.SaveUploadedFile(item), SubProductId = Id__ }
                    );
            }


            model.Quantity = Quantity_;
            model.Price = Price_;

            cmsContext.SubProduct.Attach(model);
            cmsContext.Entry(model).Property(a => a.Quantity).IsModified = true;
            cmsContext.Entry(model).Property(a => a.Price).IsModified = true;
            cmsContext.SaveChanges();

            return RedirectToAction("Edit_Product", new { id = model.ProductId });

        }


        public ActionResult Read_SubProduct([DataSourceRequest] DataSourceRequest request,Guid? id)
        {
            Guid guid = (Guid)_userService.GetMyCenterIdWeb();

            List< SubProduct> result = cmsContext.SubProduct
                .Where(a=>a.ProductId==id && !a.IsDeleted)
                .Include(a => a.SubProductImage)
                .Include(a => a.SubproductCharacteristics).ThenInclude(a=>a.SubproductCharacteristicsTranslation)
                .ToList();

            foreach (var item in result)
            {
                if (item.SubproductCharacteristics == null || item.SubproductCharacteristics.Count() == 0)
                {
                    item.SubproductCharacteristics = new List<SubproductCharacteristics>();
                    item.SubproductCharacteristics.Add(
                        new SubproductCharacteristics
                        {
                            SubproductCharacteristicsTranslation = new List<SubproductCharacteristicsTranslation> {
                                        new SubproductCharacteristicsTranslation{ LangCode="en-US", Description=" "},
                                        new SubproductCharacteristicsTranslation{ LangCode="ar", Description=" "},
    }
                        }
                        );
                }
            }

            return Json(result.ToDataSourceResult(request));
        }

        public async Task<IActionResult> Destroy_SubProduct([DataSourceRequest] DataSourceRequest request, SubProduct task)
        {
            task.DeleteFromDbAsync();
            return Json(_localizer["Success"]);
        }

        public async Task<IActionResult> Edit_Product(Guid id)
        {
            ViewBag.DispalyName = _localizer["Edit Product"];
            ViewBag.PreviousActionDispalyName = _localizer["Products"];
            ViewBag.PreviousAction = "Index";
            ViewBag.ErrorMessage = "";

            CmsDataAccess.DbModels.Product model = cmsContext.Product.Find(id);

            model = model.GetFromDb();

            return View("Ecommerce/_EditProduct", model);

        }

        [HttpPost]
        public async Task<IActionResult> Edit_Product(CmsDataAccess.DbModels.Product model)
        {
            ViewBag.DispalyName = _localizer["Edit Product"];
            ViewBag.PreviousActionDispalyName = _localizer["Products"];
            ViewBag.PreviousAction = "Index";
            ViewBag.ErrorMessage = "";

            cmsContext.ProductTranslation.Attach(model.ProductTranslation[0]);
            cmsContext.Entry(model.ProductTranslation[0]).State = EntityState.Modified;

            cmsContext.ProductTranslation.Attach(model.ProductTranslation[1]);
            cmsContext.Entry(model.ProductTranslation[1]).State = EntityState.Modified;

            cmsContext.SaveChanges();


            cmsContext.Product.Attach(model);
            cmsContext.Entry(model).Property(a => a.ProductCategoriesId).IsModified = true;
            cmsContext.SaveChanges();
            

            return View("Ecommerce/_EditProduct", model);

        }

        public ActionResult Read_SubproductCharacteristics([DataSourceRequest] DataSourceRequest request, Guid? id, string preferredCulture = "en-US")
        {
            Guid guid = (Guid)_userService.GetMyCenterIdWeb();
            var result = cmsContext.SubproductCharacteristics
                .Where(a=>a.SubProductId==(Guid) id)
                .Include(a => a.SubproductCharacteristicsTranslation)
                .ToList().ToDataSourceResult(request);
            return Json(result);
        }

        public async Task<IActionResult> Create_SubproductCharacteristics([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SubproductCharacteristics> ContactInfos)
        {
            Guid guid = (Guid)_userService.GetMyCenterIdWeb();

            //if (ModelState.IsValid)
            {
                foreach (var item in ContactInfos)
                {
                    cmsContext.SubproductCharacteristics.Add(
                        new SubproductCharacteristics
                        {
                            SubProductId = guid,
                            SubproductCharacteristicsTranslation = new List<SubproductCharacteristicsTranslation>
                            {
                                new SubproductCharacteristicsTranslation
                                {
                                    Description=item.SubproductCharacteristicsTranslation[0].Description,
                                    LangCode="en-us"
                                },
                                 new SubproductCharacteristicsTranslation
                                {
                                    Description=item.SubproductCharacteristicsTranslation[1].Description,
                                    LangCode="ar"
                                }
                            }
                        }
                        );
                }

                cmsContext.SaveChanges();
                return Json(_localizer["Success"]);
            }



        }

        [AcceptVerbs("Post")]
        public async Task<IActionResult> Update_SubproductCharacteristics([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SubproductCharacteristics> clinicSpecialties)
        {
            {
                foreach (var updatedItem in clinicSpecialties)
                {
                    cmsContext.SubproductCharacteristicsTranslation.Attach(updatedItem.SubproductCharacteristicsTranslation[0]);
                    cmsContext.Entry(updatedItem.SubproductCharacteristicsTranslation[0]).State = EntityState.Modified;



                    cmsContext.SubproductCharacteristicsTranslation.Attach(updatedItem.SubproductCharacteristicsTranslation[1]);
                    cmsContext.Entry(updatedItem.SubproductCharacteristicsTranslation[1]).State = EntityState.Modified;

                    cmsContext.SaveChanges();
                }
                return Json(_localizer["Success"]);
            }
        }

        public async Task<IActionResult> Destroy_SubproductCharacteristics([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SubproductCharacteristics> ContactInfos)
        {
            {
                foreach (var item in ContactInfos)
                {
                    item.DeleteFromDb();
                }
                return Json(_localizer["Success"]);
            }
        }



        [HttpPost]
        public async Task<IActionResult> ChangeProductStatus(Guid Id)
        {


            CmsDataAccess.DbModels.Product product =cmsContext.Product.Find(Id);
            product.IsVisible = !product.IsVisible;
            cmsContext.Entry(product).Property(a=>a.IsVisible).IsModified = true;
            cmsContext.SaveChanges();

            return Json("Ok");

        }







    }

}

