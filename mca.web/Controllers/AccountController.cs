using mca.web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mca.dal;

namespace mca.web.Controllers
{
    public class AccountController : Controller
    {

        [HttpGet]
        //[OutputCache(Duration = 30, VaryByParam = "none")]
        public ActionResult Login()
        {
            LoginViewModel login = new LoginViewModel { };
            return View(login);
        }


        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserDAL user = new UserDAL { };
                var q = user.Get(model.Email, model.Password);
                if (q != null)
                    return RedirectToAction("Home", "Products");
                else
                    ModelState.AddModelError("Please provide valid user name and password", "Email");
            }
            return View(model);
        }


        [HttpGet]        
        public ActionResult Registrator()
        {
            LoginViewModel login = new LoginViewModel { };
            return View(login);
        }

        [HttpPost]
        public ActionResult Registrator(RegistorViewModel model)
        {         
            return View(model);
        }
    }
}