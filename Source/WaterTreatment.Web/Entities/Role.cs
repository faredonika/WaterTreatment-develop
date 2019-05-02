using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using System.Collections.Generic;

namespace WaterTreatment.Web.Entities
{

    public class UserRole : IdentityUserRole<int> { }
    public class UserClaim : IdentityUserClaim<int> { }
    public class UserLogin : IdentityUserLogin<int> { }

    public class Role : IdentityRole<int, UserRole>, INamedEntity {

        public Role()
        {
            Actions = new HashSet<RoleAction>();
            SubSections = new HashSet<SubSection>();
        }

        public virtual ICollection<RoleAction> Actions { get; set; }
        public virtual ICollection<SubSection> SubSections { get; set; }

        public IEnumerable<LandingAction> OrderedLandingActions { get { return Actions.OrderBy(x => x.Order).Select(x => x.Action); } }

    }
    
}