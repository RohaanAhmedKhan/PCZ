using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PCZ.ViewModels
{
    public class DeliverToCustomerViewModal
    {
        public string jobNo { get; set; }
        [Required]
        public bool  Deliver{ get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime  date { get; set; }
    }
}