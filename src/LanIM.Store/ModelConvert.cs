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
    class ModelConvert<T>
    {
        /// <summary>
        /// 将数据表转换成对象T
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="mappings"></param>
        /// <returns></returns>
        public List<T> Convert(DataTable dt, IInstanceCreater<T> creater, params ColumnMapping[] mappings)
        {
            List<T> list = new List<T>();
            if(dt == null)
            {
                return list;
            }
            foreach (DataRow row in dt.Rows)
            {
                T t = creater.CreateInstance(row);
                Type type = t.GetType();

                foreach (ColumnMapping mapping in mappings)
                {
                    PropertyInfo pi = type.GetProperty(mapping.Property);
                    if(pi != null)
                    {
                        object obj = row[mapping.Column];
                        if (obj is DBNull)
                        {
                            obj = null;
                        }
                        pi.SetValue(t, obj);
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
