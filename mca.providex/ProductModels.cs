using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace mca.providex
{   
    public class ProductDataModel
    {
        public List<ProductDataItem> Items { get; set; }
    }

    public class ProductDataItem
    {
        public string MonthYear { get; set; }      
        public string TopCustomer1 { get; set; } = string.Empty;
        public decimal TopCustomer1Qty { get; set; } = 0;
        public string TopCustomer2 { get; set; } = string.Empty;
        public decimal TopCustomer2Qty { get; set; } = 0;
        public string TopCustomer3 { get; set; } = string.Empty;
        public decimal TopCustomer3Qty { get; set; } = 0;
        public List<ProductDataLineItem> Items { get; set; }
    }

    public class ProductDataLineItem
    {      
        public string WarehouseCode { get; set; }
        public decimal Customer1Qty { get; set; } = 0;
        public decimal Customer2Qty { get; set; } = 0;
        public decimal Customer3Qty { get; set; } = 0;
        public decimal QuantityOnHand { get; set; } = 0;
        public decimal Transfer { get; set; } = 0;
        public decimal ShippedQuantity { get; set; } = 0;
        public decimal PurhaseOrder { get; set; } = 0;
    }
    
}
