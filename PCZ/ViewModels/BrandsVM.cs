using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PCZ.Models;

namespace PCZ.ViewModels {
    public class BrandsVM : ViewModel {

        public Brand Brand { get; set; }
        public IEnumerable<Brand> Brands { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}