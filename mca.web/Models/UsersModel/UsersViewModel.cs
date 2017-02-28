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
        [Required]
        public string FirstName { set; get; }
        [Required]
        public string LastName { set; get; }
        [Required]
        [EmailAddress]
        public string UserName { set; get; }
        [Required]
        public string Password { set; get; }
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { set; get; }
        [Required]
        public int RoleID { set; get; }
        public List<SelectListItem> RolesList { set; get; }
    }

    #endregion
}