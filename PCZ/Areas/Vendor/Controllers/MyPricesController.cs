using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PCZ.Models;
using PCZ.ViewModels;

namespace PCZ.Areas.Vendor.Controllers
{
    [CheckAuthorization (Roles = "Vendor")]
    public class MyPricesController : Controller
    {

        PCZDbContext db = new PCZDbContext();
        
        public ActionResult Index()
        {
            var vm = new IssuesVM();
            vm.Issues = db.Issue.ToList();
            return View(vm);
        }
    }
}