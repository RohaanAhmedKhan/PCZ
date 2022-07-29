using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PCZ.Helpers;
using PCZ.Models;
using PCZ.ViewModels;

namespace PCZ.Areas.Admin.Controllers
{
    [CheckAuthorization(Roles = "Admin")]
    public class VendorsController : MasterController {
        PCZDbContext db = new PCZDbContext();

        public ActionResult Index(int? id, int? act) {
            var vm = new VendorsVM();
            vm.Action = act ?? 0;
            vm.Vendors = db.Vendor.ToList();

            if (id != null) {
                vm.Vendor = db.Vendor.Find(id);
            }
            return View(vm);
        }

        public ActionResult Create() {
            return View();
        }

        [HttpPost]
        public ActionResult Create(VendorsVM vm, HttpPostedFileBase Image) {

            try {

                vm.Vendor.PersonalCode = "VN" + (db.Vendor.Count() + 1).ToString("000");

                try {
                    vm.Vendor.User.ImgUrl = saveImage(Image, "PFP-" + vm.Vendor.PersonalCode);
                }
                catch (Exception e) {
                    vm.ErrorMessage = e.Message;
                    return View(vm);
                }

                vm.Vendor.User.RoleID = 3;

                db.User.Add(vm.Vendor.User);
                db.SaveChanges();

                var acc = new account();
                acc.salt = hash.getSalt(32);
                acc.email = vm.email;
                acc.password = hash.generateHash(vm.email, vm.pass, acc.salt);
                acc.active = true;
                acc.id = vm.Vendor.User.Id;
                db.account.Add(acc);
                
                db.Vendor.Add(vm.Vendor);
                db.SaveChanges();

                return RedirectToAction("Index", new { id = vm.Vendor.Id });
            }
            catch (Exception e) {
                vm.ErrorMessage = e.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult Update(int id) {
            var vm = new VendorsVM();
            vm.Vendor = db.Vendor.Find(id);
            return View(vm);
        }

        [HttpPost]
        public ActionResult Update(VendorsVM vm, HttpPostedFileBase Image) {

            var old = db.Vendor.AsNoTracking().Where(d => d.Id == vm.Vendor.Id).Include(d => d.User).Single();

            try {

                var u = db.User.Find(old.Id);
                u.FName = vm.Vendor.User.FName;
                u.LName = vm.Vendor.User.LName;
                vm.Vendor.User = null;

                if (Image != null) {
                    deleteImage(old.User.ImgUrl);
                    u.ImgUrl = saveImage(Image, vm.Vendor.Id + "_" + old.User.FName);
                    db.Entry(u).State = EntityState.Modified;
                }


                db.Entry(u).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                db.Entry(vm.Vendor).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                db.SaveChanges();

                return RedirectToAction("Index", new { id = vm.Vendor.Id });
            }
            catch (Exception e) {
                vm.ErrorMessage = e.Message;
                return View(vm);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id) {

            var doc = db.Vendor.Find(id);
            db.Vendor.Remove(doc);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AcountActive(int id, bool status) {

            //var vm = new ViewModel();

            var vn = db.Vendor.Find(id);
            if(vn != null) {
                vn.User.account.active = status;
                db.Entry(vn).State = EntityState.Modified;
                db.SaveChanges(); 
            }
            else {
                
            }
            //return Json(list, JsonRequestBehavior.AllowGet);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Jobs(int VendorId, int? JobId, char t = 'p') {

            var vm = new VendorsVM();
            vm.Vendor = db.Vendor.Find(VendorId);
            //vm.Jobs = db.Job.Where(j => j.VendorID == VendorId && j.Status == "Pending");
            vm.Jobs = db.Job.Where(j => j.VendorID == VendorId && j.Status == "Pending" );
            return View(vm);
        }

        [HttpGet]
        public ActionResult JobHistory(int Id)
        {
            var vm = new RepairsVM();
            vm.Jobs = db.Job.Where(j => j.VendorID == Id);
            vm.Action = 1;
            vm.VendorID = Id;
            return View(vm);
        }


        [HttpGet]
        public ActionResult VendorCancelledInvoice(int Id)
        {
            var vm = new RepairsVM();
            vm.Jobs = db.Job.Where(j => j.VendorID == Id && j.Status == "Cancelled");
            return View(vm);
        }


        [HttpGet]
        public ActionResult VendorPaymentHistory(int Id)
        {
            HistoryViewModal modal = new HistoryViewModal();
            var jobs = db.Job.Where(x => x.VendorID == Id);
            var payments = db.Payment;
            var vendor = db.Vendor.Where(x => x.Id == Id).FirstOrDefault();
            var user = db.User.Where(x => x.Id == Id).FirstOrDefault();
            var V = jobs.AsEnumerable().Join(payments,
                                 j => j.Id,
                                 p => p.JobID,
                                 (j, p) => new { jobs = j, payments = p }).Where(x => x.jobs.VendorID == Id)
                                  .ToList();
            modal.paid = V.Where(x => x.jobs.Paid == true).Count();
            modal.unpaid = V.Where(x => x.jobs.Paid != true).Count();
            modal.pamount = V.Where(x => x.jobs.Paid == true).Sum(x => x.payments.Payment_Amount);
            modal.upamount = V.Where(x => x.jobs.Paid != true).Sum(x => x.payments.Payment_Amount);
           // modal.payments = db.Payment.Where(x => x.VendorID == Id);
            modal.BuisinessName = vendor.BusinessName;
            modal.ImgURl = user.ImgUrl;
            modal.actualBalance = vendor.Balance;
            return View(modal);
        }

    }
}