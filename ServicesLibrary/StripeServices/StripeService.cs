using Azure.Core;
using CmsDataAccess.DbModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServicesLibrary.MedicalCenterServices;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLibrary.StripeServices
{
    public interface IStripeService
    {
        public List<SubscriptionPlan> GetAllSubscriptionPlan(string? langCode = "en-us");
        public SubscriptionPlan GetSubscriptionPlanById(Guid Id, string? langCode = "en-us");

        public string AddNewSubscriptionPlan(
    List<SubscriptionPlanTranslation> SubscriptionPlanTranslation,
    string PriceRecuencyInterval,
    long PriceAmount,
    long SetUpCost=0,
    int? FreeDays = 0
    );
        public string UpdateSubscriptionPlan(
    Guid Id,
    List<SubscriptionPlanTranslation> SubscriptionPlanTranslation,
    string PriceRecuencyInterval,
    long PriceAmount,
    long SetUpCost,
    int? FreeDays = 0
);

        public string DeleteSubscriptionPlan(Guid Id);
    }

    public class StripeService : IStripeService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMedicalCenterService medicalCenterService;

        private readonly ApplicationDbContext cmsContext;
        private readonly UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public StripeService(
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
        }


        public List<SubscriptionPlan> GetAllSubscriptionPlan(string? langCode = "en-us")
        {
            if (langCode.IsNullOrEmpty())
            {
                return cmsContext.SubscriptionPlan
                .Include(a => a.SubscriptionPlanTranslation)
                .Where(a => !a.IsDeleted).ToList();
            }
            return cmsContext.SubscriptionPlan
                .Include(a => a.SubscriptionPlanTranslation.Where(a=>a.LangCode==langCode))
                .Where(a => !a.IsDeleted).ToList();
        }

        public SubscriptionPlan GetSubscriptionPlanById(Guid Id,string? langCode = "en-us")
        {
            if (langCode.IsNullOrEmpty())
            {
                return cmsContext.SubscriptionPlan
                .Include(a => a.SubscriptionPlanTranslation)
                .FirstOrDefault(a => a.Id == Id);
            }
            return cmsContext.SubscriptionPlan
                .Include(a => a.SubscriptionPlanTranslation.Where(a => a.LangCode == langCode))
                .FirstOrDefault(a => a.Id == Id);
        }

        public string AddNewSubscriptionPlan(
            List<SubscriptionPlanTranslation> SubscriptionPlanTranslation,
            string PriceRecuencyInterval,
            long PriceAmount,
            long SetUpCost,
            int? FreeDays=0
            )
        {
            using (var transaction=cmsContext.Database.BeginTransaction())
            {
                try
                {

                    SubscriptionPlan model = new SubscriptionPlan();

                    model.PriceCurrency = "AED";
                    model.PriceRecuencyInterval = PriceRecuencyInterval;
                    model.FreeDays =(int) FreeDays;
                    model.PriceAmount = PriceAmount;


                    model.SubscriptionPlanTranslation = new List<SubscriptionPlanTranslation>();

                    foreach (var item in SubscriptionPlanTranslation)
                    {
                        model.SubscriptionPlanTranslation.Add(
                            new SubscriptionPlanTranslation
                            {
                                Description = item.Description,
                                Name= item.Name,
                                LangCode = item.LangCode,  
                            }
                            );
                    }

                    Stripe.Product result;

                    StripeConfiguration.ApiKey = cmsContext.MySystemConfiguration.FirstOrDefault().StripePrivateKey;

                    string Name = SubscriptionPlanTranslation.Where(a => a.LangCode == "en-us").FirstOrDefault().Name;
                    string Description = SubscriptionPlanTranslation.Where(a => a.LangCode == "en-us").FirstOrDefault().Description;

                    var options = new ProductCreateOptions
                    {
                        Name = Name,
                        DefaultPriceData = new ProductDefaultPriceDataOptions
                        {
                            UnitAmount = PriceAmount * 100,
                            Currency = model.PriceCurrency,
                            Recurring = new ProductDefaultPriceDataRecurringOptions
                            {
                                Interval = model.PriceRecuencyInterval, //"month",//  "year"
                            },
                        },
                        Description = Description,
                        Expand = new List<string> { "default_price" },
                    };

                    var service = new ProductService();
                    var product = service.Create(options);
                    model.PriceId = product.DefaultPriceId;
                    model.StripeId = product.Id;
                    model.SetUpCost = SetUpCost;

                    cmsContext.SubscriptionPlan.Add(model);
                    cmsContext.SaveChanges();

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


        public string UpdateSubscriptionPlan(
            Guid Id,
            List<SubscriptionPlanTranslation> SubscriptionPlanTranslation,
            string PriceRecuencyInterval,
            long PriceAmount,
            long SetUpCost,
            int? FreeDays = 0
    )
        {
            using (var transaction = cmsContext.Database.BeginTransaction())
            {
                try
                {

                    SubscriptionPlan model = cmsContext.SubscriptionPlan.Include(a=>a.SubscriptionPlanTranslation).FirstOrDefault(a=>a.Id==Id);

                    string NewName = SubscriptionPlanTranslation.Where(a => a.LangCode == "en-us").FirstOrDefault().Name;
                    string NewDescription = SubscriptionPlanTranslation.Where(a => a.LangCode == "en-us").FirstOrDefault().Description;

                    string OldName = model.SubscriptionPlanTranslation.Where(a => a.LangCode == "en-us").FirstOrDefault().Name;
                    string OldDescription = model.SubscriptionPlanTranslation.Where(a => a.LangCode == "en-us").FirstOrDefault().Description;

                    bool update_price = false;
                    bool update_ = false;

                    if (model.PriceAmount != PriceAmount)
                    {
                        update_price = true;
                    }

                    if (NewName != OldName || NewDescription != OldDescription)
                    {
                        update_ = true;
                    }

                    model.FreeDays =(int) FreeDays;
                    model.SetUpCost = SetUpCost;
                    model.PriceAmount = PriceAmount;
                    model.PriceCurrency = "AED";
                    model.PriceRecuencyInterval = (PriceRecuencyInterval == "0") ? "month" : "year";

                    model.SubscriptionPlanTranslation = SubscriptionPlanTranslation;
                    

                    cmsContext.SubscriptionPlan.Attach(model);
                    cmsContext.Entry(model).State=EntityState.Modified;
                    cmsContext.SaveChanges();

                    Stripe.Product result;

                    StripeConfiguration.ApiKey = cmsContext.MySystemConfiguration.FirstOrDefault().StripePrivateKey;


                    if (update_price)
                    {
                        var DefaultPriceData = new PriceCreateOptions
                        {
                            UnitAmount = model.PriceAmount * 100,// 100 $
                            Currency = model.PriceCurrency,// "usd",
                            Recurring = new PriceRecurringOptions
                            {
                                Interval = model.PriceRecuencyInterval, //"month",//  "year"
                            },
                            Product = model.StripeId
                        };

                        var newPriceService = new PriceService();
                        var newPrice = newPriceService.Create(DefaultPriceData);

                        var options = new ProductUpdateOptions
                        {
                            Name = NewName,// "SubsicribeProductPrice",
                            Description =NewDescription,
                            DefaultPrice = newPrice.Id,
                        };

                        var service = new ProductService();

                        var u = service.Update(model.StripeId, options);

                        var deletePrice = new PriceUpdateOptions
                        {
                            Active = false,
                        };

                        var serviceDelete = new PriceService();

                        var d = serviceDelete.Update(model.PriceId, deletePrice);

                        model.PriceId = newPrice.Id;
                        cmsContext.SubscriptionPlan.Attach(model);
                        cmsContext.Entry(model).Property(x => x.PriceId).IsModified = true;
                        cmsContext.SaveChanges();

                    }

                    if (update_)
                    {
                        var options = new ProductUpdateOptions
                        {
                            Name = NewName,// "SubsicribeProductPrice",

                            Description = NewDescription,

                        };
                        var service = new ProductService();

                        var s = service.Update(model.StripeId, options);
                    }


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


        public string DeleteSubscriptionPlan(Guid Id)
        {
            using (var transaction = cmsContext.Database.BeginTransaction())
            {
                try
                {
                    SubscriptionPlan model=cmsContext.SubscriptionPlan.Include(a=>a.SubscriptionPlanTranslation)
                        .FirstOrDefault(a=>a.Id==Id);

                    if (cmsContext.CenterAdmin.Where(a=>a.StripeSubcriptionId== model.StripeId).Any())
                    {
                        model.IsDeleted = true;
                        cmsContext.SubscriptionPlan.Attach(model);
                        cmsContext.Entry(model).Property(a=>a.IsDeleted).IsModified=true;
                        cmsContext.SaveChanges();
                    }

                    else
                    {
                        StripeConfiguration.ApiKey = cmsContext.MySystemConfiguration.FirstOrDefault().StripePrivateKey;

                        var options = new ProductUpdateOptions
                        {
                            Active = false,
                        };

                        var service = new ProductService();
                        var product = service.Update(model.StripeId, options);

                        cmsContext.SubscriptionPlan.Remove(model);
                        cmsContext.SaveChanges();
                    }

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
