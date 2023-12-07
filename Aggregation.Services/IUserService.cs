using Aggregation_DataModels.Models;
using Azure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Aggregation_Services
{
    public interface IUserService
    {
        int GetId();
        string GetName();
        string GetUserId();
        bool HasAccessToConfiguration();
        bool HasAccessToDisable();
        bool HasAccessToQueue();
        bool HasAccessToRefreshJob();
        bool HasAccessToUpdateClock();
        bool HasAdminAccess();
    }
}
