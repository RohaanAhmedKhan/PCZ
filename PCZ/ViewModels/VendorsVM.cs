using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PCZ.Models;

namespace PCZ.ViewModels {
    public class VendorsVM : ViewModel {

        public Vendor Vendor { get; set; }
        public IEnumerable<Job> Jobs { get; set; }
        public IEnumerable<Vendor> Vendors { get; set; }
        public IEnumerable<Payment> payments { get; set; }
        public User User { get; set; }
        public String email { get; set; }
        public String pass { get; set; }
    }
}