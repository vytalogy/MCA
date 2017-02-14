using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mca.providex;


namespace mca.web.Controllers
{
    [Restricted]
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

        [HttpGet]
        public JsonResult Autocomplete(string Prefix)
        {
            if (string.IsNullOrEmpty(Prefix))
                return null;

            ProductDAL _product = new ProductDAL { };
            var EmpDet = _product.GetItemByFilter(Prefix, ProductEnums.SearchByItemCode).Select(s => new { Name = s.Text, Id = s.Value });
            return Json(EmpDet, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult _ProductFilter(string filter,string Header)
        {
            ViewBag.Header = Header;
            ProductDAL _product = new ProductDAL();
            var EmpDet = _product.GetItemByFilter(filter, ProductEnums.SearchByItemCodeDesc).Select(s => new SelectListItem { Text = s.Text, Value = s.Value });
            return PartialView(EmpDet);
        }
    }
}