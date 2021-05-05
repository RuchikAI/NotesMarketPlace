using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notes_MarketPlace.Models
{
    public class SearchNotesModel
    {
        public SellerNote Note { get; set; }
        public int AverageRating { get; set; }
        public int TotalRating { get; set; }
        public int TotalSpam { get; set; }
    }
}