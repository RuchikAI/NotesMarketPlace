using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Notes_MarketPlace.Models
{
    
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression("^[A-Za-z0-9@]+$")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}