using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mca.providex;


namespace mca.web.Controllers
{
    public class ProductsController : Controller
    {
        [HttpGet]
        public ActionResult Home()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Search()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Selector()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Detail(string productID)
        {
            ViewBag.productID = productID;
            return View();
        }

        [HttpGet]
        public ActionResult Forecasting()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Analytics()
        {
            return View();
        }
        
        [HttpPost]
        public JsonResult Autocomplete(string Prefix)
        {
            ProductDAL _product = new ProductDAL();
            var EmpDet = _product.GetItemList(Prefix).Select(s => new { Name = s.Text, Id = s.Value });
            return Json(EmpDet, JsonRequestBehavior.AllowGet);
        }
    }
}