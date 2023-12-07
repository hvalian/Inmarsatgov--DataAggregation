using Aggregation_DataModels.Models;
using Azure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Aggregation_Services
{
    public interface IAggregationRepository
    {
        bool ConfigurationExists(int? id);

        Task<bool> ConfigurationExistsAsync(int? id);

        IEnumerable<TbActivity> GetActivities();

        Task<List<TbActivity>> GetActivitiesAsync();

        List<TbActivityLogger> GetActivityLoggerByConfigId(int? id);

        Task<List<TbActivityLogger>> GetActivityLoggerByConfigIdAsync(int? id);

        TbConfiguration? GetConfigurationById(int? id);

        Task<TbConfiguration?> GetConfigurationByIdAsync(int? id);

        TbConfiguration? GetConfigurationByKey(string key);

        Task<TbConfiguration?> GetConfigurationByKeyAsync(string key);

        IEnumerable<TbConfiguration> GetConfigurations();

        Task<List<TbConfiguration>> GetConfigurationsAsync();

        VwGetDashbordDatum? GetDashbordData();

        Task<VwGetDashbordDatum?> GetDashbordDataAsync();

        TbJob? GetJob(int? id);

        Task<TbJob?> GetJobAsync(int? id);

        AggregationJobDetails GetJobDetails(int interval, DateTime startDate, DateTime endDate);

        Task<AggregationJobDetails> GetJobDetailsAsync(int interval, DateTime startDate, DateTime endDate);
        
        IEnumerable<TbJobType> GetJobTypes(bool withSelect);

        Task<List<TbJobType>> GetJobTypesAsync(bool withSelect);

        int GetJobCounts();

        IEnumerable<TbJob> GetJobs();

        IEnumerable<VwGetJob> GetJobs(DateTime StartDate);

        IEnumerable<TbJob> GetJobs(DateTime StartDate, DateTime EndDate, int selectedInterval, int selectedJobType, int selectedStatus);

        IEnumerable<TbInterval> GetIntervals(bool withSelect);

        Task<List<TbInterval>> GetIntervalsAsync(bool withSelect);

        IEnumerable<VwRnodeTypeMetric> GetMetrics();

        Task<List<VwRnodeTypeMetric>> GetMetricsAsync();

        IEnumerable<RtbNodeType> GetNodeTypes();

        Task<List<RtbNodeType>> GetNodeTypesAsync();

        IEnumerable<TbPriority> GetPriorities();

        Task<List<TbPriority>> GetPrioritiesAsync();

        IEnumerable<TbStatus> GetStatuses(bool withSelect);

        Task<List<TbStatus>> GetStatusesAsync(bool withSelect);

        Task<TbUser?> GetUserAsync(int id);

        TbUser? GetUser(string userId);

        Task<List<TbUser>> GetUsersAsync();

        IEnumerable<TbWebServer> GetWebServers();

        Task<bool> CancelJob(int jobId, int userId);

        Task<bool> RefreshJob(int jobId, int priority, int id, string userId);

        Task SaveConfiguration(TbConfiguration configuration, string oldValue, int userId);

        Task SaveUser(TbUser user, bool isNew, bool updateAdminstratorValue);

        Task<bool> UpdateClock(int jobId, int id);
    }
}