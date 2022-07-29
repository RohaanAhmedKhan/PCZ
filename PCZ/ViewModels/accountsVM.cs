using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCZ.ViewModels {
    public class accountsVM : ViewModel {
        public string uname { get; set; }
        public string pass { get; set; }
        public string returnUrl { get; set; }
        public bool isRemember { get; set; }
        public string emailError1 { get; set; }
        public string pwdError1 { get; set; }
        public int[] Roles { get; set; }
    }
}