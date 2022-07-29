using PCZ.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PCZ.ViewModels
{
    public class ProductViewModel
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int  Quantity { get; set; }
        [Required]
        public string Description { get; set; }
        public IEnumerable<SelectListItem> _color { get; set; }
        [Required]
        public string color { get; set; }
        public IEnumerable<SelectListItem> _condition { get; set; }
        [Required]
        public string condition { get; set; }
        [Required]
        public int Price { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        [Required]
        public int Category { get; set; }
        public string CategoryName { get; set; }

        public string ErrorMessage { get; set; }

        public int Action { get; set; }
        public string ImagePath { get; set; }
        public string  size { get; set; }

        public List<Product> products { get; set; }
        public List<PCZ.Models.Vendor>  locations{ get; set; }
        
    }
}