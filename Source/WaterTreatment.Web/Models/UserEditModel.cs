using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WaterTreatment.Web.Entities;

namespace WaterTreatment.Web.Models
{
    public class UserEditModel
    {

        public UserEditModel()
        {

        }

        public UserEditModel(User user)
        {
            Id = user.Id;
            Username = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            LocationId = user.Location.Id;
            Email = user.Email;
            RoleId = user.Roles.First().RoleId;
            VendorId = user.Vendor.Id;
            SiteList = string.Join(",", user.userSiteAccess.Select(s => s.Id).ToArray());
            SystemList = string.Join(",", user.BuildingSystems.Select(b => b.Id));
            InviteCode = user.InviteCode;
        }

        public int Id { get; set; }

        [Required]
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? LocationId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public int VendorId { get; set; }
        public string SiteList { get; set; }
        public string SystemList { get; set; }
        public bool IsLocked { get; set; }
        public bool IsActive { get; set; }
        public string InviteCode { get; set; }
        public string SelectedState { get; set; }
        public bool IsInternational { get; set; }
    }
}