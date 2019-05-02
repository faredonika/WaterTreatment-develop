using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Models
{
    public class InviteModel
    {

        public string Password { get; set; }
        public string PasswordCheck { get; set; }

        public int Id { get; set; }
        public string InviteCode { get; set; }
        public string Username { get; set; }

    }
}