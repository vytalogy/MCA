using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace mca.web.Models
{
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

}