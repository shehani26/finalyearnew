using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELDER_NUTRICIANTS.Models
{
    public class ElderVM
    {
        public int ID { get; set; }
        public string FULL_NAME { get; set; }
        public string NIC { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string GENDER { get; set; }
        public string CONTACT_NO { get; set; }
        public string ADDRESS { get; set; }
     
        public string PROFIL_PIC { get; set; }

        public string E_GURDIAN_CODE { get; set; }
        public Nullable<int> ELDER_ID { get; set; }
        public string G_FULL_NAME { get; set; }
        public string RELATIONSHIP { get; set; }
        public string G_CONTACT_NO { get; set; }
        public string EMAIL { get; set; }
        public string G_ADDRESS { get; set; }

        public HttpPostedFileBase Img_File_F { get; set; }

    }
}