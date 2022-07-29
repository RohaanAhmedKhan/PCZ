using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PCZ.Models;
using PCZ.ViewModels;

namespace PCZ.Areas.Admin.Controllers
{
    [CheckAuthorization(Roles = "Admin")]
    public class DevicesController : MasterController
    {
        PCZDbContext db = new PCZDbContext();
        string error = null;
        public ActionResult Index(int? id, int? act) {
            var vm = new DevicesVM();

            vm.Action = act ?? 0;
            vm.Devices = db.Device.ToList();
            vm.CategoryList = GetCategoriesSelectList(null);
            vm.BrandList = new SelectList(db.Brand.Where(b => b.TypeId == 1).ToList().OrderBy(d => d.Name), "Id", "Name");
            if (id != null) {
                vm.Device = db.Device.Find(id);

                vm.CategoryList = GetCategoriesSelectList(vm.Device.Brand.TypeId);
                vm.BrandList = new SelectList(db.Brand.Where(b => b.TypeId == vm.Device.Brand.TypeId).ToList().OrderBy(d => d.Name),
                    "Id", "Name", vm.Device.BrandId);

            }

            vm.ErrorMessage = ViewBag.Err;
            return View(vm);
        }

        [HttpPost]
        public ActionResult Create(DevicesVM vm) {

            vm.CategoryList = GetCategoriesSelectList(vm.CategoryID);
            vm.BrandList = new SelectList(db.Brand.Where(b => b.TypeId == vm.CategoryID).ToList().OrderBy(d => d.Name),
                "Id", "Name", vm.Device.BrandId);
             
            try {
                db.Device.Add(vm.Device);
                db.SaveChanges();

                ViewBag.Err = "Success";
                return RedirectToAction("Index", new { id = vm.Device.Id});
            }
            catch (Exception e) {
                vm.ErrorMessage = e.Message;
                return View(vm);
            }

        }

        [HttpPost]
        public ActionResult Update(DevicesVM vm) {

            vm.CategoryList = GetCategoriesSelectList(vm.CategoryID);
            vm.BrandList = new SelectList(db.Brand.Where(b => b.TypeId == vm.CategoryID).ToList().OrderBy(d => d.Name),
                "Id", "Name", vm.Device.BrandId);

            try {
                var dt = db.Device.Find(vm.Device.Id);
                dt.Name = vm.Device.Name;
                dt.BrandId = vm.Device.BrandId;

                db.Entry(dt).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = vm.Device.Id, act = 1 });
            }
            catch (Exception e) {
                vm.ErrorMessage = e.Message;
                return View(vm);
            }
        }
    }
}