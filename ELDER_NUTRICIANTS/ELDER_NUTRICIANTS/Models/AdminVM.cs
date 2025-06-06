using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELDER_NUTRICIANTS.Models
{
    public class AdminVM
    {
        public int ID { get; set; }
        public string USER_NAME { get; set; }
       
        public string FULL_NAME { get; set; }
        public string EMAIL { get; set; }
        public string ROLE { get; set; }
      
 
        public string PIC { get; set; }

        public string PASSWORD { get; set; }

        public string CONFIRM_PASSWORD { get; set; }

        public HttpPostedFileBase Img_File_F { get; set; }
    }
}