using mca.web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mca.dal;
using mca.web.Helpers;

namespace mca.web.Controllers
{
    [NotRestricted]
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
            string errorMsg = string.Empty;
            if (ModelState.IsValid)
            {
                model.Email = model.Email.TrimStartEnd();
                model.Password = model.Password.TrimStartEnd();
                UserDAL user = new UserDAL { };
                var q = user.Get(model.Email, model.Password,out errorMsg);
                if (q != null)
                {
                    System.Web.HttpContext.Current.Session["isLogin"] = "true";
                    System.Web.HttpContext.Current.Session["UserId"] = q.id;
                    System.Web.HttpContext.Current.Session["UserName"] = q.UserName;
                    System.Web.HttpContext.Current.Session["IsActive"] = q.Active ? "true" : "false";
                    System.Web.HttpContext.Current.Session["RoleName"] = q.RoleName;

                    return RedirectToAction("Home", "Products");
                }
                else
                {
                    TempData["error"] = "Please provide valid user name and password";
                    ModelState.AddModelError("Please provide valid user name and password", "Email");
                }
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

        [HttpGet]
        public ActionResult Logout()
        {           
            Session["isLogin"] = "false";
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();

            HttpCookie LoginAuthKey = new HttpCookie("AuthKey", "-");
            LoginAuthKey.Expires = DateTime.Now.AddDays(-30d);
            Response.Cookies.Add(LoginAuthKey);

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            
            return RedirectToAction("Login", "Account");
        }
    }
}