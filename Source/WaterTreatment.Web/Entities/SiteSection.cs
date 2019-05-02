using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaterTreatment.Web.Entities
{

    public class SiteSection : NamedEntity
    {

        public string Controller { get; set; }
        public string Action { get; set; }

    }

}