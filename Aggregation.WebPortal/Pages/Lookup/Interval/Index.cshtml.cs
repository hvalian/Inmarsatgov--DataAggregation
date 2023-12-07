using Aggregation_DataModels.Models;
using Aggregation_Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Lookup.Interval
{
    public class IndexModel : PageModel
    {
        private readonly IAggregationRepository aggregationRepository;

        public IndexModel(IAggregationRepository aggregationRepository)
        {
            this.aggregationRepository = aggregationRepository;
        }

        public IEnumerable<TbInterval> Intervals { get; set; } = default!;

        public void OnGet()
        {
            Intervals = aggregationRepository.GetIntervals(false).OrderBy(i => i.Id).Take(1000).ToList();
        }
    }
}
