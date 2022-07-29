using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PCZ.Models;

namespace PCZ.ViewModels {
    public class IssuesVM : ViewModel {
        public Issue Issue { get; set; }
        public IEnumerable<Issue> Issues { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public IEnumerable<SelectListItem> DeviceList { get; set; }
        public int Cat { get; set; }
    }
}