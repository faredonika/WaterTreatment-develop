using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Vendor { get; set; }
        public string Sites { get; set; }
        public int SiteCount { get; set; }
        public string SiteIdList { get; set; }
        public bool IsLocked { get; set; }
        public bool IsActive { get; set; }
    }
}