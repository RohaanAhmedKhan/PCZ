using PCZ.Models;
using PCZ.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PCZ.Areas.Admin.Controllers
{

    public class DashboardController : Controller
    {
        PCZDbContext db = new PCZDbContext();
        string VName;
        public ActionResult Index()
        {

            var vm = new AdminDashboardVM();

            vm.Notifications = db.Notification.Where(x => !x.IsRead && x.UserId == 14);

            using (var ctx = new PCZDbContext())
            {
                vm.vendors = ctx.VendorMessage
                                    .SqlQuery("select  CAST( (select count(*) from VendorMessage where isread=0 and VendorID= V.VendorID) as varchar) Message,VendorID  ,RecieveTime ,IsRead ,Sender, Id from VendorMessage V  where v.RecieveTime = (Select max(RecieveTime) from VendorMessage where VendorID = v.VendorID) order by RecieveTime Desc  ")
                                    .ToList<VendorMessage>();
            }
            vm.Pending = db.Job.Where(x => x.Status == "Pending").Count();
            vm.vend = db.Job.Where(x => x.Status == "Completed").Count();
            List<Messages> msgs = new List<Messages>();
            vm.Messages = msgs;
            return View(vm);
        }

        public ActionResult LoadChat(int vid)
        {

            Session["VendorID"] = vid;

            var vm = new AdminDashboardVM();
            vm.Notifications = db.Notification.Where(x => !x.IsRead && x.UserId == 14);
            vm.VendorID = vid;
            vm.Pending = db.Job.Where(x => x.Status == "Pending").Count();
            vm.vend = db.Vendor.Count();
            VName = GetVendorName(vid);
            ViewBag.VName = VName;
            List<AdminMessage> am = db.AdminMessage.Where(x => x.RecipentID == vid).OrderByDescending(x => x.RecieveTime).ToList();
            List<VendorMessage> vms = db.VendorMessage.Where(x => x.VendorID == vid).OrderByDescending(x => x.RecieveTime).ToList();
            List<Messages> msgs = new List<Messages>();
            UpdateMessageStatus(vms);
            using (var ctx = new PCZDbContext())
            {
                //select * from VendorMessage V  where v.RecieveTime = (Select max(RecieveTime) from VendorMessage where VendorID = v.VendorID) order by RecieveTime Desc
                vm.vendors = ctx.VendorMessage
                                    .SqlQuery("select  CAST( (select count(*) from VendorMessage where isread=0 and VendorID= V.VendorID) as varchar) Message,VendorID  ,RecieveTime ,IsRead ,Sender, Id from VendorMessage V  where v.RecieveTime = (Select max(RecieveTime) from VendorMessage where VendorID = v.VendorID) order by RecieveTime Desc  ")
                                    .ToList<VendorMessage>();
            }
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
            ViewBag.VendorImg = db.User.Where(x => x.Id == vid && x.RoleID == 3).Select(x => x.ImgUrl).FirstOrDefault();
            return View("index", vm);
        }
        [HttpGet]
        public PartialViewResult VendorChats()
        {
            var vm = new AdminDashboardVM();
            using (var ctx = new PCZDbContext())
            {
                vm.vendors = ctx.VendorMessage
                                    .SqlQuery("select  CAST( (select count(*) from VendorMessage where isread=0 and VendorID= V.VendorID) as varchar) Message,VendorID  ,RecieveTime ,IsRead ,Sender, Id from VendorMessage V  where v.RecieveTime = (Select max(RecieveTime) from VendorMessage where VendorID = v.VendorID) order by RecieveTime Desc ")
                                    .ToList<VendorMessage>();
            }

            return PartialView(@"~/Areas/Admin/Views/Dashboard/_VendorChats.cshtml", vm);

        }


        [HttpGet]
        public PartialViewResult VendorChatDetails()
        {
            int vid = Convert.ToInt32(Session?["VendorID"]);
            if (vid != 0)
            {
                var vm = new AdminDashboardVM();
                vm.VendorID = vid;
                List<AdminMessage> am = db.AdminMessage.Where(x => x.RecipentID == vid).OrderByDescending(x => x.RecieveTime).ToList();
                List<VendorMessage> vms = db.VendorMessage.Where(x => x.VendorID == vid).OrderByDescending(x => x.RecieveTime).ToList();
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
                ViewBag.VendorImg = db.User.Where(x => x.Id == vid && x.RoleID == 3).Select(x => x.ImgUrl).FirstOrDefault();

                return PartialView(@"~/Areas/Admin/Views/Dashboard/_VendorChatDetails.cshtml", vm);

            }
            return PartialView(@"~/Areas/Vendor/Views/RepairJobs/_ChatBox.cshtml", new accountsVM { });
        }

        public void UpdateMessageStatus(List<VendorMessage> msgs)
        {
            foreach (var msg in msgs)
            {
                if (msg.IsRead == false)
                {
                    var vm = db.VendorMessage.Find(msg.Id);
                    vm.IsRead = true;
                    db.Entry(vm).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
        public string GetVendorName(int vid)
        {
            VName = db.Vendor.Where(x => x.Id == vid).Select(x => x.BusinessName).ToList().FirstOrDefault();
            ViewBag.VName = VName;
            return VName;
        }

        public int GetAdminID()
        {
            string email = User.Identity.Name.ToString();

            int AID = 0;
            List<User> users = db.User.ToList();
            List<account> account = db.account.ToList();
            var V = users.AsEnumerable().Join(account,
                                  U => U.Id,
                                  A => A.id,
                                  (U, A) => new { users = U, account = A })
                                   .Where(x => x.account.email == email && x.users.RoleID == 2)
                                   .Select(y => y.users.Id).ToList();
            AID = V.FirstOrDefault();
            return AID;
        }

        [HttpPost]
        public JsonResult SendMessage(string msg, int vid)
        {
            if (!String.IsNullOrEmpty(msg) && !String.IsNullOrEmpty(vid.ToString()))
            {
                string VName = GetVendorName(vid);
                int AID = GetAdminID();
                AdminMessage am = new AdminMessage
                {
                    IsRead = false,
                    Message = msg,
                    RecieveTime = DateTime.Now,
                    RecipentID = vid,
                    AdminID = AID
                };
                db.AdminMessage.Add(am);
                db.SaveChanges();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}

