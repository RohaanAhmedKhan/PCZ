//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PCZ.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class VendorMessage
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public int VendorID { get; set; }
        public System.DateTime RecieveTime { get; set; }
        public bool IsRead { get; set; }
        public string Sender { get; set; }
    
        public virtual User User { get; set; }
    }
}
