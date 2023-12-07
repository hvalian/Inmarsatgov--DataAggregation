using Aggregation_DataModels.Models;
using Aggregation_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aggregation.WebPortal.Pages.Jobs.Queue
{
    public class IndexModel : PageModel
    {
        private readonly IAggregationRepository aggregationRepository;
        private readonly IUserService userRepository;

        public IndexModel(IAggregationRepository aggregationRepository,  IUserService userRepository)
        {
            this.aggregationRepository = aggregationRepository;
            this.userRepository = userRepository;
        }

        #region Properties
        [BindProperty]
        public List<int> AreChecked { get; set; }

        [BindProperty]
        public IEnumerable<TbJob> Jobs { get; set; } = default!;
        #endregion

        public bool HasAccessToQueue()
        {
            return userRepository.HasAccessToQueue();
        }

        public bool ShowDeleteButton()
        {
            return (aggregationRepository.GetJobCounts() > 0);
        }

        public void OnGet()
        {
            Jobs = aggregationRepository.GetJobs();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            foreach (int jobId in AreChecked)
            {
                bool result = await aggregationRepository.CancelJob(jobId, userRepository.GetId());
            }

            Jobs = aggregationRepository.GetJobs();

            return Page();
        }
    }
}
