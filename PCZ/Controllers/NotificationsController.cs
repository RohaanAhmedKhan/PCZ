using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PCZ.Models;

namespace PCZ.Controllers {

    [CheckAuthorization]
    public class NotificationsController : MasterController {

        PCZDbContext db = new PCZDbContext();

        public ActionResult Read(int id) {

            var ntf = db.Notification.Find(id);

            if (ntf != null) {
                ntf.IsRead = true;
                db.Entry(ntf).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                //string uri = "/Jobs/Details?Id=" + ntf.;
                return Redirect(ntf.URL);

            }
            else {
                return Redirect(Request.UrlReferrer.AbsolutePath);
            }
        }

    }
}