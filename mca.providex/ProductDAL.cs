using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using mca.providex;
using System.Collections;
using Dapper;
using System.Dynamic;

namespace mca.providex
{
    public class ProductDAL : BaseClass
    {
        public List<CustomListItem> GetItemByFilter(string filter, ProductEnums value)
        {
            List<CustomListItem> List = new List<CustomListItem> { };
            string query = string.Empty;
            if (value == ProductEnums.SearchByItemCode)
                query = @"select ItemCode,ItemCodeDesc from CI_Item where itemcode like @filter +'%' or ItemCodeDesc like '%' + @filter + '%'";
            else if (value == ProductEnums.SearchByItemCodeDesc)
                query = @"select ItemCode,ItemCodeDesc from CI_Item where ItemCodeDesc like'%' + @filter + '%'";

            List<dynamic> _data = this._db.Query<dynamic>(query, new { filter = filter }).ToList();
            if (_data != null && _data.Count() > 0)
                List = _data.Select(item => new CustomListItem { Value = item.ItemCode, Text = item.ItemCodeDesc }).ToList();

            _data = null;
            query = string.Empty;
            return List;
        }

        public ProductModels GetItemByFilter(string filter)
        {
            string query = @"select CI_Item = ItemCode,ItemCodeDesc,UDF_BRAND from CI_Item where itemcode = @filter";
            ProductModels _data = this._db.Query<ProductModels>(query, new { filter = filter }).FirstOrDefault();
            return _data;
        }

        public _ProductModel GetItemWareHouse(string filter)
        {
            _ProductModel model = new _ProductModel { Items = new List<_ProductItem> { } };
            int lastdayofmonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month == 12 ? DateTime.Now.Month : DateTime.Now.Month + 1);
            string startdate = string.Format("{0:yyyy-MM}", DateTime.Now.AddMonths(-24)) + "-01";
            string enddate = string.Format("{0:yyyy-MM}", DateTime.Now.AddMonths(+13)) + "-" + lastdayofmonth;

            string query = @"select YEAR = YEAR(TransactionDate), MONTH = MONTH(TransactionDate), MMM = UPPER(left(DATENAME(MONTH, TransactionDate), 3))
                                FROM IM_ItemTransactionHistory where ItemCode = @filter and (TransactionDate >= @startdate and TransactionDate <= @enddate)
                                GROUP BY YEAR(TransactionDate), MONTH(TransactionDate), DATENAME(MONTH, TransactionDate)
                                ORDER BY YEAR desc, MONTH desc";

            List<dynamic> _data = this._db.Query<dynamic>(query, new { filter = filter, startdate = startdate, enddate = enddate }).ToList();
            foreach (var item in _data)
            {                
                lastdayofmonth = DateTime.DaysInMonth(int.Parse(item.YEAR.ToString()), int.Parse(item.MONTH.ToString()));
                startdate = item.YEAR.ToString() + "-" + item.MONTH.ToString() + "-01";
                enddate = item.YEAR.ToString() + "-" + item.MONTH.ToString() + "-" + lastdayofmonth;

                _ProductItem _ProductItem = new _ProductItem { MonthYear = item.MMM.ToString() + ", " + item.YEAR.ToString(), Items = new List<_ProductDetail> { } };

                query = @"select top 3 Quantity=sum(TransactionQty),CustomerNo From IM_ItemTransactionHistory
                         where ItemCode = @filter AND (TransactionDate BETWEEN @startdate AND @enddate) and CustomerNo is not null
                         group by CustomerNo order by CustomerNo";
                List<dynamic> TopCustomer = this._db.Query<dynamic>(query, new { filter = filter, startdate = startdate, enddate = enddate }).ToList();
                if (TopCustomer != null && TopCustomer.Count() > 0)
                {
                    if (TopCustomer.Count() == 3)
                    {
                        _ProductItem.TopCustomer1 = TopCustomer[0].CustomerNo;
                        _ProductItem.TopCustomer1Qty = Math.Abs(TopCustomer[0].Quantity).ToString();

                        _ProductItem.TopCustomer2 = TopCustomer[1].CustomerNo;
                        _ProductItem.TopCustomer2Qty = Math.Abs(TopCustomer[1].Quantity).ToString();

                        _ProductItem.TopCustomer3 = TopCustomer[2].CustomerNo;
                        _ProductItem.TopCustomer3Qty = Math.Abs(TopCustomer[2].Quantity).ToString();
                    }
                    else if (TopCustomer.Count() == 2)
                    {
                        _ProductItem.TopCustomer1 = TopCustomer[0].CustomerNo;
                        _ProductItem.TopCustomer1Qty = Math.Abs(TopCustomer[0].Quantity).ToString();

                        _ProductItem.TopCustomer2 = TopCustomer[1].CustomerNo;
                        _ProductItem.TopCustomer2Qty = Math.Abs(TopCustomer[1].Quantity).ToString();
                    }
                    else if (TopCustomer.Count() == 1)
                    {
                        _ProductItem.TopCustomer1 = TopCustomer[0].CustomerNo;
                        _ProductItem.TopCustomer1Qty = Math.Abs(TopCustomer[0].Quantity).ToString();
                    }                    
                }
                TopCustomer = null;

                query = @"select WarehouseCode From IM_ItemTransactionHistory where ItemCode = @filter 
                            AND (TransactionDate BETWEEN @startdate AND @enddate) group by WarehouseCode";
                List<dynamic> WarehouseCode = this._db.Query<dynamic>(query, new { filter = filter, startdate = startdate, enddate = enddate }).ToList();
                if (WarehouseCode != null && WarehouseCode.Count() > 0)
                {
                    decimal TotalQuantityOnHand = 0;
                    foreach (var subitem in WarehouseCode)
                    {
                        _ProductDetail detail = new _ProductDetail { };
                        detail.WarehouseCode = subitem.WarehouseCode;

                        query = @"select top 3 Quantity=sum(TransactionQty) From IM_ItemTransactionHistory
                         where ItemCode = @filter AND (TransactionDate BETWEEN @startdate AND @enddate) and CustomerNo is not null
                         and WarehouseCode = @WarehouseCode group by CustomerNo order by CustomerNo";
                        List<dynamic> TopCustomer1 = this._db.Query<dynamic>(query, new { filter = filter, startdate = startdate, enddate = enddate, WarehouseCode = detail.WarehouseCode }).ToList();
                        if (TopCustomer1 != null && TopCustomer1.Count() > 0)
                        {
                            if (TopCustomer1.Count() == 3)
                            {                                
                                detail.Customer1Qty = Math.Abs(TopCustomer1[0].Quantity).ToString();
                                detail.Customer2Qty = Math.Abs(TopCustomer1[1].Quantity).ToString();                               
                                detail.Customer3Qty = Math.Abs(TopCustomer1[2].Quantity).ToString();
                            }
                            else if (TopCustomer1.Count() == 2)
                            {
                                detail.Customer1Qty = Math.Abs(TopCustomer1[0].Quantity).ToString();
                                detail.Customer2Qty = Math.Abs(TopCustomer1[1].Quantity).ToString();
                            }
                            else if (TopCustomer1.Count() == 1)
                            {
                                detail.Customer3Qty = Math.Abs(TopCustomer1[0].Quantity).ToString();
                            }
                        }
                        TopCustomer1 = null;

                        query = @"select QuantityOnHand From IM_ItemWarehouse where ItemCode = @filter and WarehouseCode = @WarehouseCode";
                        List<dynamic> QuantityList = this._db.Query<dynamic>(query, new { filter = filter, WarehouseCode = detail.WarehouseCode, }).ToList();
                        if (QuantityList != null)
                        {
                            foreach (var qItem in QuantityList)
                            {
                                if (qItem.QuantityOnHand > 0)
                                {
                                    TotalQuantityOnHand += Convert.ToDecimal(qItem.QuantityOnHand);
                                    detail.QuantityOnHand = qItem.QuantityOnHand.ToString();
                                }
                                else
                                { detail.QuantityOnHand = "0"; }                                
                            }
                        }                    

                        _ProductItem.Items.Add(detail);                        
                    }

                    _ProductItem.TotalOnHand = $"{TotalQuantityOnHand:0}";
                    TotalQuantityOnHand = 0;
                }
                
                model.Items.Add(_ProductItem);
                WarehouseCode = null;
            }
            _data = null;
            return model;
        }
    }
}
