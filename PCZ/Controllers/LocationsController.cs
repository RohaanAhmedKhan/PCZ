using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PCZ.Models;

namespace PCZ.Controllers
{
    public class LocationsController : Controller
    {
        private PCZDbContext db = new PCZDbContext();

        public ActionResult Index()
        {
            var vendor = db.Vendor.Include(v => v.User);
            return View(vendor.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
