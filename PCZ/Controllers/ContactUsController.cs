using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using PCZ.Models;
using System.Data.Entity.Validation;

namespace PCZ.Controllers {
    public class ContactUsController : Controller {

        PCZDbContext db = new PCZDbContext();
        [HttpPost]
        public ActionResult Index(string email, string message, string name)
        {
            var msg = new Messages();
            msg.Name = name;
            msg.Message = message;
            msg.Email = email;
            msg.RecievedTime = DateTime.Now;
            try {
                db.Messages.Add(msg);
                db.SaveChanges();
                return Content("Message Recieved");
            }
            catch (DbEntityValidationException e) {
                return Content("Sorry, there was an error, please send your message again");
            }
        }

    }
}