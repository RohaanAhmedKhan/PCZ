using PCZ.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PCZ
{
    public class MasterController : Controller {

        public int CurrentUserID {
            get {
                using (PCZDbContext db = new PCZDbContext())
                    return db.account.Where(a => a.email == HttpContext.User.Identity.Name).Single().id;
            }
        }

        public string CurrentUserName {
            get {
                return ((FormsIdentity)(HttpContext.User.Identity)).Ticket.Name;
            }
        }

        internal string saveImage(HttpPostedFileBase image, string name) {

            string filename = name.Replace("-", "") + Path.GetExtension(image.FileName);
            string fileLocation = Path.Combine(Server.MapPath("~/Content/Images/"), filename);

            image.SaveAs(fileLocation);
            return "/Content/Images/" + filename;
        }

        internal void deleteImage(string url) {

            string file = Path.Combine(Server.MapPath(url));
            System.IO.File.Delete(file);

        }


        [HttpPost]
        public JsonResult GetBrands(int cat) {

            SelectList list;
            using (var db1 = new PCZDbContext()) {
                list = new SelectList(db1.Brand.Where(s => s.TypeId == cat).OrderBy(b => b.Name).ToList(), "Id", "Name");
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDevices(int cat) {

            var list = GetDevicesSelectList(cat, null);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetIssues(int cat) {

            var list = GetIssueSelectList(cat, null);
            return Json(list, JsonRequestBehavior.AllowGet);

        }
        internal IEnumerable<SelectListItem> GetDevicesSelectList(int cat, int? select) {
            var dd = new List<SelectListItem>();
            using (var db2 = new PCZDbContext()) {
                foreach (var device in db2.Device.Where(d => d.Brand.TypeId == cat).OrderBy(d => d.Brand.Name)) {
                    var item = new SelectListItem();
                    item.Text = device.Brand.Name + " - " + device.Name;
                    item.Value = device.Id.ToString();
                    item.Selected = (select != null && device.Id == (int)select);
                    dd.Add(item);
                }
            }
            return dd;
        }
        internal IEnumerable<SelectListItem> GetCategoriesSelectList(int? select) {
            var dd = new List<SelectListItem>();
            using (var db2 = new PCZDbContext()) {
                foreach (var device in db2.DeviceType.ToList().OrderBy(d => d.Id)) {
                    var item = new SelectListItem();
                    item.Text = device.Name;
                    item.Value = device.Id.ToString();
                    item.Selected = (select != null && device.Id == (int)select);
                    dd.Add(item);
                }
            }
            return dd;
        }
        internal IEnumerable<SelectListItem> GetIssueSelectList(int device, List<JobIssues> selected) {
            var dd = new List<SelectListItem>();
            using (var db2 = new PCZDbContext()) {
                foreach (var issue in db2.Issue.Where( i => i.DeviceId == device).OrderBy(d => d.Id)) {
                    var item = new SelectListItem();
                    item.Text = issue.Description + " - " + issue.Price.ToString("N0");
                    item.Value = issue.Id.ToString();
                    item.Selected = (selected != null && selected.Any( s => s.IssueId == issue.Id));
                    dd.Add(item);
                }
            }
            return dd;
        }
    }
}