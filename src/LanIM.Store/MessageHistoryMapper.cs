using Com.LanIM.Store.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Store
{
    public class MessageHistoryMapper
    {
        public void Add(Message m)
        {
            ModelConvert<Message> convert = new ModelConvert<Message>();

            SQLiteParameter[] parameters = convert.CreateParameters(m, 
                new ColumnMapping("C_TYPE", "Type"),
                new ColumnMapping("C_TIME", "Time"),
                new ColumnMapping("C_FROM_USER_ID", "FromUserId"),
                new ColumnMapping("C_TO_USER_ID", "ToUserId"),
                new ColumnMapping("C_CONTENT", "Content"));

            LanIMStore.Instance.ExecuteNoQuery(Sql.ADD_MESSAGE, parameters);
        }
    }
}
