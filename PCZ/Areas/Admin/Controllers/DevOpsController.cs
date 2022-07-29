using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PCZ.Models;

namespace PCZ.Areas.Admin.Controllers
{
    [CheckAuthorization(Roles = "Admin")]

    public class DevOpsController : Controller
    {
        PCZDbContext db = new PCZDbContext();

        [HttpGet]
        public ActionResult RunQuery() {
            return View();
        }

      
        [HttpPost]
        public ActionResult RunQuery(string command) {
            string output = "Success";
                try {
                    db.Database.ExecuteSqlCommand(command);
                    StringWriter sw = new StringWriter();
                    db.Database.Log = sw.Write;
                    sw.Write(output);
                    ViewBag.output = output;
                    db.SaveChanges();
                }
                catch (Exception e) {
                    ViewBag.output = e.Message;
                }

            return View();
        }

    }
}