using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PCZ.Models;
using PCZ.ViewModels;

namespace PCZ.Controllers
{
    public class PriceController : Controller
    {

        PCZDbContext db = new PCZDbContext();

        public ActionResult Index(int cat = -1)
        {
            var vm = new IssuesVM();
            vm.Issues = db.Issue.Where(i => i.Device.Brand.TypeId == cat).ToList();
            vm.CategoryList = new SelectList(db.DeviceType.ToList(), "Id", "Name", cat);
            return View(vm);
        }
    }
}