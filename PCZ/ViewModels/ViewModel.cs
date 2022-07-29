using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCZ.ViewModels {
    public class ViewModel {
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
        public int Action { get; set; }
        public bool Success { get; set; }
    }
}