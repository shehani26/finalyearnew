using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELDER_NUTRICIANTS.Models
{
    public class DoctorVM
    {
        public int ID { get; set; }
        public string D_CODE { get; set; }
        public string FULL_NAME { get; set; }
        public string GENDER { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string SPECIALIZATION { get; set; }
        public string MEDICAL_LICENSE_NO { get; set; }
        public string CONTACT_NO { get; set; }
        public string EMAIL { get; set; }
        public string REGISTERED_HOSPITAL { get; set; }
        public string ADDRESS { get; set; }
        public Nullable<System.DateTime> JOINED_DATE { get; set; }
   
        public string PROFILE_IMAGE { get; set; }
  
        public HttpPostedFileBase Img_File_F { get; set; }

        public string PASSWORD { get; set; }

        public string CONFIRM_PASSWORD { get; set; }
    }
}