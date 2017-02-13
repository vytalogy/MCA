using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
        [Required]
        [EmailAddress]
        public String Email { set; get; }
        [Required]
        public String Password { set; get; }
        [Required]
        public String SecretQuestion { set; get; }
        [Required]
        public String SecretAnswer { set; get; }
    }

    #endregion
}