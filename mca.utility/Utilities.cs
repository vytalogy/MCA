using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace mca.utility
{
    public static class Utilities
    {
        public static T ConvertToEntity<T>(DataTable dt) where T : class, new()
        {
            if (dt == null || dt.Rows.Count == 0) return null;
            return ConvertDataRowToEntity<T>(dt.Rows[0]);
        }
        public static List<T> ConvertToList<T>(DataTable dt) where T : class, new()
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0) return null;
                return (from DataRow row in dt.Rows select ConvertDataRowToEntity<T>(row)).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static T ConvertDataRowToEntity<T>(DataRow row) where T : class, new()
        {
            var objType = typeof(T);
            var obj = Activator.CreateInstance<T>();

            try
            {
                foreach (DataColumn column in row.Table.Columns)
                {

                    var property = objType.GetProperty(column.ColumnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (property == null || !property.CanWrite) continue;
                    var value = row[column.ColumnName];
                    if (value == DBNull.Value) value = null;

                    try
                    {
                        property.SetValue(obj, value, null);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
