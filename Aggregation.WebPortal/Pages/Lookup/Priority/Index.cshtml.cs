using Aggregation_DataModels.Models;
using Aggregation_Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Lookup.Priority
{
    public class IndexModel : PageModel
    {
        private readonly IAggregationRepository aggregationRepository;

        public IndexModel(IAggregationRepository aggregationRepository)
        {
            this.aggregationRepository = aggregationRepository;
        }

        public IEnumerable<TbPriority> Priorities { get; set; } = default!;

        public void OnGet()
        {
            Priorities = aggregationRepository.GetPriorities().OrderBy(i => i.Description).Take(1000).ToList();
        }
    }
}
