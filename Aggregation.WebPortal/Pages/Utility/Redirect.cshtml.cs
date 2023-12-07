using Aggregation_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using System.Security.Policy;

namespace Aggregation.WebPortal.Pages.Utility
{
    public class RedirectModel : PageModel
    {
        public IActionResult OnGet()
        {
            var redirectURL = Request.Query["redirecturl"];

            return Redirect(redirectURL);
        }
    }
}
