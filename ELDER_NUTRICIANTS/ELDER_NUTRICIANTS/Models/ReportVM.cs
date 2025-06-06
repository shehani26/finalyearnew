using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELDER_NUTRICIANTS.Models
{
    public class ReportVM
    {
        public int ID { get; set; }
        public Nullable<System.DateTime> READING_DATE { get; set; }
        public Nullable<decimal> GLUCOSE_LEVEL { get; set; }
        public string SOURCE { get; set; }
        public string DESCRIPTION { get; set; }
  



        public Nullable<System.DateTime> REPORT_UPLOAD_DATE { get; set; }
        public string REPORT { get; set; }
        public HttpPostedFileBase Img_File_F { get; set; }

        public V_ELDER ve { get; set; }

        public List<V_ELDER_BLOOD_RPT> vebr { get; set; }


        public int CAREGIVER_ID  { get; set; }

        public List<V_ASSIGN_CAREGIVER> vac { get; set; }

        public List<CAREGIVER> ca { get; set; }

        public List<V_ASSIGN_ROOMS> vars { get; set; }

        public List<ROOM> ro { get; set; }
    }
}