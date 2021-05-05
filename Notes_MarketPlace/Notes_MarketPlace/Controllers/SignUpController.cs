using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Notes_MarketPlace.Models;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Data.Entity.Validation;

namespace Notes_MarketPlace.Controllers
{

    public class SignUpController : Controller
    {
        DataBaseEntities1 Result = new DataBaseEntities1();
        

        [HttpGet]
        [Route("SignUp")]
        public ActionResult SignUp()
        {
            SignUpModel obj = new SignUpModel();
            return View(obj);
            
        }

        [HttpPost]
        [Route("SignUp")]
        public ActionResult SignUp(SignUpModel xyz)
        {

            if (ModelState.IsValid)
            {
                if (!Result.Users.Any(x => x.EmailID == xyz.EmailID))
                {
                    User abc = new User();
                    

                    abc.FirstName = xyz.FirstName;
                    abc.IsActive = true;
                    abc.LastName = xyz.LastName;
                    abc.Password = xyz.Password;
                    abc.EmailID = xyz.EmailID;
                    abc.CreatedDate = DateTime.Now;
                    abc.RoleID = 3; // Role --> Member
                    Result.Users.Add(abc);

                    Result.SaveChanges();

                    int id = abc.ID;



                    if (id > 0)
                    {
                        SendEmail(xyz);
                        ViewBag.Success = "Your account has been successfully created. Please Verify Email.";
                    }

                    return View(xyz);
                }
                else
                {
                    ModelState.AddModelError("Error", "Email is Already exists!");
                    return View();
                }
            }

            return View();
        }
        private void SendEmail(SignUpModel xyz)
        {
            using (MailMessage mm = new MailMessage("ijeruokguyihu@gmail.com", xyz.EmailID))
            {
                mm.Subject = "Note MarketPlace - Email Verification";
                string body = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplate/Email_Verification.html")))
                {
                    body = reader.ReadToEnd();
                }

                var ConfirmartionLink = Url.Action("ConfirmEmail", "SignUp", new { UserId = xyz.EmailID, PassWord = xyz.Password }, protocol: Request.Url.Scheme);
                body = body.Replace("{UserName}", xyz.FirstName);
                body = body.Replace("{ConfirmationLink}", ConfirmartionLink);

                mm.Body = body;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;

                NetworkCredential Network = new NetworkCredential("ijeruokguyihu@gmail.com", "001100R@63");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = Network;
                smtp.Port = 587;
                smtp.Send(mm);
            }
        }

        [Route("SignUp/ConfirmEmail")]
        public ActionResult ConfirmEmail(string UserId, string PassWord)
        {
            var Check = Result.Users.Where(x => x.EmailID == UserId).FirstOrDefault();
            if (Check != null)
            {
                if (Check.Password.Equals(PassWord))
                {
                    Check.IsEmailVerified = true;

                    Result.SaveChanges();
                    Result.Dispose();

                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    return Content("Invalid Credentials");
                }
            }
            return Content("Invalid Credentials");
        }
    }

}
