using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mca.providex;
using mca.web.Models;
using mca.web.Helpers;
using mca.dal;
using mca.model;

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
            dynamic _productDetail = _product.GetItemDetail(productID);
            ProductDetailModel model = new ProductDetailModel { Items = new List<mca.web.Models._ProductItem> { } };
            model.ProductId = _productDetail.CI_Item;
            model.Description = !string.IsNullOrEmpty(_productDetail.ItemCodeDesc) ? _productDetail.ItemCodeDesc : "";
            model.Brand = !string.IsNullOrEmpty(_productDetail.UDF_BRAND) ? _productDetail.UDF_BRAND : "None";

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

            _productDetail = SubItems = null;
            return View(model);
        }

        [HttpGet]
        public ActionResult Forecasting(string productID)
        {
            TempData["alert"] = "Oop";
            ProductDAL _product = new ProductDAL { };
            ProductUpdateModel model = new ProductUpdateModel { isDataFound = false };
            if (!string.IsNullOrEmpty(productID))
            {
                dynamic _productDetail = _product.GetItemDetail(productID);
                model = new ProductUpdateModel { };
                model.ProductId = _productDetail.CI_Item;
                model.Description = !string.IsNullOrEmpty(_productDetail.ItemCodeDesc) ? _productDetail.ItemCodeDesc : "";
                model.Brand = !string.IsNullOrEmpty(_productDetail.UDF_BRAND) ? _productDetail.UDF_BRAND : "None";
                model.isDataFound = true;

                model.List = new List<SelectListItem> { };
                for (int i = 1; i <= 12; i++)
                {
                    var currentmont = string.Format("{0:MMMM}", DateTime.Now.AddMonths(i));
                    model.List.Add(new SelectListItem { Selected = false, Text = currentmont, Value = string.Empty });
                }
            }
            
            return View(model);
        }

        [HttpPost]
        public ActionResult Forecasting(ProductUpdateModel _data)
        {
            mca.model.ProductUpdateDataModel model = new mca.model.ProductUpdateDataModel
            {
                ProductCode = _data.ProductId,
                Description = _data.Description,
                isYearly = _data.isYearly == "Annual" ? true : false,
                Defaultvalue = _data.Defaultvalue,
                CreatedBy = Auth.UserID,
                CreatedByName = Auth.UserName,
                ModifyBy = Auth.UserID,
                ModifyByName = Auth.UserName,
            };

            model.List = new List<mca.model.SelectList> { };
            if (_data.List != null && _data.List.Count() > 0)
            {
                model.List.AddRange(_data.List.Select(item => new mca.model.SelectList
                {
                    Selected = item.Selected,
                    Text = item.Text,
                    Value = model.isYearly ? "3" : item.Value, //only 3 value assign when yearly is selected.
                }));
            }

            mca.dal.ProdcutDAL _product = new mca.dal.ProdcutDAL { };
            var isSuccess = _product.AddProduct(model);
            if (string.IsNullOrEmpty(isSuccess.Item2))
            {
                _product.AddForecasting(model.List, isSuccess.Item1);
                TempData["alert"] = "success";
            }
            else { TempData["alert"] = "error"; TempData["msg"] = isSuccess.Item2; }
            return View(_data);
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