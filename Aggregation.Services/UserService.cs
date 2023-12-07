using Aggregation_DataModels.Models;
using Aggregation_Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;

namespace Aggregation_Services
{
    public class UserService : IUserService
    {
        private readonly AggregationDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private TbUser? user;
            
        public UserService(AggregationDbContext contex, IHttpContextAccessor httpContextAccessor)
        {
            this.context = contex;
            this.httpContextAccessor = httpContextAccessor;
            this.user = GetCurrentUser();
        }

        public TbUser? GetCurrentUser()
        {
            TbUser? currentUser = null;

            if (this.httpContextAccessor.HttpContext != null)
            {
                string host = this.httpContextAccessor.HttpContext.Request.Host.ToString().ToUpper();

                if (host != null && host.Contains("LOCALHOST") && !host.Contains("44365"))
                {
                    return currentUser;
                }
            }

            if (this.httpContextAccessor.HttpContext != null)
            {
                currentUser = this.context.TbUsers.FirstOrDefault(u => u.UserId == this.httpContextAccessor.HttpContext.User.Identity.Name);

                if (currentUser != null && !currentUser.Active)
                {
                    currentUser = null;
                }
            }

            return currentUser;
        }

        int IUserService.GetId()
        {
            return (user == null) ? 0 : user.Id;
        }

        string IUserService.GetName()
        {
            return (user == null) ? string.Empty : user.Name;
        }

        string IUserService.GetUserId()
        {
            return (user == null) ? string.Empty : user.UserId;
        }

        bool IUserService.HasAccessToConfiguration()
        {
            return (user == null) ? false : (user.Active && user.HasAccessToConfiguration);
        }

        bool IUserService.HasAccessToDisable()
        {
            return (user == null) ? false : (user.Active && (user.HasAccessToConfiguration || user.HasAccessToQueue || user.HasAccessToRefreshJob || user.HasAccessToUpdateClock));
        }

        bool IUserService.HasAccessToQueue()
        {
            return (user == null) ? false : (user.Active && user.HasAccessToQueue);
        }

        bool IUserService.HasAccessToRefreshJob()
        {
            return (user == null) ? false : (user.Active && user.HasAccessToRefreshJob);
        }

        bool IUserService.HasAccessToUpdateClock()
        {
            return (user == null) ? false : (user.Active && user.HasAccessToUpdateClock);
        }

        bool IUserService.HasAdminAccess()
        {
            return (user == null) ? false : (user.Active && user.HasAdminAccess);
        }
    }
}