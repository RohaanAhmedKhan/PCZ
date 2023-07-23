using MailKit.Net.Smtp;
using PCZ.Models;
using PCZ.ViewModels;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PCZ.Areas.Admin.Controllers
{
    [CheckAuthorization(Roles = "Admin")]

    public class MessagesController : Controller
    {

        private PCZDbContext db = new PCZDbContext();

        // GET: Admin/Messages
        public ActionResult Index()
        {
            var list = db.Messages.OrderBy(m => m.Id).ThenBy(n => !n.IsRead).ToList();
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
            if (!messages.IsRead)
            {
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

        public JsonResult ReplyToCustomer(MessagesViewModel model)
        {
            string body = $"{model.ReplyMessage}";

            var res = SendEmail(
                 "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",
                 $"Phone and Computer Zone.Answering your query {model.Name}.",
                 body,
                 new List<string> { model.Email },
                 "info@phonencomputerzone.com"
                 ).Result;
           // var res = SendEmail("info@phonencomputerzone.com", model.Email, $"Phone and Computer Zone. Answering your query. {model.Name}", body, model.Name);
            return Json("Meesage Sent" + res);
        }


        public async Task<bool> SendEmail(string apiKey, string subject,
          string message, List<string> emails, string fromEmail)
        {


            try
            {
                System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;


                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("info@phonencomputerzone.com", "PhonenComputerZone");
                var subject1 = subject;
                var to = new EmailAddress(emails.FirstOrDefault());
                var plainTextContent = message;
                var htmlContent = message;
                var msg = MailHelper.CreateSingleEmail(from, to, subject1, plainTextContent, htmlContent);
                var task = Task.Run(() => client.SendEmailAsync(msg));
                task.Wait();
                var response = task.Result;

                return response.IsSuccessStatusCode;

            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
        public bool SendEmail(string from, string to, string subject, string text, string name)
        {
            //System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            var senderEmail = new MailAddress(from, "info@phonencomputerzone.com");
            var receiverEmail = new MailAddress(to, name);
            var password = "5N_6q1il";
            var sub = subject;
            var body = text;
            var smtp = new System.Net.Mail.SmtpClient
            {
                Host = "webmail.phonencomputerzone.com",
                Port = 25,
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

            //var smtp = new SmtpClient
            //{
            //    Host = "webmail.phonencomputerzone.com",
            //    Port = 25,
            //    EnableSsl = true,
            //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //    UseDefaultCredentials = false,
            //    Credentials = new NetworkCredential(senderEmail.Address, password)
            //};
            //using (var mess = new MailMessage(senderEmail, receiverEmail)
            //{
            //    Subject = subject,
            //    Body = body,
            //    IsBodyHtml = true
            //})
            //    //{
            //    smtp.Send(mess);



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
