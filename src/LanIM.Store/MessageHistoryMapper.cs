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
            if (m is IPrepare)
            {
                //如果是需要准备的文件，事先准备工作
                (m as IPrepare).Prepare();
            }

            ModelConvert<Message> convert = new ModelConvert<Message>();

            SQLiteParameter[] parameters = convert.CreateParameters(m, 
                new ColumnMapping("C_TYPE", "Type"),
                new ColumnMapping("C_TIME", "Time"),
                new ColumnMapping("C_FROM_USER_ID", "FromUserId"),
                new ColumnMapping("C_TO_USER_ID", "ToUserId"),
                new ColumnMapping("C_CONTENT", "Content"),
                new ColumnMapping("C_FLAG", "Flag"));

            int id = LanIMStore.Instance.Insert(Sql.ADD_MESSAGE, parameters);
            m.ID = id;
        }

        public void UpdateState(Message m)
        {
            ModelConvert<Message> convert = new ModelConvert<Message>();

            SQLiteParameter[] parameters = convert.CreateParameters(m,
                new ColumnMapping("C_ID", "ID"),
                new ColumnMapping("C_FLAG", "Flag"));

            LanIMStore.Instance.Update(Sql.UPDATE_MESSAGE_STATE, parameters);
        }

        /// <summary>
        /// 查找从id之前的消息
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Message> QueryUserLatestMessages(string userID, long id)
        {
            if(id < 0)
            {
                id = long.MaxValue;
            }

            DataTable dt = LanIMStore.Instance.Query(Sql.QUERY_USER_LATEST_MESSAGE,
                new SQLiteParameter("@USER_ID", userID),
                new SQLiteParameter("@ID", id));

            ModelConvert<Message> convert = new ModelConvert<Message>();
            IInstanceCreater<Message> ic = new MessageInstanceCreater("C_TYPE");

            List<Message> list = convert.Convert(dt, ic,
                new ColumnMapping("C_ID", "ID"),
                new ColumnMapping("C_TIME", "Time"),
                new ColumnMapping("C_FROM_USER_ID", "FromUserId"),
                new ColumnMapping("C_TO_USER_ID", "ToUserId"),
                new ColumnMapping("C_CONTENT", "Content"),
                new ColumnMapping("C_FLAG", "Flag"));

            //按发送顺序
            list.Reverse();

            foreach (Message m in list)
            {
                if(m is IPost)
                {
                    (m as IPost).Post();
                }
            }
            return list;
        }
    }
}
