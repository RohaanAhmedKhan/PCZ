using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using PCZ.Models;

namespace PCZ.Models
{
    public class emailFormatter
    {

        static string signature = "<br />" +
                        "<br />" +
                        "<p>Phone N Computer Zone Ltd." + DateTime.Now;

        public static MailMessage message(string subject, string body, string recepient) {

            var message = new MailMessage();
            message.To.Add(new MailAddress(recepient));
            message.Subject = subject;

            message.Body = body;
            message.IsBodyHtml = true;

            return message;
        }

        public static string passwordReset(string resetCode, string name) {

            string message;

            message = "<h2>Hello " + name + "</h2><br/>" +
                      "<p> You requested a password reset for your account on " + DateTime.Now.ToString() + " </p><br/>" +
                      "<p> To reset your password follow the below link and enter the following code in the password reset form </p><br/>" +
                      "<label><b>Reset Code: " + resetCode + "</b></label><br/>" +
                      "<label>Reset Link: </label>" +
                      "<p> https://www.phonenncomputerzone.com/account/resetPassword/ </p>" +  
                      signature;

            return message;
        }
        

    }

}