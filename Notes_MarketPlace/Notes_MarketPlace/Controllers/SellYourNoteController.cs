
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web.Mvc;
using System.IO.Compression;
using System.Linq;

using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Net;
using System.Net.Mail;


using System.Web.UI.WebControls;
using Notes_MarketPlace.Models;
using System.Web.Security;


namespace Notes_MarketPlace.Controllers
{
    public class SellYourNoteController : Controller
    {

        DataBaseEntities1 Context = new DataBaseEntities1();

        [HttpGet]
        public ActionResult Dashboard(string search1, string search2, string sort1, string sort2, int page1 = 1, int page2 = 1)

        {

            // viewbag for searching, sorting and pagination
            ViewBag.Sort1 = sort1;
            ViewBag.Sort2 = sort2;
            ViewBag.Page1 = page1;
            ViewBag.Page2 = page2;
            ViewBag.Search1 = search1;
            ViewBag.Search2 = search2;

            var submittedforreviewid = 11;//Context.ReferenceDatas.Where(x => x.Value.ToLower() == "submitted for review").Select(x => x.ID).FirstOrDefault();
            var inreviewid = 12;//Context.ReferenceDatas.Where(x => x.Value.ToLower() == "in review").Select(x => x.ID).FirstOrDefault();
            var draftid = 9;// Context.ReferenceDatas.Where(x => x.Value.ToLower() == "draft").Select(x => x.ID).FirstOrDefault();
            var rejectedid = 14;// Context.ReferenceDatas.Where(x => x.Value.ToLower() == "rejected").Select(x => x.ID).FirstOrDefault();
            var publishedid = 13;// Context.ReferenceDatas.Where(x => x.Value.ToLower() == "published").Select(x => x.ID).FirstOrDefault();

            DashboardModel dashboardModel = new DashboardModel();

            User user = Context.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            dashboardModel.NumberOfSoldNotes = Context.Downloads.Where(x => x.Seller == user.ID && x.IsSellerHasAllowedDownload == true && x.AttachmentPath != null).Count();
            dashboardModel.MoneyEarned = Context.Downloads.Where(x => x.Seller == user.ID && x.IsSellerHasAllowedDownload == true && x.AttachmentPath != null).Select(x => x.PurchasedPrice).Sum();
            dashboardModel.MyDownloads = Context.Downloads.Where(x => x.Downloader == user.ID && x.IsSellerHasAllowedDownload == true && x.AttachmentPath != null).Count();
            dashboardModel.MyRejectedNotes = Context.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == rejectedid && x.IsActive == true).Count();
            dashboardModel.BuyerRequest = Context.Downloads.Where(x => x.Seller == user.ID && x.IsSellerHasAllowedDownload == false && x.AttachmentPath == null).Count();

            // get inprogress notes

            dashboardModel.InProgressNotes = from note in Context.SellerNotes
                                             where (note.Status == draftid || note.Status == submittedforreviewid || note.Status == inreviewid) && note.SellerID == user.ID
                                             select new InProgressNote
                                             {
                                                 NoteID = note.ID,
                                                 Title = note.Title,
                                                 Category = note.NoteCategory.Name,
                                                 Status = note.ReferenceData.Value,
                                                 AddedDate = note.CreatedDate.Value
                                             };




            // if search1 is not empty then get search result in inprogressnote
            if (!string.IsNullOrEmpty(search1))
            {
                search1 = search1.ToLower();
                dashboardModel.InProgressNotes = dashboardModel.InProgressNotes.Where(x => x.Title.ToLower().Contains(search1) ||
                                                                                                   x.Category.ToLower().Contains(search1) ||
                                                                                                   x.Status.ToLower().Contains(search1)
                                                                                             ).ToList();
            }

            // get published notes
            dashboardModel.PublishedNotes = from note in Context.SellerNotes
                                            where note.Status == publishedid && note.SellerID == user.ID
                                            select new PublishedNote
                                            {
                                                NoteID = note.ID,
                                                Title = note.Title,
                                                Category = note.NoteCategory.Name,
                                                SellType = note.IsPaid == true ? "Paid" : "Free",
                                                Price = note.SellingPrice,
                                                PublishedDate = note.PublishedDate.Value
                                            };
            // if search2 is not empty get search result in publishednote 
            if (!string.IsNullOrEmpty(search2))
            {
                search2 = search2.ToLower();
                dashboardModel.PublishedNotes = dashboardModel.PublishedNotes.Where(x => x.Title.ToLower().Contains(search2) ||
                                                                                                  x.Category.ToLower().Contains(search2) ||
                                                                                                  x.SellType.ToLower().Contains(search2) ||
                                                                                                  x.Price.ToString().Contains(search2)
                                                                                            ).ToList();
            }

            // sorting table
            dashboardModel.InProgressNotes = SortTableInProgressNote(sort1, dashboardModel.InProgressNotes);
            dashboardModel.PublishedNotes = SortTablePublishNote(sort2, dashboardModel.PublishedNotes);

            // count total results
            ViewBag.TotalPagesInProgress = Math.Ceiling(dashboardModel.InProgressNotes.Count() / 5.0);
            ViewBag.TotalPagesInPublished = Math.Ceiling(dashboardModel.PublishedNotes.Count() / 5.0);

            // show results according to pagination
            dashboardModel.InProgressNotes = dashboardModel.InProgressNotes.Skip((page1 - 1) * 5).Take(5);
            dashboardModel.PublishedNotes = dashboardModel.PublishedNotes.Skip((page2 - 1) * 5).Take(5);

            return View(dashboardModel);
        }


        // sorting for inprogress table
        private IEnumerable<InProgressNote> SortTableInProgressNote(string sort, IEnumerable<InProgressNote> table)
        {
            switch (sort)
            {
                case "CreatedDate_Asc":
                    {
                        table = table.OrderBy(x => x.AddedDate);
                        break;
                    }
                case "CreatedDate_Desc":
                    {
                        table = table.OrderByDescending(x => x.AddedDate);
                        break;
                    }
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
                case "Status_Asc":
                    {
                        table = table.OrderBy(x => x.Status);
                        break;
                    }
                case "Status_Desc":
                    {
                        table = table.OrderByDescending(x => x.Status);
                        break;
                    }
                default:
                    {
                        table = table.OrderByDescending(x => x.AddedDate);
                        break;
                    }
            }
            return table;
        }

        // sorting for published note table
        private IEnumerable<PublishedNote> SortTablePublishNote(string sort, IEnumerable<PublishedNote> table)
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
                case "PublishedDate_Asc":
                    {
                        table = table.OrderBy(x => x.PublishedDate);
                        break;
                    }
                case "PublishedDate_Desc":
                    {
                        table = table.OrderByDescending(x => x.PublishedDate);
                        break;
                    }
                case "IsPaid_Asc":
                    {
                        table = table.OrderBy(x => x.SellType);
                        break;
                    }
                case "IsPaid_Desc":
                    {
                        table = table.OrderByDescending(x => x.SellType);
                        break;
                    }
                case "SellingPrice_Asc":
                    {
                        table = table.OrderBy(x => x.Price);
                        break;
                    }
                case "SellingPrice_Desc":
                    {
                        table = table.OrderByDescending(x => x.Price);
                        break;
                    }
                default:
                    {
                        table = table.OrderByDescending(x => x.PublishedDate);
                        break;
                    }
            }
            return table;
        }




        [HttpGet]
        public ActionResult SearchNotes()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        [Route("SellYourNote/AddNotes")]
        public ActionResult AddNotes()
        {
            // create add note viewmodel and set values in dropdown list
            AddNoteViewModel viewModel = new AddNoteViewModel
            {
                NoteCategoryList = Context.NoteCategories.ToList(),
                NoteTypeList = Context.NoteTypes.ToList(),
                CountryList = Context.Countries.ToList()
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [Route("SellYourNote/AddNotes")]
        public ActionResult AddNotes(AddNoteViewModel xyz)
        {
            // check if upload note is null or not
            /*if (xyz.UploadNotes[0] == null)
            {
                ModelState.AddModelError("UploadNotes", "This field is required");
                xyz.NoteCategoryList = Context.NoteCategories.ToList();
                xyz.NoteTypeList = Context.NoteTypes.ToList();
                xyz.CountryList = Context.Countries.ToList();
                return View(xyz);
            }
            // check and raise error for note preview is null for paid notes
            if (xyz.IsPaid == true && xyz.NotesPreview == null)
            {
                ModelState.AddModelError("NotesPreview", "This field is required if selling type is paid");
                xyz.NoteCategoryList = Context.NoteCategories.ToList();
                xyz.NoteTypeList = Context.NoteTypes.ToList();
                xyz.CountryList = Context.Countries.ToList();
                return View(xyz);
            }*/

            /*foreach (HttpPostedFileBase file in xyz.UploadNotes)
            {
                if (!System.IO.Path.GetExtension(file.FileName).Equals(".pdf"))
                {
                    ModelState.AddModelError("UploadNotes", "Only PDF Format is allowed");
                    xyz.NoteCategoryList = Context.NoteCategories.ToList();
                    xyz.NoteTypeList = Context.NoteTypes.ToList();
                    xyz.CountryList = Context.Countries.ToList();
                    return View(xyz);
                }
            }*/


            // check model state
            if (ModelState.IsValid)
            {
                // create seller note object
                SellerNote abc = new SellerNote();


                User user = Context.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);

                abc.SellerID = user.ID;
                abc.Title = xyz.Title.Trim();
                abc.Status = Context.ReferenceDatas.Where(x => x.Value.ToLower() == "draft").Select(x => x.ID).FirstOrDefault();
                abc.Category = xyz.Category;
                abc.NoteType = xyz.NoteType;
                abc.NumberOfPages = xyz.NumberofPages;
                abc.Descripton = xyz.Description.Trim();
                abc.UniversityName = xyz.UniversityName.Trim();
                abc.Country = xyz.Country;
                abc.Course = xyz.Course.Trim();
                abc.CourseCode = xyz.CourseCode.Trim();
                abc.Professor = xyz.Professor.Trim();
                abc.IsPaid = xyz.IsPaid;
                if (abc.IsPaid)
                {
                    abc.SellingPrice = xyz.SellingPrice;
                }
                else
                {
                    abc.SellingPrice = 0;
                }
                abc.CreatedDate = DateTime.Now;
                abc.CreatedBy = user.ID;
                abc.IsActive = true;

                // add note in database and save
                Context.SellerNotes.Add(abc);
                Context.SaveChanges();

                // get seller note
                abc = Context.SellerNotes.Find(abc.ID);

                // if display picture is not null then save picture into directory and directory path into database
                if (xyz.DisplayPicture != null)
                {
                    string displaypicturefilename = System.IO.Path.GetFileName(xyz.DisplayPicture.FileName);
                    string displaypicturepath = "~/Members/" + user.ID + "/" + abc.ID + "/";
                    CreateDirectoryIfMissing(displaypicturepath);
                    string displaypicturefilepath = Path.Combine(Server.MapPath(displaypicturepath), displaypicturefilename);
                    abc.DisplayPicture = displaypicturepath + displaypicturefilename;
                    xyz.DisplayPicture.SaveAs(displaypicturefilepath);
                }

                // if note preview is not null then save picture into directory and directory path into database
                if (xyz.NotesPreview != null)
                {
                    string notespreviewfilename = System.IO.Path.GetFileName(xyz.NotesPreview.FileName);
                    string notespreviewpath = "~/Members/" + user.ID + "/" + abc.ID + "/";
                    CreateDirectoryIfMissing(notespreviewpath);
                    string notespreviewfilepath = Path.Combine(Server.MapPath(notespreviewpath), notespreviewfilename);
                    abc.NotesPreview = notespreviewpath + notespreviewfilename;
                    xyz.NotesPreview.SaveAs(notespreviewfilepath);
                }

                // update note preview path and display picture path and save changes
                Context.SellerNotes.Attach(abc);
                Context.Entry(abc).Property(x => x.DisplayPicture).IsModified = true;
                Context.Entry(abc).Property(x => x.NotesPreview).IsModified = true;
                Context.SaveChanges();

                // attachement files
                foreach (HttpPostedFileBase file in xyz.UploadNotes)
                {
                    // check if file is null or not
                    if (file != null)
                    {
                        // save file in directory
                        string notesattachementfilename = System.IO.Path.GetFileName(file.FileName);
                        string notesattachementpath = "~/Members/" + user.ID + "/" + abc.ID + "/Attachements/";
                        CreateDirectoryIfMissing(notesattachementpath);
                        string notesattachementfilepath = Path.Combine(Server.MapPath(notesattachementpath), notesattachementfilename);
                        file.SaveAs(notesattachementfilepath);

                        // create object of sellernotesattachement 
                        SellerNotesAttachement notesattachements = new SellerNotesAttachement
                        {
                            NoteID = abc.ID,
                            FileName = notesattachementfilename,
                            FilePath = notesattachementpath,
                            CreatedDate = DateTime.Now,
                            CreatedBy = user.ID,
                            IsActive = true
                        };

                        // save seller notes attachement
                        Context.SellerNotesAttachements.Add(notesattachements);
                        Context.SaveChanges();
                    }
                }
                return RedirectToAction("Home", "Home");
            }
            // if model state is not valid
            else
            {
                // create object of xyz
                AddNoteViewModel viewModel = new AddNoteViewModel
                {
                    NoteCategoryList = Context.NoteCategories.ToList(),
                    NoteTypeList = Context.NoteTypes.ToList(),
                    CountryList = Context.Countries.ToList()
                };

                return View(viewModel);
            }
        }

        [Route("SellYourNote/DeleteDraft/{id}")]
        public ActionResult DeleteDraft(int id)
        {
            // get notes using id
            SellerNote note = Context.SellerNotes.Where(x => x.ID == id && x.IsActive == true).FirstOrDefault();
            // if note is not found
            if (note == null)
            {
                return HttpNotFound();
            }
            // get attachement files using note id
            IEnumerable<SellerNotesAttachement> noteattachement = Context.SellerNotesAttachements.Where(x => x.NoteID == id && x.IsActive == true).ToList();
            // if noteattachement count is 0
            if (noteattachement.Count() == 0)
            {
                return HttpNotFound();
            }
            // filepaths for note and note attachements
            string notefolderpath = Server.MapPath("~/Members/" + note.SellerID + "/" + note.ID);
            string noteattachmentfolderpath = Server.MapPath("~/Members/" + note.SellerID + "/" + note.ID + "/Attachements");

            // get directory 
            DirectoryInfo notefolder = new DirectoryInfo(notefolderpath);
            DirectoryInfo attachementnotefolder = new DirectoryInfo(noteattachmentfolderpath);
            // empty directory
            EmptyFolder(attachementnotefolder);
            EmptyFolder(notefolder);
            // delete directory
            Directory.Delete(notefolderpath);

            // remove note from database
            Context.SellerNotes.Remove(note);

            // remove attachement from database
            foreach (var item in noteattachement)
            {
                SellerNotesAttachement attachement = Context.SellerNotesAttachements.Where(x => x.ID == item.ID).FirstOrDefault();
                Context.SellerNotesAttachements.Remove(attachement);
            }

            // save changes
            Context.SaveChanges();

            return RedirectToAction("Dashboard");
        }


        [Authorize]
        [HttpGet]
        [Route("SellYourNote/EditNotes/{id}")]
        public ActionResult EditNotes(int id)
        {
            // get logged in user
            User user = Context.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            // get note
            SellerNote note = Context.SellerNotes.Where(x => x.ID == id && x.IsActive == true && x.SellerID == user.ID).FirstOrDefault();
            // get note attachement
            SellerNotesAttachement attachement = Context.SellerNotesAttachements.Where(x => x.NoteID == id).FirstOrDefault();
            if (note != null)
            {
                // create object of edit note viewmodel
                EditNotesViewModel xyz = new EditNotesViewModel
                {
                    ID = note.ID,
                    NoteID = note.ID,
                    Title = note.Title,
                    Category = note.Category,
                    Picture = note.DisplayPicture,
                    Note = attachement.FilePath,
                    NumberofPages = note.NumberOfPages,
                    Description = note.Descripton,
                    NoteType = note.NoteType,
                    UniversityName = note.UniversityName,
                    Course = note.Course,
                    CourseCode = note.CourseCode,
                    Country = note.Country,
                    Professor = note.Professor,
                    IsPaid = note.IsPaid,
                    SellingPrice = note.SellingPrice,
                    Preview = note.NotesPreview,
                    NoteCategoryList = Context.NoteCategories.ToList(),
                    NoteTypeList = Context.NoteTypes.ToList(),
                    CountryList = Context.Countries.ToList()
                };

                // return viewmodel to edit notes page
                return View(xyz);
            }
            else
            {
                // if note not found
                return HttpNotFound();
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("SellYourNote/EditNotes/{id}")]
        public ActionResult EditNotes(int id, EditNotesViewModel notes)
        {
            // check if model state is valid or not
            if (ModelState.IsValid)
            {
                // get logged in user
                var user = Context.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
                // get note 
                var sellernotes = Context.SellerNotes.Where(x => x.ID == id && x.IsActive == true && x.SellerID == user.ID).FirstOrDefault();
                // if sellernote null
                if (sellernotes == null)
                {
                    return HttpNotFound();
                }
                // check if note is paid or preview is not null
                if (notes.IsPaid == true && notes.Preview == null && sellernotes.NotesPreview == null)
                {
                    //ModelState.AddModelError("NotesPreview", "This field is required if selling type is paid");
                    @ViewBag.Notespreview = "This field is required if selling type is paid";
                    return View(notes);
                }
                // get note attachement 
                var notesattachement = Context.SellerNotesAttachements.Where(x => x.NoteID == notes.NoteID && x.IsActive == true).ToList();

                // attache note object and update
                Context.SellerNotes.Attach(sellernotes);
                sellernotes.Title = notes.Title.Trim();
                sellernotes.Category = notes.Category;
                sellernotes.NoteType = notes.NoteType;
                sellernotes.NumberOfPages = notes.NumberofPages;
                sellernotes.Descripton = notes.Description.Trim();
                sellernotes.Country = notes.Country;
                sellernotes.UniversityName = notes.UniversityName.Trim();
                sellernotes.Course = notes.Course.Trim();
                sellernotes.CourseCode = notes.CourseCode.Trim();
                sellernotes.Professor = notes.Professor.Trim();
                if (notes.IsPaid == true)
                {
                    sellernotes.IsPaid = true;
                    sellernotes.SellingPrice = notes.SellingPrice;
                }
                else
                {
                    sellernotes.IsPaid = false;
                    sellernotes.SellingPrice = 0;
                }
                sellernotes.ModifiedDate = DateTime.Now;
                sellernotes.ModifiedBy = user.ID;
                Context.SaveChanges();

                // if display picture is not null
                if (notes.DisplayPicture != null)
                {
                    // if note object has already previously uploaded picture then delete it
                    if (sellernotes.DisplayPicture != null)
                    {
                        string path = Server.MapPath(sellernotes.DisplayPicture);
                        FileInfo file = new FileInfo(path);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }

                    // save updated profile picture in directory and save directory path in database
                    string displaypicturefilename = System.IO.Path.GetFileName(notes.DisplayPicture.FileName);
                    string displaypicturepath = "~/Members/" + user.ID + "/" + sellernotes.ID + "/";
                    CreateDirectoryIfMissing(displaypicturepath);
                    string displaypicturefilepath = Path.Combine(Server.MapPath(displaypicturepath), displaypicturefilename);
                    sellernotes.DisplayPicture = displaypicturepath + displaypicturefilename;
                    notes.DisplayPicture.SaveAs(displaypicturefilepath);
                }

                // if note preview is not null
                if (notes.NotesPreview != null)
                {
                    // if note object has already previously uploaded note preview then delete it
                    if (sellernotes.NotesPreview != null)
                    {
                        string path = Server.MapPath(sellernotes.NotesPreview);
                        FileInfo file = new FileInfo(path);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }

                    // save updated note preview in directory and save directory path in database
                    string notespreviewfilename = System.IO.Path.GetFileName(notes.NotesPreview.FileName);
                    string notespreviewpath = "~/Members/" + user.ID + "/" + sellernotes.ID + "/";
                    CreateDirectoryIfMissing(notespreviewpath);
                    string notespreviewfilepath = Path.Combine(Server.MapPath(notespreviewpath), notespreviewfilename);
                    sellernotes.NotesPreview = notespreviewpath + notespreviewfilename;
                    notes.NotesPreview.SaveAs(notespreviewfilepath);
                }

                // check if user upload notes or not
                if (notes.UploadNotes[0] != null)
                {
                    // if user upload notes then delete directory that have previously uploaded notes
                    string path = Server.MapPath(notesattachement[0].FilePath);
                    DirectoryInfo dir = new DirectoryInfo(path);
                    EmptyFolder(dir);

                    // remove previously uploaded attachement from database
                    foreach (var item in notesattachement)
                    {
                        SellerNotesAttachement attachement = Context.SellerNotesAttachements.Where(x => x.ID == item.ID).FirstOrDefault();
                        Context.SellerNotesAttachements.Remove(attachement);
                    }

                    // add newly uploaded attachement in database and save it in database
                    foreach (HttpPostedFileBase file in notes.UploadNotes)
                    {
                        // check if file is null or not
                        if (file != null)
                        {
                            // save file in directory
                            string notesattachementfilename = System.IO.Path.GetFileName(file.FileName);
                            string notesattachementpath = "~/Members/" + user.ID + "/" + sellernotes.ID + "/Attachements/";
                            CreateDirectoryIfMissing(notesattachementpath);
                            string notesattachementfilepath = Path.Combine(Server.MapPath(notesattachementpath), notesattachementfilename);
                            file.SaveAs(notesattachementfilepath);

                            // create object of sellernotesattachement 
                            SellerNotesAttachement notesattachements = new SellerNotesAttachement
                            {
                                NoteID = sellernotes.ID,
                                FileName = notesattachementfilename,
                                FilePath = notesattachementpath,
                                CreatedDate = DateTime.Now,
                                CreatedBy = user.ID,
                                IsActive = true
                            };

                            // save seller notes attachement
                            Context.SellerNotesAttachements.Add(notesattachements);
                            Context.SaveChanges();
                        }
                    }
                }

                return RedirectToAction("Dashboard", "SellYourNote");
            }
            else
            {
                return RedirectToAction("EditNotes", new { id = notes.ID });
            }

        }


        [Route("SellYourNote/Publish")]
        public ActionResult PublishNote(int id)
        {
            // get note
            var note = Context.SellerNotes.Find(id);

            // if note is not found
            if (note == null)
            {
                return HttpNotFound();
            }
            // get logged in user
            var user = Context.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            // seller full name
            string sellername = user.FirstName + " " + user.LastName;

            if (user.ID == note.SellerID)
            {
                // update note status from draft to submitted for review
                Context.SellerNotes.Attach(note);
                note.Status = 11;
                //note.Status = Context.ReferenceDatas.Where(x => x.Value == "Submitted For Review").Select(x => x.ID).FirstOrDefault();
                // note.Status = Context.ReferenceDatas.Select(x => x.ID).FirstOrDefault(x => x.Value == "Submitted For Review");


                note.ModifiedDate = DateTime.Now;
                note.ModifiedBy = user.ID;
                Context.SaveChanges();

                // send mail to admin for publish note request
                PublishNotemail(note.Title, sellername);
            }

            return RedirectToAction("Home", "Home");
        }

        // send mail to admin for publish note request
        public void PublishNotemail(string note, string seller)
        {
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/") + "PublishNote" + ".cshtml");
            body = body.Replace("ViewBag.SellerName", seller);
            body = body.Replace("ViewBag.NoteTitle", note);
            body = body.ToString();

            // get support email
            var fromemail = Context.SystemConfigurations.Where(x => x.Key == "supportemail").FirstOrDefault();
            var tomail = Context.SystemConfigurations.Where(x => x.Key == "notifyemail").FirstOrDefault();

            // set from, to, subject, body
            string from, to, subject;
            from = fromemail.Value.Trim();
            to = tomail.Value.Trim();
            subject = seller + " sent his note for review";
            StringBuilder sb = new StringBuilder();
            sb.Append(body);
            body = sb.ToString();

            using (MailMessage mm = new MailMessage(from, to))
            {
                mm.Subject = subject;
                mm.Body = body;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    EnableSsl = true
                };
                NetworkCredential Network = new NetworkCredential(from, "001100R@63");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = Network;
                smtp.Port = 587;
                smtp.Send(mm);
            }


        }

        // delete files from directory or empty directory
        private void EmptyFolder(DirectoryInfo directory)
        {
            // check if directory have files
            if (directory.GetFiles() != null)
            {
                // delete all files
                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }
            }

            // check if directory have subdirectory
            if (directory.GetDirectories() != null)
            {
                // call emptyfolder and delete subdirectory
                foreach (DirectoryInfo subdirectory in directory.GetDirectories())
                {
                    EmptyFolder(subdirectory);
                    subdirectory.Delete();
                }
            }

        }

        // create directory
        private void CreateDirectoryIfMissing(string folderpath)
        {
            // check if directory exists
            bool folderalreadyexists = Directory.Exists(Server.MapPath(folderpath));
            // if directory does not exists then create
            if (!folderalreadyexists)
                Directory.CreateDirectory(Server.MapPath(folderpath));
        }

    }
}

