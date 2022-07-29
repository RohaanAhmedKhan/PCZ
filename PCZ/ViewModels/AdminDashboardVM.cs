using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PCZ.Models;

namespace PCZ.ViewModels {
    public class AdminDashboardVM {
        public IEnumerable<Notification> Notifications { get; set; }
        public List<VendorMessage> vendors { get; set; }

        public int VMsgCount { get; set; }

        public int VendorID { get; set; }
        public int Pending { get; set; }
        public int vend { get; set; }

        public List<Messages> Messages { get; set; }
        public string msg { get; set; }
    }
}