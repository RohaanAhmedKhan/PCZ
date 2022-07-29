using PCZ.Models;
using PCZ.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PCZ.Areas.Admin.Controllers
{
    public class PaymentController : Controller
    {
        PCZDbContext db = new PCZDbContext();
        // GET: Admin/Payment
        public ActionResult Index(int pid)
        {
            var actualBalance = db.Job.Where(x => (x.Status == "Completed" || x.Status == "Pending") && x.VendorID == pid).Select(z => (double?)z.TotalCost).Sum() ?? 0  ;
            var remainingBalance = db.Vendor.Where(x => x.Id == pid).Select(x => x.Balance).FirstOrDefault();
            var paidBalance = Convert.ToDouble(actualBalance) - Convert.ToDouble(remainingBalance);

            PaymentViewModel model = new PaymentViewModel
            {
                Action = 2,
                RemainingBalance = remainingBalance,
                PaidBalance = paidBalance,
                VendorName = db.Vendor.Where(x => x.Id == pid).Select(z => z.BusinessName).FirstOrDefault(),
                VendorID = pid,
                payments = db.PaymentHistory.Where(x => x.VendorID== pid).ToList(),
                actualBalance = actualBalance
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(PaymentViewModel model)
        {
            TimeSpan ts  = DateTime.Now.TimeOfDay;
            DateTime date = model.DateOfPay.Date + ts;
            PaymentHistory payment = new PaymentHistory
            {
                Amount = model.Amount,
                Description = model.Description,
                VendorID = model.VendorID,
                DateOfPayment = date
            };
            db.PaymentHistory.Add(payment);
            db.SaveChanges();

            var vendor = db.Vendor.Where(x => x.Id == model.VendorID).FirstOrDefault();
            if (vendor.Balance == null)
            {
                vendor.Balance = 0.0D;
            }
            double bal = (double)vendor.Balance;
            vendor.Balance = bal - Convert.ToDouble(model.Amount);
            db.Entry(vendor).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", new { pid = model.VendorID });
        }

        [HttpGet]
        public ActionResult Edit(int prdID)
        {
            PaymentHistory p = db.PaymentHistory.Where(x => x.ID == prdID).FirstOrDefault();
            int vendorId = p.VendorID;
            var actualBalance = db.Job.Where(x => (x.Status == "Completed" || x.Status == "Pending") && x.VendorID == vendorId).Select(z => (double?)z.TotalCost).Sum() ?? 0;
            var remainingBalance = db.Vendor.Where(x => x.Id == vendorId).Select(x => x.Balance).FirstOrDefault();
            var paidBalance = Convert.ToDouble(actualBalance) - Convert.ToDouble(remainingBalance);


            PaymentViewModel model = new PaymentViewModel
            {   Action = 1,
                RemainingBalance = remainingBalance,
                PaidBalance = paidBalance,
                VendorName = db.Vendor.Where(x => x.Id == p.VendorID).Select(z => z.BusinessName).FirstOrDefault(),
                VendorID = p.VendorID,
                payments = db.PaymentHistory.Where(x => x.VendorID == vendorId).ToList() ,
                Description = p.Description,
                oldAmount = p.Amount,
                Amount = p.Amount,
                DateOfPay = p.DateOfPayment,
                ID = p.ID,
                actualBalance = actualBalance
            };
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult Edit(PaymentViewModel model)
        {
            double oldAmount = 0D;
            TimeSpan ts = DateTime.Now.TimeOfDay;
            DateTime date = model.DateOfPay.Date + ts;
            var vendor = db.Vendor.Where(x => x.Id == model.VendorID).FirstOrDefault();

            PaymentHistory payment = db.PaymentHistory.Where(x => x.ID == model.ID).FirstOrDefault();
            if (vendor.Balance == null)
            {
                vendor.Balance = 0.0D;
            }
            double bal = (double)vendor.Balance;

            oldAmount = payment.Amount;
            vendor.Balance = bal + oldAmount;           
            payment.Amount = model.Amount;
            payment.Description = model.Description;
            payment.DateOfPayment = date;
            payment.VendorID = model.VendorID;
            db.Entry(payment).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();


            double bal1 = (double)vendor.Balance;
            vendor.Balance = bal1 -  Convert.ToDouble(model.Amount);
            db.Entry(vendor).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", new { pid = payment.VendorID });
        }

        [HttpPost]
        public ActionResult Delete(int prdID)
        {
            PaymentHistory payment = db.PaymentHistory.Where(x => x.ID == prdID).FirstOrDefault();
            db.PaymentHistory.Remove(payment);
            db.SaveChanges();

            var vendor = db.Vendor.Where(x => x.Id == payment.VendorID).FirstOrDefault();
            if (vendor.Balance == null)
            {
                vendor.Balance = 0.0D;
            }
            double bal = (double)vendor.Balance;
            vendor.Balance = bal + Convert.ToDouble(payment.Amount);
            db.Entry(vendor).State = EntityState.Modified;
            db.SaveChanges();
            return  RedirectToAction("Index", new { pid = payment.VendorID });
        }
    }
}