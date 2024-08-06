using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CmsDataAccess;

namespace CmsDataAccess.MobileViewModels.AuthModels
{

    //public class ClaimRequirementAttribute : TypeFilterAttribute
    //{
    //    public ClaimRequirementAttribute(string claimType, string claimValue) : base(typeof(ClaimRequirementFilter))
    //    {
    //        Arguments = new object[] { new Claim(claimType, claimValue) };
    //    }
    //}
    //public class ClaimRequirementFilter : IAuthorizationFilter
    //{
    //    readonly Claim _claim;

    //    public ClaimRequirementFilter(Claim claim)
    //    {
    //        _claim = claim;
    //    }

    //    public void OnAuthorization(AuthorizationFilterContext context)
    //    {
    //        var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type && c.Value == _claim.Value);
    //        if (!hasClaim)
    //        {
    //            context.Result = new ForbidResult();
    //        }
    //    }
    //}
    //public class CheckIfActiveAttribute : TypeFilterAttribute
    //{
    //    public CheckIfActiveAttribute() : base(typeof(CheckIfActiveFilter))
    //    {
            
    //    }
    //}
    //public class CheckIfActiveFilter : IAuthorizationFilter
    //{

    //    public CheckIfActiveFilter()
    //    {
    //    }

    //    public void OnAuthorization(AuthorizationFilterContext context)
    //    {
    //        Guid UserId=Guid.Parse(context.HttpContext.User.FindFirstValue(ClaimTypes.SerialNumber));

    //        CMDbContext db = new CMDbContext();

    //        if (!db.UserCredentials.Find(UserId).Active)
    //        {
    //            context.Result = new ObjectResult(new { status = StatusCodes.Status403Forbidden, data = "", message = "Your account is not active, go to subscription page!" });


    //            //context.Result = new ForbidResult();
    //        }
    //    }
    //}


}
