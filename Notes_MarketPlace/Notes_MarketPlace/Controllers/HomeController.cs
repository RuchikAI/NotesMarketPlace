using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Notes_MarketPlace.Models;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Web.Security;

namespace Notes_MarketPlace.Controllers
{
    public class HomeController : Controller
    {
        [Route("Home")]
        public ActionResult Home()
        {
            return View();
        }

        [Route("FAQ")]
        public ActionResult FAQ()
        {
            return View();
        }
        [HttpGet]
        [Route("ContactUs")]
        public ActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        [Route("ContactUs")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ContactUs(ContactModel xyz)
        {
            if(ModelState.IsValid)
            {
                var body = "<p>Email From: {1}</p><p>Hello,</p><p>{2}</p><br><p>Regards,</p><p>{0}</p>";
                var Message = new MailMessage();


                Message.To.Add(new MailAddress("ijeruokguyihu@gmail.com"));

                Message.From = new MailAddress(xyz.EmailID);
                Message.Subject = xyz.Subject;
                Message.Body = string.Format(body,xyz.FullName, xyz.EmailID, xyz.Comments);
                Message.IsBodyHtml = true;
                using(var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "ijeruokguyihu@gmail.com",
                        Password = "001100R@63"
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(Message);
                }

            }
            return View();
        }
    }
}