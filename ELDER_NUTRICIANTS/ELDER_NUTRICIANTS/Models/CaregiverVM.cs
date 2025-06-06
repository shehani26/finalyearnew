using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELDER_NUTRICIANTS.Models
{
    public class CaregiverVM
    {
        public int ID { get; set; }
        public string CAREGIVER_CODE { get; set; }
        public string FULL_NAME { get; set; }
        public string GENDER { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string NIC { get; set; }
        public string CONTACT_NO { get; set; }
        public string EMAIL { get; set; }
        public string ADDRESS { get; set; }
        public string PROFILE_IMG { get; set; }
        public string HIGHER_QULIFICATION { get; set; }
        public Nullable<int> YEAR_OF_EXPERIENCE { get; set; }
     

        public HttpPostedFileBase Img_File_F { get; set; }

        public string PASSWORD { get; set; }

        public string CONFIRM_PASSWORD { get; set; }
    }
}