using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace mca.providex
{    
    public enum ProductEnums
    {
        SearchByItemCode = 1,
        SearchByItemCodeDesc = 2,
    }

    public class _ProductModel
    {
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
        public string Customer1Qty { get; set; } = string.Empty;
        public string Customer2Qty { get; set; } = string.Empty;
        public string Customer3Qty { get; set; } = string.Empty;
        public string QuantityOnHand { get; set; }       
    }   
}
