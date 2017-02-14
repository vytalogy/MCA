using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using mca.providex;

namespace mca.providex
{
    public class ProductDAL : BaseClass
    {
        public ProductDAL(String conn) : base(conn)
        {

        }

        public ProductDAL()
        {

        }

        public List<CustomListItem> GetItemList(string CI_Item)
        {
            List<CustomListItem> List = new List<CustomListItem> { };
            SqlConnection conn = null;
            SqlDataReader reader;
            try
            {
                string sql = "select  * from  CI_Item where itemcode like'" + CI_Item + "%'";
                conn = new System.Data.SqlClient.SqlConnection(this.conn);
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    List.Add(new CustomListItem { Value = reader.GetValue(0).ToString(), Text = reader.GetValue(2).ToString() });
                }
                reader.Close();
                cmd.Dispose();
                conn.Close();

            }
            catch (SqlException ex)
            {
                // DAL.Utilities.SaveException(ex);
            }
            catch (Exception ex)
            {
                //DAL.Utilities.SaveException(ex);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn = null;
            }
            return List;
        }

        public List<CustomListItem> GetItemByFilter(string filter)
        {
            List<CustomListItem> List = new List<CustomListItem> { };
            SqlConnection conn = null;
            SqlDataReader reader;
            try
            {
                string sql = "select  * from  CI_Item where ItemCodeDesc like'%" + filter + "%'";
                conn = new System.Data.SqlClient.SqlConnection(this.conn);
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    List.Add(new CustomListItem { Value = reader.GetValue(0).ToString(), Text = reader.GetValue(2).ToString() });
                }
                reader.Close();
                cmd.Dispose();
                conn.Close();

            }
            catch (SqlException ex)
            {
                // DAL.Utilities.SaveException(ex);
            }
            catch (Exception ex)
            {
                //DAL.Utilities.SaveException(ex);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn = null;
            }
            return List;
        }
    }
}
