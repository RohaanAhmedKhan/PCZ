using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using PCZ.Models;
using PCZ.ViewModels;

namespace PCZ.Areas.Admin.Controllers
{
    [CheckAuthorization (Roles = "Admin")]

    public class MessagesController : Controller
    {
        private PCZDbContext db = new PCZDbContext();

        // GET: Admin/Messages
        public ActionResult Index()
        {
            var list = db.Messages.OrderBy( m =>  m.Id).ThenBy(n=>!n.IsRead).ToList();
            return View(list);
        }

        // GET: Admin/Messages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Messages messages = db.Messages.Find(id);
            MessagesViewModel model = new MessagesViewModel
            {
                Email = messages.Email,
                Id = messages.Id,
                IsRead = messages.IsRead,
                Message = messages.Message,
                Name = messages.Name,
                RecievedTime = messages.RecievedTime
            };
            if (messages == null)
            {
                return HttpNotFound();
            }
            if (!messages.IsRead) {
                messages.IsRead = true;
                db.Entry(messages).State = EntityState.Modified;
                db.SaveChanges();
            }
            return View(model);
        }

        // GET: Admin/Messages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Messages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Email,Message")] Messages messages)
        {
            if (ModelState.IsValid)
            {
                db.Messages.Add(messages);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(messages);
        }
        [HttpPost]

        public ActionResult ReplyToCustomer(MessagesViewModel model)
        {
            string body = $"<h2> http://phonencomputerzone.com/ </h2>" +
                          $"<hr>" +
                          $"{model.ReplyMessage}";

            SendEmail("info@phonencomputerzone.com", model.Email, $"Phone and Computer Zone. Answering your query. {model.Name}", body, model.Name);
            return RedirectToAction("Index");
        }

    public bool SendEmail(string from, string to, string subject, string text, string name)
    {
            var senderEmail = new MailAddress(from, "info@phonencomputerzone.com");
            var receiverEmail = new MailAddress(to, name);
            var password = "5N_6q1il";
            var sub = subject;
            var body = text;
            var smtp = new SmtpClient
            {
                Host = "webmail.phonencomputerzone.com",
                Port = 465,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail.Address, password)
            };
            using (var mess = new MailMessage(senderEmail, receiverEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                //{
                smtp.Send(mess);




            //System.Net.Mail.MailMessage objMM = new System.Net.Mail.MailMessage();
            //objMM.From = new MailAddress(from, "john doe");
            //objMM.To.Add(new MailAddress(to));    //Note: this To a collection
            //objMM.Subject = "Subject1";
            //objMM.Body = "Hello world this is my text";
            //objMM.IsBodyHtml = true;

            //SmtpClient smtp = new SmtpClient("phonencomputerzone.com");
            //smtp.Credentials = new NetworkCredential(from, password);
            //smtp.Send(objMM);


            return true;
    }


    // GET: Admin/Messages/Edit/5
    public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Messages messages = db.Messages.Find(id);
            if (messages == null)
            {
                return HttpNotFound();
            }
            return View(messages);
        }

        // POST: Admin/Messages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Email,Message")] Messages messages)
        {
            if (ModelState.IsValid)
            {
                db.Entry(messages).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(messages);
        }

        // GET: Admin/Messages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Messages messages = db.Messages.Find(id);
            if (messages == null)
            {
                return HttpNotFound();
            }
            return View(messages);
        }

        // POST: Admin/Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Messages messages = db.Messages.Find(id);
            db.Messages.Remove(messages);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
