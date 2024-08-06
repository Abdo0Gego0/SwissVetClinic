
using CmsDataAccess;
using CmsDataAccess.DbModels;
using CmsDataAccess.Models;
using CmsDataAccess.Utils.ProductUtil;
using CmsResources;
using CmsWeb.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Nancy.Security;
using Org.BouncyCastle.Asn1.Cms;
using ServicesLibrary.MedicalCenterServices;
using ServicesLibrary.StripeServices;
using ServicesLibrary.UserServices;
using Stripe;
using Stripe.Climate;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Resources;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CmsApi.API.ProductsAPI
{
    [ApiController]
    [Route("[controller]")]
    public class CenterProductsController : ControllerBase
    {

        private readonly IHubContext<MyHub> _hubContext;

        private readonly IStringLocalizer<Messages> _localizer;
        private readonly IMedicalCenterService medicalCenterService;

        private readonly IStripeService stripeService;


        private readonly ILogger<CenterProductsController> _logger;

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;

        private readonly IEmailSender _emailSender;


        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext cmsContext;

        public CenterProductsController(
            IHubContext<MyHub> hubContext,

            IStripeService stripeService_,
             IMedicalCenterService medicalCenterService_,
            IStringLocalizer<Messages> localizer,
            IConfiguration config,
            IUserService userService,
            ApplicationDbContext _cMDbContext,

            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,


            ILogger<CenterProductsController> logger)
        {

            _hubContext = hubContext;

            stripeService = stripeService_;

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

        }


        /// <summary>
        /// get baisc types of center products
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-products-category")]
        public ActionResult<object> GetProductsCategory()
        {
            string lang = _userService.GetMyLanguage().ToLower();

            // Debugging: Log the language code
            Console.WriteLine($"Language Code: {lang}");

            var categories = cmsContext.ProductCategories
                .Include(a => a.ProductCategoriesTranslation)
                .ToList();

            // Debugging: Log the retrieved categories and translations
            foreach (var category in categories)
            {
                Console.WriteLine($"Category ID: {category.Id}");
                foreach (var translation in category.ProductCategoriesTranslation)
                {
                    Console.WriteLine($"  Translation ID: {translation.Id}, LangCode: {translation.LangCode.ToLower()}, Name: {translation.Name}");
                }
            }

            var res = categories.Select(a => new
            {
                Id = a.Id,
                Name = a.ProductCategoriesTranslation
                    .FirstOrDefault(t => t.LangCode.ToLower() == lang)?.Name ?? "No translation available",
                Image = a.ImageFullPath
            }).ToList();

            // Debugging: Log the resulting data
            Console.WriteLine("Resulting Data:");
            foreach (var item in res)
            {
                Console.WriteLine($"  ID: {item.Id}, Name: {item.Name}, Image: {item.Image}");
            }

            return new ObjectResult(new
            {
                status = StatusCodes.Status200OK,
                data = res,
                message = ""
            });
        }






        /// <summary>
        /// Get Products of the center
        /// </summary>
        /// <param name="text">name filter (contains)</param>
        /// <param name="CategoryId"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        [HttpGet("get-products")]
        public async Task<IActionResult> GetProducts(
    string? text,
    Guid? CategoryId,
    int PageSize = 50,
    int PageIndex = 1
)
        {
            if (PageSize > 50)
            {
                PageSize = 50;
            }

            string lang = _userService.GetMyLanguage();
            Guid? userId = _userService.GetMyId();

            try
            {
                using (var cmsContext = new ApplicationDbContext())
                {
                    // Initial query to fetch products
                    var query = cmsContext.Product
                        .Include(a => a.ProductTranslation)
                        .Include(a => a.SubProduct)
                        .Where(a => a.IsVisible && !a.IsDeleted);

                    if (CategoryId.HasValue)
                    {
                        query = query.Where(a => a.ProductCategoriesId == CategoryId.Value);
                    }

                    if (!string.IsNullOrEmpty(text))
                    {
                        query = query.Where(a =>
                            (a.ProductTranslation.Any(pt => pt.LangCode == lang && pt.Name.Contains(text))) ||
                            (a.ProductTranslation.Any(pt => pt.LangCode == lang && pt.KeyWords.Contains(text))) ||
                            (a.ProductTranslation.Any(pt => pt.LangCode == lang && pt.Description.Contains(text)))
                        );
                    }

                    var prods = await query
                        .Skip((PageIndex - 1) * PageSize)
                        .Take(PageSize)
                        .ToListAsync();

                    // Load additional related data
                    var subProductIds = prods.SelectMany(p => p.SubProduct ?? Enumerable.Empty<SubProduct>())
                                             .Select(sp => sp.Id)
                                             .Distinct()
                                             .ToList();
                    var characteristicsTranslations = await cmsContext.SubproductCharacteristicsTranslation
                        .Where(tr => subProductIds.Contains(tr.SubproductCharacteristicsId) && tr.LangCode == lang)
                        .ToListAsync();

                    List<Guid> favouriteProductIds = new List<Guid>();
                    if (userId.HasValue)
                    {
                        favouriteProductIds = await cmsContext.Favourite
                            .Where(f => f.PetOwnerId == userId.Value && f.ProductId.HasValue)
                            .Select(f => f.ProductId.Value)
                            .ToListAsync();
                    }

                    List<object> res = new List<object>();

                    foreach (var item in prods)
                    {
                        var productTranslation = item.ProductTranslation?.FirstOrDefault(pt => pt.LangCode == lang);
                        string productName;
                        if (productTranslation == null)
                        {
                            _logger.LogWarning($"No translation found for product {item.Id} in language {lang}. Available languages: {string.Join(", ", item.ProductTranslation?.Select(pt => pt.LangCode) ?? new string[0])}");
                            productName = item.ProductTranslation?.FirstOrDefault()?.Name ?? "No translation available";
                        }
                        else
                        {
                            productName = productTranslation.Name ?? "Name not set in translation";
                        }

                        var subProducts = item.SubProduct ?? Enumerable.Empty<SubProduct>();

                        var subProductDetails = subProducts.Select(su => new
                        {
                            Id = su.Id,
                            Images = su.SubProductImage?.Select(a => a.ImageFullPath).ToList() ?? new List<string>(),
                            Quantity = su.Quantity,
                            Price = su.Price,
                            Characteristics = characteristicsTranslations
                                .Where(tr => su.SubproductCharacteristics != null &&
                                             su.SubproductCharacteristics.Any(sct => sct.Id == tr.SubproductCharacteristicsId))
                                .Select(tr => tr.Description)
                                .ToList(),
                            Available = su.Quantity > 0,
                            IsInBasket = ProductHanlder.IsSubProductInBasket(su.Id, userId),
                        }).ToList();

                        res.Add(new
                        {
                            Id = item.Id,
                            Name = productName,
                            Description = productTranslation?.Description ?? string.Empty,
                            SKU = productTranslation?.SKU ?? string.Empty,
                            KeyWords = productTranslation?.KeyWords ?? string.Empty,
                            CategoryId = item.ProductCategoriesId,
                            IsInFavourite = favouriteProductIds.Contains(item.Id),
                            ExpireDate = item.ExpireDate,
                            Type = item.Type,
                            Details = subProductDetails,
                            Review = item.GetReview()
                        });
                    }

                    return Ok(new
                    {
                        status = StatusCodes.Status200OK,
                        data = res,
                        message = "Product List"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting products");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }





        [HttpGet("get-product-by-id")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            string lang = _userService.GetMyLanguage();
            Guid? userId = _userService.GetMyId();
            try
            {
                using (var cmsContext = new ApplicationDbContext())
                {
                    var product = await cmsContext.Product
                        .Include(p => p.ProductTranslation)
                        .Include(p => p.SubProduct)
                        .ThenInclude(sp => sp.SubProductImage)
                        .Include(p => p.SubProduct)
                        .ThenInclude(sp => sp.SubproductCharacteristics)
                        .ThenInclude(sct => sct.SubproductCharacteristicsTranslation)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (product == null)
                    {
                        return NotFound(new
                        {
                            status = StatusCodes.Status404NotFound,
                            data = "",
                            message = "Product not found"
                        });
                    }

                    var productTranslation = product.ProductTranslation?.FirstOrDefault(pt => pt.LangCode == lang);
                    string productName;
                    if (productTranslation == null)
                    {
                        _logger.LogWarning($"No translation found for product {id} in language {lang}. Available languages: {string.Join(", ", product.ProductTranslation?.Select(pt => pt.LangCode) ?? new string[0])}");
                        productName = product.ProductTranslation?.FirstOrDefault()?.Name ?? "No translation available";
                    }
                    else
                    {
                        productName = productTranslation.Name ?? "Name not set in translation";
                    }

                    var subProductDetails = product.SubProduct?.Select(su => new
                    {
                        Id = su.Id,
                        Images = su.SubProductImage?.Select(a => a.ImageFullPath).ToList() ?? new List<string>(),
                        Quantity = su.Quantity,
                        Price = su.Price,
                        Characteristics = su.SubproductCharacteristics?
                            .SelectMany(sct => sct.SubproductCharacteristicsTranslation
                                .Where(tr => tr.LangCode == lang)
                                .Select(tr => tr.Description ?? "No description"))
                            .ToList() ?? new List<string>(),
                        Available = su.Quantity > 0,
                        IsInBasket = ProductHanlder.IsSubProductInBasket(su.Id, userId)
                    }).Cast<object>().ToList() ?? new List<object>();

                    var response = new
                    {
                        Id = product.Id,
                        Name = productName,
                        Description = productTranslation?.Description ?? string.Empty,
                        SKU = productTranslation?.SKU ?? string.Empty,
                        KeyWords = productTranslation?.KeyWords ?? string.Empty,
                        CategoryId = product.ProductCategoriesId,
                        IsInFavourite = userId.HasValue && await cmsContext.Favourite
                            .AnyAsync(f => f.PetOwnerId == userId.Value && f.ProductId == id),
                        ExpireDate = product.ExpireDate,
                        Type = product.Type,
                        Details = subProductDetails,
                        Review = product.GetReview()
                        
                    };

                    return Ok(new
                    {
                        status = StatusCodes.Status200OK,
                        data = response,
                        message = "Product Details"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting product details for product {ProductId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpPost("add-or-remove-to-favourite"), Authorize]
        public async Task<IActionResult> ChangeFavourite(Guid id)
        {

            Guid? userId = _userService.GetMyId();

            if (userId == null)
            {
                return new ObjectResult(new
                {
                    status = StatusCodes.Status200OK,
                    data = "",
                    message = "You must register first"
                });
            }

            if (cmsContext.Favourite.Any(a => a.ProductId == id && a.PetOwnerId == userId))
            {
                Favourite favourite = cmsContext.Favourite.FirstOrDefault(a => a.ProductId == id && a.PetOwnerId == (Guid)userId);
                cmsContext.Favourite.Remove(favourite);
                cmsContext.SaveChanges();

                return new ObjectResult(new
                {
                    status = StatusCodes.Status200OK,
                    data = "",
                    message = "Removed Successfully"
                });

            }
            else
            {
                cmsContext.Favourite.Add(new Favourite { ProductId = id, PetOwnerId = (Guid)userId });
                cmsContext.SaveChanges();

                return new ObjectResult(new
                {
                    status = StatusCodes.Status200OK,
                    data = "",
                    message = "Added Successfully"
                });
            }

        }

        [HttpGet("get-favourite"), Authorize]
        public async Task<IActionResult> GetFavourite()
        {
            string lang = _userService.GetMyLanguage();
            Guid? userId = _userService.GetMyId();

            if (userId == null)
            {
                return Unauthorized(new
                {
                    status = StatusCodes.Status401Unauthorized,
                    data = "",
                    message = "You must register first"
                });
            }

            try
            {
                List<Guid?> favNullable = await cmsContext.Favourite
                    .Where(a => a.PetOwnerId == userId)
                    .Select(a => a.ProductId)
                    .ToListAsync();

                List<Guid> fav = favNullable
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .ToList();

                List<object> res = new List<object>();

                foreach (var item in fav)
                {
                    var product = await cmsContext.Product
                        .Include(p => p.ProductTranslation.Where(pt => pt.LangCode == lang))
                        .Include(p => p.SubProduct)
                        .ThenInclude(sp => sp.SubProductImage)
                        .Include(p => p.SubProduct)
                        .ThenInclude(sp => sp.SubproductCharacteristics)
                        .ThenInclude(sct => sct.SubproductCharacteristicsTranslation.Where(tr => tr.LangCode == lang))
                        .FirstOrDefaultAsync(p => p.Id == item);

                    if (product != null)
                    {
                        var subProductDetails = product.SubProduct?.Select(su => new
                        {
                            Id = su.Id,
                            Images = su.SubProductImage?.Select(a => a.ImageFullPath).ToList() ?? new List<string>(),
                            Quantity = su.Quantity,
                            Price = su.Price,
                            Characteristics = su.SubproductCharacteristics?
                   .SelectMany(sct => sct.SubproductCharacteristicsTranslation
                       .Where(tr => tr.LangCode == lang)
                       .Select(tr => tr.Description ?? "No description"))
                   .ToList() ?? new List<string>(),
                            Available = su.Quantity > 0,
                            IsInBasket = ProductHanlder.IsSubProductInBasket(su.Id, userId)
                        }).Cast<object>().ToList() ?? new List<object>();

                        res.Add(new
                        {
                            Id = product.Id,
                            Name = product.ProductTranslation?.FirstOrDefault(pt => pt.LangCode == lang)?.Name ?? "No translation available",
                            Description = product.ProductTranslation?.FirstOrDefault(pt => pt.LangCode == lang)?.Description ?? string.Empty,
                            SKU = product.ProductTranslation?.FirstOrDefault(pt => pt.LangCode == lang)?.SKU ?? string.Empty,
                            KeyWords = product.ProductTranslation?.FirstOrDefault(pt => pt.LangCode == lang)?.KeyWords ?? string.Empty,
                            CategoryId = product.ProductCategoriesId,
                            ExpireDate = product.ExpireDate,
                            Details = subProductDetails,
                            Review = product.GetReview(),
                            IsInFavourite = true,
                            IsVisible = product.IsVisible,
                            CreateDate = product.CreateDate
                        });
                    }
                }

                return Ok(new
                {
                    status = StatusCodes.Status200OK,
                    data = res,
                    message = "Favourite List"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting favourite products");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }



        [HttpPost("clear-favourite"), Authorize]
        public async Task<IActionResult> ClearFavourite()
        {
            string lang = _userService.GetMyLanguage();

            Guid? userId = _userService.GetMyId();

            if (userId == null)
            {
                return new ObjectResult(new
                {
                    status = StatusCodes.Status200OK,
                    data = "",
                    message = "You must register first"
                });
            }

            List<Favourite> fav = cmsContext.Favourite.Where(a => a.PetOwnerId == userId).ToList();
            cmsContext.Favourite.RemoveRange(fav);
            cmsContext.SaveChanges();

            return new ObjectResult(new
            {
                status = StatusCodes.Status200OK,
                data = "",
                message = "Favourite List Cleared"
            });
        }


        [HttpPost("add-or-remove-to-basket"), Authorize]
        public async Task<IActionResult> ChangeBasket(Guid id)
        {

            Guid? userId = _userService.GetMyId();

            if (userId == null)
            {
                return new ObjectResult(new
                {
                    status = StatusCodes.Status200OK,
                    data = "",
                    message = "You must register first"
                });
            }

            if (cmsContext.Basket.Any(a => a.SubProductId == id && a.PetOwnerId == userId))
            {
                Basket favourite = cmsContext.Basket.FirstOrDefault(a => a.SubProductId == id && a.PetOwnerId == (Guid)userId);
                cmsContext.Basket.Remove(favourite);
                cmsContext.SaveChanges();

                return new ObjectResult(new
                {
                    status = StatusCodes.Status200OK,
                    data = "",
                    message = "Removed Successfully"
                });

            }
            else
            {
                cmsContext.Basket.Add(new Basket { SubProductId = id, PetOwnerId = (Guid)userId });
                cmsContext.SaveChanges();

                return new ObjectResult(new
                {
                    status = StatusCodes.Status200OK,
                    data = "",
                    message = "Added Successfully"
                });
            }

        }
        [HttpPost("change-item-quantity-basket"), Authorize]
        public async Task<IActionResult> ChangeQuantityinBasket(Guid id, bool IncreaseOrDecrease)
        {
            Guid? userId = _userService.GetMyId();

            if (userId == null)
            {
                return new ObjectResult(new
                {
                    status = StatusCodes.Status200OK,
                    data = "",
                    message = "You must register first"
                });
            }

            Basket basket = cmsContext.Basket.FirstOrDefault(a => a.SubProductId == id && a.PetOwnerId == userId);

            if (IncreaseOrDecrease)
            {
                basket.Quantity = basket.Quantity + 1;
                cmsContext.Basket.Attach(basket);
                cmsContext.SaveChanges();

                return new ObjectResult(new
                {
                    status = StatusCodes.Status200OK,
                    data = "",
                    message = "Increased"
                });

            }
            else
            {
                basket.Quantity = Math.Min(basket.Quantity - 1, 0);
                cmsContext.Basket.Attach(basket);
                cmsContext.SaveChanges();

                return new ObjectResult(new
                {
                    status = StatusCodes.Status200OK,
                    data = "",
                    message = "Decreased"
                });
            }

        }


        [HttpGet("get-basket"), Authorize]
        public async Task<IActionResult> GetBasket()
        {
            string lang = _userService.GetMyLanguage();
            Guid? userId = _userService.GetMyId();

            if (userId == null)
            {
                return new ObjectResult(new
                {
                    status = StatusCodes.Status400BadRequest,
                    data = "",
                    message = "You must register first"
                });
            }

            try
            {
                List<Guid> basketSubProductIds = await cmsContext.Basket
                    .Where(a => a.PetOwnerId == userId)
                    .Select(a => a.SubProductId)
                    .ToListAsync();

                List<object> res = new List<object>();

                foreach (var subProductId in basketSubProductIds)
                {
                    var basketItem = await cmsContext.Basket
                        .FirstOrDefaultAsync(a => a.SubProductId == subProductId && a.PetOwnerId == userId);

                    if (basketItem != null)
                    {
                        var productDetails = await Task.Run(() => ProductHanlder.GetSubProductById(subProductId, userId, lang));

                        res.Add(new
                        {
                            BasketId = basketItem.Id,
                            Quantity = basketItem.Quantity,
                            Product = productDetails
                        });
                    }
                }

                return new ObjectResult(new
                {
                    status = StatusCodes.Status200OK,
                    data = res,
                    message = "Basket List"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting basket items");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpPost("clear-basket"), Authorize]
        public async Task<IActionResult> ClearBasket()
        {
            string lang = _userService.GetMyLanguage();

            Guid? userId = _userService.GetMyId();

            if (userId == null)
            {
                return new ObjectResult(new
                {
                    status = StatusCodes.Status200OK,
                    data = "",
                    message = "You must register first"
                });
            }

            List<Basket> fav = cmsContext.Basket.Where(a => a.PetOwnerId == userId).ToList();
            cmsContext.Basket.RemoveRange(fav);
            cmsContext.SaveChanges();

            return new ObjectResult(new
            {
                status = StatusCodes.Status200OK,
                data = "",
                message = "Basket List Cleared"
            });
        }


        [HttpPost("send-product-review"), Authorize]
        public async Task<IActionResult> ReviewProduct(Guid id, int value, string comment)
        {
            string lang = _userService.GetMyLanguage();

            Guid? userId = _userService.GetMyId();

            if (userId == null)
            {
                return new ObjectResult(new
                {
                    status = StatusCodes.Status200OK,
                    data = "",
                    message = "You must register first"
                });
            }

            if (cmsContext.ProductReview.Any(a => a.ProductId == id && a.PetOwnerId == (Guid)userId))
            {
                ProductReview productReview = cmsContext.ProductReview.FirstOrDefault(a => a.ProductId == id && a.PetOwnerId == (Guid)userId);
                cmsContext.ProductReview.Remove(productReview);
                cmsContext.SaveChanges();
            }

            ProductReview model = new ProductReview
            {
                ProductId = id,
                Comment = comment,
                Value = value,

            };
            cmsContext.ProductReview.Add(model);
            cmsContext.SaveChanges();

            return new ObjectResult(new
            {
                status = StatusCodes.Status200OK,
                data = "",
                message = "Added Successfully"
            });
        }

        [HttpGet("get-my-orders"), Authorize]
        public async Task<IActionResult> GetMyOrders(DateTime? StartDate,
        DateTime? EndDate,
        int? OrderStaus,
        int PageSize = 50, int PageIndex = 1)
        {


            if (PageSize >= 50)
            {
                PageSize = 50;
            }

            Guid? userId = (Guid)_userService.GetMyId();

            if (userId == null)
            {
                return new ObjectResult(new
                {
                    status = StatusCodes.Status400BadRequest,
                    data = "",
                    message = "You Must Login First"
                });
            }

            Expression<Func<COrder, bool>> initialExpression = a => a.PetOwnerId == userId;
            Expression<Func<COrder, bool>> startDateExpression = a => StartDate != DateTime.MinValue && a.CreatedDate >= StartDate;
            Expression<Func<COrder, bool>> endDateExpression = a => EndDate != DateTime.MinValue && a.CreatedDate <= EndDate;
            Expression<Func<COrder, bool>> orderStatusExpression = a => OrderStaus != null && a.Status == OrderStaus;

            // Combine all conditions
            var combinedExpression = initialExpression;

            if (StartDate != DateTime.MinValue && StartDate != null)
            {
                combinedExpression = combinedExpression.AndAlso(startDateExpression);
            }

            if (EndDate != DateTime.MinValue && StartDate != null)
            {
                combinedExpression = combinedExpression.AndAlso(endDateExpression);
            }

            if (OrderStaus != null)
            {
                combinedExpression = combinedExpression.AndAlso(orderStatusExpression);
            }

            List<COrder> cOrders = cmsContext.COrder.Where(combinedExpression)
                .Skip((PageIndex - 1) * PageSize).Take(PageSize)
                .ToList();

            return new ObjectResult(new
            {
                status = StatusCodes.Status200OK,
                data = cOrders.ToList(),
                message = "Orders List"
            });
        }








        [HttpGet("get-stripe-public-key"), Authorize]
        public async Task<IActionResult> GetStripePublicKey()
        {
            return new ObjectResult(new
            {
                status = StatusCodes.Status400BadRequest,
                data = cmsContext.MySystemConfiguration.FirstOrDefault().StripePublicKey,
                message = "Stripe Public Key"
            });
        }


        [HttpGet("get-order-by-id"), Authorize]
        public async Task<IActionResult> GetMyOrders(Guid id)
        {
            Guid? userId = (Guid)_userService.GetMyId();

            if (userId == null)
            {
                return new ObjectResult(new
                {
                    status = StatusCodes.Status400BadRequest,
                    data = "",
                    message = "You Must Login First"
                });
            }

            COrder cOrders = cmsContext.COrder.Include(a => a.COrderItems).FirstOrDefault(a => a.Id == id);

            if (cOrders == null)
            {
                return new ObjectResult(new
                {
                    status = StatusCodes.Status400BadRequest,
                    data = "",
                    message = "This order does not exist"
                });
            }

            return new ObjectResult(new
            {
                status = StatusCodes.Status200OK,
                data = cOrders,
                message = "Order details"
            });
        }


        [HttpPost("delete-order"), Authorize]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {

            Guid? userId = (Guid)_userService.GetMyId();

            if (userId == null)
            {
                return new ObjectResult(new
                {
                    status = StatusCodes.Status400BadRequest,
                    data = "",
                    message = "You Must Login First"
                });
            }

            COrder cOrders = cmsContext.COrder.Include(a => a.COrderItems).FirstOrDefault(a => a.Id == id);


            if (cOrders == null)
            {
                return new ObjectResult(new
                {
                    status = StatusCodes.Status400BadRequest,
                    data = "",
                    message = "This order does not exist"
                });
            }


            if (cOrders.Status > 0)
            {
                return new ObjectResult(new
                {
                    status = StatusCodes.Status200OK,
                    data = "",
                    message = "This order in progress cannot be deleted"
                });
            }

            cmsContext.COrder.Remove(cOrders);
            cmsContext.SaveChanges();

            await _hubContext.Clients.All.SendAsync("RefreshNewOrderCount_", _userService.GetMyCenterIdWeb().ToString());

            return new ObjectResult(new
            {
                status = StatusCodes.Status200OK,
                data = "",
                message = "Removed Successfully"
            });
        }




        [HttpPost("go-to-checkout"), Authorize]
        public async Task<IActionResult> GoToCheckOut(string RecipientAddress, string RecipientTelephone, string? CustomerNotes = "")
        {
            if (string.IsNullOrWhiteSpace(RecipientAddress))
            {
                return new ObjectResult(new
                {
                    status = StatusCodes.Status400BadRequest,
                    data = "",
                    message = "Please fill in your address"
                });
            }

            Guid? userId = _userService.GetMyId();
            if (userId == null)
            {
                return new ObjectResult(new
                {
                    status = StatusCodes.Status400BadRequest,
                    data = "",
                    message = "You must log in first"
                });
            }

            var baskets = await cmsContext.Basket
                .Where(a => a.PetOwnerId == userId.Value)
                .ToListAsync();

            if (baskets == null || !baskets.Any())
            {
                return new ObjectResult(new
                {
                    status = StatusCodes.Status400BadRequest,
                    data = "",
                    message = "Your basket is empty"
                });
            }

            List<COrderItems> cOrderItems = new List<COrderItems>();

            foreach (var item in baskets)
            {
                double? availableQnt =  ProductHanlder.GetAvailableQuantityOfSubProduct(item.SubProductId, item.Quantity);

                if (availableQnt < 0)
                {
                    return new ObjectResult(new
                    {
                        status = StatusCodes.Status400BadRequest,
                        data = $"Available quantity is {item.Quantity + availableQnt}",
                        message = $"Product is not available, product id {item.SubProductId}"
                    });
                }

                var subProduct = await cmsContext.SubProduct.FindAsync(item.SubProductId);

                if (subProduct == null)
                {
                    return new ObjectResult(new
                    {
                        status = StatusCodes.Status400BadRequest,
                        data = "",
                        message = $"Product not found, product id {item.SubProductId}"
                    });
                }

                COrderItems temp = new COrderItems
                {
                    SubProductId = item.SubProductId,
                    ItemPrice = subProduct.Price ?? 0, // Assuming Price is nullable
                    ItemQuantity = item.Quantity,
                };
                cOrderItems.Add(temp);
            }

            COrder cOrder = new COrder
            {
                CreatedDate = DateTime.Now, // Assuming this is UTC and you handle timezone elsewhere
                CustomerNotes = CustomerNotes,
                PetOwnerId = userId.Value,
                RecipientAddress = RecipientAddress,
                RecipientTelephone = string.IsNullOrWhiteSpace(RecipientTelephone) ? _userService.GetMyPhone() : RecipientTelephone,
                Status = 0,
                OrderNumber = await cmsContext.COrder.CountAsync() + 1,
                COrderItems = cOrderItems
            };

            cmsContext.COrder.Add(cOrder);
            await cmsContext.SaveChangesAsync();

            return new ObjectResult(new
            {
                status = StatusCodes.Status200OK,
                data = new { OrderId = cOrder.Id, Cost = cOrder.TotalCost },
                message = "Confirm payment"
            });
        }





        [HttpPost("process-payment"), Authorize]
        public async Task<IActionResult> ProcessPayment(Guid id, string? cardToken, string paymentType = "card")
        {
            Guid? UserId = _userService.GetMyId();

            if (UserId == null)
            {
                return new ObjectResult(new
                {
                    status = StatusCodes.Status400BadRequest,
                    data = "",
                    message = "You Must Login First"
                });
            }

            COrder order = cmsContext.COrder.Include(a => a.COrderItems).FirstOrDefault(a => a.Id == id);

            if (order == null)
            {
                return new ObjectResult(new
                {
                    status = StatusCodes.Status400BadRequest,
                    data = "",
                    message = "This order does not exist"
                });
            }

            if (order.Status > 0)
            {
                return new ObjectResult(new
                {
                    status = StatusCodes.Status400BadRequest,
                    data = "",
                    message = "This order is already processed"
                });
            }

            foreach (var item in order.COrderItems)
            {
                double? availableQnt = ProductHanlder.GetAvailableQuantityOfSubProduct(item.SubProductId, item.ItemQuantity);

                if (availableQnt < 0)
                {
                    return new ObjectResult(new
                    {
                        status = StatusCodes.Status400BadRequest,
                        data = $"Available Quantity is {item.ItemQuantity + availableQnt}",
                        message = $"Product is not available, product id {item.SubProductId}"
                    });
                }
            }

            foreach (var item in order.COrderItems)
            {
                ProductHanlder.DecreaseQuantity(item.SubProductId, item.ItemQuantity);
            }

            if (paymentType == "cash")
            {
                // Handle cash payment
                using (var transaction = cmsContext.Database.BeginTransaction())
                {
                    try
                    {
                        order.Status = 2; // Mark as In Progress for cash payments
                        cmsContext.COrder.Attach(order);
                        cmsContext.Entry(order).Property(a => a.Status).IsModified = true;
                        cmsContext.SaveChanges();

                        transaction.Commit();

                        await _hubContext.Clients.All.SendAsync("RefreshNewOrderCount_", _userService.GetMyCenterIdWeb().ToString());

                        List<Basket> fav = cmsContext.Basket.Where(a => a.PetOwnerId == UserId).ToList();
                        cmsContext.Basket.RemoveRange(fav);
                        cmsContext.SaveChanges();

                        return new ObjectResult(new
                        {
                            status = StatusCodes.Status200OK,
                            data = "",
                            message = "Order processed with cash payment"
                        });
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new ObjectResult(new
                        {
                            status = StatusCodes.Status400BadRequest,
                            data = "",
                            message = ex.ToString()
                        });
                    }
                }
            }
            else if (paymentType == "card")
            {
                if (string.IsNullOrEmpty(cardToken))
                {
                    return new ObjectResult(new
                    {
                        status = StatusCodes.Status400BadRequest,
                        data = "",
                        message = "Card token is required for card payments"
                    });
                }

                // Handle card payment
                StripeConfiguration.ApiKey = cmsContext.MySystemConfiguration.FirstOrDefault()?.StripePrivateKey;

                PetOwner customerDto = cmsContext.PetOwner.Include(a => a.User).FirstOrDefault(a => a.Id == UserId);

                var paymentMethodCreateOptions = new PaymentMethodCreateOptions
                {
                    Type = paymentType,
                    Card = new PaymentMethodCardOptions
                    {
                        Token = cardToken,
                    },
                };

                string pMethodId;

                try
                {
                    var PMService = new PaymentMethodService();
                    var pmMethod = PMService.Create(paymentMethodCreateOptions);
                    pMethodId = pmMethod.Id;
                }
                catch (Exception ex)
                {
                    return new ObjectResult(new
                    {
                        status = StatusCodes.Status400BadRequest,
                        data = "",
                        message = ex.ToString()
                    });
                }

                try
                {
                    var paymentIntentOptions = new PaymentIntentCreateOptions
                    {
                        Amount = (long)order.TotalCost * 100,
                        Currency = "AED",
                        Confirm = true,
                        PaymentMethod = pMethodId,
                        AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                        {
                            Enabled = true,
                            AllowRedirects = "never"
                        },
                        Metadata = new Dictionary<string, string>
                {
                    { "FullName", customerDto.FirstName + " " + customerDto.LastName },
                    { "UserName", customerDto.User.UserName },
                    { "Email", customerDto.User.Email },
                    { "Type", "0001# New Order" },
                    { "CustomOperationCode", "0001" },
                    { "Date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
                }
                    };

                    var service = new PaymentIntentService();
                    service.Create(paymentIntentOptions);
                }
                catch (Exception ex)
                {
                    return new ObjectResult(new
                    {
                        status = StatusCodes.Status400BadRequest,
                        data = "",
                        message = ex.ToString()
                    });
                }

                using (var transaction = cmsContext.Database.BeginTransaction())
                {
                    try
                    {
                        order.Status = 1; // Mark as Paid for card payments
                        cmsContext.COrder.Attach(order);
                        cmsContext.Entry(order).Property(a => a.Status).IsModified = true;
                        cmsContext.SaveChanges();

                        transaction.Commit();

                        await _hubContext.Clients.All.SendAsync("RefreshNewOrderCount_", _userService.GetMyCenterIdWeb().ToString());

                        List<Basket> fav = cmsContext.Basket.Where(a => a.PetOwnerId == UserId).ToList();
                        cmsContext.Basket.RemoveRange(fav);
                        cmsContext.SaveChanges();

                        return new ObjectResult(new
                        {
                            status = StatusCodes.Status200OK,
                            data = "",
                            message = "Order Redirected to the Store Staff"
                        });
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new ObjectResult(new
                        {
                            status = StatusCodes.Status400BadRequest,
                            data = "",
                            message = ex.ToString()
                        });
                    }
                }
            }
            else
            {
                return new ObjectResult(new
                {
                    status = StatusCodes.Status400BadRequest,
                    data = "",
                    message = "Invalid payment type"
                });
            }
        }



    }


}


    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            var parameter = Expression.Parameter(typeof(T));

            var body = Expression.AndAlso(
                Expression.Invoke(first, parameter),
                Expression.Invoke(second, parameter)
            );

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }




//var options1 = new TokenCreateOptions
//{
//    Card = new TokenCardOptions
//    {
//        Number = "4242424242424242",
//        ExpMonth = "5",
//        ExpYear = "2024",
//        Cvc = "314",
//    },
//};
//var service1 = new TokenService();
//service1.Create(options1);


//var options_ = new PaymentMethodCreateOptions
//{
//    Type = "card",
//    Card = new PaymentMethodCardOptions
//    {
//        Number = "4242424242424242",
//        ExpMonth = 8,
//        ExpYear = 2026,
//        Cvc = "314",
//    },
//};
//var service_ = new PaymentMethodService();
//var xx=service_.Create(options_);

//var options__ = new PaymentIntentCreateOptions
//{
//    Amount = 2000,
//    Currency = "AED",
//    Confirm=true,
//    PaymentMethod=xx.Id,

//};
//var service__ = new PaymentIntentService();
//service__.Create(options__);
