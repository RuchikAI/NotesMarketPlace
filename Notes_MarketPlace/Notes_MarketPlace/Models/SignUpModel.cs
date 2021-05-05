using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Notes_MarketPlace.Models
{
    public class SignUpModel
    {

        public int ID { get; set; }

        [Required(AllowEmptyStrings=false, ErrorMessage = "First Name is Required.")]
        public string FirstName { get; set; }



        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is Required.")]
        public string LastName { get; set; }



        [Required(AllowEmptyStrings = false, ErrorMessage = "Email ID is Required.")]
        public string EmailID { get; set; }



        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is Required.")]
        public string Password { get; set; }



        [Required(AllowEmptyStrings = false, ErrorMessage = "Confirm-Password is Required.")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password should be same.")]
        public string ConfirmPassword { get; set; }
    }
}