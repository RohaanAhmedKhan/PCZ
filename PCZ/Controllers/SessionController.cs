using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.SessionState;
using System.Web.Mvc;
using System.Web.Security;
using System.Text;
using PCZ.ViewModels;
using PCZ.Models;
using System.Security.Cryptography;
using PCZ.Helpers;

namespace PCZ.Controllers {
    public class sessionController : Controller {

        private static PCZDbContext db;
        private static int saltLength = 32;

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult login(string returnUrl) {

            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            accountsVM vm = new accountsVM();
            vm.emailError1 = "";

            try {
                vm.returnUrl = returnUrl != null ? returnUrl : Request.UrlReferrer.AbsolutePath != null ?
                                                                        Request.UrlReferrer.AbsolutePath : "~/home";
                return View(vm);
            }
            catch (Exception a) {
                vm.ErrorMessage = a.Message;
                return View(vm);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult logout() {
            try {
                FormsAuthentication.SignOut();
                HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
                Session.Clear();
                System.Web.HttpContext.Current.Session.RemoveAll();

                return Redirect("~/");
            }
            catch {
                throw;
            }
        }

        private ActionResult redirectToLocal(string returnULR = "") {
            try {

                if (!String.IsNullOrWhiteSpace(returnULR) && Url.IsLocalUrl(returnULR))
                    return Redirect(returnULR);

                TempData["ErrorMSG"] = "Signed In!";

                return View();
            }
            catch {
                throw;
            }
        }

        private void rememberSignIn(string userName, bool isPersistant = false) {
            {
                //Clear previous cookies
                FormsAuthentication.SignOut();
                //Write new cookie
                FormsAuthentication.SetAuthCookie(userName, isPersistant);

            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult login(accountsVM vm) {
            string savedHash = string.Empty;
            byte[] savedSalt = new byte[saltLength];
            if (string.IsNullOrEmpty(vm.returnUrl))
                vm.returnUrl = "~/Home";

            try {

                using (db = new PCZDbContext()) {

                    //check VM Validity
                    if (!ModelState.IsValid)
                        return View(vm);

                    account userAcc = new account();
                    if (!db.account.Any(u => u.email == vm.uname)) {
                        vm.emailError1 = "This email is not registered";
                        return View(vm);
                    }
                    else {
                        
                        userAcc = db.account.Where(u => u.email == vm.uname).Single();
                        Session["userEmail"] = vm.uname;
                        savedHash = userAcc.password;
                        savedSalt = userAcc.salt;

                        if (!userAcc.active){
                            vm.emailError1 = "Acount Disabled, Contact the admin!";
                            return View(vm);
                        }


                        if (hash.compareHash(vm.uname, vm.pass, savedSalt, savedHash)) {

                            if (!userAcc.active) {
                                TempData["ErrorMSG"] = "Account Suspended! Can not log in.";
                                return View(vm);
                            }

                            //set Remember me

                            var authTicket = new FormsAuthenticationTicket(
                                                      1,
                                                      userAcc.email,
                                                      DateTime.Now,
                                                      DateTime.Now.AddDays(1),  // expiry
                                                      vm.isRemember,  //true to remember
                                                      userAcc.User.FName + " " + userAcc.User.LName + "|" + userAcc.User.ImgUrl, //roles 
                                                      "/"
                                                    );

                            //encrypt the ticket and add it to a cookie
                            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
                            cookie.Expires = DateTime.Now.AddDays(1);
                            Response.Cookies.Add(cookie);

                            //rememberSignIn(vm.uname, vm.isRemember);

                            //set session id
                            Session["UserID"] = userAcc.id;

                            return redirectToLocal(vm.returnUrl);
                        }
                        else {

                            vm.pwdError1 = "Wrong Password, LOGIN FAILED!";
                            return View(vm);
                        }
                    }

                }
            }
            catch {
                throw;
            }

        }



    }
}