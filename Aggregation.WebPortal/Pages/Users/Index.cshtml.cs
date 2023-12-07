using Aggregation_DataModels.Models;
using Aggregation_Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace Website.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly IAggregationRepository aggregationRepository;
        private readonly IToastNotification toastNotification;
        private readonly IUserService userRepository;

        public IndexModel(IAggregationRepository aggregationRepository, IToastNotification toastNotification, IUserService userRepository)
        {
            this.aggregationRepository = aggregationRepository;
            this.toastNotification = toastNotification;
            this.userRepository = userRepository;
        }

        public IList<TbUser> TbUser { get; set; } = default!;

        public async Task OnGetAsync()
        {
            TbUser = await aggregationRepository.GetUsersAsync();
        }
    }
}
