using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaterTreatment.Web.Entities
{
    public class User : IdentityUser<int, UserLogin, UserRole, UserClaim>, INamedEntity
    {
        public User()
        {
            userSiteAccess = new HashSet<Site>();
            BuildingSystems = new HashSet<BuildingSystem>();
        }


        [NotMapped]
        public string Name { get { return UserName; } set { UserName = value; } }

        [StringLength(128)]
        public string FirstName { get; set; }

        [StringLength(128)]
        public string LastName { get; set; }

        public virtual Location Location { get; set; }

        public bool IsActive { get; set; }

        public string InviteCode { get; set; }
        public string ResetCode { get; set; }
        public DateTime? ResetCodeExpiration { get; set; }
        public DateTime? EulaAgreedOn { get; set; }

        public virtual Vendor Vendor { get; set; }
        public virtual ICollection<Site> userSiteAccess { get; set; }
        public virtual ICollection<AuditEntity> AuditEntity { get; set; }
        public virtual ICollection<BuildingSystem> BuildingSystems { get; set; }
    }
}