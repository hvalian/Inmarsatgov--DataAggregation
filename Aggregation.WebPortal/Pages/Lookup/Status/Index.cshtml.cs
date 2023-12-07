using Aggregation_DataModels.Models;
using Aggregation_Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Lookup.Status
{
    public class IndexModel : PageModel
    {
        private readonly IAggregationRepository aggregationRepository;

        public IndexModel(IAggregationRepository aggregationRepository)
        {
            this.aggregationRepository = aggregationRepository;
        }

        public IEnumerable<TbStatus> Statuses { get; set; } = default!;

        public void OnGet()
        {
            Statuses = aggregationRepository.GetStatuses(false).OrderBy(i => i.Description).Take(1000).ToList();
        }
    }
}
