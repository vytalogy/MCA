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

        [OutputCache(Duration = 90, VaryByParam = "q")]
        [Restricted]
        [HttpGet]
        public ActionResult Index(string q)
        {
            TempData["alert"] = TempData["alert"] == null ? string.Empty : TempData["alert"];
            TempData["msg"] = TempData["msg"] == null ? string.Empty : TempData["msg"];
            var query = _userDal.GetAll(q);
            ViewBag.qurey = !string.IsNullOrEmpty(q) ? q : string.Empty;
            return View(query);
        }
       
        [HttpGet]       
        public ActionResult Login()
        {            
            return View(new LoginViewModel { });
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {            
            string errorMsg = string.Empty;
            if (ModelState.IsValid)
            {                
                model.Email = model.Email.TrimStartEnd();
                model.Password = model.Password.TrimStartEnd().EncryptData();               
                var q = _userDal.Get(model.Email, model.Password,out errorMsg);
                if (q != null)
                {
                    System.Web.HttpContext.Current.Session["isLogin"] = "true";
                    System.Web.HttpContext.Current.Session["UserId"] = q.id;
                    System.Web.HttpContext.Current.Session["Email"] = q.Email;
                    System.Web.HttpContext.Current.Session["IsActive"] = q.Active ? "true" : "false";
                    System.Web.HttpContext.Current.Session["RoleName"] = q.RoleName;
                    System.Web.HttpContext.Current.Session["FirstName"] = q.FirstName;
                    System.Web.HttpContext.Current.Session["LastName"] = q.LastName;                               
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

        [OutputCache(Duration = 30, VaryByParam = "none")]
        [Restricted]
        [HttpGet]        
        public ActionResult Create()
        {         
            return View();
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
                    Email = model.Email,
                    CreatedBy = Auth.UserID,
                    Password = model.Password.EncryptData(),
                    RoleID = model.RoleID,
                };
                bool isSuccess = _userDal.Create(_user, out errorMsg);
                if (isSuccess)
                {
                    TempData["alert"] = "success";
                    TempData["msg"] = "Record saved successfully.";
                    return RedirectToAction("Index", "Account");
                }
                else
                { TempData["alert"] = "error";
                    TempData["msg"] = "Record not saved successfully.";
                }
            }

            return View(model);
        }

        [OutputCache(Duration = 30, VaryByParam = "id")]
        [Restricted]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null) 
                return HttpNotFound();            

            var _data = _userDal.GetByUserId(id);
            if (_data == null)            
                return HttpNotFound();           
            
            RegistorViewModel model = new RegistorViewModel
            {
                FirstName = _data.FirstName,
                LastName = _data.LastName,
                Email = _data.Email,
                RoleID = _data.RoleID,
                Password = _data.Password.DecryptData(),
            };
           
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
                    Email = model.Email,
                    Password = model.Password.EncryptData(),
                    RoleID = model.RoleID,
                };
                bool isSuccess = _userDal.Update(_user, out errorMsg);
                if (isSuccess)
                {
                    TempData["alert"] = "success";
                    TempData["msg"] = "Record saved successfully.";
                    return RedirectToAction("Index", "Account");
                }
                else
                {
                    TempData["alert"] = "error";
                    TempData["msg"] = "Record not saved successfully.";
                }
            }
            return View(model);
        }

        [HttpGet]
        public JsonResult Delete(int? id)
        {
            if (id == Auth.UserID)
            {
                TempData["alert"] = "error";
                TempData["msg"] = "Please login with another user then try again.";
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            bool isSuccess = _userDal.Delete(id);
            if (isSuccess)
            {
                TempData["alert"] = "success";
                TempData["msg"] = "Record deleted successfully.";
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                TempData["alert"] = "error";
                TempData["msg"] = "Please try again, record is not deleted.";
                return Json(true, JsonRequestBehavior.AllowGet);
            }
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

        [HttpGet]
        public ActionResult CheckExistingEmail(string Email,int? id)
        {
            bool ifEmailExist = false;
            try
            {
                ifEmailExist = _userDal.UserEmailAddress(Email,id) ? true : false;
                return Json(!ifEmailExist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
    }
}