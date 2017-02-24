using mca.model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using System.Dynamic;

namespace mca.dal
{
    public class ProdcutDAL : BaseDAL
    {
        public Tuple<int,string> AddProduct(ProductUpdateDataModel _data)
        {
            int prodcutId = 0;     
            try
            {
                int value = isProductExist(_data.ProductCode);
                if (value > 0)
                {
                    prodcutId = value;
                    _data.ID = value;
                    UpdateProduct(_data);                    
                }
                else
                {
                    string query = @"INSERT INTO [dbo].[Products]
                                   ([ProductCode]
                                    ,[ProductDescription]
                                    ,[isYearly]
                                    ,[ForeCastingStartDate]
                                    ,[ForeCastingEndDate]
                                    ,[CreatedBy]
                                    ,[CreatedByName]
                                    ,[CreatedByDate])
                                VALUES
                                    (@ProductCode,
                                    @ProductDescription,
                                    @isYearly,
                                    @ForeCastingStartDate,
                                    @ForeCastingEndDate,
                                    @CreatedBy,
                                    @CreatedByName,
                                    @CreatedByDate) 
                                select @@IDENTITY ";

                    prodcutId = this._db.Query<int>(query, new
                    {
                        ProductCode = _data.ProductCode,
                        ProductDescription = _data.Description,
                        isYearly = _data.isYearly,
                        ForeCastingStartDate = DateTime.Now.AddMonths(1),
                        ForeCastingEndDate = DateTime.Now.AddMonths(12),
                        CreatedBy = _data.CreatedBy,
                        CreatedByName = _data.CreatedByName,
                        CreatedByDate = DateTime.Now,
                    }).FirstOrDefault();

                }
                return Tuple.Create(prodcutId, string.Empty);
            }
            catch (Exception exe)
            {
                return Tuple.Create(0, exe.Message);
            }
        }

        public bool UpdateProduct(ProductUpdateDataModel _data)
        {
            string query = @"UPDATE [Products]
                             SET [ProductCode] = @ProductCode
                             ,[ProductDescription] = @ProductDescription
                             ,[isYearly] = @isYearly
                             ,[ForeCastingStartDate] = @ForeCastingStartDate
                             ,[ForeCastingEndDate] = @ForeCastingEndDate
                             ,[ModifyBy] = @ModifyBy
                             ,[ModifyByName] = @ModifyByName
                             ,[ModifyByDate] = @ModifyByDate
                              WHERE ID=@ID";

            this._db.Query<int>(query, new
            {
                ID = _data.ID,
                ProductCode = _data.ProductCode,
                ProductDescription = _data.Description,
                isYearly = _data.isYearly,
                ForeCastingStartDate = DateTime.Now.AddMonths(1),
                ForeCastingEndDate = DateTime.Now.AddMonths(12),
                ModifyBy = _data.ModifyBy,
                ModifyByName = _data.ModifyByName,
                ModifyByDate = DateTime.Now,
            }).FirstOrDefault();

            return true;
        }            
        
        public bool AddForecasting(List<SelectList> lit,int ProductID)
        {
            try
            {
                string query = @"DELETE FROM [ForeCasting] WHERE productid=@ProductID";
                this._db.Query(query, new
                {
                    ProductID = ProductID,
                });

                List<ForeCasting> objlist = new List<ForeCasting> { };
                int count = 1;
                foreach (var item in lit)
                {
                    int Month = DateTime.Now.AddMonths(count).Month;
                    int Year = DateTime.Now.Year;
                    if (count == 11  || count == 12)
                        Year = DateTime.Now.AddYears(1).Year;

                    var firstDayOfMonth = new DateTime(Year, Month, 1);
                    //var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                    ForeCasting obj = new ForeCasting { };
                    obj.ProductID = ProductID;
                    obj.MonthValue = item.Value;
                    obj.MonthName = item.Text;
                    obj.MonthDate = firstDayOfMonth;
                    obj.Selected = item.Selected;
                    objlist.Add(obj);

                    count = count + 1;
                }

                query = @"INSERT INTO [ForeCasting] ([ProductID],[MonthValue],[MonthName],[MonthDate],[Selected])
                          VALUES (@ProductID,@MonthValue,@MonthName,@MonthDate,@Selected)";

                this._db.Execute(query, objlist);

                objlist = null;
                return true;
            }
            catch(Exception exe)
            {
                return false;
            }
        }

        public dynamic isProductExist(string ProductCode)
        {
            string query = @"select ID From [Products] where productcode=@ProductCode";
            dynamic value = this._db.Query<dynamic>(query, new
            {
                ProductCode = ProductCode,

            }).FirstOrDefault();

            if (value != null)
                return value.ID;
            else
                return 0;
        }
    }
}
