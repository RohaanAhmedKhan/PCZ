using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using PCZ.Models;
using System.Security.Principal;
using System.Web.Security;

namespace PCZ.Models {
    public class CheckAuthorization : AuthorizeAttribute {
        public override void OnAuthorization(AuthorizationContext filterContext) {
            account user;

            if (!HttpContext.Current.Request.IsAuthenticated) {
                if (filterContext.HttpContext.Request.IsAjaxRequest()) {
                    filterContext.HttpContext.Response.StatusCode = 302;
                    filterContext.HttpContext.Response.End();
                }
                else {
                    filterContext.Result = new RedirectResult(System.Web.Security.FormsAuthentication.LoginUrl + "?returnUrl=" +
                         filterContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.RawUrl));
                }
            }
            else {

                using (PCZDbContext db = new PCZDbContext()) {
                    user = db.account.Where(u => u.email.Equals(filterContext.HttpContext.User.Identity.Name))
                                            .Include(u => u.User).Single();

                    if (!user.active) {
                        FormsAuthentication.SignOut();
                        filterContext.HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
                        System.Web.HttpContext.Current.Session.RemoveAll();
                        filterContext.HttpContext.Response.Redirect("~/", false);
                    }

                    filterContext.HttpContext.Session["User Name"] = user.User.FName;
                    filterContext.HttpContext.Session["Profile Picture"] = user.User.ImgUrl;

                    if (!user.User.Role.Name.Equals(Roles)) {
                        //filterContext.HttpContext.Response.StatusCode = 404;
                        //filterContext.HttpContext.Response.End();

                        filterContext.HttpContext.Response.Redirect("~/", false);
                    }
                }

            }
        }

    }
}