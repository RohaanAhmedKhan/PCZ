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
    public class DeviceTypesController : Controller
    {
        PCZDbContext db = new PCZDbContext();
        string error = null;
        public ActionResult Index(int? id, int? act) {
            var vm = new DeviceTypesVM();

            vm.Action = act ?? 0;
            vm.DeviceTypes = db.DeviceType.ToList();

            if (id != null) {
                vm.DeviceType = db.DeviceType.Find(id);
            }

            vm.ErrorMessage = ViewBag.Err;
            return View(vm);
        }

        [HttpPost]
        public ActionResult Create(DeviceTypesVM vm) {

            try {
                db.DeviceType.Add(vm.DeviceType);
                db.SaveChanges();

                ViewBag.Err = "Success";
                return RedirectToAction("Index", new { id = vm.DeviceType.Id, act = 1 });
            }
            catch (Exception e) {
                vm.ErrorMessage = e.Message;
                return View(vm);
            }

        }

        [HttpPost]
        public ActionResult Update(DeviceTypesVM vm) {

            try {
                var dt = db.DeviceType.Find(vm.DeviceType.Id);
                dt.Name = vm.DeviceType.Name;
                db.Entry(dt).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = vm.DeviceType.Id, act = 1 });
            }
            catch (Exception e) {
                vm.ErrorMessage = e.Message;
                return View(vm);
            }
        }
    }
}