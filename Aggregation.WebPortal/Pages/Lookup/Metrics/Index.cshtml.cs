using Aggregation_DataModels.Models;
using Aggregation_Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Lookup.MetricDefultValue
{
    public class IndexModel : PageModel
    {
        private readonly IAggregationRepository aggregationRepository;

        public IndexModel(IAggregationRepository aggregationRepository)
        {
            this.aggregationRepository = aggregationRepository;
        }

        public IEnumerable<VwRnodeTypeMetric> MetricDefaultValues;

        public void OnGet()
        {
            MetricDefaultValues = aggregationRepository.GetMetrics().OrderBy(i => i.NodeTypeId).ThenBy(i => i.MetricKey).Take(1000).ToList();
        }
    }
}
