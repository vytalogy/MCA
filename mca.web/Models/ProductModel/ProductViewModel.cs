using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mca.web.Models
{
    #region Product Detail Models
    public class ProductViewModel
    {
        public string ProductId { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public List<ProductViewItem> Items { get; set; }
    }

    public class ProductViewItem
    {
        public string MonthYear { get; set; }

        public string TopCustomer1 { get; set; } = string.Empty;
        public decimal TopCustomer1Qty { get; set; } = 0;
        public string TopCustomer2 { get; set; } = string.Empty;
        public decimal TopCustomer2Qty { get; set; } = 0;
        public string TopCustomer3 { get; set; } = string.Empty;
        public decimal TopCustomer3Qty { get; set; } = 0;
        public List<ProductViewLineItem> Items { get; set; }
    }

    public class ProductViewLineItem
    {
        public string WarehouseCode { get; set; }
        public decimal QuantityOnHand { get; set; } = 0;
        public decimal Customer1Qty { get; set; } = 0;
        public decimal Customer2Qty { get; set; } = 0;
        public decimal Customer3Qty { get; set; } = 0;
        public decimal Transfer { get; set; } = 0;
        public decimal ShippedQuantity { get; set; } = 0;
        public decimal PurhaseOrder { get; set; } = 0;
        public decimal ProjActQuantity { get; set; } = 0;
    }

    #endregion


    #region Update Model
    /// <summary>
    /// Product
    /// </summary>
    public class ProductForcastingModel
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