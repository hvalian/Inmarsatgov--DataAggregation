using Aggregation_DataModels.Models;
using Aggregation_Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Configuration
{
    public class IndexModel : PageModel
    {
        private readonly IAggregationRepository aggregationRepository;

        public IndexModel(IAggregationRepository aggregationRepository)
        {
            this.aggregationRepository = aggregationRepository;
        }

        public IEnumerable<TbConfiguration> TbConfiguration { get; set; } = default!;

        public void OnGet()
        {
            TbConfiguration = aggregationRepository.GetConfigurations().OrderBy(i => i.Description).Take(1000).ToList();
        }
    }
}
