using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PCZ.Models;
using PCZ.ViewModels;

namespace PCZ.Areas.Admin.Controllers {

    [CheckAuthorization(Roles = "Admin")]
    public class PricesController : MasterController {

        PCZDbContext db = new PCZDbContext();
        string error = null;
        public ActionResult Index(int? id, int? act) {
            var vm = new IssuesVM();

            vm.Action = act ?? 0;
            vm.Issues = db.Issue.ToList();
            if (id != null) {
                vm.Issue = db.Issue.Find(id);

                vm.CategoryList = GetCategoriesSelectList(vm.Issue.Device.Brand.TypeId);
                vm.DeviceList = GetDevicesSelectList(vm.Issue.Device.Brand.TypeId, vm.Issue.DeviceId);
            }
            else {

                vm.CategoryList = GetCategoriesSelectList(null);
                vm.DeviceList = GetDevicesSelectList(1, null);
            }

            vm.ErrorMessage = ViewBag.Err;
            return View(vm);
        }

        [HttpPost]
        public ActionResult Create(IssuesVM vm) {

            vm.CategoryList = GetCategoriesSelectList(vm.Cat);
            vm.DeviceList = GetDevicesSelectList(vm.Cat, vm.Issue.DeviceId);


            try {
                db.Issue.Add(vm.Issue);
                db.SaveChanges();

                ViewBag.Err = "Success";
                return RedirectToAction("Index");
            }
            catch (Exception e) {
                vm.ErrorMessage = e.Message;
                return View(vm);
            }

        }

        [HttpPost]
        public ActionResult Update(IssuesVM vm) {

            vm.CategoryList = GetCategoriesSelectList(vm.Cat);
            vm.DeviceList = GetDevicesSelectList(vm.Cat, vm.Issue.DeviceId);


            try {
                var dt = db.Issue.Find(vm.Issue.Id);
                dt.DeviceId = vm.Issue.DeviceId;
                dt.DeliveryTime = vm.Issue.DeliveryTime;
                dt.Description = vm.Issue.Description;
                dt.Price = vm.Issue.Price;
                dt.TradePrice = vm.Issue.TradePrice;

                db.Entry(dt).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = vm.Issue.Id, act = 1 });
            }
            catch (Exception e) {
                vm.ErrorMessage = e.Message;
                return View(vm);
            }

        }

    }
}