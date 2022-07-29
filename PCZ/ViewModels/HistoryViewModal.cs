using PCZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCZ.ViewModels
{
    public class HistoryViewModal
    {
        public int paid { get; set; }
        public int unpaid { get; set; }
        public int pamount { get; set; }
        public int upamount { get; set; }
        public IEnumerable<Payment> payments { get; set; }
        public List<Job> jobs { get; set; }

        public string BuisinessName { get; set; }

        public string ImgURl { get; set; }

        public double? remainingBalance { get; set; }
        public double? paidBalance { get; set; }
        public double? actualBalance { get; set; }

    }
}