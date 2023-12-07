using Aggregation_DataModels.Models;
using Aggregation_Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Lookup.NodeType
{
    public class IndexModel : PageModel
    {
        private readonly IAggregationRepository aggregationRepository;

        public IndexModel(IAggregationRepository aggregationRepository)
        {
            this.aggregationRepository = aggregationRepository;
        }

        public IEnumerable<RtbNodeType> NodeTypes { get; set; } = default!;

        public void OnGet()
        {
            NodeTypes = aggregationRepository.GetNodeTypes().OrderBy(i => i.NodeTypeId).Take(1000).ToList();
        }
    }
}
