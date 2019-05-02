using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WaterTreatment.Web.Entities;
using Microsoft.AspNet.Identity;

namespace WaterTreatment.Web.Services
{

    public interface IUserService
    {
        bool HasContext { get; }
        User CurrentUser { get; }
        bool IsAdmin();
        bool HasFullSiteAccess();
        bool IsSiteAdmin();
        bool IsExecutiveReportViewer();
        bool IsReportViewer();
        bool IsDataRecorder();
        bool IsAuditRecorder();
        bool CanEditUser(User AgainstUser);
        IEnumerable<Site> GetSites();
        IEnumerable<Site> GetActiveSites();
        bool CanEditSite(Site AgainstSite);
        bool CanViewReport(Report AgainstReport);
        IEnumerable<User> GetUsers();
    }

    public class UserService : IUserService
    {

        private readonly WTContext Context;

        public UserService(WTContext context)
        {
            Context = context;
        }

        public bool HasContext
        {
            get { return HttpContext.Current != null; }
        }


        public User CurrentUser
        {
            get
            {
                var userid = HttpContext.Current.User.Identity.GetUserId<int>();
                return Context.Users.FirstOrDefault(x => x.Id == userid);
            }
        }

        public bool IsAdmin()
        {
            return CurrentUser.Roles.Any(x => x.RoleId == Context.Ref.Roles.SystemAdministrator.Id);
        }

        public bool HasFullSiteAccess()
        {
            return IsAdmin() || CurrentUser.Roles.Any(x => x.RoleId == Context.Ref.Roles.ExecutiveReportViewer.Id);
        }

        public bool IsSiteAdmin()
        {
            return CurrentUser.Roles.Any(x => x.RoleId == Context.Ref.Roles.SiteAdministrator.Id);
        }

        public bool IsExecutiveReportViewer() {
            return CurrentUser.Roles.Any(x => x.RoleId == Context.Ref.Roles.ExecutiveReportViewer.Id);
        }

        public bool IsReportViewer() {
            return CurrentUser.Roles.Any(x => x.RoleId == Context.Ref.Roles.ReportViewer.Id);
        }

        public bool IsDataRecorder() {
            return CurrentUser.Roles.Any(x => x.RoleId == Context.Ref.Roles.DataRecorder.Id);
        }

        public bool IsAuditRecorder() {
            return CurrentUser.Roles.Any(x => x.RoleId == Context.Ref.Roles.AuditRecorder.Id);
        }

        public bool CanEditUser(User AgainstUser)
        {

            //If you are a system admin you can edit everyone, even other sys admins
            if (IsAdmin())
                return true;

            //If you are not a system admin you must be a site admin or else you cannot edit.
            //These are the only two roles allowed to edit users
            if (!IsSiteAdmin())
                return false;
            
            //Site admins cannot edit other site admins or system admins so disallow that.
            if (AgainstUser.Roles.Any(x => x.RoleId == Context.Ref.Roles.SiteAdministrator.Id || x.RoleId == Context.Ref.Roles.SystemAdministrator.Id))
                return false;

            //Finally if they can edit the editted user belows to one of the sites the current user has.
            return CurrentUser.userSiteAccess.Select(x => x).Intersect(AgainstUser.userSiteAccess.Select(x => x)).Any();
        }

        public IEnumerable<Site> GetSites()
        {
            return HasFullSiteAccess() ? Context.Sites : CurrentUser.userSiteAccess.Select(x => x);
        }

        public IEnumerable<Site> GetActiveSites()
        {
            return GetSites().Where(x => x.IsActive);
        }

        public bool CanEditSite(Site AgainstSite)
        {
            return IsAdmin() || (IsSiteAdmin() && CurrentUser.userSiteAccess.Select(x => x).Any(x => x.Id == AgainstSite.Id));
        }

        public bool CanViewReport(Report AgainstReport) {
            if (IsAdmin()) {
                return true;
            }

            if (CurrentUser.Id == AgainstReport.User.Id) {
                return true;
            }

            var sameSite = CurrentUser.userSiteAccess.Select(x => x).Any(x => x.Id == AgainstReport.Site.Id);
            if (sameSite && IsSiteAdmin()) {
                return true;
            }

            if (AgainstReport.SubmissionDate.HasValue) {
                if (IsExecutiveReportViewer()) {
                    return true;
                }

                if (sameSite && IsReportViewer()) {
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<User> GetUsers()
        {
            if (IsAdmin())
                return Context.Users.Where(u => u.IsActive).ToList();

            var siteIds = CurrentUser.userSiteAccess.Select(s => s.Id);
            var users = Context.Users.Where(u => u.userSiteAccess.Any(s => siteIds.Contains(s.Id)));

            var adminRoleIds = new List<int>
            {
                Context.Ref.Roles.SystemAdministrator.Id,
                Context.Ref.Roles.SiteAdministrator.Id
            };

            users = users.Where(u => !u.Roles.Any(r => adminRoleIds.Contains(r.RoleId)));
            users = users.Where(u => u.Id != CurrentUser.Id);

            return users.Where(u => u.IsActive).ToList();
        }

    }

}