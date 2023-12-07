using Aggregation_DataModels.Models;
using Aggregation_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace Website.Pages.Configuration
{
    public class DetailsModel : PageModel
    {
        private readonly IAggregationRepository aggregationRepository;
        private readonly IToastNotification toastNotification;

        public DetailsModel(IAggregationRepository aggregationRepository, IToastNotification toastNotification)
        {
            this.aggregationRepository = aggregationRepository;
            this.toastNotification = toastNotification;
        }

        public List<TbActivityLogger> TbActivityLoggers { get; set; }

        public TbConfiguration TbConfiguration { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbconfiguration = await aggregationRepository.GetConfigurationByIdAsync(id);

            if (tbconfiguration == null)
            {
                return NotFound();
            }

            TbActivityLoggers = await aggregationRepository.GetActivityLoggerByConfigIdAsync(id);

            TbConfiguration = tbconfiguration;

            return Page();
        }
    }
}
