using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PCZ.Models;

namespace PCZ.ViewModels {
    public class RepairsVM : ViewModel {

        public Job Job { get; set; }
        public IEnumerable<Job> Jobs { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public IEnumerable<SelectListItem> DeviceList { get; set; }
        public IEnumerable<SelectListItem> IssueList { get; set; }
        public Payment payment { get; set; }
        public int Amount { get; set; }
        public IEnumerable<int> IssuesAr { get; set; }
        public int Cat { get; set; }
        public IEnumerable<SelectListItem> statusList { get; set; }
        public IEnumerable<SelectListItem> paidList { get; set; }
        public string status { get; set; }
        [Required]
        public string paid  { get; set; }
        public string preUrl { get; set; }
        public int Action { get; set; }
        public int VendorID { get; set; }
    }
}