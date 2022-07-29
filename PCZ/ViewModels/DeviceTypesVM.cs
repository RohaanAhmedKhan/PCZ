using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PCZ.Models;

namespace PCZ.ViewModels {
    public class DeviceTypesVM : ViewModel {
        public DeviceType DeviceType { get; set; }
        public IEnumerable<DeviceType> DeviceTypes { get; set; }
    }
}