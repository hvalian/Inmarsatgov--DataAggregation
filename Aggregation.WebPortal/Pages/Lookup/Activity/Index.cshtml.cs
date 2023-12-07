using Aggregation_DataModels.Models;
using Aggregation_Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Lookup.Activity
{
    public class IndexModel : PageModel
    {
        private readonly IAggregationRepository aggregationRepository;

        public IndexModel(IAggregationRepository aggregationRepository)
        {
            this.aggregationRepository = aggregationRepository;
        }

        public IEnumerable<TbActivity> Activities { get; set; } = default!;

        public void OnGet()
        {
            Activities = aggregationRepository.GetActivities().OrderBy(i => i.Id).Take(1000).ToList();
        }
    }
}
