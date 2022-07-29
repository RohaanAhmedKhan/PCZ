using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PCZ.Models;
using PCZ.ViewModels;

namespace PCZ.Areas.Admin.Controllers
{
    [CheckAuthorization (Roles = "Admin")]
    public class BrandsController : Controller
    {
        PCZDbContext db = new PCZDbContext();
        string error = null;
        public ActionResult Index(int? id, int? act) {
            var vm = new BrandsVM();

            vm.Action = act ?? 0;
            vm.Brands = db.Brand.ToList();
            vm.CategoryList = new SelectList(db.DeviceType.ToList().OrderBy(d => d.Name), "Id", "Name");
            if (id != null) {
                vm.Brand = db.Brand.Find(id);
            }

            vm.ErrorMessage = ViewBag.Err;
            return View(vm);
        }

        [HttpPost]
        public ActionResult Create(BrandsVM vm) {

            vm.CategoryList = new SelectList(db.DeviceType.ToList().OrderBy(d => d.Name), 
                "Id", "Name", vm.Brand.TypeId);

            try {
                db.Brand.Add(vm.Brand);
                db.SaveChanges();
               
                ViewBag.Err = "Success";
                return RedirectToAction("Index", new { id = vm.Brand.Id, act = 1 });
            }
            catch (Exception e) {
                vm.ErrorMessage = e.Message;
                return View(vm);
            }

        }

        [HttpPost]
        public ActionResult Update(BrandsVM vm) {

            vm.CategoryList = new SelectList(db.DeviceType.ToList().OrderBy(d => d.Name),
                "Id", "Name", vm.Brand.TypeId);

            try {
                var dt = db.Brand.Find(vm.Brand.Id);
                dt.Name = vm.Brand.Name;
                dt.TypeId = vm.Brand.TypeId;

                db.Entry(dt).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = vm.Brand.Id, act = 1 });
            }
            catch (Exception e) {
                vm.ErrorMessage = e.Message;
                return View(vm);
            }
        }
    }
}