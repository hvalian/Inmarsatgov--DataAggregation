using Aggregation_DataModels.Models;
using Aggregation_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aggregation.WebPortal.Pages.Jobs.Historical
{
    public class DetailsModel : PageModel
    {
        private readonly IAggregationRepository aggregationRepository;

        public DetailsModel(IAggregationRepository aggregationRepository)
        {
            this.aggregationRepository = aggregationRepository;
        }

        public string PreviousPage { get; set; }

        public string PreviousPageLabel { get; set; }

        public TbJob TbJob { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var query_PreviousPage = Request.Query["returnTo"];

            PreviousPageLabel = "List";
            PreviousPage = "/Jobs/Historical/Index";

            if (!string.IsNullOrWhiteSpace(query_PreviousPage))
            {
                if (query_PreviousPage == "Dashboard")
                {
                    PreviousPageLabel = "Dashboard";
                    PreviousPage = "/Index";
                }
            }

            if (id == null)
            {
                return NotFound();
            }

            var tbjob = await aggregationRepository.GetJobAsync(id);
            if (tbjob == null)
            {
                return NotFound();
            }
            else
            {
                TbJob = tbjob;
            }

            return Page();
        }
    }
}
