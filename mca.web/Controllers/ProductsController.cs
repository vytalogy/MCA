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
        #region Lazy Object Initializer

        private Lazy<ProductRepository> repository = new Lazy<ProductRepository>(() =>
        {
            var cache = new ProductRepository { };
            return cache;
        });

        public ProductRepository _ProductRepo
        {
            get { return repository.Value; }
        }

        private Lazy<ProdcutDAL> productDAL = new Lazy<ProdcutDAL>(() =>
        {
            var cache = new ProdcutDAL { };
            return cache;
        });

        public ProdcutDAL _ProductDAL
        {
            get { return productDAL.Value; }
        }

        #endregion

        [OutputCache(Duration = 30, VaryByParam = "none")]
        [HttpGet]
        public ActionResult Home()
        {
            return View();
        }

        [OutputCache(Duration = 30, VaryByParam = "none")]
        [HttpGet]
        public ActionResult Search()
        {
            return View();
        }

        [OutputCache(Duration = 30, VaryByParam = "none")]
        [HttpGet]
        public ActionResult Selector()
        {
            return View();
        }

        [OutputCache(Duration = 90, VaryByParam = "productID")]
        [HttpGet]
        public ActionResult Detail(string productID)
        {            
            dynamic _productDetail = _ProductRepo.GetItemDetail(productID);
            ProductViewModel model = new ProductViewModel { Items = new List<ProductViewItem> { } };
            model.ProductId = _productDetail.CI_Item;
            model.Description = !string.IsNullOrEmpty(_productDetail.ItemCodeDesc) ? _productDetail.ItemCodeDesc : string.Empty;
            model.Brand = !string.IsNullOrEmpty(_productDetail.UDF_BRAND) ? _productDetail.UDF_BRAND : "None";          
            
            var SubItems = _ProductRepo.GetItemWareHouse(productID);
            model.Items.AddRange(SubItems.Items.Select(item => new ProductViewItem
            {
                MonthYear = item.MonthYear,
                TopCustomer1 = item.TopCustomer1,
                TopCustomer1Qty = item.TopCustomer1Qty,
                TopCustomer2 = item.TopCustomer2,
                TopCustomer2Qty = item.TopCustomer2Qty,
                TopCustomer3 = item.TopCustomer3,
                TopCustomer3Qty = item.TopCustomer3Qty,
                Items = item.Items.Select(subItem => new ProductViewLineItem
                {
                    Customer1Qty = subItem.Customer1Qty,
                    Customer2Qty = subItem.Customer2Qty,
                    Customer3Qty = subItem.Customer3Qty,
                    WarehouseCode = subItem.WarehouseCode,
                    Transfer = subItem.Transfer,
                    ShippedQuantity = subItem.ShippedQuantity,
                    PurhaseOrder = subItem.PurhaseOrder,
                    QuantityOnHand = subItem.QuantityOnHand,
                    ProjActQuantity = subItem.ProjActQuantity,
                }).ToList(),
            }));

            _productDetail = SubItems = null;
            return View(model);
        }

        [OutputCache(Duration = 30, VaryByParam = "productID")]
        [HttpGet]
        public ActionResult Forecasting(string productID)
        {
            TempData["alert"] = "Oop";           
            ProductForcastingModel model = new ProductForcastingModel { isDataFound = false };
            if (!string.IsNullOrEmpty(productID))
            {
                dynamic _productDetail = _ProductRepo.GetItemDetail(productID);
                model = new ProductForcastingModel { };
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
        public ActionResult Forecasting(ProductForcastingModel _data)
        {
            mca.model.ProductUpdateDataModel model = new ProductUpdateDataModel
            {
                ProductCode = _data.ProductId,
                Description = _data.Description,
                isYearly = _data.isYearly == "Annual" ? true : false,
                Defaultvalue = _data.Defaultvalue,
                CreatedBy = Auth.UserID,
                CreatedByName = Auth.Email,
                ModifyBy = Auth.UserID,
                ModifyByName = Auth.Email,
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

            var isSuccess = _ProductDAL.AddProduct(model);
            if (string.IsNullOrEmpty(isSuccess.Item2))
            {
                _ProductDAL.AddForecasting(model.List, isSuccess.Item1);
                TempData["alert"] = "success";
            }
            else { TempData["alert"] = "error"; TempData["msg"] = isSuccess.Item2; }
            return View(_data);
        }

        [OutputCache(Duration = 30, VaryByParam = "none")]
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
            
            var Item = _ProductRepo.GetItemByFilter(Prefix, CustomEnum.SearchByItemCode).Select(s => new
            {
                Name = s[1].ConvertToString(),
                Id = s[0].ConvertToString()
            });

            return Json(Item, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult _ProductFilter(string filter, string Header)
        {
            ViewBag.Header = Header;
            var Item = _ProductRepo.GetItemByFilter(filter, CustomEnum.SearchByItemCodeDesc).Select(s => new SelectListItem
            {
                Text = s[1].ConvertToString(),
                Value = s[0].ConvertToString()
            });

            return PartialView(Item);
        }

        [OutputCache(Duration = 30, VaryByParam = "itemCode;Month;Year;")]
        [HttpGet]
        public PartialViewResult _GetItemWareHouseDetail(string itemCode, string Month, string Year)
        {
            var Item = _ProductRepo.GetItemWareHouseDetail(itemCode, Month,Year);
            return PartialView(Item);
        }
    }
}