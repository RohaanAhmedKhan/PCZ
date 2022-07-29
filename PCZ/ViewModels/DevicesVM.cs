using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PCZ.Models;

namespace PCZ.ViewModels {
    public class DevicesVM : ViewModel {

        public Device Device { get; set; }
        public IEnumerable<Device> Devices { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public IEnumerable<SelectListItem> BrandList { get; set; }
        public int CategoryID { get; set; }
    }
}