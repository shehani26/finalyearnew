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
    
    public partial class ROOM_ASSIGNED
    {
        public int ID { get; set; }
        public Nullable<int> ROOM_ID { get; set; }
        public Nullable<int> ELDER_ID { get; set; }
        public Nullable<System.DateTime> ASSIGNED_DATE { get; set; }
        public Nullable<System.DateTime> RELEASED_DATE { get; set; }
        public string ASSIGNED_BY { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<int> STATUS { get; set; }
        public Nullable<System.DateTime> ENTED_DATE { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
    
        public virtual ELDER ELDER { get; set; }
        public virtual ROOM ROOM { get; set; }
    }
}
