using Aggregation_DataModels.Models;
using Aggregation_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Website.Pages.Configuration
{
    public class RadioButtonInputProperties
    {
        public string Name { get; set; }
        public string ElementId { get; set; }
        public bool IsSelected { get; set; }
        public bool IsDisabled { get; set; }
    }

    public class EditModel : PageModel
    {
        private readonly IAggregationRepository aggregationRepository;
        private readonly IToastNotification toastNotification;
        private readonly IUserService userRepository;

        public EditModel(IAggregationRepository aggregationRepository, IToastNotification toastNotification, IUserService userRepository)
        {
            this.aggregationRepository = aggregationRepository;
            this.toastNotification = toastNotification;
            this.userRepository = userRepository;
        }

        #region Properties
        [BindProperty]
        public TbConfiguration TbConfiguration { get; set; } = default!;

        [BindProperty]
        public string BoolValue { get; set; }
        public string[] BoolValues = new[] { "True", "False" };

        [BindProperty]
        public List<RadioButtonInputProperties> BoolRadioButtons { get; set; }

        [BindProperty]
        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description Of Configuration item is Required")]
        [StringLength(maximumLength: 255, MinimumLength = 10)]
        public string Description { get; set; } = default!;

        [BindProperty]
        [DataType(DataType.Date)]
        [Display(Name = "Action View - OpenPeriod")]
        public DateTime ActionViewOpenPeriod { get; set; } = default!;

        [BindProperty]
        [Display(Name = "ActivityLog Retention Period(Number Of Hours) ")]
        [Range(24, 96)]
        public int Retention_ActivityLog_NumberOfHours { get; set; } = default!;

        [BindProperty]
        [Display(Name = " Command Timeout(Seconds)")]
        [Range(30, 300)]
        public int CommandTimeout { get; set; } = default!;

        [BindProperty]
        [DataType(DataType.Date)]
        [Display(Name = "Historical View - OpenPeriod")]
        public DateTime HistoricalViewOpenPeriod { get; set; } = default!;

        [BindProperty]
        [Display(Name = "Job Start Delay(Minutes)")]
        [Range(5, 305)]
        public int Job_StartTimeDelay { get; set; } = default!;

        [BindProperty]
        [Display(Name = "Number Of Days For Stats")]
        [Range(7, 42)]
        public int NumberOfDaysForStats { get; set; } = default!;

        [BindProperty]
        [Display(Name = "Number Of Refresh")]
        [Range(1, 7)]
        public int NumberOfRefresh { get; set; } = default!;

        [BindProperty]
        [Display(Name = "Project Name")]
        [StringLength(maximumLength: 255, MinimumLength = 10)]
        public string ProjectName { get; set; } = default!;

        [BindProperty]
        [Display(Name = "Refresh Interval(In Days)")]
        [Range(1, 7)]
        public int Refresh_Interval { get; set; } = default!;

        [BindProperty]
        [Display(Name = "Refresh Daily Job Once")]
        public string Refresh_DailyJobOnce { get; set; }

        [BindProperty]
        [Display(Name = "Refresh Enabled ")]
        public string Refresh_Enabled { get; set; }

        [BindProperty]
        [Display(Name = "ReRun Job Start Delay(Minutes) ")]
        [Range(15, 45)]
        public int ReRun_StartTimeDelay { get; set; } = default!;

        [BindProperty]
        [Display(Name = "SP Command Timeout(Seconds)  ")]
        [Range(30, 1500)]
        public int SP_CommandTimeout { get; set; } = default!;

        [BindProperty]
        [Display(Name = "Suspend Job Processing After")]
        public DateTime Job_SuspendProcessingAfter { get; set; } = default!;
        #endregion

        public bool HasAccessToConfiguration()
        {
            return userRepository.HasAccessToConfiguration();
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbconfiguration = await aggregationRepository.GetConfigurationByIdAsync(id);

            TbConfiguration = tbconfiguration;

            if ((bool)TbConfiguration.ReadOnly)
            {
                return Redirect("../Utility/Unauthorized");
            }

            BoolRadioButtons = new List<RadioButtonInputProperties>()
            {
                new RadioButtonInputProperties() { Name = "True", ElementId = "True" },
                new RadioButtonInputProperties() { Name = "False", ElementId = "False" }

            };

            Description = tbconfiguration.Description;

            if (TbConfiguration.Key == "ActionViewOpenPeriod")
            {
                ActionViewOpenPeriod = DateTime.Parse(TbConfiguration.Value);
            }
            else if (TbConfiguration.Key == "CommandTimeout")
            {
                CommandTimeout = Int32.Parse(TbConfiguration.Value);
            }
            else if (TbConfiguration.Key == "HistoricalViewOpenPeriod")
            {
                HistoricalViewOpenPeriod = DateTime.Parse(TbConfiguration.Value);
            }
            else if (TbConfiguration.Key == "Job_StartTimeDelay")
            {
                Job_StartTimeDelay = Int32.Parse(TbConfiguration.Value);
            }
            else if (TbConfiguration.Key == "Job_SuspendProcessingAfter")
            {
                string defaultValue = Request.Query["defaultValue"];
                Job_SuspendProcessingAfter = (string.IsNullOrEmpty(defaultValue)) ? DateTime.Parse(TbConfiguration.Value) : DateTime.Parse(defaultValue);
            }
            else if (TbConfiguration.Key == "NumberOfDaysForStats")
            {
                NumberOfDaysForStats = Int32.Parse(TbConfiguration.Value);
            }
            else if (TbConfiguration.Key == "NumberOfRefresh")
            {
                NumberOfRefresh = Int32.Parse(TbConfiguration.Value);
            }
            else if (TbConfiguration.Key == "ProjectName")
            {
                ProjectName = TbConfiguration.Value;
            }
            else if (TbConfiguration.Key == "Refresh_DailyJobOnce")
            {
                Refresh_DailyJobOnce = (TbConfiguration.Value == "1") ? "True" : "False";
            }
            else if (TbConfiguration.Key == "Refresh_Enabled")
            {
                Refresh_Enabled = (TbConfiguration.Value == "1") ? "True" : "False";
            }
            else if (TbConfiguration.Key == "Refresh_Interval")
            {
                Refresh_Interval = Int32.Parse(TbConfiguration.Value);
            }
            else if (TbConfiguration.Key == "Retention_ActivityLog_NumberOfHours")
            {
                Retention_ActivityLog_NumberOfHours = Int32.Parse(TbConfiguration.Value);
            }
            else if (TbConfiguration.Key == "ReRun_StartTimeDelay")
            {
                ReRun_StartTimeDelay = Int32.Parse(TbConfiguration.Value);
            }
            else if (TbConfiguration.Key == "SP_CommandTimeout")
            {
                SP_CommandTimeout = Int32.Parse(TbConfiguration.Value);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!HasAccessToConfiguration())
            {
                toastNotification.AddErrorToastMessage("Your request could not be processed becase of lack of permission!");
                return Redirect("../Utility/Unauthorized");
            }

            if (TbConfiguration.Key == "ActionViewOpenPeriod")
            {
                if (ActionViewOpenPeriod > System.DateTime.Now)
                {
                    ModelState.AddModelError("ActionViewOpenPeriod", $"Invalid Value. Value must be less than or equal to {System.DateTime.Now.Date.ToString()}.");
                }
            }
            else if (TbConfiguration.Key == "HistoricalViewOpenPeriod")
            {
                if (ActionViewOpenPeriod > System.DateTime.Now)
                {
                    ModelState.AddModelError("HistoricalViewOpenPeriod", $"Invalid Value. Value must be less than or equal to {System.DateTime.Now.Date.ToString()}.");
                }
            }
            else if (TbConfiguration.Key == "Job_SuspendProcessingAfter")
            {
                var tbnewConfiguration = await aggregationRepository.GetConfigurationByKeyAsync("LastHourlyJob_Scheduled_DateTime");

                DateTime minConfigDate = DateTime.ParseExact(tbnewConfiguration.Value, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
                DateTime maxConfigDate = DateTime.ParseExact("2099/12/31 23:59:59", "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);

                if (Job_SuspendProcessingAfter < minConfigDate || Job_SuspendProcessingAfter > maxConfigDate)
                {
                    ModelState.AddModelError("Job_SuspendProcessingAfter", $"Invalid Value. Valid range is between {minConfigDate.ToString()} to {maxConfigDate.ToString()}.");
                }
            }

            if (ModelState.IsValid)
            {
                string oldValue = TbConfiguration.Value;

                TbConfiguration.Description = Description;

                if (TbConfiguration.Key == "ActionViewOpenPeriod")
                {
                    TbConfiguration.Value = ActionViewOpenPeriod.ToString("yyyy/MM/dd");
                }
                else if (TbConfiguration.Key == "CommandTimeout")
                {
                    TbConfiguration.Value = CommandTimeout.ToString();
                }
                if (TbConfiguration.Key == "HistoricalViewOpenPeriod")
                {
                    TbConfiguration.Value = HistoricalViewOpenPeriod.ToString("yyyy/MM/dd");
                }
                else if (TbConfiguration.Key == "Job_StartTimeDelay")
                {
                    TbConfiguration.Value = Job_StartTimeDelay.ToString();
                }
                else if (TbConfiguration.Key == "Job_SuspendProcessingAfter")
                {
                    TbConfiguration.Value = Job_SuspendProcessingAfter.ToString("yyyy/MM/dd HH:mm:ss");
                }
                else if (TbConfiguration.Key == "NumberOfDaysForStats")
                {
                    TbConfiguration.Value = NumberOfDaysForStats.ToString();
                }
                else if (TbConfiguration.Key == "NumberOfRefresh")
                {
                    TbConfiguration.Value = NumberOfRefresh.ToString();
                }
                else if (TbConfiguration.Key == "ProjectName")
                {
                    TbConfiguration.Value = ProjectName;
                }
                else if (TbConfiguration.Key == "Refresh_DailyJobOnce")
                {
                    TbConfiguration.Value = (Refresh_DailyJobOnce == "True" ? "1" : "0");
                }
                else if (TbConfiguration.Key == "Refresh_Enabled")
                {
                    TbConfiguration.Value = (Refresh_Enabled == "True" ? "1" : "0");
                }
                else if (TbConfiguration.Key == "Refresh_Interval")
                {
                    TbConfiguration.Value = Refresh_Interval.ToString();
                }
                else if (TbConfiguration.Key == "Retention_ActivityLog_NumberOfHours")
                {
                    TbConfiguration.Value = Retention_ActivityLog_NumberOfHours.ToString();
                }
                else if (TbConfiguration.Key == "ReRun_StartTimeDelay")
                {
                    TbConfiguration.Value = ReRun_StartTimeDelay.ToString();
                }
                else if (TbConfiguration.Key == "SP_CommandTimeout")
                {
                    TbConfiguration.Value = SP_CommandTimeout.ToString();
                }

                await aggregationRepository.SaveConfiguration(TbConfiguration, oldValue, userRepository.GetId());

                toastNotification.AddSuccessToastMessage("your request has been processed successfully!");

                return Redirect("../Configuration/Index");
            }

            return Page();
        }
    }
}
