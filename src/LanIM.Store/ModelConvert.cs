using Com.LanIM.Store.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Store
{
    public class ModelConvert<T>
    {
        public List<T> Convert(DataTable dt, params ColumnMapping[] mappings)
        {
            List<T> list = new List<T>();

            foreach (DataRow row in dt.Rows)
            {
                T t = Activator.CreateInstance<T>();
                Type type = t.GetType();

                foreach (ColumnMapping mapping in mappings)
                {
                    PropertyInfo pi = type.GetProperty(mapping.Property);
                    if(pi != null)
                    {
                        pi.SetValue(t, row[mapping.Column]);
                    }
                }

                list.Add(t);
            }
            return list;
        }

        public SQLiteParameter[] CreateParameters(T obj, params ColumnMapping[] mappings)
        {
            Type type = obj.GetType();

            List<SQLiteParameter> list = new List<SQLiteParameter>();

            foreach (ColumnMapping mapping in mappings)
            {
                    PropertyInfo pi = type.GetProperty(mapping.Property);
                    if (pi != null)
                    {

                    object v = pi.GetValue(obj);

                    SQLiteParameter param = new SQLiteParameter("@" + mapping.Column, v);
                    list.Add(param);
                }
            }

            return list.ToArray();
        }
    }
}
