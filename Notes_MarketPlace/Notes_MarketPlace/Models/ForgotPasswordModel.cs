using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Notes_MarketPlace.Models
{
    public class ForgotPasswordModel
    {
        [Required(AllowEmptyStrings =false,ErrorMessage ="EmailID is required")]
        public string Email { get; set; }
    }
}