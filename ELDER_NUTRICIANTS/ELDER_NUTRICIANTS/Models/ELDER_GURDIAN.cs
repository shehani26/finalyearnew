//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ELDER_NUTRICIANTS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ELDER_GURDIAN
    {
        public int ID { get; set; }
        public string E_GURDIAN_CODE { get; set; }
        public Nullable<int> ELDER_ID { get; set; }
        public string FULL_NAME { get; set; }
        public string RELATIONSHIP { get; set; }
        public string CONTACT_NO { get; set; }
        public string EMAIL { get; set; }
        public string ADDRESS { get; set; }
        public Nullable<int> STATUS { get; set; }
    
        public virtual ELDER ELDER { get; set; }
    }
}
