using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Notes_MarketPlace.Models;

namespace Notes_MarketPlace.Models
{
    public class AddNoteModel
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public int Category { get; set; }

        public string DisplayPicture { get; set; }

        public string UploadNotes { get; set; }

        public int NoteType { get; set; }

        public Nullable<int> NumberofPages { get; set; }

        public Nullable<int> Country { get; set; }

        public string UniversityName { get; set; }

        public string Course { get; set; }

        public string CourseCode { get; set; }

        public string Professor { get; set; }

        public string Description { get; set; }

        public string SellType { get; set; }

        public decimal SellingPrice { get; set; }

        public string NotePreview { get; set; }

        public void Maptomodel(SellerNote Note, SellerNotesAttachement Attachement)
        {
            Note.Title = Title;
            Note.Category = Category;
            Note.DisplayPicture = DisplayPicture;
            Note.NoteType = NoteType;
            Note.NumberOfPages = NumberofPages;
            Note.Descripton = Description;
            Note.UniversityName = UniversityName;
            Note.Country = Country;
            Note.Course = Course;
            Note.CourseCode = CourseCode;
            Note.Professor = Professor;
            Note.SellingPrice = SellingPrice;
            Note.NotesPreview = NotePreview;
            Attachement.FileName = UploadNotes;
        }

    }
}