using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mca.dal
{
    public class BaseDAL
    {
        private IDbConnection __db;
        protected IDbConnection _db
        {
            get
            {
                return GetDBConn();
            }
        }

        protected IDbConnection GetDBConn(bool bForceNew = false)
        {
            if (bForceNew)
            {
                //Make a new connection and return it. May be required in the future.
                return new SqlConnection(ConfigurationManager.ConnectionStrings["connection"].ToString());
            }
            if (__db != null)
            {
                return __db;
            }
            __db = new SqlConnection(ConfigurationManager.ConnectionStrings["connection"].ToString());
            return __db;
        }      
    }
}
