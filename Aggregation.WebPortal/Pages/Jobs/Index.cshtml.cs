using Aggregation_DataModels.Models;
using Aggregation_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;
using System.Globalization;

namespace Website.Pages.Jobs
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

        #region Properties
        [BindProperty]
        public DateTime EndDate { get; set; } = default!;

        [BindProperty]
        public List<SelectListItem> Intervals { get; set; }

        [BindProperty]
        public List<SelectListItem> JobTypes { get; set; }

        [BindProperty]
        public int SelectedInterval { get; set; }

        [BindProperty]
        public int SelectedJobType { get; set; }

        [BindProperty]
        public int SelectedStatus { get; set; }

        [BindProperty]
        public DateTime StartDate { get; set; } = default!;

        [BindProperty]
        public List<SelectListItem> Statuses { get; set; }

        public IEnumerable<VwGetJob> Jobs { get; set; } = default!;
        #endregion

        public bool HasAccessToRefreshJob()
        {
            return userRepository.HasAccessToRefreshJob();
        }

        public bool HasAccessToUpdateClock()
        {
            return userRepository.HasAccessToUpdateClock();
        }

        public DateTime MaxDate()
        {
            return System.DateTime.Now.Date;
        }

        public DateTime MinDate()
        {
            return System.DateTime.Now.AddDays(-15).Date;
        }

        private void Init()
        {
            string sdate = (HttpContext.Session.GetString("StartDate") == default) ? DateTime.Now.ToString("MM/dd/yyyy") : HttpContext.Session.GetString("StartDate");
            string edate = (HttpContext.Session.GetString("EndDate") == default) ? DateTime.Now.ToString("MM/dd/yyyy") : HttpContext.Session.GetString("EndDate");

            HttpContext.Session.SetString("StartDate", sdate);
            HttpContext.Session.SetString("EndDate", edate);

            TbConfiguration? configuration = aggregationRepository.GetConfigurationByKey("ActionViewOpenPeriod");
            DateTime openPeriod = (configuration != null) ? DateTime.ParseExact(configuration.Value, "yyyy/MM/dd", CultureInfo.InvariantCulture) : System.DateTime.MinValue;

            StartDate = DateTime.ParseExact(sdate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            EndDate = DateTime.ParseExact(edate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            if (StartDate == System.DateTime.MinValue)
            {
                ModelState.AddModelError("StartDate", "Invalid date");
            }
            else
            {
                if (openPeriod == System.DateTime.MinValue)
                {
                    ModelState.AddModelError("StartDate", "Configuration contains an invalid date");
                }
                else if (StartDate < openPeriod || StartDate > System.DateTime.Now)
                {
                    ModelState.AddModelError("StartDate", $"Input date is invalid. Valide range is between {openPeriod.ToShortDateString()} to {System.DateTime.Now.ToShortDateString()}.");
                }
                else
                {
                    Jobs = aggregationRepository.GetJobs(StartDate);
                }
            }
        }

        public void OnGet()
        {
            Init();
        }

        public void OnPost()
        {
            HttpContext.Session.SetString("StartDate", StartDate.ToString("MM/dd/yyyy"));
            HttpContext.Session.SetString("EndDate", StartDate.ToString("MM/dd/yyyy"));

            Init();
        }

        public async Task<IActionResult> OnGetUpdateClock()
        {
            if (!HasAccessToUpdateClock())
            {
                toastNotification.AddErrorToastMessage("Your request could not be processed becase of lack of permission!");

                return Redirect("../Utility/Unauthorized");
            }

            var jobId = Request.Query["id"];

            var result = await aggregationRepository.UpdateClock(int.Parse(jobId), userRepository.GetId());

            toastNotification.AddSuccessToastMessage("your request has been processed successfully!");

            return Redirect("../Index");
        }

        public async Task<IActionResult> OnGetUpdateRefresh()
        {
            if (!HasAccessToUpdateClock())
            {
                toastNotification.AddErrorToastMessage("Your request could not be processed becase of lack of permission!");

               return Redirect("../Utility/Unauthorized");
            }

            var jobId = Request.Query["id"];
            var priority = Request.Query["priority"];

            var result = await aggregationRepository.RefreshJob(int.Parse(jobId), int.Parse(priority), userRepository.GetId(), userRepository.GetUserId());

            toastNotification.AddSuccessToastMessage("your request has been processed successfully!");

            return Redirect("../Jobs/Index");
        }
    }
}