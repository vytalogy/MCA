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
using System.Globalization;

namespace mca.providex
{
    public class ProductRepository : BaseClass
    {
        public List<ArrayList> GetItemByFilter(string filter, CustomEnum value)
        {
            List<ArrayList> List = new List<ArrayList> { };
            string query = string.Empty;
            if (value == CustomEnum.SearchByItemCode)
                // query = @"select ItemCode,ItemCodeDesc from CI_Item where itemcode like @filter +'%' or ItemCodeDesc like '%' + @filter + '%'";
                query = @"select distinct ItemCode,ItemCodeDesc from CI_Item item
                        where item.ItemCode in (select ItemCode from IM_ItemWarehouse where WarehouseCode not in  ('FR', '0000', 'CHN', 'DCH','USA'))
                        and (item.itemcode like @filter +'%' or item.ItemCodeDesc like '%' + @filter + '%')";
            else if (value == CustomEnum.SearchByItemCodeDesc)
                //query = @"select ItemCode,ItemCodeDesc from CI_Item where ItemCodeDesc like'%' + @filter + '%'";
                query = @"select distinct ItemCode,ItemCodeDesc from CI_Item item
                          where item.ItemCode in (select ItemCode from IM_ItemWarehouse where WarehouseCode not in  ('FR', '0000', 'CHN', 'DCH','USA'))
                          and item.ItemCodeDesc like '%' + @filter + '%'";

            List<dynamic> _data = this._db.Query<dynamic>(query, new { filter = filter }).ToList();
            if (_data != null && _data.Count() > 0)
            {
                List = _data.Select(item => new ArrayList
                {
                    item.ItemCode,
                    item.ItemCodeDesc
                }).ToList();
            }

            _data = null;
            query = string.Empty;
            return List;
        }

        public dynamic GetItemDetail(string filter)
        {
            string query = @"select CI_Item = ItemCode,ItemCodeDesc,UDF_BRAND from CI_Item where itemcode = @filter";
            dynamic _data = this._db.Query<dynamic>(query, new { filter = filter }).FirstOrDefault();
            return _data;
        }

        public ProductDataModel GetItemWareHouse(string filter)
        {
            ProductDataModel model = new ProductDataModel
            {
                Items = new List<ProductDataItem> { }
            };

            int lastdayofmonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month == 12 ? DateTime.Now.Month : DateTime.Now.Month + 1);
            // string startdate = "2016-02-01";
            //string enddate = "2016-02-29";

            string startdate = string.Format("{0:yyyy-MM}", DateTime.Now.AddMonths(-24)) + "-01";
            string enddate = string.Format("{0:yyyy-MM}", DateTime.Now.AddMonths(+13)) + "-" + lastdayofmonth;

            string query = @"SELECT YEAR = YEAR(TRANSACTIONDATE), MONTH = MONTH(TRANSACTIONDATE), MMM = UPPER(LEFT(DATENAME(MONTH, TRANSACTIONDATE), 3))
                                FROM IM_ITEMTRANSACTIONHISTORY WHERE ITEMCODE = @filter AND (TRANSACTIONDATE >= @startdate AND TRANSACTIONDATE <= @enddate)
                                and WarehouseCode not in  ('FR', '0000', 'CHN', 'DCH','USA') and CustomerNo is not null and TransactionCode='so'
                                GROUP BY YEAR(TRANSACTIONDATE), MONTH(TRANSACTIONDATE), DATENAME(MONTH, TRANSACTIONDATE)
                                ORDER BY YEAR ASC, MONTH ASC";
            List<dynamic> _data = this._db.Query<dynamic>(query, new
            {
                filter = filter,
                startdate = startdate,
                enddate = enddate

            }).ToList();
            foreach (var item in _data)
            {
                lastdayofmonth = DateTime.DaysInMonth(int.Parse(item.YEAR.ToString()), int.Parse(item.MONTH.ToString()));
                startdate = item.YEAR.ToString() + "-" + item.MONTH.ToString() + "-01";
                enddate = item.YEAR.ToString() + "-" + item.MONTH.ToString() + "-" + lastdayofmonth;

                ProductDataItem _ProductItem = new ProductDataItem
                {
                    MonthYear = item.MMM.ToString() + ", " + item.YEAR.ToString(),
                    Items = new List<ProductDataLineItem> { }
                };
                List<string> WarehouseCode = new List<string> { };
                #region Top 3 Customers

                string TopCustomerquery = @"select WarehouseCode,customerno,'TopQty' = sum(HISTORY.TransactionQty) from IM_ITEMTRANSACTIONHISTORY HISTORY 
                               where HISTORY.itemcode =  @filter and HISTORY.TransactionCode = 'so'
                               and (HISTORY.TransactionDate >= @startdate and HISTORY.TransactionDate <= @enddate)
                               and WarehouseCode not in  ('FR', '0000', 'CHN', 'DCH','USA') and HISTORY.CustomerNo is not null
                               group by HISTORY.WarehouseCode, HISTORY.customerno order by sum(HISTORY.TransactionQty)";

                dynamic TopCustomer = this._db.Query<dynamic>(TopCustomerquery, new
                {
                    filter = filter,
                    startdate = startdate,
                    enddate = enddate
                }).ToList();

                if (TopCustomer != null) //top customers and all warehouse of respected month.
                {
                    if (TopCustomer.Count >= 3)
                    {
                        _ProductItem.TopCustomer1 = TopCustomer[0].customerno;
                        _ProductItem.TopCustomer1Qty = TopCustomer[0].TopQty;

                        _ProductItem.TopCustomer2 = TopCustomer[1].customerno;
                        _ProductItem.TopCustomer2Qty = TopCustomer[1].TopQty;

                        _ProductItem.TopCustomer3 = TopCustomer[2].customerno;
                        _ProductItem.TopCustomer3Qty = TopCustomer[2].TopQty;
                    }
                    else if (TopCustomer.Count >= 2)
                    {
                        _ProductItem.TopCustomer1 = TopCustomer[0].customerno;
                        _ProductItem.TopCustomer1Qty = TopCustomer[0].TopQty;

                        _ProductItem.TopCustomer2 = TopCustomer[1].customerno;
                        _ProductItem.TopCustomer2Qty = TopCustomer[1].TopQty;
                    }
                    else if (TopCustomer.Count >= 1)
                    {
                        _ProductItem.TopCustomer1 = TopCustomer[0].customerno;
                        _ProductItem.TopCustomer1Qty = TopCustomer[0].TopQty;
                    }

                    foreach (var warehouse in TopCustomer)
                        WarehouseCode.Add(warehouse.WarehouseCode);

                    WarehouseCode = WarehouseCode.Distinct().OrderBy(o => o).ToList();
                }

                #endregion

                #region Detail line Item
                
                foreach (var subitem in WarehouseCode)
                {
                    ProductDataLineItem detail = new ProductDataLineItem { QuantityOnHand = 0 };
                    detail.WarehouseCode = subitem;

                    string subquery = @"SELECT 
                                      (SELECT SUM(TransactionQty) FROM IM_ITEMTRANSACTIONHISTORY
                                      WHERE ItemCode = @filter and TransactionCode='so'
		                              and (TransactionDate >= @startdate and TransactionDate <= @enddate)
		                              and WarehouseCode not in ('FR', '0000', 'CHN', 'DCH','USA') 
                                      and WarehouseCode = @WarehouseCode and CustomerNo = @TopCustomer1) Customer1Qty,

                                      (SELECT SUM(TransactionQty) FROM IM_ITEMTRANSACTIONHISTORY
                                      WHERE ItemCode = @filter and TransactionCode='so'
		                              and (TransactionDate >= @startdate and TransactionDate <= @enddate)
		                              and WarehouseCode not in ('FR', '0000', 'CHN', 'DCH','USA') 
                                      and WarehouseCode = @WarehouseCode and CustomerNo = @TopCustomer2) Customer2Qty,

                                      (SELECT SUM(TransactionQty) FROM IM_ITEMTRANSACTIONHISTORY
                                      WHERE ItemCode = @filter and TransactionCode='so'
		                              and (TransactionDate >= @startdate and TransactionDate <= @enddate)
		                              and WarehouseCode not in ('FR', '0000', 'CHN', 'DCH','USA') 
                                      and WarehouseCode = @WarehouseCode and CustomerNo = @TopCustomer3) Customer3Qty,

                                      (SELECT SUM(TransactionQty) FROM IM_ITEMTRANSACTIONHISTORY
                                      WHERE ItemCode = @filter and TransactionCode='so'
		                              and (TransactionDate >= @startdate and TransactionDate <= @enddate)
		                              and WarehouseCode not in ('FR', '0000', 'CHN', 'DCH','USA') and WarehouseCode = @WarehouseCode) ShippedQty,

                                      (SELECT SUM(TransactionQty) FROM IM_ITEMTRANSACTIONHISTORY
                                      WHERE ItemCode = @filter and TransactionCode='po'
		                              and (TransactionDate >= @startdate and TransactionDate <= @enddate)
                              		  and WarehouseCode not in ('FR', '0000', 'CHN', 'DCH','USA') and WarehouseCode = @WarehouseCode) OnOrderQty,

		                              (SELECT SUM(TransactionQty) FROM IM_ITEMTRANSACTIONHISTORY
                                      WHERE ItemCode = @filter and TransactionCode='it'
		                              and (TransactionDate >= @startdate and TransactionDate <= @enddate)
		                              and WarehouseCode not in ('FR', '0000', 'CHN', 'DCH','USA') and WarehouseCode = @WarehouseCode) TransferQty";

                    dynamic _subData = this._db.Query<dynamic>(subquery, new
                    {
                        filter = filter,
                        startdate = startdate,
                        enddate = enddate,
                        WarehouseCode = subitem,
                        TopCustomer1 = _ProductItem.TopCustomer1,
                        TopCustomer2 = _ProductItem.TopCustomer2,
                        TopCustomer3 = _ProductItem.TopCustomer3,
                    }).FirstOrDefault();

                    if (_subData != null)
                    {
                        if (_subData.Customer1Qty != null)
                            detail.Customer1Qty = _subData.Customer1Qty < 0 ? _subData.Customer1Qty : 0;
                        else
                            detail.Customer1Qty = 0;

                        if (_subData.Customer2Qty != null)
                            detail.Customer2Qty = _subData.Customer2Qty < 0 ? _subData.Customer2Qty : 0;
                        else
                            detail.Customer2Qty = 0;

                        if (_subData.Customer3Qty != null)
                            detail.Customer3Qty = _subData.Customer3Qty < 0 ? _subData.Customer3Qty : 0;
                        else
                            detail.Customer3Qty = 0;

                        if (_subData.ShippedQty != null)
                            detail.ShippedQuantity = _subData.ShippedQty;
                        else
                            detail.ShippedQuantity = 0;

                        if (_subData.OnOrderQty != null)
                            detail.PurhaseOrder = _subData.OnOrderQty;
                        else
                            detail.PurhaseOrder = 0;

                        if (_subData.TransferQty != null)
                            detail.Transfer = _subData.TransferQty;
                        else
                            detail.Transfer = 0;


                        detail.ProjActQuantity = ((detail.ShippedQuantity + detail.Transfer) + detail.PurhaseOrder);
                    }
                    
                    _ProductItem.Items.Add(detail);
                    detail = null;
                    _subData = null;
                }

                WarehouseCode = null;
                #endregion

                model.Items.Add(_ProductItem);
                _ProductItem = null;
            }
            _data = null;
            return model;
        }

        public Dictionary<string, Dictionary<string, decimal>> GetItemWareHouseDetail(string filter, string Month, string Year)
        {            
            Month = Month.TrimStart();
            Year = Year.TrimStart();
            Dictionary<string, Dictionary<string, decimal>> ReturnList = new Dictionary<string, Dictionary<string, decimal>> { };
            List<string> _AllCustomers = new List<string> { };
            List<string> _AllWareHouses = new List<string> { };
            string collectionCustomer = string.Empty;

            int monthInDigit = DateTime.ParseExact(Month, "MMM", CultureInfo.InvariantCulture).Month; // Month name to number.
            string startdate = Year + "-" + monthInDigit + "-01";
            string enddate = Year + "-" + monthInDigit + "-" + DateTime.DaysInMonth(int.Parse(Year), monthInDigit); // get last of the month;

            #region Group Data

            string query = @"select WarehouseCode, customerno from IM_ITEMTRANSACTIONHISTORY HISTORY where HISTORY.itemcode = @filter
                             and HISTORY.TransactionCode = 'so' and(HISTORY.TransactionDate >= @startdate and HISTORY.TransactionDate <= @enddate)
                             and WarehouseCode not in  ('FR', '0000', 'CHN', 'DCH','USA')
                             group by WarehouseCode,customerno";

            List<dynamic> _GroupList = this._db.Query<dynamic>(query, new
            {
                filter = filter,
                startdate = startdate,
                enddate = enddate
            }).ToList();

           
            if (_GroupList != null)
            {
                foreach (var item in _GroupList)
                {
                    _AllCustomers.Add(item.customerno);
                    _AllWareHouses.Add(item.WarehouseCode);
                }

                _AllCustomers = _AllCustomers.Distinct().OrderBy(o => o).ToList();
                _AllWareHouses = _AllWareHouses.Distinct().OrderBy(o => o).ToList();
            }

            #endregion 

            if (_AllCustomers.Count() > 0)
                collectionCustomer = "'" + string.Join("','", _AllCustomers) + "'";            

            Dictionary<string, decimal> innerlist = null;
            foreach (var wareHouse in _AllWareHouses)
            {
                innerlist = new Dictionary<string, decimal> { };
                query = @"select customerno,'SalesOrder' = sum(HISTORY.TransactionQty) from IM_ITEMTRANSACTIONHISTORY HISTORY where HISTORY.itemcode = @filter
                          and HISTORY.TransactionCode='so' and (HISTORY.TransactionDate >= @startdate and HISTORY.TransactionDate <= @enddate)
                          and WarehouseCode not in  ('FR', '0000', 'CHN', 'DCH','USA') and WarehouseCode = @WarehouseCode
                          and customerno in (" + collectionCustomer + ") group by customerno";
                var customerlist = this._db.Query(query, new
                {
                    filter = filter,
                    startdate = startdate,
                    enddate = enddate,
                    WarehouseCode = wareHouse,
                }).ToDictionary(row => (string)row.customerno, row => (decimal)row.SalesOrder);


                if (customerlist != null && customerlist.Count > 0)
                {
                    foreach (var item in _AllCustomers)
                    {
                        var match = customerlist.Where(w => w.Key.Equals(item)).ToDictionary(i => i.Key, i => i.Value).FirstOrDefault();
                        if (!string.IsNullOrEmpty(match.Key))
                            innerlist.Add(match.Key, match.Value);
                        else
                            innerlist.Add(item, 0);
                    }                                
                }
                ReturnList.Add(wareHouse, innerlist);
            }

            collectionCustomer = string.Empty;
            _AllCustomers = null;
            _AllWareHouses = null;
            innerlist = null;
            return ReturnList;
        }
    }
}
