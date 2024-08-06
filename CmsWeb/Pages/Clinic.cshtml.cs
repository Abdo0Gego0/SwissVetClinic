using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CmsWeb.Pages
{
    public class ClinicModel : PageModel
    {
        public void OnGet()
        {
            string? culture = Request.Query["culture"];
              if(culture!=null)
            {
                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions { Expires= DateTimeOffset.UtcNow .AddYears(1)}
                    
                    );

                string returnUrl = Request.Headers["Referer"].ToString()?? "/";
                Response.Redirect( returnUrl );
            }

        }
    }
}
