using Aggregation_DataModels.Models;
using Aggregation_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Jobs
{
    public class DetailsModel : PageModel
    {
        private readonly IAggregationRepository aggregationRepository;

        public DetailsModel(IAggregationRepository aggregationRepository)
        {
            this.aggregationRepository = aggregationRepository;
        }

        public AggregationJobDetails JobDetails { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbjob = await aggregationRepository.GetJobAsync(id);
            if (tbjob == null)
            {
                return NotFound();
            }

            var jobDetails = await aggregationRepository.GetJobDetailsAsync((int)tbjob.Interval, tbjob.AggregationStartDateTime, tbjob.AggregationEndDateTime);

            if (jobDetails == null)
            {
                return NotFound();
            }
            else
            {
                JobDetails = jobDetails;
            }

            return Page();
        }
    }
}
