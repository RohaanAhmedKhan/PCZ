using PCZ.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PCZ.ViewModels
{
    public class PaymentViewModel
    {
        public int ID { get; set; }
        [Required]
        public string Description { get; set; }
        public double? PaidBalance { get; set; }
        public double? actualBalance { get; set; }

        public double? RemainingBalance { get; set; }
        public double oldAmount { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public double Amount { get; set; }
        public string VendorName { get; set; }
        public int VendorID { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime DateOfPay { get; set; }
        public int Action { get; set; }

        public string ImagePath { get; set; }

        public List<PaymentHistory> payments { get; set; }
    }
}