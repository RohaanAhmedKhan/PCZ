using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using PCZ.ViewModels;
using PCZ.Models;

namespace PCZ.Controllers {
    public class HomeController : Controller {

        PCZDbContext db = new PCZDbContext();
        public ActionResult Index() {

                if (Request.IsAuthenticated) {
                    var acc = db.account.Where(u => u.email.Equals(HttpContext.User.Identity.Name))
                                                .Single();

                    switch (acc.User.Role.Name) {
                        case "Admin":
                            return Redirect("~/Admin/Dashboard/");
                        case "Vendor":
                            return Redirect("~/Vendor/VendorDashboard/");

                    }
                }
                else
                {
                    ProductViewModel model = new ProductViewModel();
                    model.products = db.Product.ToList();
                List<Vendor> vs = new List<Vendor>();
                    List<account> abc = db.account.Where(z => z.active == true).ToList();
                foreach (Vendor v in db.Vendor)
                {
                    foreach (var a in abc)
                    {
                        if (v.Id== a.id)
                        {
                            vs.Add(v);
                        }
                    }
                    
                }
                model.locations = vs;
                return View(model);
                }
            return View();            
        }
        public ActionResult ProductSlider()
        {
            ProductViewModel model = new ProductViewModel();
            model.products = db.Product.ToList();
            return View(model);
        }
     

    }
}