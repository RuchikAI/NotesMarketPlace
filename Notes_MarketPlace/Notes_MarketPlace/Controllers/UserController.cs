using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Mail;

using LazZiya.ImageResize;
using System.Drawing;

using System.Web.Mvc;
using Notes_MarketPlace.Models;


namespace Notes_MarketPlace.Controllers
{
    public class UserController : Controller
    {
        readonly DataBaseEntities1 Context = new DataBaseEntities1();


        [HttpGet]
        [Route("MyProfile")]
        public ActionResult MyProfile()
        {
            var user = Context.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            var userprofile = Context.UserProfiles.Where(x => x.User_ID == user.ID).FirstOrDefault();

            UserProfileModel userProfileModel = new UserProfileModel();

            if (userprofile != null)
            {
                userProfileModel.CountryList = Context.Countries.Where(x => x.IsActive == true).ToList();
                userProfileModel.GenderList = Context.ReferenceDatas.Where(x => x.RefCategory.ToLower() == "gender").ToList();
                userProfileModel.FirstName = user.FirstName;
                userProfileModel.LastName = user.LastName;
                userProfileModel.City = userprofile.City;
                userProfileModel.UserID = user.ID;
                userProfileModel.Email = user.EmailID;
                userProfileModel.DOB = userprofile.DOB;
                userProfileModel.PhoneNumberCountryCode = userprofile.Phone_number___Country_Code;
                userProfileModel.PhoneNumber = userprofile.Phone_number;
                userProfileModel.Gender = userprofile.Gender;
                userProfileModel.AddressLine1 = userprofile.Address_Line_1;
                userProfileModel.AddressLine2 = userprofile.Address_Line_2;
                userProfileModel.City = userprofile.City;
                userProfileModel.State = userprofile.State;
                userProfileModel.ZipCode = userprofile.Zip_Code;
                userProfileModel.Country = userprofile.Country;
                userProfileModel.University = userprofile.University;
                userProfileModel.College = userprofile.College;
                userProfileModel.ProfilePictureUrl = userprofile.Profile_Picture;
            }
            else
            {
                userProfileModel.CountryList = Context.Countries.Where(x => x.IsActive == true).ToList();
                userProfileModel.GenderList = Context.ReferenceDatas.Where(x => x.RefCategory.ToLower() == "gender").ToList();
                userProfileModel.FirstName = user.FirstName;
                userProfileModel.LastName = user.LastName;
                userProfileModel.UserID = user.ID;
                userProfileModel.Email = user.EmailID;
            }

            return View(userProfileModel);
        }

        [HttpPost]
        [Route("MyProfile")]
        public ActionResult MyProfile(UserProfileModel userProfileModel)
        {
            var user = Context.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            var userprofile = Context.UserProfiles.Where(x => x.User_ID == user.ID).FirstOrDefault();

            if (ModelState.IsValid)
            {
                if (userProfileModel.ProfilePicture != null)
                {
                    var profilePicturesize = userProfileModel.ProfilePicture.ContentLength;
                    if (profilePicturesize > 10 * 1024 * 1024)
                    {
                        ViewBag.ProfilePicturesize = "Image size limit is 10 MB";
                        userProfileModel.CountryList = Context.Countries.Where(x => x.IsActive == true).ToList();
                        userProfileModel.GenderList = Context.ReferenceDatas.Where(x => x.RefCategory.ToLower() == "gender").ToList();
                        return View(userProfileModel);
                    }

                }
                if (userprofile != null)
                {
                    userprofile.DOB = userProfileModel.DOB;
                    userprofile.Gender = userProfileModel.Gender;
                    userprofile.Phone_number___Country_Code = userProfileModel.PhoneNumberCountryCode.Trim();
                    userprofile.Phone_number = userProfileModel.PhoneNumber.Trim();
                    userprofile.Address_Line_1 = userProfileModel.AddressLine1.Trim();
                    userprofile.Address_Line_2 = userProfileModel.AddressLine2.Trim();
                    userprofile.City = userProfileModel.City.Trim();
                    userprofile.State = userProfileModel.State.Trim();
                    userprofile.Zip_Code = userProfileModel.ZipCode.Trim();
                    userprofile.Country = userProfileModel.Country.Trim();
                    userprofile.University = userProfileModel.University.Trim();
                    userprofile.College = userProfileModel.College.Trim();
                    userprofile.ModifiedDate = DateTime.Now;
                    userprofile.ModifiedBy = user.ID;




                    // check if loggedin user's profile picture is not null and user upload new profile picture then delete old one
                    if (userProfileModel.ProfilePicture != null && userprofile.Profile_Picture != null)
                    {
                        string path = Server.MapPath(userprofile.Profile_Picture);
                        FileInfo file = new FileInfo(path);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }

                    if (userProfileModel.ProfilePicture != null)
                    {
                        string filename = Path.GetFileName(userProfileModel.ProfilePicture.FileName);
                        string fileextension = Path.GetExtension(userProfileModel.ProfilePicture.FileName);
                        string newfilename = "DP_" + DateTime.Now.ToString("ddMMyyyy_hhmmss") + fileextension;
                        string profilepicturepath = "~/Members/" + userprofile.User_ID + "/";

                        CreateDirectoryIfMissing(profilepicturepath);
                        string path = Path.Combine(Server.MapPath(profilepicturepath), newfilename);
                        userprofile.Profile_Picture = profilepicturepath + newfilename;
                        userProfileModel.ProfilePicture.SaveAs(path);

                        /*var img = Image.FromFile(profilepicturepath);
                        var scaleimage = ImageResize.Scale(img, 40, 40);
                        scaleimage.SaveAs(path);*/

                        userProfileModel.ProfilePictureUrl = profilepicturepath;


                    }

                    Context.Entry(userprofile).State = EntityState.Modified;
                    Context.SaveChanges();

                    // update first name and lastname and save
                    user.FirstName = userProfileModel.FirstName.Trim();
                    user.LastName = userProfileModel.LastName.Trim();
                    Context.Entry(user).State = EntityState.Modified;
                    Context.SaveChanges();
                }

                else
                {
                    UserProfile Profile = new UserProfile();
                    Profile.User_ID = user.ID;
                    Profile.DOB = userProfileModel.DOB;
                    Profile.Gender = userProfileModel.Gender;
                    Profile.Phone_number___Country_Code = userProfileModel.PhoneNumberCountryCode.Trim();
                    Profile.Phone_number = userProfileModel.PhoneNumber.Trim();
                    Profile.Address_Line_1 = userProfileModel.AddressLine1.Trim();
                    Profile.Address_Line_2 = userProfileModel.AddressLine2.Trim();
                    Profile.City = userProfileModel.City.Trim();
                    Profile.State = userProfileModel.State.Trim();
                    Profile.Zip_Code = userProfileModel.ZipCode.Trim();
                    Profile.Country = userProfileModel.Country.Trim();
                    Profile.University = userProfileModel.University.Trim();
                    Profile.College = userProfileModel.College.Trim();
                    Profile.CreatedDate = DateTime.Now;
                    Profile.CreatedBy = user.ID;
                    if (user.RoleID == 3)
                    {
                        Profile.SecondaryEmailAddress = user.EmailID;
                    }
                    else if (user.RoleID == 2)
                    {
                        Profile.SecondaryEmailAddress = userProfileModel.Email;
                    }
                    else
                    {

                    }



                    if (userProfileModel.ProfilePicture != null)
                    {
                        string filename = Path.GetFileName(userProfileModel.ProfilePicture.FileName);
                        string fileextension = Path.GetExtension(userProfileModel.ProfilePicture.FileName);
                        string newfilename = "DP_" + DateTime.Now.ToString("ddMMyyyy_hhmmss") + fileextension;
                        string profilepicturepath = "~/Members/" + Profile.User_ID + "/";
                        CreateDirectoryIfMissing(profilepicturepath);
                        string path = Path.Combine(Server.MapPath(profilepicturepath), newfilename);
                        Profile.Profile_Picture = profilepicturepath + newfilename;
                        userProfileModel.ProfilePicture.SaveAs(path);
                    }

                    Context.UserProfiles.Add(Profile);
                    //Context.Entry(Profile).State = EntityState.Modified;

                    Context.SaveChanges();



                    /* // update first name and lastname and save
                     user.FirstName = userProfileModel.FirstName.Trim();
                     user.LastName = userProfileModel.LastName.Trim();
                     Context.Entry(user).State = EntityState.Modified;
                     Context.SaveChanges();*/

                }

                return RedirectToAction("Search", "SearchNotes");
            }

            else
            {
                userProfileModel.CountryList = Context.Countries.Where(x => x.IsActive == true).ToList();
                userProfileModel.GenderList = Context.ReferenceDatas.Where(x => x.RefCategory.ToLower() == "gender").ToList();
                return View(userProfileModel);
            }
        }



        // Create Directory
        private void CreateDirectoryIfMissing(string folderpath)
        {
            // check if directory is exists or not
            bool folderalreadyexists = Directory.Exists(Server.MapPath(folderpath));
            // if directory is not exists then create directory
            if (!folderalreadyexists)
                Directory.CreateDirectory(Server.MapPath(folderpath));
        }


        [HttpGet]

        [Route("MyDownloads")]
        public ActionResult MyDownloads(string search, string sort, int page = 1)
        {
            // viewbag for searching, sorting and pagination
            ViewBag.Search = search;
            ViewBag.Sort = sort;
            ViewBag.PageNumber = page;

            // get logged in user
            var user = Context.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            // get downloaded notes
            IEnumerable<MyDownloadsModel> mydownloads = from download in Context.Downloads
                                                        join users in Context.Users on download.Seller equals users.ID
                                                        join review in Context.SellerNotesReviews on download.ID equals review.AgainstDownloadsID into r
                                                        from notereview in r.DefaultIfEmpty()
                                                        where download.Downloader == user.ID && download.IsSellerHasAllowedDownload == true && download.AttachmentPath != null
                                                        select new MyDownloadsModel
                                                        {
                                                            NoteID = download.NoteID,
                                                            DownloadID = download.ID,
                                                            Title = download.NoteTitle,
                                                            Category = download.NoteCategory,
                                                            Seller = users.EmailID,
                                                            SellType = download.IsPaid == true ? "Paid" : "Free",
                                                            Price = download.PurchasedPrice,
                                                            DownloadedDate = download.AttachmentDownloadedDate.Value,
                                                            NoteDownloaded = download.IsAttachmentDownloaded,
                                                            ReviewID = notereview.ID,
                                                            Rating = notereview.Ratings,
                                                            Comment = notereview.Comments
                                                        };

            // get searched result if search is not empty
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                mydownloads = mydownloads.Where(x => x.Title.ToLower().Contains(search) ||
                                                     x.Category.ToLower().Contains(search) ||
                                                     x.Seller.ToLower().Contains(search) ||
                                                     x.Price.ToString().ToLower().Contains(search) ||
                                                     x.SellType.ToLower().Contains(search)
                                               );
            }

            // sorting result
            mydownloads = SortTableMyDownloads(sort, mydownloads);


            // viewbag for count total pages
            ViewBag.TotalPages = Math.Ceiling(mydownloads.Count() / 10.0);
            /* model.Count = mydownloads.Count();*/

            // show result based on pagination
            mydownloads = mydownloads.Skip((page - 1) * 10).Take(10);

            // return results
            return View(mydownloads);
        }

        // sorting for my downloads results
        private IEnumerable<MyDownloadsModel> SortTableMyDownloads(string sort, IEnumerable<MyDownloadsModel> table)
        {
            switch (sort)
            {
                case "Title_Asc":
                    {
                        table = table.OrderBy(x => x.Title);
                        break;
                    }
                case "Title_Desc":
                    {
                        table = table.OrderByDescending(x => x.Title);
                        break;
                    }
                case "Category_Asc":
                    {
                        table = table.OrderBy(x => x.Category);
                        break;
                    }
                case "Category_Desc":
                    {
                        table = table.OrderByDescending(x => x.Category);
                        break;
                    }
                case "Seller_Asc":
                    {
                        table = table.OrderBy(x => x.Seller);
                        break;
                    }
                case "Seller_Desc":
                    {
                        table = table.OrderByDescending(x => x.Seller);
                        break;
                    }
                case "Type_Asc":
                    {
                        table = table.OrderBy(x => x.SellType);
                        break;
                    }
                case "Type_Desc":
                    {
                        table = table.OrderByDescending(x => x.SellType);
                        break;
                    }
                case "Price_Asc":
                    {
                        table = table.OrderBy(x => x.Price);
                        break;
                    }
                case "Price_Desc":
                    {
                        table = table.OrderByDescending(x => x.Price);
                        break;
                    }
                case "DownloadedDate_Asc":
                    {
                        table = table.OrderBy(x => x.DownloadedDate);
                        break;
                    }
                case "DownloadedDate_Desc":
                    {
                        table = table.OrderByDescending(x => x.DownloadedDate);
                        break;
                    }
                default:
                    {
                        table = table.OrderByDescending(x => x.DownloadedDate);
                        break;
                    }
            }
            return table;
        }

        [HttpPost]
        [Route("Note/AddReview")]
        public ActionResult AddReview(SellerNotesReview notereview)
        {
            // check if comment is null or not
            if (String.IsNullOrEmpty(notereview.Comments))
            {
                return RedirectToAction("MyDownloads");
            }

            // check if rating is between 1 to 5
            if (notereview.Ratings < 1 || notereview.Ratings > 5)
            {
                return RedirectToAction("MyDownloads");
            }

            // get Download object for check if user is downloaded note or not
            var notedownloaded = Context.Downloads.Where(x => x.ID == notereview.AgainstDownloadsID && x.IsAttachmentDownloaded == true).FirstOrDefault();

            // user can provide review after downloading the note
            if (notedownloaded != null)
            {
                //get logged in user
                var user = Context.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

                // check if user already provided review or not
                var alreadyprovidereview = Context.SellerNotesReviews.Where(x => x.AgainstDownloadsID == notereview.AgainstDownloadsID && x.IsActive == true).FirstOrDefault();

                // if user not provide review then add review
                if (alreadyprovidereview == null)
                {
                    // create a sellernotesreview object and initialize it
                    SellerNotesReview review = new SellerNotesReview();

                    review.NoteID = notereview.NoteID;
                    review.AgainstDownloadsID = notereview.AgainstDownloadsID;
                    review.ReviewedByID = user.ID;
                    review.Ratings = notereview.Ratings;
                    review.Comments = notereview.Comments;
                    review.CreatedDate = DateTime.Now;
                    review.CreatedBy = user.ID;
                    review.IsActive = true;

                    // save sellernotesreview into database
                    Context.SellerNotesReviews.Add(review);
                    Context.SaveChanges();

                    return RedirectToAction("MyDownloads");
                }
                // if user is already provided review then edit it
                else
                {
                    alreadyprovidereview.Ratings = notereview.Ratings;
                    alreadyprovidereview.Comments = notereview.Comments;
                    alreadyprovidereview.ModifiedDate = DateTime.Now;
                    alreadyprovidereview.ModifiedBy = user.ID;

                    // update review and save in database
                    Context.Entry(alreadyprovidereview).State = EntityState.Modified;
                    Context.SaveChanges();

                    return RedirectToAction("MyDownloads");
                }
            }
            return RedirectToAction("MyDownloads");
        }

        [HttpPost]

        [Route("Note/ReportSpam")]
        public ActionResult SpamReport(FormCollection form)
        {
            // get download id by form 
            int downloadid = Convert.ToInt32(form["downloadid"]);

            // get ReportedIssues object 
            var alreadyreportedspam = Context.SellerNotesReportedIssues.Where(x => x.AgainstDownloadID == downloadid).FirstOrDefault();

            // if user not slready reported spam 
            if (alreadyreportedspam == null)
            {
                //get logged in user
                var user = Context.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

                // store logged in user name into variable
                string membername = user.FirstName + " " + user.LastName;

                // create a spam report object
                SellerNotesReportedIssue spamnote = new SellerNotesReportedIssue();

                spamnote.NoteID = Convert.ToInt32(form["noteid"]);
                spamnote.AgainstDownloadID = downloadid;
                spamnote.ReportedByID = user.ID;
                spamnote.Remarks = form["spamreport"];
                spamnote.CreatedDate = DateTime.Now;
                spamnote.CreatedBy = user.ID;

                // save spam report object into database
                Context.SellerNotesReportedIssues.Add(spamnote);
                Context.SaveChanges();

                // send mail to admin that buyer reported the notes as inappropriate
                /*SpamReportTemplate(spamnote, membername);*/
            }
            return RedirectToAction("MyDownloads");
        }

        // intializing the template that we want to send to admin for marking note as inappropriate
        /* private void SpamReportTemplate(SellerNotesReportedIssue spam, string membername)
         {
             string from, to, body, subject;

             // get all texts from SpamReport.cshtml from EmailTemplate
             body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/") + "SpamReport" + ".cshtml");

             // get notes and user by using SellerNotesReportedIssue object
             var note = Context.SellerNotes.Find(spam.NoteID);
             var seller = Context.Users.Find(note.SellerID);

             // replace some text with note title, seller name and user's name who mark this note as inappropriate
             body = body.Replace("ViewBag.SellerName", seller.FirstName + " " + seller.LastName);
             body = body.Replace("ViewBag.NoteTitle", note.Title);
             body = body.Replace("ViewBag.ReportedBy", membername);
             body = body.ToString();

             // get support email
             var fromemail = Context.SystemConfigurations.Where(x => x.Name == "supportemail").FirstOrDefault();
             var tomail = Context.SystemConfigurations.Where(x => x.Name == "notifyemail").FirstOrDefault();

             // set from, to, subject, body
             from = fromemail.Value.Trim();
             to = tomail.Value.Trim();
             subject = membername + " Reported an issue for " + note.Title;
             StringBuilder sb = new StringBuilder();
             sb.Append(body);
             body = sb.ToString();

             // create object of MailMessage
             MailMessage mail = new MailMessage();
             mail.From = new MailAddress(from, "NotesMarketplace");
             mail.To.Add(new MailAddress(to));
             mail.Subject = subject;
             mail.Body = body;
             mail.IsBodyHtml = true;

             // send mail (NotesMarketplace/SendMail/)
             SendingEmail.SendEmail(mail);
         }*/

        [HttpGet]
        [Route("MyRejectedNotes")]
        public ActionResult MyRejectedNotes(string search, string sort, int page = 1)
        {
            // viewbag for searching, sorting and pagination
            ViewBag.Search = search;
            ViewBag.Sort = sort;
            ViewBag.PageNumber = page;

            // get logged in user
            var user = Context.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            // get rejected id
            var rejectedid = Context.ReferenceDatas.Where(x => x.Value.ToLower() == "rejected").Select(x => x.ID).FirstOrDefault();

            // get user's rejected notes 
            IEnumerable<SellerNote> rejectednotes = Context.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == rejectedid && x.IsActive == true).ToList();

            // get searched result if search is not empty
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                rejectednotes = rejectednotes.Where(x => x.Title.ToLower().Contains(search) ||
                                                     x.NoteCategory.Name.ToLower().Contains(search) ||
                                                     x.AdminRemarks.ToLower().Contains(search)).ToList();
            }

            // sort result
            rejectednotes = SortTableMyRejectedNotes(sort, rejectednotes);

            // viewbag for count total pages
            ViewBag.TotalPages = Math.Ceiling(rejectednotes.Count() / 10.0);

            // show result based on pagination
            rejectednotes = rejectednotes.Skip((page - 1) * 10).Take(10).ToList();

            // return rejectedd notes result
            return View(rejectednotes);
        }

        //sorting for my rejected notes
        private IEnumerable<SellerNote> SortTableMyRejectedNotes(string sort, IEnumerable<SellerNote> table)
        {
            switch (sort)
            {
                case "Title_Asc":
                    {
                        table = table.OrderBy(x => x.Title);
                        break;
                    }
                case "Title_Desc":
                    {
                        table = table.OrderByDescending(x => x.Title);
                        break;
                    }
                case "Category_Asc":
                    {
                        table = table.OrderBy(x => x.NoteCategory.Name);
                        break;
                    }
                case "Category_Desc":
                    {
                        table = table.OrderByDescending(x => x.NoteCategory.Name);
                        break;
                    }
                case "Remark_Asc":
                    {
                        table = table.OrderBy(x => x.AdminRemarks);
                        break;
                    }
                case "Remark_Desc":
                    {
                        table = table.OrderByDescending(x => x.AdminRemarks);
                        break;
                    }
                default:
                    {
                        table = table.OrderByDescending(x => x.ModifiedDate);
                        break;
                    }
            }
            return table;
        }

        [HttpGet]
        [Route("MyRejectedNotes/{noteid}/Clone")]
        public ActionResult CloneNote(int noteid)
        {
            // get logged in user
            var user = Context.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            // get rejected note by id
            var rejectednote = Context.SellerNotes.Find(noteid);

            // create object of sellernote for create clone of note
            SellerNote clonenote = new SellerNote();

            clonenote.SellerID = rejectednote.SellerID;
            clonenote.Status = Context.ReferenceDatas.Where(x => x.Value.ToLower() == "draft").Select(x => x.ID).FirstOrDefault();
            clonenote.Title = rejectednote.Title;
            clonenote.Category = rejectednote.Category;
            clonenote.NoteType = rejectednote.NoteType;
            clonenote.NumberOfPages = rejectednote.NumberOfPages;
            clonenote.Descripton = rejectednote.Descripton;
            clonenote.UniversityName = rejectednote.UniversityName;
            clonenote.Country = rejectednote.Country;
            clonenote.Course = rejectednote.Course;
            clonenote.CourseCode = rejectednote.CourseCode;
            clonenote.Professor = rejectednote.Professor;
            clonenote.IsPaid = rejectednote.IsPaid;
            clonenote.SellingPrice = rejectednote.SellingPrice;
            clonenote.CreatedBy = user.ID;
            clonenote.CreatedDate = DateTime.Now;
            clonenote.IsActive = true;

            // save note in database
            Context.SellerNotes.Add(clonenote);
            Context.SaveChanges();

            // get clonenote 
            clonenote = Context.SellerNotes.Find(clonenote.ID);

            // if display picture is not null then copy file from rejected note's folder to clone note's new folder
            if (rejectednote.DisplayPicture != null)
            {
                var rejectednotefilepath = Server.MapPath(rejectednote.DisplayPicture);
                var clonenotefilepath = "~/Members/" + user.ID + "/" + clonenote.ID + "/";

                var filepath = Path.Combine(Server.MapPath(clonenotefilepath));

                FileInfo file = new FileInfo(rejectednotefilepath);

                Directory.CreateDirectory(filepath);
                if (file.Exists)
                {
                    System.IO.File.Copy(rejectednotefilepath, Path.Combine(filepath, Path.GetFileName(rejectednotefilepath)));
                }

                clonenote.DisplayPicture = Path.Combine(clonenotefilepath, Path.GetFileName(rejectednotefilepath));
                Context.SaveChanges();
            }

            // if note preview is not null then copy file from rejected note's folder to clone note's new folder
            if (rejectednote.NotesPreview != null)
            {
                var rejectednotefilepath = Server.MapPath(rejectednote.NotesPreview);
                var clonenotefilepath = "~/Members/" + user.ID + "/" + clonenote.ID + "/";

                var filepath = Path.Combine(Server.MapPath(clonenotefilepath));

                FileInfo file = new FileInfo(rejectednotefilepath);

                Directory.CreateDirectory(filepath);

                if (file.Exists)
                {
                    System.IO.File.Copy(rejectednotefilepath, Path.Combine(filepath, Path.GetFileName(rejectednotefilepath)));
                }

                clonenote.NotesPreview = Path.Combine(clonenotefilepath, Path.GetFileName(rejectednotefilepath));
                Context.SaveChanges();
            }

            // attachment path of rejected note and clone note
            var rejectednoteattachement = Server.MapPath("~/Members/" + user.ID + "/" + rejectednote.ID + "/Attachements/");
            var clonenoteattachement = "~/Members/" + user.ID + "/" + clonenote.ID + "/Attachements/";

            var attachementfilepath = Path.Combine(Server.MapPath(clonenoteattachement));

            // create directory for attachement folder
            Directory.CreateDirectory(attachementfilepath);

            // get attachements files from rejected note and copy to clone note
            foreach (var files in Directory.GetFiles(rejectednoteattachement))
            {

                FileInfo file = new FileInfo(files);

                if (file.Exists)
                {
                    System.IO.File.Copy(file.ToString(), Path.Combine(attachementfilepath, Path.GetFileName(file.ToString())));
                }

                // save attachment in database
                SellerNotesAttachement attachement = new SellerNotesAttachement();
                attachement.NoteID = clonenote.ID;
                attachement.FileName = Path.GetFileName(file.ToString());
                attachement.FilePath = clonenoteattachement;
                attachement.CreatedDate = DateTime.Now;
                attachement.CreatedBy = user.ID;
                attachement.IsActive = true;

                Context.SellerNotesAttachements.Add(attachement);
                Context.SaveChanges();
            }
            return RedirectToAction("Dashboard", "SellYourNotes");
        }

        [HttpGet]
        [Route("MySoldNotes")]
        public ActionResult MySoldNotes(string search, string sort, int page = 1)
        {
            //for searching, sorting and pagination
            ViewBag.Search = search;
            ViewBag.Sort = sort;
            ViewBag.PageNumber = page;

            //get logged in user
            var user = Context.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            //get my sold notes
            IEnumerable<MySoldNotesModel> mysoldnotes = from download in Context.Downloads
                                                        join users in Context.Users on download.Downloader equals users.ID
                                                        where download.Seller == user.ID && download.IsSellerHasAllowedDownload == true && download.AttachmentPath != null
                                                        select new MySoldNotesModel
                                                        {
                                                            DownloadID = download.ID,
                                                            NoteID = download.NoteID,
                                                            Title = download.NoteTitle,
                                                            Category = download.NoteCategory,
                                                            Buyer = users.EmailID,
                                                            SellType = download.IsPaid == true ? "Paid" : "Free",
                                                            Price = download.PurchasedPrice,
                                                            DownloadedDate = download.AttachmentDownloadedDate.Value,
                                                            NoteDownloaded = download.IsAttachmentDownloaded
                                                        };

            //get searched result if search is not empty
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                mysoldnotes = mysoldnotes.Where(x => x.Title.ToLower().Contains(search) ||
                                                     x.Category.ToLower().Contains(search) ||
                                                     x.Buyer.ToLower().Contains(search) ||
                                                     x.Price.ToString().ToLower().Contains(search) ||
                                                     x.SellType.ToLower().Contains(search)
                                               ).ToList();
            }

            //sort result
            mysoldnotes = SortTableMySoldNotes(sort, mysoldnotes);

            //count total pages
            ViewBag.TotalPages = Math.Ceiling(mysoldnotes.Count() / 10.0);

            //show result based on pagination
            mysoldnotes = mysoldnotes.Skip((page - 1) * 10).Take(10);

            return View(mysoldnotes);
        }

        //sorting for my my sold notes
        private IEnumerable<MySoldNotesModel> SortTableMySoldNotes(string sort, IEnumerable<MySoldNotesModel> table)
        {
            switch (sort)
            {
                case "Title_Asc":
                    {
                        table = table.OrderBy(x => x.Title);
                        break;
                    }
                case "Title_Desc":
                    {
                        table = table.OrderByDescending(x => x.Title);
                        break;
                    }
                case "Category_Asc":
                    {
                        table = table.OrderBy(x => x.Category);
                        break;
                    }
                case "Category_Desc":
                    {
                        table = table.OrderByDescending(x => x.Category);
                        break;
                    }
                case "Buyer_Asc":
                    {
                        table = table.OrderBy(x => x.Buyer);
                        break;
                    }
                case "Buyer_Desc":
                    {
                        table = table.OrderByDescending(x => x.Buyer);
                        break;
                    }
                case "Type_Asc":
                    {
                        table = table.OrderBy(x => x.SellType);
                        break;
                    }
                case "Type_Desc":
                    {
                        table = table.OrderByDescending(x => x.SellType);
                        break;
                    }
                case "Price_Asc":
                    {
                        table = table.OrderBy(x => x.Price);
                        break;
                    }
                case "Price_Desc":
                    {
                        table = table.OrderByDescending(x => x.Price);
                        break;
                    }
                case "DownloadedDate_Asc":
                    {
                        table = table.OrderBy(x => x.DownloadedDate);
                        break;
                    }
                case "DownloadedDate_Desc":
                    {
                        table = table.OrderByDescending(x => x.DownloadedDate);
                        break;
                    }
                default:
                    {
                        table = table.OrderByDescending(x => x.DownloadedDate);
                        break;
                    }
            }
            return table;
        }
    }
}