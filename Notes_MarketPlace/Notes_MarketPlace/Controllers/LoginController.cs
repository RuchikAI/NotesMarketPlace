using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Notes_MarketPlace.Models;
using System.Web.Security;
using System.Net;
using System.Net.Mail;
using System.IO;

namespace Notes_MarketPlace.Controllers
{
    public class LoginController : Controller
    {
        DataBaseEntities1 Context = new DataBaseEntities1();

        [HttpGet]
        [Route("Login")]
        public ActionResult Login()
        {
            LoginModel obj = new LoginModel();
            return View(obj);
        }

        [HttpPost]
        [Route("Login")]
        public ActionResult Login(LoginModel xyz)
        {
            var abc = Context.Users.Where(x => x.EmailID == xyz.Email && x.Password == xyz.Password).FirstOrDefault();
            if (abc == null)
            {
                //ModelState.AddModelError("Error", "Email and Password is not Matching");
                @ViewBag.error = "Email and Password is not Matching";
                return View();
            }
            else if (abc.IsEmailVerified == true && abc.RoleID == 3)
            {
                if(xyz.RememberMe==true)
                {
                    FormsAuthentication.SetAuthCookie(abc.EmailID,true);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(abc.EmailID, false);
                }
                
                Session["EmailID"] = abc.EmailID;
                var LoginFirstTime = Context.UserProfiles.FirstOrDefault(x => x.User_ID == abc.ID);
                if (LoginFirstTime == null)
                {
                    return RedirectToAction("MyProfile", "User");
                }
                else
                {
                    return RedirectToAction("Search", "SearchNotes");
                }

            }

            else if (abc.IsEmailVerified == true && abc.RoleID == 2)
            {
                FormsAuthentication.SetAuthCookie(xyz.Email, xyz.RememberMe);
                Session["EmailID"] = xyz.Email;
                return RedirectToAction("Dashboard", "SellYourNote");
            }
            else
            {
                @ViewBag.Email = "Please verify your account";
            }
            return View();
        }
        [Route("Logout")]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Home", "Home");
        }

        [HttpGet]
        [Route("ForgotPassWord")]
        public ActionResult ForgotPassWord()
        {
            ForgotPasswordModel obj = new ForgotPasswordModel();
            return View();
        }

        [HttpPost]
        [Route("ForgotPassWord")]

        public ActionResult ForgotPassWord(ForgotPasswordModel xyz)
        {
            if (Context.Users.Any(model => model.EmailID == xyz.Email))
            {
                ForgotPassSentEmail(xyz);
                ViewBag.Success = "Your password has been changed successfully and newly generated password is sent on your registered email address.";
                return View();
            }
            else
            {
                //ModelState.AddModelError("Error", "Email Id does not exists");
                @ViewBag.error = "Email Id does not exists";
                return View();
            }
        }

        private void ForgotPassSentEmail(ForgotPasswordModel xyz)
        {
            var check = Context.Users.Where(x => x.EmailID == xyz.Email).FirstOrDefault();
            using (MailMessage mm = new MailMessage("ijeruokguyihu@gmail.com", xyz.Email))
            {
                mm.Subject = "NoteMarketPlace - Temporary Password";

                var body = "<p>Hello,</p> <p>Your newly generated password is:<p> <p>{0}</p> <p>Thanks,</p><p>Team Notes MarketPlace</p>";
                string NewPassword = GeneratePassword().ToString();
                body = string.Format(body, NewPassword);
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

                if (NewPassword != null)
                {
                    var Replace = Context.Users.Where(x => x.EmailID == xyz.Email).FirstOrDefault();
                    if (Replace != null)
                    {
                        Replace.Password = NewPassword;

                        Context.SaveChanges();
                    }
                }
            }
        }

        public string GeneratePassword()
        {
            string PassLength = "6";
            string NewPass = "";

            String AllowChar = "";

            AllowChar = "1,2,3,4,5,6,7,8,9,0";

            char[] Seperated = { ',' };

            string[] arr = AllowChar.Split(Seperated);

            string IDString = "";
            string Temp = "";

            Random Rand = new Random();

            for (int i = 0; i < Convert.ToInt32(PassLength); i++)
            {
                Temp = arr[Rand.Next(0, arr.Length)];
                IDString += Temp;
                NewPass = IDString;
            }
            return NewPass;
        }


        [HttpGet]
        
        [Route("ChangePassword")]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        
       
        [Route("ChangePassword")]
        public ActionResult ChangePassword(ChangePassModel model)
        {
            // check if model state is valid or not
            if (ModelState.IsValid)
            {
                // get logged in user
                var loggedinuser = Context.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
                // check if user is logged in or not
                if (loggedinuser != null)
                {
                    // match old password
                    if (loggedinuser.Password == model.OldPassword)
                    {
                        // update password
                        loggedinuser.Password = model.NewPassword;
                        loggedinuser.ModifiedDate = DateTime.Now;
                        loggedinuser.ModifiedBy = loggedinuser.ID;
                        Context.Users.Attach(loggedinuser);
                        Context.Entry(loggedinuser).Property(x => x.Password).IsModified = true;
                        Context.Entry(loggedinuser).Property(x => x.ModifiedDate).IsModified = true;
                        Context.Entry(loggedinuser).Property(x => x.ModifiedBy).IsModified = true;
                        Context.SaveChanges();

                        return RedirectToAction("login");
                    }
                    else
                    {
                        // password mismatch error
                        ModelState.AddModelError("OldPassword", "Your old password is not match with your current pasword");
                        return View(model);
                    }
                }
                // if user is not logged in 
                else
                {
                    return RedirectToAction("Login");
                }
            }
            else
            {
                return View(model);
            }
        }












    }
}