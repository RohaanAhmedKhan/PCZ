using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PCZ.Models;
using PCZ.ViewModels;

namespace PCZ.Areas.Admin.Controllers
{
    [CheckAuthorization(Roles = "Admin")]
    public class JobsController : MasterController
    {

        PCZDbContext db = new PCZDbContext();


        public ActionResult Index(string type, int? year) {
            var vm = new RepairsVM();

            vm.Jobs = db.Job.Where(j => j.SubmitDate.Year == year );
            vm.preUrl = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
            return View(vm);
        }

        [HttpGet]
        public ActionResult VendorJobs(int VendorId, int? JobId, char t = 'p') {

            var vm = new VendorsVM();
            vm.Vendor = db.Vendor.Find(VendorId);
            //List<Job> jobs = db.Job.Where(j => j.VendorID == VendorId).ToList();
            vm.payments = db.Payment.ToList();
            //var V = jobs.AsEnumerable().Join(payments,
            //                     j => j.Id,
            //                     p => p.JobID,
            //                     (j, p) => new { jobs = j, payments= p}).Where(x=> x.jobs.VendorID == VendorId)
            //                      .ToList();
            vm.Jobs = db.Job.Where(j => j.VendorID == VendorId && j.Status != "Completed" && j.Paid == false);
            return View(vm); 
        }
        
        [HttpGet]
        public ActionResult Details(int id) {
            var vm = new RepairsVM();
            vm.Job = db.Job.Find(id);
            vm.Success = !(vm.Job == null);
            return View(vm);
        }

        [HttpGet]
        public ActionResult Edit(int Id) {
            var vm = new RepairsVM();
            vm.Job = db.Job.Find(Id);
            vm.payment = db.Payment.Where(x => x.JobID == Id).FirstOrDefault();
            vm.statusList = new SelectList(new List<SelectListItem>
                    {
                        new SelectListItem { Selected = false, Text = "Completed", Value = "Completed"},
                        new SelectListItem { Selected = false, Text = "Pending", Value = "Pending"},
                        new SelectListItem { Selected = false, Text = "Cancelled", Value = "Cancelled"},
                         //new SelectListItem { Selected = false, Text = "Refunded", Value = "Refunded"},
                    }, "Value", "Text");
            vm.paidList = new SelectList(new List<SelectListItem>
                    {
                        new SelectListItem { Selected = false, Text = "Paid", Value = "Paid"},
                        new SelectListItem { Selected = false, Text = "UnPaid", Value = "UnPaid"},
                    }, "Value", "Text");
            var a = db.DeviceType.Min(d => d.Id);
            vm.CategoryList = GetCategoriesSelectList(vm.Job.Device.Brand.TypeId);
            vm.DeviceList = GetDevicesSelectList(a, vm.Job.DeviceId);
            vm.IssueList = GetIssueSelectList(vm.Job.DeviceId, vm.Job.JobIssues.ToList());
            ViewBag.Status = vm.Job.Status;
           if(vm.Job.Paid == true)
            {
                ViewBag.Paid = "Paid";
            }
            if (vm.Job.Paid == false)
            {
                ViewBag.Paid = "UnPaid";
            }
            vm.Success = !(vm.Job == null);
            return View(vm);
        }

        [HttpPost]
        public ActionResult Edit(RepairsVM vm) {

            var job = db.Job.Find(vm.Job.Id);
            var vendor = db.Vendor.Where(x => x.Id == job.VendorID).SingleOrDefault();
            if (vendor.Balance == null)
            {
                vendor.Balance = 0.0D;
            }
            double bal = (double)vendor.Balance;
            vendor.Balance = bal - Convert.ToDouble(job.TotalCost);

            //if(vm.Amount != 0)
            //{
            //    var p = db.Payment.Where(q => q.JobID == job.Id).FirstOrDefault();
            //    if(p.Payment_Amount != p.Recieved_Amount || p.Paid == false)
            //    {
            //        p.Recieved_Amount = p.Recieved_Amount + vm.Amount;
            //        p.Remaining_Amount = p.Payment_Amount - p.Recieved_Amount;
            //        p.DateRecieved = DateTime.Now;
            //    }
            //    if(p.Recieved_Amount == p.Payment_Amount)
            //    {
            //        p.Paid = true;
            //        job.Paid = true;

            //    }
            //    if (vm.Amount > p.Payment_Amount || p.Recieved_Amount > p.Payment_Amount)
            //    {
            //    }
            //    db.Entry(p).State = EntityState.Modified;
            //    db.SaveChanges();
            //}

            foreach (var issue in job.JobIssues.ToList())
            {
                if (!vm.IssuesAr.Any(i => i == issue.Id))
                {
                    db.JobIssues.Remove(db.JobIssues.Find(issue.Id));
                }
            }

            job.CustomerEmail = vm.Job.CustomerEmail;
            job.CustomerName = vm.Job.CustomerName;
            job.CustomerPhone = vm.Job.CustomerPhone;
            job.OtherCharges = vm.Job.OtherCharges;
            job.OtherIssue = vm.Job.OtherIssue;
            job.ReturnDate = vm.Job.ReturnDate;
            //if(vm.paid == "Paid")
            //{
            //    job.Paid = true;

            //}
            //if (vm.paid == "UnPaid")
            //{
            //    job.Paid = false;

            //}
            if (vm.status != null)
            {
                job.Status = vm.status;
            }
            int tp = 0;

            if (vm.IssuesAr != null && vm.IssuesAr.Count() > 0) {
                job.TotalCost = 0;
                foreach (var item in vm.IssuesAr) {
                    var issue = db.Issue.Find(item);
                    job.JobIssues.Add(new JobIssues { IssueId = item, Cost = issue.TradePrice });
                    job.TotalCost += issue.TradePrice;
                    tp += issue.TradePrice;
                }
            }
            job.TotalCost += vm.Job.OtherCharges;
            db.Entry(job).State = EntityState.Modified;
            db.SaveChanges();
            double bal1 = (double)vendor.Balance;
            if (vm.status != "Cancelled" || vm.status != "Refunded")
            {
                vendor.Balance = bal1 + tp + vm.Job.OtherCharges;
            }
           
            db.Entry(vendor).State = EntityState.Modified;
            db.SaveChanges();
            //return RedirectToAction("Details", new { id = vm.Job.Id });
            //return Redirect(Request.UrlReferrer.ToString());            //return RedirectToAction("Vendorjobs", new { VendorId = job.VendorID});
            string dtl = "/Admin/Vendors/Jobs/?VendorId=" + job.VendorID;
            return Redirect(dtl);         

        }
    }
}