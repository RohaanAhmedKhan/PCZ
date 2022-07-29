using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using PCZ.Helpers;
using PCZ.Models;

namespace PCZ.Controllers {

    public class AccountController : Controller {
        PCZDbContext db = new PCZDbContext();
        // GET: Account
        public ActionResult ForgotPassword() {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ForgotPassword(string email) {

                if (db.account.Any(a => a.email == email)) {

                    var ac = db.account.Where(a => a.email == email).Single();
                    ac.resetCode = generateResetCode();
                    db.Entry(ac).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    using (var smtp = new SmtpClient()) {

                        string body = emailFormatter.passwordReset(ac.resetCode, ac.User.FName + " " + ac.User.LName);

                        var Email = emailFormatter.message("Password Reset", body, ac.email);

                        await smtp.SendMailAsync(Email);
                    }

                }
                else {
                    ViewBag.emailMsg = "No account found with that email!";
                    ViewBag.email = email;
                    return View();
                }
            

            return View("resetPassword", new { email = email });

        }
        public ActionResult ResetPassword() {

            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(string email, string code, string password) {

            ViewBag.email = email;
            email = email.ToLower();
            if (db.account.Any(a => a.email.Equals(email))) {
                account ac = db.account.Where(a => a.email == email).Single();
                if (ac.resetCode != null && ac.resetCode.Equals(code)) {
                    ac.password = hash.generateHash(ac.email, password, ac.salt);
                    ac.resetCode = null;
                    db.Entry(ac).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    ViewBag.message = "Password Changed Successfully!";
                }
                else
                    ViewBag.message = "Code Does Not Match";
            }
            else
                ViewBag.emMsg = "No acount found with that email";


            return View();
        }

        private string generateResetCode() {

            string code = System.IO.Path.GetRandomFileName();
            return code.Replace(".", "");

        }
    }
}