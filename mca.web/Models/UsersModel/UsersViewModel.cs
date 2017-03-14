using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mca.web.Models
{

    #region Login User

    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }


    public class RegistorViewModel
    {
        public int id { set; get; }

        [Required(ErrorMessage = "The first name is required.")]
        public string FirstName { set; get; }

        [Required(ErrorMessage = "The last name is required.")]
        public string LastName { set; get; }

        [Required(ErrorMessage = "The email is required.")]
        [EmailAddress]
        [Remote("CheckExistingEmail","Account",AdditionalFields ="id", ErrorMessage = "Email already exists!")]
        public string Email { set; get; }

        [StringLength(20, MinimumLength = 10, ErrorMessage = "Password at least 10 alphabets")]
        [Required(ErrorMessage = "The password is required.")]
        public string Password { set; get; }

        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { set; get; }

        [Required(ErrorMessage = "The role is required.")]
        public int RoleID { set; get; }       
    }

    #endregion
}