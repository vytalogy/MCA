using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mca.web.Models
{
    #region Product Detail Models
    public class ProductDetailModel
    {
        public string ProductId { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public List<_ProductItem> Items { get; set; }
    }

    public class _ProductItem
    {
        public string MonthYear { get; set; }
        public string TotalOnHand { get; set; }

        public string TopCustomer1 { get; set; } = string.Empty;
        public string TopCustomer1Qty { get; set; } = string.Empty;
        public string TopCustomer2 { get; set; } = string.Empty;
        public string TopCustomer2Qty { get; set; } = string.Empty;
        public string TopCustomer3 { get; set; } = string.Empty;
        public string TopCustomer3Qty { get; set; } = string.Empty;
        public List<_ProductDetail> Items { get; set; }
    }

    public class _ProductDetail
    {
        public string WarehouseCode { get; set; }
        public string QuantityOnHand { get; set; }
        public string Customer1Qty { get; set; } = string.Empty;
        public string Customer2Qty { get; set; } = string.Empty;
        public string Customer3Qty { get; set; } = string.Empty;
    }

    #endregion


    #region Update Model
    public class ProductUpdateModel
    {
        public string ProductId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public bool isDataFound { get; set; }
        /// <summary>
        /// Using for foreCasting and default value should be 3
        /// </summary>
        public string Defaultvalue { get; set; } = "3";
        public string isYearly { get; set; } = "Annual";
        public List<SelectListItem> List { get; set; }
    }   

    #endregion
}