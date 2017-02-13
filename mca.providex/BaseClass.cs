using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mca.providex
{
    public class BaseClass
    {       
        protected String conn = System.Configuration.ConfigurationManager.ConnectionStrings["providexDB"].ConnectionString;
        public BaseClass(String conn)
        {
            this.conn = conn;
        }
        public BaseClass()
        {
        }
        protected SqlParameter GetSqlParameter(String name, System.Data.SqlDbType _SqlDbType, object value)
        {
            SqlParameter _SqlParameter = new SqlParameter((name.StartsWith("@") ? name : "@" + name), _SqlDbType);
            if (value == null)
                _SqlParameter.Value = System.DBNull.Value;
            else
                _SqlParameter.Value = value;
            return _SqlParameter;
        }
    }
}
