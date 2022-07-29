using PCZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCZ.ViewModels
{
    public class VendorDashboardVM
    {
        public IEnumerable<AdminMessage> AdminMessages { get; set; }
        public IEnumerable<VendorMessage> VendorMessages { get; set; }

        public List<Messages> Messages { get; set; }
        public string msg { get; set; }
        public int Pending { get; set; }
        public int vend { get; set; }
        public string jsonMessages { get; set; }

    }
}