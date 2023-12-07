using Aggregation_DataModels.Models;
using Aggregation_Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Lookup
{
    public class IndexModel : PageModel
    {
        private readonly IAggregationRepository aggregationRepository;

        public IndexModel(IAggregationRepository aggregationRepository)
        {
            this.aggregationRepository = aggregationRepository;
        }

        public IEnumerable<TbJobType> JobTypes { get; set; } = default!;

        public void OnGet()
        {
            JobTypes = aggregationRepository.GetJobTypes(false).OrderBy(i => i.Description).Take(1000).ToList();
        }
    }
}
