using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mca.providex;
using mca.web.Models;
using mca.web.Helpers;

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
            ProductDAL _product = new ProductDAL { };
            dynamic EmpDet = _product.GetItemDetail(productID);
            ProductDetailModel model = new ProductDetailModel { Items = new List<mca.web.Models._ProductItem> { } };
            model.ProductId = EmpDet.CI_Item;
            model.Description = !string.IsNullOrEmpty(EmpDet.ItemCodeDesc) ? EmpDet.ItemCodeDesc : "";
            model.Brand = !string.IsNullOrEmpty(EmpDet.UDF_BRAND) ? EmpDet.UDF_BRAND : "None";

            var SubItems = _product.GetItemWareHouse(productID);
            model.Items.AddRange(SubItems.Items.Select(item => new mca.web.Models._ProductItem
            {
                MonthYear = item.MonthYear,
                TotalOnHand = item.TotalOnHand,
                TopCustomer1 = item.TopCustomer1,
                TopCustomer1Qty = item.TopCustomer1Qty,
                TopCustomer2 = item.TopCustomer2,
                TopCustomer2Qty = item.TopCustomer2Qty,
                TopCustomer3 = item.TopCustomer3,
                TopCustomer3Qty = item.TopCustomer3Qty,
                Items = item.Items.Select(subItem => new mca.web.Models._ProductDetail
                {
                    Customer1Qty = subItem.Customer1Qty,
                    Customer2Qty = subItem.Customer2Qty,
                    Customer3Qty = subItem.Customer3Qty,
                    WarehouseCode = subItem.WarehouseCode,
                    QuantityOnHand = $"{subItem.QuantityOnHand.ConvertToDecimal():0}",
                }).ToList(),
            }));

            EmpDet = SubItems = null;
            return View(model);
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
            var EmpDet = _product.GetItemByFilter(Prefix, ProductEnums.SearchByItemCode).Select(s => new { Name = s[1].ConvertToString(), Id = s[0].ConvertToString() });
            return Json(EmpDet, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult _ProductFilter(string filter,string Header)
        {
            ViewBag.Header = Header;
            ProductDAL _product = new ProductDAL();
            var EmpDet = _product.GetItemByFilter(filter, ProductEnums.SearchByItemCodeDesc).Select(s => new SelectListItem { Text = s[1].ConvertToString(), Value = s[0].ConvertToString() });
            return PartialView(EmpDet);
        }
    }
}