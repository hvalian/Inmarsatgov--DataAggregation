using Aggregation_DataModels.Models;
using Aggregation_Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;
using System.Globalization;

namespace Aggregation.WebPortal.Pages.Jobs.Historical
{
    public class IndexModel : PageModel
    {
        private readonly IAggregationRepository aggregationRepository;
        private readonly IToastNotification toastNotification;

        public IndexModel(IAggregationRepository aggregationRepository, IToastNotification toastNotification)
        {
            this.aggregationRepository = aggregationRepository;
            this.toastNotification = toastNotification;
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

        public IEnumerable<TbJob> Jobs { get; set; } = default!;
        #endregion

        private void Init()
        {
            string sdate = (HttpContext.Session.GetString("StartDate") == default) ? DateTime.Now.ToString("MM/dd/yyyy") : HttpContext.Session.GetString("StartDate");
            string edate = (HttpContext.Session.GetString("EndDate") == default) ? DateTime.Now.ToString("MM/dd/yyyy") : HttpContext.Session.GetString("EndDate");

            StartDate = DateTime.ParseExact(sdate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            EndDate = DateTime.ParseExact(edate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            SelectedInterval = (HttpContext.Session.GetInt32("SelectedInterval") == default) ? 0 : (int)HttpContext.Session.GetInt32("SelectedInterval");
            SelectedJobType = (HttpContext.Session.GetInt32("SelectedJobType") == default) ? 0 : (int)HttpContext.Session.GetInt32("SelectedJobType");
            SelectedStatus = (HttpContext.Session.GetInt32("SelectedStatus") == default) ? 0 : (int)HttpContext.Session.GetInt32("SelectedStatus");

            JobTypes = aggregationRepository.GetJobTypes(true).Select(a =>
                new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Description,
                    Selected = a.Id == SelectedJobType
                }).ToList();

            Intervals = aggregationRepository.GetIntervals(true).Select(a =>
                new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Description,
                    Selected = a.Id == SelectedInterval
                }).ToList();

            Statuses = aggregationRepository.GetStatuses(true).Select(a =>
                new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Description,
                    Selected = a.Id == SelectedStatus
                }).ToList();

            TbConfiguration? configuration = aggregationRepository.GetConfigurationByKey("HistoricalViewOpenPeriod");
            DateTime openPeriod = (configuration != null) ? DateTime.ParseExact(configuration.Value, "yyyy/MM/dd", CultureInfo.InvariantCulture) : System.DateTime.MinValue;

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
                else if (StartDate < openPeriod || StartDate > System.DateTime.Now || EndDate > System.DateTime.Now || StartDate > EndDate)
                {
                    ModelState.AddModelError("StartDate", $"Input date is invalid. Valide range is between {openPeriod.ToShortDateString()} to {System.DateTime.Now.ToShortDateString()}.");
                }
                else
                {
                    Jobs = aggregationRepository.GetJobs(StartDate, EndDate, SelectedInterval, SelectedJobType, SelectedStatus);
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
            HttpContext.Session.SetString("EndDate", EndDate.ToString("MM/dd/yyyy"));
            HttpContext.Session.SetInt32("SelectedInterval", SelectedInterval);
            HttpContext.Session.SetInt32("SelectedJobType", SelectedJobType);
            HttpContext.Session.SetInt32("SelectedStatus", SelectedStatus);

            Init();
        }
    }
}
