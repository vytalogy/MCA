using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace mca.providex
{
    public class BaseClass
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
                return new SqlConnection(ConfigurationManager.ConnectionStrings["providexDB"].ToString());
            }
            if (__db != null)
            {
                return __db;
            }
            __db = new SqlConnection(ConfigurationManager.ConnectionStrings["providexDB"].ToString());
            return __db;
        }       
    }
}
