using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PCZ.Models;

namespace PCZ.Areas.Vendor.Controllers
{
    public class ContactAdminController : MasterController
    {

        PCZDbContext db = new PCZDbContext();

        public ActionResult Index(){

            var msgs = db.VendorMessage.Where(m => m.VendorID == CurrentUserID)
                    .OrderBy(m => m.RecieveTime);
            return View(msgs);
        }

        public ActionResult Send(string message) {

            var msg = new VendorMessage();
            msg.Message = message;
            msg.RecieveTime = DateTime.Now;
            msg.IsRead = false;
            msg.Sender = "Vendor";
            msg.VendorID = CurrentUserID;

            db.VendorMessage.Add(msg);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}