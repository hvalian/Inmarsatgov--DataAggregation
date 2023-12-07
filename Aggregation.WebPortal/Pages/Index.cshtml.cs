using Aggregation_DataModels.Enums;
using Aggregation_DataModels.Models;
using Aggregation_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace Website.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IAggregationRepository aggregationRepository;
        private readonly ILogger<IndexModel> logger;
        private readonly IToastNotification toastNotification;
        private readonly IUserService userRepository;

        public IndexModel(IAggregationRepository aggregationRepository, ILogger<IndexModel> logger, IToastNotification toastNotification, IUserService userRepository)
        {
            this.aggregationRepository = aggregationRepository;
            this.logger = logger;
            this.toastNotification = toastNotification;
            this.userRepository = userRepository;
        }

        #region Properties
        [BindProperty]
        public int? AverageProcessingTime_DailyJob { get; set; } = default!;

        [BindProperty]
        public int? AverageProcessingTime_HourlyJob { get; set; } = default!;

        [BindProperty]
        public bool JobProcessingHasIssues { get; set; } = default!;

        [BindProperty]
        public bool JobProcessingIsSuspended { get; set; } = default!;

        [BindProperty]
        public DateTime? LastErrJobAggregationDateTime { get; set; } = default!;

        [BindProperty]
        public int? LastErrJobElapsedtime { get; set; } = default!;

        [BindProperty]
        public int? LastErrJobId { get; set; } = default!;

        [BindProperty]
        public string LastErrJobInterval { get; set; } = default!;

        [BindProperty]
        public DateTime? LastErrJobStartDateTime { get; set; } = default!;

        [BindProperty]
        public DateTime? LastErrJobEndDateTime { get; set; } = default!;

        [BindProperty]
        public string LastErrJobStatus { get; set; } = default!;

        [BindProperty]
        public DateTime? LastJobAggregationDateTime { get; set; } = default!;

        [BindProperty]
        public int? LastJobElapsedtime { get; set; } = default!;

        [BindProperty]
        public int? LastJobId { get; set; } = default!;

        [BindProperty]
        public string LastJobInterval { get; set; } = default!;

        [BindProperty]
        public DateTime? LastJobStartDateTime { get; set; } = default!;

        [BindProperty]
        public DateTime? LastJobEndDateTime { get; set; } = default!;

        [BindProperty]
        public string LastJobStatus { get; set; } = default!;

        [BindProperty]
        public DateTime? NextJobAggregationDateTime { get; set; } = default!;

        [BindProperty]
        public int? NextJobId { get; set; } = default!;

        [BindProperty]
        public string NextJobInterval { get; set; } = default!;

        [BindProperty]
        public DateTime? NextJobStartDateTime { get; set; } = default!;

        [BindProperty]
        public DateTime? NextJobStartAfterDateTime { get; set; } = default!;

        [BindProperty]
        public string NextJobStatus { get; set; } = default!;

        [BindProperty]
        public int? NextJobWillStartInMinutes { get; set; } = default!;

        [BindProperty]
        public int? CommandTimeout { get; set; }

        [BindProperty]
        public int? JobStartTimeDelay { get; set; }

        [BindProperty]
        public DateTime? JobSuspendProcessingAfter { get; set; }

        [BindProperty]
        public string NodeStatus { get; set; }

        [BindProperty]
        public int? NumberOfDaysForStats { get; set; }

        [BindProperty]
        public int? NumberOfRefresh { get; set; }

        [BindProperty]
        public string ProjectName { get; set; }

        [BindProperty]
        public bool? RefreshEnabled { get; set; }

        [BindProperty]
        public int? RefreshInterval { get; set; }

        [BindProperty]
        public bool? RefreshDailyJobOnce { get; set; }

        [BindProperty]
        public int? RetentionActivityLogNumberOfHours { get; set; }

        [BindProperty]
        public int? SpCommandTimeout { get; set; }

        public VwGetDashbordDatum DashbordDatum { get; set; } = default!;
        #endregion

        public string GetDisableEnableConfirmationText()
        {
            string newStatus = (JobProcessingIsSuspended ? "enable" : "disable");

            if (HasAccessToDisable())
            {
                return $"To {newStatus} Aggregation process, click on the button below:";
            }
            else
            {
                return $"You don't have a permission to {newStatus} Aggregation process";
            }
        }

        private void Init()
        {
            AverageProcessingTime_DailyJob = DashbordDatum.AverageProcessingTimeDailyJob;
            AverageProcessingTime_HourlyJob = DashbordDatum.AverageProcessingTimeHourlyJob;
            JobProcessingHasIssues = (bool)DashbordDatum.JobProcessingHasIssues;
            JobProcessingIsSuspended = (bool)DashbordDatum.JobProcessingIsSuspended;
            LastErrJobAggregationDateTime = DashbordDatum.LastErrJobAggregationDateTime;
            LastErrJobElapsedtime = DashbordDatum.LastErrJobElapsedtime;
            LastErrJobId = DashbordDatum.LastErrJobId;
            LastErrJobInterval = DashbordDatum.LastErrJobInterval;
            LastErrJobStartDateTime = DashbordDatum.LastErrJobStartDateTime;
            LastErrJobEndDateTime = DashbordDatum.LastErrJobEndDateTime;
            LastErrJobStatus = DashbordDatum.LastErrJobStatus;
            LastJobAggregationDateTime = DashbordDatum.LastJobAggregationDateTime;
            LastJobElapsedtime = DashbordDatum.LastJobElapsedtime;
            LastJobId = DashbordDatum.LastJobId;
            LastJobInterval = DashbordDatum.LastJobInterval;
            LastJobStartDateTime = DashbordDatum.LastJobStartDateTime;
            LastJobEndDateTime = DashbordDatum.LastJobEndDateTime;
            LastJobStatus = DashbordDatum.LastJobStatus;
            NextJobAggregationDateTime = DashbordDatum.NextJobAggregationDateTime;
            NextJobId = DashbordDatum.NextJobId;
            NextJobInterval = DashbordDatum.NextJobInterval;
            NextJobStartDateTime = DashbordDatum.NextJobStartDateTime;
            NextJobStartAfterDateTime = DashbordDatum.NextJobStartAfterDateTime;
            NextJobStatus = DashbordDatum.NextJobStatus;
            NextJobWillStartInMinutes = DashbordDatum.NextJobWillStartInMinutes;
            CommandTimeout = DashbordDatum.CommandTimeout;
            SpCommandTimeout = DashbordDatum.SpCommandTimeout;
            RefreshEnabled = DashbordDatum.RefreshEnabled;
            RefreshInterval = DashbordDatum.RefreshInterval;
            JobStartTimeDelay = DashbordDatum.JobStartTimeDelay;
            JobSuspendProcessingAfter = DashbordDatum.JobSuspendProcessingAfter;
            NumberOfDaysForStats = DashbordDatum.NumberOfDaysForStats;
            NumberOfRefresh = DashbordDatum.NumberOfRefresh;
            ProjectName = DashbordDatum.ProjectName;
            RetentionActivityLogNumberOfHours = DashbordDatum.RetentionActivityLogNumberOfHours;
        }

        public bool IsCriticalError()
        {
            return this.LastErrJobId == this.LastJobId;
        }

        public bool HasAccessToDisable()
        {
            return userRepository.HasAccessToDisable();
        }

        public void OnGet()
        {
            DashbordDatum = aggregationRepository.GetDashbordData();
            Init();
        }

        public IActionResult OnGetDisplayJobDetail()
        {
            string id = Request.Query["id"];
            return Redirect("../Jobs/Historical/Details?id=" + id + "&returnTo=Dashboard");
        }

        public async Task<IActionResult> OnGetUpdateJobProcessingState()
        {
            if (!HasAccessToDisable())
            {
                toastNotification.AddErrorToastMessage("Your request could not be processed becase of lack of permission!");
                return Redirect("../Utility/Unauthorized");
            }

            TbConfiguration configuration = await aggregationRepository.GetConfigurationByKeyAsync("Disabled");

            string oldValue = configuration.Value;

            configuration.Value = (configuration == null || configuration.Value == "0") ? "1" : "0";
            JobProcessingIsSuspended = (configuration.Value == "1");

            await aggregationRepository.SaveConfiguration(configuration, oldValue, userRepository.GetId());

            toastNotification.AddSuccessToastMessage("your request has been processed successfully!");

            return Redirect("./Index");
        }
    }
}