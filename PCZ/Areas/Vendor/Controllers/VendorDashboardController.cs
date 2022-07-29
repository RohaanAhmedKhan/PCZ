using Newtonsoft.Json;
using PCZ.Models;
using PCZ.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace PCZ.Areas.Vendor.Controllers
{
    public class VendorDashboardController : Controller
    {
        // GET: Vendor/VendorDashboard
        PCZDbContext db = new PCZDbContext();
        public int VID;
        public string VName;
        public ActionResult Index()
        {
            var vm = new VendorDashboardVM();
             VID = GetVendorID();
            VName = GetVendorName(VID);
            ViewBag.VName = VName;

            vm.AdminMessages = db.AdminMessage.Where(x => x.RecipentID == VID).OrderByDescending(x => x.RecieveTime);
            vm.VendorMessages = db.VendorMessage.Where(x => x.VendorID == VID).OrderByDescending(x => x.RecieveTime);
            List<VendorMessage> vms = vm.VendorMessages.ToList();
            List<AdminMessage> am = vm.AdminMessages.ToList();
            List<Messages> msgs = new List<Messages>();

            foreach (var msg in am)
            {
                Messages m = new Messages()
                {
                    Name = "Haroon",
                    Email = "admin",
                    Message = msg.Message,
                    RecievedTime = msg.RecieveTime
                };
                msgs.Add(m);
            }
            foreach (var msg in vms)
            {
                Messages m = new Messages()
                {
                    Name = msg.Sender,
                    Email = "vendor",
                    Message = msg.Message,
                    RecievedTime = msg.RecieveTime
                };
                msgs.Add(m);
            }

            vm.Messages = msgs.OrderBy(x => x.RecievedTime).ToList();
            msgs = msgs.OrderBy(x => x.RecievedTime).ToList();
            vm.jsonMessages = JsonConvert.SerializeObject(msgs);
            vm.Pending = db.Job.Where(x => x.Status == "Pending").Count();
            vm.vend = db.Vendor.Count();
            ViewBag.VendorImg = db.User.Where(x => x.Id == VID && x.RoleID == 3).Select(x => x.ImgUrl).FirstOrDefault();
            ViewBag.Pending = db.Job.Where(x => x.VendorID == VID && x.Status == "Pending").Count();
            ViewBag.Cancelled = db.Job.Where(x => x.VendorID == VID && x.Status == "Cancelled").Count();
            ViewBag.Unpaid = db.Job.Where(x => x.VendorID == VID && x.CustomerPaid== false && x.Status == "Completed").Count();
            ViewBag.Invoices= db.Job.Where(x => x.VendorID == VID && x.Status == "Completed" && x.CustomerPaid == true).Count();
            return View(vm);
        }
        public int GetVendorID()
        {
            string email = User.Identity.Name.ToString();
            
            int VID = 0;
            List<User> users = db.User.ToList();
            List<account> account = db.account.ToList();
             var V = users.AsEnumerable().Join(account,
                                   U => U.Id,
                                   A => A.id,
                                   (U, A) => new { users = U, account = A })
                                    .Where(x => x.account.email == email && x.users.RoleID == 3)
                                    .Select(y => y.users.Id).ToList();
            VID = V.FirstOrDefault();
            return VID;
        }

        public string GetVendorName( int vid)
        {
            VName = db.Vendor.Where(x => x.Id == vid).Select(x => x.BusinessName).ToList().FirstOrDefault();
            ViewBag.VName = VName;
            return VName;
        }

        [HttpPost]
        public JsonResult SendMessage(string msg)
        {
            if (!String.IsNullOrEmpty(msg))
            {
                int ID = GetVendorID();
                string VName = GetVendorName(ID);
                VendorMessage vm = new VendorMessage
                {
                    IsRead = false,
                    Message = msg,
                    RecieveTime = DateTime.Now,
                    VendorID = ID,
                    Sender = VName
                };
                db.VendorMessage.Add(vm);
                db.SaveChanges();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
          
        }

        [HttpGet]
        public PartialViewResult GetMessages()
        {
            var vm = new VendorDashboardVM();
            VID = GetVendorID();
            vm.AdminMessages = db.AdminMessage.Where(x => x.RecipentID == VID).OrderByDescending(x => x.RecieveTime);
            vm.VendorMessages = db.VendorMessage.Where(x => x.VendorID == VID).OrderByDescending(x => x.RecieveTime);
            List<VendorMessage> vms = vm.VendorMessages.ToList();
            List<AdminMessage> am = vm.AdminMessages.ToList();
            List<Messages> msgs = new List<Messages>();
            ViewBag.VendorImg = db.User.Where(x => x.Id == VID).Select(x => x.ImgUrl).FirstOrDefault();

            foreach (var msg in am)
            {
                Messages m = new Messages()
                {
                    Name = "Haroon",
                    Email = "admin",
                    Message = msg.Message,
                    RecievedTime = msg.RecieveTime
                };
                msgs.Add(m);
            }
            foreach (var msg in vms)
            {
                Messages m = new Messages()
                {
                    Name = msg.Sender,
                    Email = "vendor",
                    Message = msg.Message,
                    RecievedTime = msg.RecieveTime
                };
                msgs.Add(m);
            }

            vm.Messages = msgs.OrderBy(x => x.RecievedTime).ToList();

            return PartialView(@"~/Areas/Vendor/Views/RepairJobs/_ChatBox.cshtml", vm);
           //return RenderPartialToString("~/Areas/Vendor/Views/RepairJobs/_ChatBox.cshtml", vm);
        }

        public static string RenderPartialToString(string controlName, object viewData)
        {
            ViewPage viewPage = new ViewPage() { ViewContext = new ViewContext() };

            viewPage.ViewData = new ViewDataDictionary(viewData);
            viewPage.Controls.Add(viewPage.LoadControl(controlName));

            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                using (HtmlTextWriter tw = new HtmlTextWriter(sw))
                {
                    viewPage.RenderControl(tw);
                }
            }

            return sb.ToString();
        }
        //[HttpGet]
        //[Route("company-info/companyinfogetapidata")]
        //[AllowAnonymous]
        //public PartialViewResult GetMessagess()
        //{

        //    HttpClient client = new HttpClient();

        //    client.BaseAddress = new Uri("URL_BASE");
        //    client.DefaultRequestHeaders.Accept.Clear();
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //    var request = client.GetAsync("URL_PATH");
        //    var json = request.Result.Content.ReadAsStringAsync().Result;

        //    JObject o = JObject.Parse(json);

        //    return PartialView(@"_CompanyFinderResults.cshtml", Model);

        //}
    }
}