using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PCZ.Models;
using PCZ.ViewModels;
using PCZ.Hubs;
using System.Globalization;

namespace PCZ.Areas.Vendor.Controllers
{
    [CheckAuthorization (Roles = "Vendor")]
    
    public class RepairJobsController : MasterController {

        PCZDbContext db = new PCZDbContext();

        public ActionResult Book() {
            var vm = new RepairsVM();
            var a = db.DeviceType.Min(d => d.Id);
            vm.CategoryList = GetCategoriesSelectList(null);
            vm.paidList = new SelectList(new List<SelectListItem>
                    {
                        new SelectListItem { Selected = false, Text = "UnPaid", Value = "UnPaid"},
                         new SelectListItem { Selected = false, Text = "Paid", Value = "Paid"}
                    }, "Value", "Text");
           
            vm.DeviceList = GetDevicesSelectList(a, null);
            vm.IssueList = GetIssueSelectList(Convert.ToInt32(vm.DeviceList.First().Value), null);
            //vm.IssueList = new SelectList(db.Issue.ToList(), "Id", "Description");
            return View(vm);
        }

        [HttpPost]
        public ActionResult Book(RepairsVM vm, HttpPostedFileBase Image) {

            try
            {
                int tp = 0;
                vm.Job.VendorID = CurrentUserID;
            
            var max = db.Job.Where(j => j.SubmitDate.Year == DateTime.Now.Year).Count() + 1;
                var c = db.Vendor.Find(vm.Job.VendorID).PersonalCode + "-" + DateTime.Now.Year.ToString().Substring(2) + "" + max.ToString("00000");

                if(vm.IssuesAr != null && vm.IssuesAr.Count() > 0) {
                    foreach(var item in vm.IssuesAr) {
                        var issue = db.Issue.Find(item);
                        vm.Job.JobIssues.Add(new JobIssues { IssueId = item, Cost = issue.TradePrice });
                        vm.Job.TotalCost += issue.TradePrice;
                        tp += issue.TradePrice;
                    }
                }

                vm.Job.JobNo = c;
                vm.Job.SubmitDate = DateTime.Now;
                vm.Job.Paid = false;
                vm.Job.Status = "Pending";
                vm.Job.TotalCost += vm.Job.OtherCharges;
              
                if (vm.paid == "Paid")
                {
                    vm.Job.CustomerPaid = true;

                }
                if (vm.paid == "UnPaid")
                {
                    vm.Job.CustomerPaid = false;

                }

                if (Image != null) {
                    vm.Job.ImgUrl = saveImage(Image, "JobImg-" + vm.Job.JobNo);
                }
                db.Job.Add(vm.Job);
                db.SaveChanges();
                db.Entry(vm.Job).State = EntityState.Modified;
                db.SaveChanges();
                var jb = db.Job.Where(x => x.JobNo == c).FirstOrDefault();
                var vendor = db.Vendor.Where(x => x.Id == CurrentUserID).SingleOrDefault();
                if(vendor.Balance == null)
                {
                    vendor.Balance = 0.0D;
                }
                double bal = (double)vendor.Balance;
                vendor.Balance =  bal + Convert.ToDouble(tp) + Convert.ToDouble(vm.Job.OtherCharges);
                db.Entry(vendor).State = EntityState.Modified;
                db.SaveChanges();
                //Payment p = new Payment();
                //p.JobID = jb.Id;
                //p.Paid = false;
                //p.Payment_Amount = tp + vm.Job.OtherCharges;
                //p.Remaining_Amount = 0;
                //p.Recieved_Amount = 0;
                //    p.VendorID = CurrentUserID;
                //    p.JobNo = c;
                //db.Payment.Add(p);
                //db.SaveChanges();

                var a = db.DeviceType.Min(d => d.Id);
                vm.CategoryList = GetCategoriesSelectList(null);
                vm.DeviceList = GetDevicesSelectList(a, null);
                vm.IssueList = new SelectList(db.Issue.ToList(), "Id", "Description");

                vm.SuccessMessage = "Repair Booked Successfully!\n" +
                                    "Invoice No: " + vm.Job.JobNo +
                                    "\nTotal Cost: Rs. " + vm.Job.TotalCost.ToString("N0") +"/";
                //vm.Job = null;

                var notification = new Notification();
            
                notification.Description = "Job No:" + vm.Job.JobNo 
                    + " created by vendor: " + vendor.BusinessName;
                notification.Title = "New Job";
                notification.Type = "Job";
                notification.UserId = 14;
                notification.IsRead = false;
                notification.URL = "/Admin/Jobs/Details/" + vm.Job.Id;

                db.Notification.Add(notification);
                db.SaveChanges();

                var msg = "Job No:" + vm.Job.JobNo + "\n Vendor: " + vendor.BusinessName;

                AdminNotificationHub.SendSwalNotification("info", "New Job Alert" ,msg);

              
                return RedirectToAction("Invoice", new { no = vm.Job.JobNo });
            }
            catch (Exception e)
            {

                vm.ErrorMessage = e.Message + "\n" + e.InnerException ?? e.StackTrace;
                vm.CategoryList = GetCategoriesSelectList(vm.Cat);
                vm.DeviceList = GetDevicesSelectList(vm.Cat, null);
                vm.IssueList = GetIssueSelectList(vm.Job.DeviceId, null);
                return View(vm);

            }

        }

        [HttpGet]
        public ActionResult Pending() {
            var vm = new RepairsVM();
            vm.Jobs = db.Job.Where(j => j.VendorID == CurrentUserID && j.Status.Equals("Pending") ).Include(j => j.JobIssues); 
            return View(vm);
        }

        [HttpGet]
        public ActionResult Cancelled() {
            var vm = new RepairsVM();
            vm.Jobs = db.Job.Where(j => j.VendorID == CurrentUserID && j.Status.Equals("Cancelled")).Include(j => j.JobIssues); 
            return View(vm);
        }

        [HttpGet]
        public ActionResult History() {
            var vm = new RepairsVM();
            vm.Jobs = db.Job.Where(j => j.VendorID == CurrentUserID && j.Status.Equals("Completed") && j.CustomerDelivered == true).Include(j => j.JobIssues);
            return View(vm);
        }

        [HttpGet]
        public ActionResult PendingDispatch()
        {
            var vm = new RepairsVM();
            vm.Jobs = db.Job.Where(j => j.VendorID == CurrentUserID && j.Status.Equals("Completed") && j.CustomerDelivered != true).Include(j => j.JobIssues);
            return View(vm);
        }

        [HttpGet]
        public ActionResult Invoice(string no) {

            var Job = db.Job.Where(j => j.JobNo.Equals(no.ToUpper())).Include( j => j.JobIssues).SingleOrDefault();
            ViewBag.InvoiceNo = db.Job.Where(j => j.VendorID == CurrentUserID && j.Status != "Cancelled").Select(j => j.Id).Count() + 1;
            return View(Job);
        }

        [HttpGet]
        public ActionResult InvoicePartial(string no)
        {
            var Job = db.Job.Where(j => j.JobNo.Equals(no.ToUpper())).Include(j => j.JobIssues).SingleOrDefault();
           
            return PartialView("_InvoicePartial", Job);
        }


        [HttpGet]
        public ActionResult Payment() {
            HistoryViewModal modal = new HistoryViewModal();
            int VID = CurrentUserID;
            modal.jobs = db.Job.Where(x=> x.VendorID == VID  && x.Status != "Cancelled").ToList();
            var jobs = db.Job.Where(x => x.VendorID == VID);

            var actualBalance = db.Job.Where(x => (x.Status == "Completed" || x.Status == "Pending") && x.VendorID == VID).Select(z => (double?)z.TotalCost).Sum() ?? 0;
            var remainingBalance = db.Vendor.Where(x => x.Id == VID).Select(x => x.Balance).FirstOrDefault();
            var paidBalance = Convert.ToDouble(actualBalance) - Convert.ToDouble(remainingBalance);


            modal.paidBalance = paidBalance;
            modal.remainingBalance = remainingBalance;  
            modal.actualBalance = actualBalance;
            modal.payments = db.PaymentHistory.Where(x => x.VendorID == VID).ToList();
            modal.VendorName = db.Vendor.Where(x => x.Id == VID).Select(z => z.BusinessName).FirstOrDefault();
            return View(modal);
        }
     
        [HttpPost]
        public ActionResult Cancel(string invNo) {
            var vm = new RepairsVM();
            try
            {
              
                if (db.Job.Any(j => j.JobNo.Equals(invNo) && j.VendorID == CurrentUserID)) {

                    var job = db.Job.Where(j => j.JobNo.Equals(invNo)).SingleOrDefault();
                    job.Status = "Cancelled";
                    db.Entry(job).State = EntityState.Modified;
                    db.SaveChanges();

                    vm.SuccessMessage = "Invoice Cancelation Successfull!";
                    vm.Job = job;

                    var vendor = db.Vendor.Where(x => x.Id == CurrentUserID).SingleOrDefault();
                    double bal = (double)vendor.Balance;
                    vendor.Balance = bal - Convert.ToDouble(job.TotalCost);
                    db.Entry(vendor).State = EntityState.Modified;
                    db.SaveChanges();

                    var notification = new Notification();
                    notification.Description = "Job No:" + job.JobNo
                        + " cancelled by vendor: " + vm.Job.Vendor.BusinessName;
                    notification.Title = "Job Cancelled";
                    notification.Type = "Job";
                    notification.UserId = 14;
                    notification.IsRead = false;
                    notification.URL = "/Admin/Jobs/Details/" + vm.Job.Id;

                    db.Notification.Add(notification);
                    db.SaveChanges();

                    var msg = "Vendor " + vm.Job.Vendor.PersonalCode + " cancelled Invoice# " + vm.Job.JobNo;
                    AdminNotificationHub.SendSwalNotification("error", "Job Cancellation Alert", msg);
                }
                else
                    vm.ErrorMessage = "No Invoice Found Wih Number: " + invNo;
            }
            catch (Exception e)
            {
                vm.ErrorMessage = e.Message;
            }
                return View(vm);
        }

        [HttpGet]
        public ActionResult Edit(int Id) {
            var vm = new RepairsVM();
            vm.Job = db.Job.Find(Id);
            int b = vm.Job.Device.Brand.DeviceType.Id;
            var a = db.DeviceType.Min(d => b);
            vm.CategoryList = GetCategoriesSelectList(b);
            vm.DeviceList = GetDevicesSelectList(a, vm.Job.DeviceId);
            vm.IssueList = GetIssueSelectList(vm.Job.DeviceId, vm.Job.JobIssues.ToList());
            vm.paidList = new SelectList(new List<SelectListItem>
                    {
                        new SelectListItem { Selected = false, Text = "Paid", Value = "Paid"},
                        new SelectListItem { Selected = false, Text = "UnPaid", Value = "UnPaid"},
                    }, "Value", "Text");
            if (vm.Job.CustomerPaid== true)
            {
                ViewBag.CustomerPaid = "Paid";
            }
            if (vm.Job.CustomerPaid == false)
            {
                ViewBag.CustomerPaid = "UnPaid";
            }
            vm.Success = !(vm.Job == null);
            return View(vm);
        }

        [HttpPost]
        public ActionResult Edit(RepairsVM vm) {
            int tp = 0;
            var job = db.Job.Find(vm.Job.Id);
            var vendor = db.Vendor.Where(x => x.Id == CurrentUserID).SingleOrDefault();
            if (vendor.Balance == null)
            {
                vendor.Balance = 0.0D;
            }
            double bal = (double)vendor.Balance;
            vendor.Balance = bal - Convert.ToDouble(job.TotalCost);

            foreach (var issue in job.JobIssues.ToList()) {
                if (!vm.IssuesAr.Any(i => i == issue.Id)) {
                    db.JobIssues.Remove(db.JobIssues.Find(issue.Id));
                }
            }

            job.CustomerEmail = vm.Job.CustomerEmail;
            job.CustomerName = vm.Job.CustomerName;
            job.CustomerPhone = vm.Job.CustomerPhone;
            job.OtherCharges = vm.Job.OtherCharges;
            job.OtherIssue = vm.Job.OtherIssue;
            job.ReturnDate = vm.Job.ReturnDate;
           job.DeviceId = vm.Job.DeviceId;
            if (vm.paid == "Paid")
            {
                job.CustomerPaid = true;

            }
            if (vm.paid == "UnPaid")
            {
                job.CustomerPaid = false;

            }

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
            vendor.Balance = bal1 + tp + vm.Job.OtherCharges;
            db.Entry(vendor).State = EntityState.Modified;
            db.SaveChanges();

            var notification = new Notification();
            notification.Description = "Job No:" + job.JobNo
                + " amended by vendor: " + job.Vendor.BusinessName;
            notification.Title = "Job Edited";
            notification.Type = "Job";
            notification.UserId = 14;
            notification.IsRead = false;
            notification.URL = "/Admin/Jobs/Details/" + job.Id;

            db.Notification.Add(notification);
            db.SaveChanges();

            var msg = "Vendor " + job.Vendor.PersonalCode + " Amended Invoice# " + job.JobNo;
            AdminNotificationHub.SendSwalNotification("Info", "Job Amend Alert", msg);

            return RedirectToAction("Invoice", new { no = job.JobNo });
        }
        [HttpGet]
        public ActionResult DeliverToCustomer(string JobID)
        {
            DeliverToCustomerViewModal vm = new DeliverToCustomerViewModal
            {
                jobNo = JobID,
                Deliver = false,
                date = DateTime.Now
            };
            return View(vm);
        }
        [HttpPost]
        public ActionResult DeliverToCustomer(DeliverToCustomerViewModal modal)
        {
            var job = db.Job.Where(x => x.JobNo == modal.jobNo).FirstOrDefault();
            job.CustomerDelivered = modal.Deliver;
            
            DateTime input = Convert.ToDateTime(modal.date).Date;
            DateTime now = DateTime.Now;
            DateTime output = new DateTime(input.Year, input.Month, input.Day, now.Hour, now.Minute, now.Second);
            
            job.CustomerDeliveredDate = output;
            db.Entry(job).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("PendingDispatch");
        }
    }
}