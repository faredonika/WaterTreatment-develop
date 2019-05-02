using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Models
{
    public class UserInfoModel
    {

        public string FullName { get; set; }
        public string RoleName { get; set; }
        public bool IsDataRecorder { get; set; }
    }
}