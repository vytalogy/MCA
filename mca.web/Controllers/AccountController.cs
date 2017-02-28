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
        #region Lazy Object Initializer

        private Lazy<UserDAL> _user = new Lazy<UserDAL>(() =>
        {
            var _user = new UserDAL { };          
            return _user;
        });

        public UserDAL _userDal
        {
            get { return _user.Value; }
        }

        #endregion
       
        [Restricted]
        [HttpGet]
        public ActionResult Index()
        {
            var query = _userDal.GetAll();
            return View(query);
        }
       
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
                var q = _userDal.Get(model.Email, model.Password,out errorMsg);
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

        [Restricted]
        [HttpGet]        
        public ActionResult Create()
        {
            RegistorViewModel model = new RegistorViewModel { RolesList = new List<SelectListItem> { } };
            model.RolesList = new List<SelectListItem> { };
            model.RolesList = _userDal.GetRoles().Select(item => new SelectListItem
            {
                 Text = item.Text,
                 Value = item.Value,
            }).ToList();
            return View(model);
        }

        [Restricted]
        [HttpPost]
        public ActionResult Create(RegistorViewModel model)
        {
            if (ModelState.IsValid)
            {
                string errorMsg = string.Empty;
                mca.model.User _user = new mca.model.User
                {

                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.UserName,
                    CreatedBy = Auth.UserID,
                    Password = model.Password,
                    RoleID = model.RoleID,
                };
                bool isSuccess = _userDal.Create(_user, out errorMsg);
                if (isSuccess)
                {
                    TempData["alert"] = "success";
                    return RedirectToAction("Index", "Account");
                }
                else
                { TempData["alert"] = "error"; }
            }

            model.RolesList = _userDal.GetRoles().Select(item => new SelectListItem
            {
                Text = item.Text,
                Value = item.Value,
            }).ToList();

            return View(model);
        }

        [Restricted]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null) {
                return HttpNotFound();
            }


            var _data = _userDal.GetByUserId(id);
            if (_data == null)
            {
                return HttpNotFound();
            }


            RegistorViewModel model = new RegistorViewModel
            {
                FirstName = _data.FirstName,
                LastName = _data.LastName,
                UserName = _data.UserName,
               // Password = _data.Password,
                RoleID = _data.RoleID,
            };

            model.RolesList = _userDal.GetRoles().Select(item => new SelectListItem
            {
                Text = item.Text,
                Value = item.Value,
            }).ToList();
            return View(model);
        }

        [Restricted]
        [HttpPost]
        public ActionResult Edit(RegistorViewModel model)
        {
            if (ModelState.ContainsKey("Password"))
                ModelState["Password"].Errors.Clear();

            if (ModelState.IsValid)
            {
                string errorMsg = string.Empty;
                mca.model.User _user = new mca.model.User
                {
                    id = model.id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.UserName,                   
                    RoleID = model.RoleID,
                };
                bool isSuccess = _userDal.Update(_user, out errorMsg);
                if (isSuccess)
                {
                    TempData["alert"] = "success";
                    return RedirectToAction("Index", "Account");
                }
                else
                { TempData["alert"] = "error"; }
            }

            model.RolesList = _userDal.GetRoles().Select(item => new SelectListItem
            {
                Text = item.Text,
                Value = item.Value,
            }).ToList();
            return View(model);
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            { return HttpNotFound(); }
            _userDal.Delete(id); 
            TempData["alert"] = "success";
            return RedirectToAction("Index", "Account");
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